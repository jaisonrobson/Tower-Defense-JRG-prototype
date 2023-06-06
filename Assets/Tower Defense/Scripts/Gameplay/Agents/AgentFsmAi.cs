using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using Core.Patterns;
using Pathfinding;

[System.Serializable]
public struct TimedAttack
{
    public float nextTimeToExecute;
    public bool isMakingAttack;
    public AttackSO attack;

    public TimedAttack(AttackSO pAttack, float pNextTimeToExecute = 0, bool pIsMakingAttack = false)
    {
        attack = pAttack;
        nextTimeToExecute = pNextTimeToExecute;
        isMakingAttack = pIsMakingAttack;
    }
}

[HideMonoScript]
public abstract class AgentFsmAi : MonoBehaviour
{
    // Protected (Variables) [START]
    [BoxGroup("Agent FSM Identity")]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    protected FiniteStateMachine currentState = null;
    [BoxGroup("Agent FSM Identity")]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    protected AgentSO agentSO;
    [BoxGroup("Agent FSM Identity")]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    protected Agent agent;
    [BoxGroup("Agent FSM Identity")]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    protected bool isDying;
    [BoxGroup("Agent FSM Identity")]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    protected float timeUntilCompletelyDie;
    // Protected (Variables) [END]

    // Private (Variables) [START]
    private GameObject agentGOBJ;
    private Animator anim;
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    private TimedAttack[] timedAttacks;
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    private bool isAllAttacksUnderCooldown = false;
    // Private (Variables) [END]    

    // Protected (Properties) [START]
    protected GameObject AgentGOBJ { get { return agentGOBJ; } }
    protected Animator Anim { get { return anim; } }
    // Protected (Properties) [END]

    // Public (Properties) [START]
    public bool IsAllAttacksUnderCooldown { get { return isAllAttacksUnderCooldown; } }
    public bool IsAgentDead { get { return agent.ActualHealth <= 0f; } }
    public bool IsAggressive { get { return agentSO.isAggressive; } }
    public bool IsMovable { get { return agentSO.isMovable; } }
    public bool IsPlayable { get { return agentSO.isPlayable; } }
    // Public (Properties) [END]

    // (Unity) Methods [START]
    protected virtual void OnEnable()
    {
        InitializeVariables();
    }

    protected virtual void Update()
    {
        UpdateFSMStates();

        UpdateAttackCooldown();

        HandleAttacking();

        HandleDying();
    }
    // (Unity) Methods [END]


    // Public (Methods) [START]
    public virtual void PoolRetrievalAction(Poolable poolable)
    {
        timedAttacks = null;
        isAllAttacksUnderCooldown = false;
        isDying = false;
    }
    public void StartDying()
    {
        isDying = true;
        timeUntilCompletelyDie = 3f;
    }
    public bool IsAttackInRange(AttackSO attack)
    {
        if (attack.minimumAttackDistance > agent.AttackRange)
            return false;

        if (agent.GetActualEnemyAgent() == null)
            return false;

        if (agent.GetDistanceBetweenAgentAndEnemy() > attack.minimumAttackDistance)
            return false;

        return true;
    }
    public TimedAttack GetNearestNotInCooldownAttack()
    {
        TimedAttack nearestAttack = timedAttacks.First();

        nearestAttack = timedAttacks
            .Where((ta) => !ta.isMakingAttack && IsAttackReady(ta))
            .Aggregate(
                nearestAttack,
                (nearest, next) => nearest.attack.minimumAttackDistance > next.attack.minimumAttackDistance ? next : nearest
            );

        return nearestAttack;
    }
    public bool IsAnyViableAttackUnderEnemyRange() => timedAttacks.Any(timedAttack => IsAttackInRange(timedAttack.attack) && IsAttackReady(timedAttack));
    public bool IsAnyAttackUnderEnemyRange() => timedAttacks.Any(timedAttack => IsAttackInRange(timedAttack.attack));
    public bool IsAllAttacksUnderEnemyRange() => timedAttacks.All(timedAttack => IsAttackInRange(timedAttack.attack));
    public bool IsMakingAnyAttack() => timedAttacks.Any(timedAttack => timedAttack.isMakingAttack);
    // Public (Methods) [END]

