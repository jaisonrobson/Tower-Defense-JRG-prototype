using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pathfinding;
using Sirenix.OdinInspector;

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