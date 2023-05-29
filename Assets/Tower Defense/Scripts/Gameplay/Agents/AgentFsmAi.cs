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
    public AttackSO attack;

    public TimedAttack(AttackSO pAttack, float pNextTimeToExecute = 0)
    {
        attack = pAttack;
        nextTimeToExecute = pNextTimeToExecute;
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
    [BoxGroup("Agent FSM Identity")]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    protected AIPath pathfinding;
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
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    private bool isMakingAttack = false;
    // Private (Variables) [END]    

    // Protected (Properties) [START]
    protected GameObject AgentGOBJ { get { return agentGOBJ; } }
    protected Animator Anim { get { return anim; } }    
    // Protected (Properties) [END]

    // Public (Properties) [START]
    public bool IsAllAttacksUnderCooldown { get { return isAllAttacksUnderCooldown; } }
    public bool IsAgentDead { get { return agent.ActualHealth <= 0f; } }
    public bool IsAgressive { get { return agentSO.isAgressive; } }
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

        UpdateWalkAnimation();

        UpdateAIDestination();
    }
    // (Unity) Methods [END]


    // Public (Methods) [START]
    public virtual void PoolRetrievalAction(Poolable poolable)
    {
        timedAttacks = null;
        isAllAttacksUnderCooldown = false;
        isMakingAttack = false;
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

        Agent enemy = agent.GetActualEnemyAgent();

        if (enemy == null)
            return false;

        Vector3 enemyClosestPoint = enemy.mainCollider.ClosestPointOnBounds(agent.transform.position);

        if (Vector3.Distance(enemyClosestPoint, agent.transform.position) > attack.minimumAttackDistance)
            return false;

        return true;
    }
    public bool IsAnyAttackUnderEnemyRange() => timedAttacks.Any(timedAttack => IsAttackInRange(timedAttack.attack));
    // Public (Methods) [END]

    // Private (Methods) [START]
    private void InitializeVariables()
    {
        isDying = false;
        timeUntilCompletelyDie = 0f;
        isMakingAttack = false;
        isAllAttacksUnderCooldown = false;
        agentGOBJ = gameObject;
        anim = gameObject.GetComponentInChildren<Animator>();
        pathfinding = AgentGOBJ.GetComponent<AIPath>();
        pathfinding.destination = transform.position;

        if (GetComponent<Creature>() != null)
            agentSO = GetComponent<Creature>().agent;
        if (GetComponent<Structure>() != null)
            agentSO = GetComponent<Structure>().agent;

        agent = GetComponent<Agent>();

        Validate_IsNull_Agent();

        CreateTimedAttacks();
    }
    private void UpdateWalkAnimation()
    {
        if (IsAgentDead)
            return;

        if (pathfinding != null && currentState.name == AgentStateEnum.WALK)
        {
            Vector3 relVelocity = transform.InverseTransformDirection(pathfinding.velocity);
            relVelocity.y = 0f;

            float maxAnimationVelocity = pathfinding.maxSpeed < 5f ? 1f : 2f;

            Anim.SetFloat("walkSpeed", Mathf.Clamp(relVelocity.magnitude / Anim.transform.lossyScale.x, 0f, maxAnimationVelocity));
        }
    }
    private void UpdateAIDestination()
    {
        if (IsAgentDead)
            return;

        if (pathfinding != null)
        {
            List<PriorityGoal> creaturePriorityEnemies = agent.GetAgentViablePriorityEnemies();

            if (creaturePriorityEnemies.Count > 0 && IsSubspawnInsideAreaLimits() && IsAgressive || creaturePriorityEnemies.Count > 0 && !IsAgentASubspawn() && IsAgressive)
            {
                PriorityGoal nearestPriorityEnemy = agent.GetAgentNearestViablePriorityEnemy();

                Agent enemyAgent = nearestPriorityEnemy.goal.GetComponent<Agent>();

                if (enemyAgent == null)
                    enemyAgent = nearestPriorityEnemy.goal.GetComponentInParent<Agent>();
                if (enemyAgent == null)
                    enemyAgent = nearestPriorityEnemy.goal.GetComponentInChildren<Agent>();

                if (enemyAgent != null)
                {
                    pathfinding.destination = GetGoalDestination(enemyAgent.mainCollider.bounds, nearestPriorityEnemy.destination);
                }
                else
                    pathfinding.destination = nearestPriorityEnemy.goal.transform.position;

                agent.ActualGoal = nearestPriorityEnemy.goal;
            }
            else if (agent.MainGoals.Count > 0)
            {
                if (IsAgentASubspawn())
                {
                    PlayableStructure ps = agent.Master.GetComponent<PlayableStructure>();

                    if (ps)
                        pathfinding.destination = ps.GoalFlag.position;
                }
                else
                {
                    Collider goalCollider = agent.MainGoals.First().goal.GetComponent<Collider>();

                    if (goalCollider != null)
                    {
                        pathfinding.destination = GetGoalDestination(goalCollider.bounds, agent.MainGoals.First().destination);
                    }
                    else
                        pathfinding.destination = agent.MainGoals.First().goal.transform.position;
                }

                agent.ActualGoal = agent.MainGoals.First().goal;
            }
        }
    }
    private bool IsAgentASubspawn() { return agent.Master != null; }
    private bool IsSubspawnInsideAreaLimits()
    {
        bool result = false;

        if (agent.Master != null)
        {
            if (Vector3.Distance(agent.Master.transform.position, agent.transform.position)
                <= (
                    (agent.Master.GetComponentInChildren<AgentEnemyColliderManager>(true).DetectionCollider.bounds.extents.magnitude / 2)
                    + (agent.GetComponentInChildren<AgentEnemyColliderManager>(true).DetectionCollider.bounds.extents.magnitude / 2)
                   )
               )
                result = true;
        }

        return result;
    }
    private Vector3 GetGoalDestination(Bounds b, int choice)
    {
        Vector3 result;

        switch (choice)
        {
            case 0:
                result = new Vector3(b.max.x, 0f, b.max.z);
                break;
            case 1:
                result = new Vector3(b.max.x, 0f, b.min.z);
                break;
            case 2:
                result = new Vector3(b.min.x, 0f, b.max.z);
                break;
            default:
                result = b.min;
                break;
        }

        return result;
    }
    private void UpdateFSMStates()
    {
        currentState = currentState.Process();
    }
    private void UpdateAttackCooldown()
    {
        if (IsAgentDead || !IsAgressive)
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
    private void HandleAttacking()
    {
        if (IsAgentDead || !IsAgressive)
            return;

        if (currentState.name == AgentStateEnum.ATTACK)
        {
            if (IsAllAttacksReady())
                isMakingAttack = false;

            for (int i = 0; i < timedAttacks.Length; i++)
            {
                if (!IsAttackInRange(timedAttacks[i].attack))
                {
                    continue;
                }

                if (Time.fixedTime >= (timedAttacks[i].nextTimeToExecute + timedAttacks[i].attack.cooldown))
                {
                    if (!isMakingAttack)
                    {
                        isMakingAttack = true;
                        
                        timedAttacks[i].nextTimeToExecute = CalculateAttackTiming(timedAttacks[i].attack, false);

                        Agent enemyAgent = agent.GetActualEnemyAgent();

                        if (enemyAgent != null)
                            enemyAgent.OnReceiveDamage(agent.Damage, timedAttacks[i].attack);
                    }
                }

                
                if (Time.fixedTime < timedAttacks[i].nextTimeToExecute)
                {
                    UpdateAttackAnimation(timedAttacks[i].attack);
                }
                else
                {
                    isMakingAttack = false;
                }
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
        if (!IsAgressive)
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