    // Private (Methods) [START]
    private void InitializeVariables()
    {
        isDying = false;
        timeUntilCompletelyDie = 0f;
        isAllAttacksUnderCooldown = false;
        agentGOBJ = gameObject;
        anim = gameObject.GetComponentInChildren<Animator>(true);

        if (GetComponent<Creature>() != null)
            agentSO = GetComponent<Creature>().agent;
        if (GetComponent<Structure>() != null)
            agentSO = GetComponent<Structure>().agent;

        agent = GetComponent<Agent>();

        Validate_IsNull_Agent();

        CreateTimedAttacks();
    }
    private void UpdateFSMStates()
    {
        currentState = currentState.Process();
    }
    private void UpdateAttackCooldown()
    {
        if (IsAgentDead || !IsAggressive)
            return;

        if (CheckIsAllAttacksUnderCooldown())
        {
            isAllAttacksUnderCooldown = true;
            GoToIdleState();
        }
        else
        {
            isAllAttacksUnderCooldown = false;
        }
    }
    private void ResetAllTimedAttacksFlag() => timedAttacks.ToList().ForEach(ta => ta.isMakingAttack = false);
    private void HandleAttacking()
    {
        if (IsAgentDead || !IsAggressive)
            return;

        if (currentState.name == AgentStateEnum.ATTACK)
        {
            if (IsAllAttacksReady())
                ResetAllTimedAttacksFlag();

            for (int i = 0; i < timedAttacks.Length; i++)
            {
                if (Time.fixedTime >= timedAttacks[i].nextTimeToExecute)
                    timedAttacks[i].isMakingAttack = false;

                if (!IsAttackInRange(timedAttacks[i].attack))
                {
                    continue;
                }

                if (Time.fixedTime >= (timedAttacks[i].nextTimeToExecute + timedAttacks[i].attack.cooldown))
                {
                    if (!IsMakingAnyAttack())
                    {
                        timedAttacks[i].isMakingAttack = true;
                        
                        timedAttacks[i].nextTimeToExecute = CalculateAttackTiming(timedAttacks[i].attack, false);

                        Agent enemyAgent = agent.GetActualEnemyAgent();

                        if (enemyAgent != null)
                            enemyAgent.OnReceiveDamage(agent.Alignment, agent.Damage, timedAttacks[i].attack);
                    }
                }

                if (Time.fixedTime < timedAttacks[i].nextTimeToExecute)
                    UpdateAttackAnimation(timedAttacks[i].attack);
            }
        }
    }
    private bool IsAllAttacksReady() => timedAttacks.All(ta => IsAttackReady(ta));
    private bool IsAttackReady(TimedAttack timedAttack) => Time.fixedTime >= (timedAttack.nextTimeToExecute + timedAttack.attack.cooldown);
    private bool CheckIsAllAttacksUnderCooldown()
    {
        bool result = true;

        for (int i = 0; i < timedAttacks.Length; i++)
        {
            if (!IsAttackUnderCooldown(timedAttacks[i]))
            {
                result = false;

                break;
            }
        }

        return result;
    }
    private bool IsAttackUnderCooldown(TimedAttack timedAttack) => Time.fixedTime < (timedAttack.nextTimeToExecute + timedAttack.attack.cooldown) && Time.fixedTime > timedAttack.nextTimeToExecute;

    private void CreateTimedAttacks()
    {
        if (!IsAggressive)
            return;

        if (timedAttacks == null)
        {
            timedAttacks = new TimedAttack[agentSO.attacks.Count];

            for (int i = 0; i < agentSO.attacks.Count; i++)
                timedAttacks[i] = new TimedAttack(agentSO.attacks[i]);
        }
    }
    private float CalculateAttackTiming(AttackSO pAttack, bool useCooldown = true)
    {
        float cd = useCooldown ? pAttack.cooldown : 0f;
        return Time.fixedTime + cd + (1f / CalculateAttackVelocity(pAttack));
    }
    private float CalculateAttackVelocity(AttackSO pAttack)
    {
        return (agent.AttackVelocity * ((float)pAttack.influenceOverAttackVelocity / 100));
    }
    private void UpdateAttackAnimation(AttackSO pAttack)
    {
        Anim.SetFloat("attackSpeed", Mathf.Clamp(CalculateAttackVelocity(pAttack), 0.03f, 5f));
    }
    // Private (Methods) [END]

    // Protected (Methods) [START]
    protected abstract void GoToIdleState();
    protected virtual void HandleDying()
    {
        if (currentState.name == AgentStateEnum.DIE && isDying)
        {
            timeUntilCompletelyDie -= Time.deltaTime;

            if (timeUntilCompletelyDie <= 0f)
            {
                GoToIdleState();
                agent.HandleDeath();
            }
        }
    }
    // Protected (Methods) [END]

    // (Validation) Methods [START]
    private void Validate_IsNull_Agent()
    {
        if (agent == null)
            throw new System.Exception("Agent not found on Finite State Machine AI: "+gameObject.name);
    }
    // (Validation) Methods [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////