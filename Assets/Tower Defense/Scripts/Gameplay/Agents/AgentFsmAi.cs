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
    public bool IsAttackPrevented { get { return agent.IsAttackPrevented; } }
    public bool IsMovementPrevented { get { return agent.IsMovementPrevented; } }
    // Public (Properties) [END]

    // (Unity) Methods [START]
    protected virtual void OnEnable()
    {
        InitializeVariables();
    }

    protected virtual void Update()
    {
        if (GameManager.instance.IsRunningAndNotPaused)
        {
            UpdateFSMStates();

            UpdateAttackCooldown();

            HandleAttacking();

            HandleDying();

            HandleSubSpawning();
        }
    }
    // (Unity) Methods [END]

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
        if (IsAgentDead || !IsAggressive || IsAttackPrevented)
            return;

        if (IsAllAttacksReady())
            ResetAllTimedAttacksFlag();

        for (int i = 0; i < timedAttacks.Length; i++)
        {
            if (Time.fixedTime >= timedAttacks[i].nextTimeToExecute)
                timedAttacks[i].isMakingAttack = false;

            if (currentState.name == AgentStateEnum.ATTACK)
            {
                if (!IsAttackInRange(timedAttacks[i].attack))
                    continue;

                if (Time.fixedTime >= (timedAttacks[i].nextTimeToExecute + timedAttacks[i].attack.cooldown))
                {
                    if (!IsMakingAnyAttack())
                    {
                        timedAttacks[i].isMakingAttack = true;

                        timedAttacks[i].nextTimeToExecute = CalculateAttackTiming(timedAttacks[i].attack, false);

                        Agent enemyAgent = agent.GetActualEnemyAgent();

                        if (enemyAgent != null)
                            Attacking.InvokeAttack(timedAttacks[i].attack, agent, enemyAgent);
                        else if (timedAttacks[i].attack.type == AttackTypeEnum.SIEGE)
                            Attacking.InvokeAttack(timedAttacks[i].attack, agent);
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
        return Time.fixedTime + cd + (1f / agent.CalculateAttackVelocity(pAttack));
    }
    private void UpdateAttackAnimation(AttackSO pAttack)
    {
        Anim.SetFloat("attackSpeed", agent.CalculateAttackVelocity(pAttack));
    }
    private void HandleSubSpawning()
    {
        if (agent.SubSpawns.Count > 0 && agent.Alignment != AlignmentEnum.GENERIC)
        {
            if (agent.GetAgent().type == AgentTypeEnum.STRUCTURE && GetComponent<PlayableStructure>() != null)
                if (!GetComponent<PlayableStructure>().IsPlaced)
                    return;

            bool modified = false;

            SubSpawn[] subs = agent.SubSpawns.ToArray();
            for (int subSpawnIndex = 0; subSpawnIndex < subs.Length; subSpawnIndex++)
            {
                if (subs[subSpawnIndex].spawnedAgents.Count < subs[subSpawnIndex].subSpawn.maxAlive)
                {
                    if (subs[subSpawnIndex].timeToNextSpawn <= 0f)
                    {
                        subs[subSpawnIndex].timeToNextSpawn = subs[subSpawnIndex].subSpawn.delay;
                        subs[subSpawnIndex].spawnedAgents.Add(SubSpawnAgent(subs[subSpawnIndex].subSpawn.creature.prefab));
                    }
                    else
                        subs[subSpawnIndex].timeToNextSpawn -= Time.deltaTime;

                    modified = true;
                }
            }

            if (modified)
                agent.SubSpawns = subs.ToList();
        }
    }
    private GameObject SubSpawnAgent(GameObject agentPrefab)
    {
        GameObject newAgent = Poolable.TryGetPoolable(agentPrefab, OnRetrieveSubSpawnPoolableAgent);

        newAgent.gameObject.GetComponent<AIPath>().Teleport(agent.GetAgentColliderBoundsInitialPosition(newAgent.transform));

        newAgent.gameObject.GetComponent<Agent>().DoSpawnFXs();

        return newAgent;
    }
    private void OnRetrieveSubSpawnPoolableAgent(Poolable poolable)
    {
        Agent newAgent = poolable.gameObject.GetComponent<Agent>();
        Agent master = agent;

        poolable.transform.SetPositionAndRotation(master.GetAgentColliderBoundsInitialPosition(poolable.transform), transform.rotation);
        newAgent.goal = master.GetAgent().type == AgentTypeEnum.CREATURE ? AgentGoalEnum.MASTER : AgentGoalEnum.FLAG;
        newAgent.Alignment = master.Alignment;
        newAgent.Master = master;
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

    // Public (Methods) [START]

    public void StartDying()
    {
        isDying = true;
        timeUntilCompletelyDie = 5f;

        agent.DoDeathFXs(timeUntilCompletelyDie);
    }
    public float GetDistanceBetweenAgentAndDestination(AttackSO attack)
    {
        Agent enemy = agent.GetActualEnemyAgent();

        if (enemy != null)
        {
            Vector3 enemyClosestPoint = enemy.mainCollider.ClosestPointOnBounds(transform.position);
            Vector3 agentClosestPoint = agent.mainCollider.ClosestPointOnBounds(enemyClosestPoint);

            return Vector3.Distance(enemyClosestPoint, agentClosestPoint);
        }
        else
            return float.PositiveInfinity;
    }
    public bool IsAttackInRange(AttackSO attack)
    {
        if (attack.type == AttackTypeEnum.SIEGE)
            return true;

        if (attack.minimumAttackDistance > agent.AttackRange)
            return false;

        if (agent.GetActualEnemyAgent() == null && attack.isEnemyTriggered)
            return false;

        float distanceBetweenAgentAndDestination = GetDistanceBetweenAgentAndDestination(attack);

        if (distanceBetweenAgentAndDestination < attack.minimumAttackDistance || distanceBetweenAgentAndDestination > attack.maximumAttackDistance || distanceBetweenAgentAndDestination > agent.AttackRange)
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
    public bool IsAnyAttackNonEnemyTriggeredInRange() => timedAttacks != null && timedAttacks.Any(timedAttack => IsAttackInRange(timedAttack.attack) && IsAttackReady(timedAttack) && !timedAttack.attack.isEnemyTriggered);
    public bool IsAnyViableAttackUnderEnemyRange() => timedAttacks != null && timedAttacks.Any(timedAttack => IsAttackInRange(timedAttack.attack) && IsAttackReady(timedAttack));
    public bool IsAnyAttackUnderEnemyRange() => timedAttacks.Any(timedAttack => IsAttackInRange(timedAttack.attack));
    public bool IsAllAttacksUnderEnemyRange() => timedAttacks.All(timedAttack => IsAttackInRange(timedAttack.attack));
    public bool IsMakingAnyAttack() => timedAttacks != null && timedAttacks.Any(timedAttack => timedAttack.isMakingAttack);
    public bool IsAnyAttackNonEnemyTriggered() => timedAttacks != null && timedAttacks.Any(timedAttack => !timedAttack.attack.isEnemyTriggered);
    public virtual void PoolRetrievalAction(Poolable poolable)
    {
        timedAttacks = null;
        isAllAttacksUnderCooldown = false;
        isDying = false;
    }
    // Public (Methods) [END]

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