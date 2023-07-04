using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pathfinding;
using Sirenix.OdinInspector;
using Core.Math;

public class CreatureFsmAi : AgentFsmAi
{
    // Private (Variables) [START]
    private Creature creature;
    // Private (Variables) [END]

    // Protected (Variables) [START]
    [BoxGroup("Agent FSM Identity")]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    protected AIPath pathfinding;
    // Protected (Variables) [END]

    // Public (Properties) [START]
    public bool IsCreatureParalyzed { get { return agent.IsAgentUnderStatusParalyze; } }
    public bool IsCreatureSleeping { get { return agent.IsAgentUnderStatusAsleep; } }
    public bool IsCreatureGrounded { get { return agent.IsAgentUnderStatusGrounded; } }
    public bool IsCreatureTaunted { get { return agent.IsAgentUnderStatusTaunt; } }
    public bool IsCreatureDrowning { get { return agent.IsAgentUnderStatusDrown; } }
    public bool IsCreatureConfused { get { return agent.IsAgentUnderStatusConfusion; } }
    // Public (Properties) [END]

    // (Unity) Methods [START]
    protected override void OnEnable()
    {
        base.OnEnable();
        creature = agent.GetComponent<Creature>();

        pathfinding = AgentGOBJ.GetComponent<AIPath>();
        pathfinding.destination = transform.position;

        currentState = new FSMStateCreatureIdle(Anim, creature, pathfinding);
    }
    protected override void Update()
    {
        base.Update();

        UpdateWalkAnimation();

        UpdateAIGoalAndDestination();

        UpdateAIPathfindingMinimumDistance();

        HandleSubspawnInsidePlayableArea();
    }
    // (Unity) Methods [END]

    // Private (Methods) [START]
    private void UpdateWalkAnimation()
    {
        if (IsAgentDead || !IsMovable)
            return;

        if (pathfinding != null && currentState.name == AgentStateEnum.WALK)
        {
            Vector3 relVelocity = transform.InverseTransformDirection(pathfinding.velocity);
            relVelocity.y = 0f;

            float maxAnimationVelocity = pathfinding.maxSpeed < 5f ? 1f : 2f;

            Anim.SetFloat("walkSpeed", Mathf.Clamp(relVelocity.magnitude / Anim.transform.lossyScale.x, 0f, maxAnimationVelocity));
        }
    }
    private void UpdateAIGoalAndDestination()
    {
        if (IsAgentDead)
            return;

        if (IsCreatureParalyzed || IsCreatureDrowning || IsCreatureSleeping)
        {
            agent.ActualGoal = null;

            return;
        }

        if (IsCreatureConfused) //MOVER ESSA SECAO DE CODIGO PARA DENTRO DO CONFUSION STATUS AFFECTOR
        {
            agent.ActualGoal = agent;

            if (RNG.Int(0, 100) > 95)
            {
                Vector3 randomNewPosition = RNG.Vector3(agent.transform.position, 1f, 3f);

                randomNewPosition.y = agent.transform.position.y;

                pathfinding.destination = randomNewPosition;
            }

            return;
        }

        if (pathfinding != null)
        {
            List<PriorityGoal> creaturePriorityEnemies = agent.GetAgentViablePriorityEnemies();

            if (creaturePriorityEnemies.Count > 0 && IsSubspawnInsideAreaLimits() && IsAggressive || creaturePriorityEnemies.Count > 0 && !IsAgentASubspawn() && IsAggressive)
            {
                PriorityGoal nearestPriorityEnemy = agent.GetAgentNearestViablePriorityEnemy();

                Agent enemyAgent = nearestPriorityEnemy.goal.GetComponent<Agent>();

                if (enemyAgent == null)
                    enemyAgent = nearestPriorityEnemy.goal.GetComponentInParent<Agent>();
                if (enemyAgent == null)
                    enemyAgent = nearestPriorityEnemy.goal.GetComponentInChildren<Agent>();

                if (IsMovable)
                {
                    if (enemyAgent != null)
                    {
                        pathfinding.destination = GetGoalDestination(enemyAgent.mainCollider.bounds, nearestPriorityEnemy.destination);
                    }
                    else
                        pathfinding.destination = nearestPriorityEnemy.goal.transform.position;
                }

                if (IsCreatureGrounded)
                    pathfinding.destination = agent.transform.position;

                agent.ActualGoal = nearestPriorityEnemy.goal;
            }
            else if (agent.MainGoals.Count > 0)
            {
                if (IsMovable)
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
                }

                if (IsCreatureGrounded)
                    pathfinding.destination = agent.transform.position;

                agent.ActualGoal = agent.MainGoals.First().goal;
            }
        }
    }
    private void UpdateAIPathfindingMinimumDistance()
    {
        if (IsAgentDead)
            return;

        if (pathfinding != null)
        {
            TimedAttack nearestAttack = GetNearestNotInCooldownAttack();

            if (nearestAttack.attack != null)
                pathfinding.endReachedDistance = nearestAttack.attack.minimumAttackDistance * 0.9f;
        }
    }
    private void HandleSubspawnInsidePlayableArea()
    {
        if (IsAgentASubspawn())
        {
            if (!IsSubspawnInsideAreaLimits(10f))
            {
                agent.transform.position = agent.Master.GetComponent<PlayableStructure>().GoalFlag.position;

                pathfinding.Teleport(agent.Master.GetComponent<PlayableStructure>().GoalFlag.position);
            }
        }
    }
    private bool IsAgentASubspawn() { return agent.Master != null; }
    private bool IsSubspawnInsideAreaLimits(float limitOffset = 0f)
    {
        bool result = false;

        if (IsAgentASubspawn())
        {
            if (Vector3.Distance(agent.Master.transform.position, agent.transform.position)
                <= (
                    (
                        (agent.Master.GetComponentInChildren<AgentEnemyDetectionColliderManager>(true).DetectionCollider.bounds.extents.magnitude / 2)
                        + (agent.GetComponentInChildren<AgentEnemyDetectionColliderManager>(true).DetectionCollider.bounds.extents.magnitude / 2)
                    )
                    + limitOffset
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
    // Private (Methods) [END]

    // Protected (Methods) [START]
    protected override void GoToIdleState()
    {
        currentState = new FSMStateCreatureIdle(Anim, creature, pathfinding);
    }
    // Protected (Methods) [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////