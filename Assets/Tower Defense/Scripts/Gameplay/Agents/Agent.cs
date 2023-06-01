using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pathfinding;
using Sirenix.OdinInspector;
using Core.Math;
using Core.Patterns;
using Core.Physics;
using Core.General;
public struct PriorityGoal
{
    public bool ignoreBattle;
    public int destination;
    public Agent goal;
}

public struct MainGoal
{
    public int destination;
    public Agent goal;
}

[System.Serializable]
public struct SubSpawn
{
    public SubSpawnSO subSpawn;
    public float timeToNextSpawn;
    public List<GameObject> spawnedAgents;

    public SubSpawn(SubSpawnSO pSubSpawn, float pTimeToNextSpawn = 0f)
    {
        spawnedAgents = new List<GameObject>();
        timeToNextSpawn = pTimeToNextSpawn;
        subSpawn = pSubSpawn;
    }
}

[HideMonoScript]
[RequireComponent(typeof(AgentUI))]
[RequireComponent(typeof(Poolable))]
[RequireComponent(typeof(Selectable))]
[RequireComponent(typeof(AgentEnemyDetectionCreator))]
public abstract class Agent : MonoBehaviour
{
    // Public (Variables) [START]    
    [BoxGroup("Agent Identity")]
    [Required]
    public Collider mainCollider;
    [TitleGroup("Agent Identity/Goals", Order = 99)]
    public AgentGoalEnum goal;
    // Public (Variables) [END]

    // Private (Variables) [START]
    [BoxGroup("Agent Identity")]
    [PropertyOrder(-1)]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    private AlignmentEnum alignment;
    [BoxGroup("Agent Identity")]
    [PropertyOrder(-1)]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    private float actualHealth = 0f;
    private float maxHealth = 0f;
    private float damage = 0f;
    private float velocity = 0f;
    private float attackVelocity = 0f;
    private float experienceToEvolve = 0f;
    private float actualExperience = 0f;
    private float visibilityArea = 0f;
    private float attackRange = 0f;
    private Vector2Int evasion = new Vector2Int(0, 0);
    [BoxGroup("Agent Identity")]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    private Agent master;
    /// <summary>
    /// The main goals are determined by the goals enum which are interpreted on the start function
    /// </summary>
    [TitleGroup("Agent Identity/Goals")]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    private MainGoal[] mainGoals = new MainGoal[0];
    /// <summary>
    /// Priority goals are enemy agents and structures found in the path of this agent.
    /// </summary>
    [TitleGroup("Agent Identity/Goals")]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    private PriorityGoal[] priorityGoals = new PriorityGoal[0];
    [BoxGroup("Agent Identity")]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    private List<SubSpawn> subSpawns;
    // Private (Variables) [END]

    // Protected (Variables) [START]
    [TitleGroup("Agent Identity/Goals")]
    [PropertyOrder(-1)]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    protected Agent actualGoal;
    // Protected (Variables) [END]

    // Public (Properties) [START]
    public float ActualHealth { get { return actualHealth; } }
    public float MaxHealth { get { return maxHealth; } }
    public float Damage { get { return damage; } }
    public float Velocity { get { return velocity; } }
    public float AttackVelocity { get { return attackVelocity; } }
    public float ExperienceToEvolve { get { return experienceToEvolve; } }
    public float ActualExperience { get { return actualExperience; } }
    public float VisibilityArea { get { return visibilityArea; } }
    public float AttackRange { get { return attackRange; } }
    public Vector2Int Evasion { get { return evasion; } }
    public Agent ActualGoal { get { FilterActualGoal(); return actualGoal; } set { actualGoal = value; } }
    public AlignmentEnum Alignment { get { return alignment; } set { alignment = value; } }
    public List<SubSpawn> SubSpawns { get { return subSpawns; } }
    public List<PriorityGoal> PriorityGoals { get { FilterPriorityGoals(); return priorityGoals.ToList(); } }
    public List<MainGoal> MainGoals { get { FilterMainGoals(); return mainGoals.ToList(); } }
    public Agent Master { get { return master; } set { master = value; } }
    public bool IsDead { get { return actualHealth <= 0f; } }
    // Public (Properties) [END]

    // (Unity) Methods [START]
    protected virtual void Start()
    {
    }
    protected virtual void Update()
    {
        HandleSubSpawning();
    }
    // (Unity) Methods [END]

    // Private (Methods) [START]    
    private void HandleSubSpawning()
    {
        if (SubSpawns.Count > 0 && alignment != AlignmentEnum.GENERIC)
        {
            if (GetAgent().type == AgentTypeEnum.STRUCTURE && GetComponent<PlayableStructure>() != null)
                if (!GetComponent<PlayableStructure>().IsPlaced)
                    return;

            bool modified = false;

            SubSpawn[] subs = SubSpawns.ToArray();
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
                subSpawns = subs.ToList();
        }
    }
    private Vector3 GetAgentColliderBoundsInitialPosition(Transform newAgent)
    {
        return new Vector3(mainCollider.bounds.min.x - (mainCollider.bounds.extents.x * 0.5f), newAgent.localScale.y / 2, mainCollider.bounds.min.z - (mainCollider.bounds.extents.z * 0.5f));
    }
    private GameObject SubSpawnAgent(GameObject agentPrefab)
    {
        GameObject newAgent = Poolable.TryGetPoolable(agentPrefab, OnRetrieveSubSpawnPoolableAgent);

        newAgent.gameObject.GetComponent<AIPath>().Teleport(GetAgentColliderBoundsInitialPosition(newAgent.transform));

        return newAgent;
    }
    public void OnRetrieveSubSpawnPoolableAgent(Poolable agent)
    {
        agent.transform.SetPositionAndRotation(GetAgentColliderBoundsInitialPosition(agent.transform), transform.rotation);
        agent.gameObject.GetComponent<Agent>().goal = GetAgent().type == AgentTypeEnum.CREATURE ? AgentGoalEnum.MASTER : AgentGoalEnum.FLAG;
        agent.gameObject.GetComponent<Agent>().Alignment = alignment;
        agent.gameObject.GetComponent<Agent>().Master = this;
    }
    public void OnInsertSubSpawnPoolableAgent(Poolable agent)
    {
        SubSpawns.ForEach(ss => {
            if (ss.spawnedAgents.Any(sa => sa == agent.gameObject))
                ss.spawnedAgents.Remove(agent.gameObject);
        });
    }
    private void ResetAgentStats()
    {
        actualHealth = GetAgent().health;
        maxHealth = GetAgent().health;
        damage = GetAgent().damage;
        velocity = GetAgent().velocity;
        attackVelocity = GetAgent().attackVelocity;
        experienceToEvolve = GetAgent().experienceToEvolve;
        actualExperience = 0f;
        visibilityArea = GetAgent().visibilityArea;
        attackRange = GetAgent().attackRange;
        evasion = GetAgent().evasion;
        subSpawns = new List<SubSpawn>(GetAgent().subspawns.ToList().Select(ssSO => new SubSpawn(ssSO)).ToList());
    }
    private void ResetMainGoals()
    {
        switch (goal)
        {
            case AgentGoalEnum.CORESTRUCTURES:
                AddMainGoal(MapManager.instance.catalystAsset);
                AddMainGoal(MapManager.instance.generatorAsset);
                AddMainGoal(MapManager.instance.sourceAsset);

                break;
            case AgentGoalEnum.FLAG://Agents subspawned by structure Agents
                AddMainGoal(Master);
                break;
            case AgentGoalEnum.MASTER://Agents subspawned by creature agents
                break;
            case AgentGoalEnum.DEFEND://Structures
                break;
        }
    }
    private void FilterActualGoal()
    {
        if (actualGoal != null && !actualGoal.gameObject.activeInHierarchy)
            actualGoal = null;
    }
    private void FilterMainGoals()
    {
        mainGoals = mainGoals.ToList().Where(mg => mg.goal.gameObject.activeInHierarchy == true && !mg.goal.IsDead).ToArray();
    }
    private void FilterPriorityGoals()
    {
        priorityGoals = priorityGoals.ToList().Where(pg => pg.goal.gameObject.activeInHierarchy == true && !pg.goal.IsDead).ToArray();
    }
    // Private (Methods) [END]

    // Protected (Methods) [START]
    public virtual void HandleDeath()
    {
        if (ActualHealth <= 0f)
        {
            if (Master != null)
                Master.OnInsertSubSpawnPoolableAgent(GetComponent<Poolable>());

            Poolable.TryPool(gameObject);
        }
    }
    // Protected (Methods) [END]

    // Public (Methods) [START]
    public abstract AgentSO GetAgent();
    public List<PriorityGoal> GetAgentViablePriorityEnemies() { return PriorityGoals.Where(pg => pg.ignoreBattle == false).ToList(); }
    public PriorityGoal GetAgentNearestViablePriorityEnemy() {
        float lastDistance = float.PositiveInfinity;

        List<PriorityGoal> viablePriorities = GetAgentViablePriorityEnemies();

        return viablePriorities.Aggregate(
            viablePriorities.First(),
            (nearest, next) => {
                float actualDistance = Vector3.Distance(next.goal.transform.position, transform.position);

                if (actualDistance < lastDistance)
                {
                    lastDistance = actualDistance;
                    return next;
                }

                return nearest;
            }
        );
    }
    public bool IsMainGoal(Agent targetAgent) { return mainGoals.Any(mg => mg.goal == targetAgent); }
    public bool IsPriorityGoal(Agent targetAgent) { return PriorityGoals.Any(pg => pg.goal == targetAgent); }
    public int GenerateRandomDestination() { return RNG.Int(0, 4); }
    public void AddPriorityGoal(Agent goal)
    {
        PriorityGoal newPriorityGoal;
        newPriorityGoal.goal = goal;
        newPriorityGoal.destination = GenerateRandomDestination();
        newPriorityGoal.ignoreBattle = TryToEvade();

        priorityGoals = priorityGoals.Concat(new[] { newPriorityGoal }).ToArray();
    }
    public void RemovePriorityGoal(Agent goal)
    {
        List<PriorityGoal> lp = priorityGoals.ToList();

        lp.RemoveAll(pg => pg.goal == goal);

        priorityGoals = lp.ToArray();
    }
    public void AddMainGoal(Agent goal)
    {
        MainGoal newMainGoal;
        newMainGoal.goal = goal;
        newMainGoal.destination = GenerateRandomDestination();

        mainGoals = mainGoals.Concat(new[] { newMainGoal }).ToArray();
    }
    public void RemoveMainGoal(Agent goal)
    {
        List<MainGoal> lmg = mainGoals.ToList();

        lmg.RemoveAll(mg => mg.goal == goal);

        mainGoals = lmg.ToArray();
    }
    public int GenerateEvasionChance() { return RNG.Int(evasion.x, evasion.y); }
    public bool TryToEvade() { return RNG.Int(0, 100) < GenerateEvasionChance(); }
    public bool OnReceiveDamage(Agent dealer, AttackSO dealerAttack)
    {
        bool result = false;

        if (!AlignmentManager.instance.IsAlignmentAnOpponent(dealer.alignment, alignment))
            return result;

        float rawValue = dealer.Damage;

        if (!TryToEvade() && rawValue > 0f)
        {
            float finalValue;

            finalValue = dealerAttack.damage.CalculateFormulaFromValue(rawValue);
            finalValue *= WeaknessesManager.instance.GetWeakness(dealerAttack.nature, GetAgent().nature);

            actualHealth = Mathf.Clamp(actualHealth - finalValue, 0f, MaxHealth);

            GetComponent<AgentUI>().GenerateFloatingText(-finalValue);

            result = true;
        }

        return result;
    }
    public Agent GetActualEnemyAgent()
    {
        Agent result = null;

        if (ActualGoal != null)
        {
            result = ActualGoal.GetComponent<Agent>();
            if (result == null)
                result = ActualGoal.GetComponentInChildren<Agent>();
            if (result == null)
                result = ActualGoal.GetComponentInParent<Agent>();
        }

        return result;
    }
    public float GetDistanceBetweenAgentAndEnemy() {
        Agent enemy = GetActualEnemyAgent();

        Vector3 enemyClosestPoint = enemy.mainCollider.ClosestPointOnBounds(transform.position);
        Vector3 agentClosesPoint = mainCollider.ClosestPointOnBounds(enemyClosestPoint);

        if (enemy != null)
            return Vector3.Distance(enemyClosestPoint, agentClosesPoint);
        else
            return float.PositiveInfinity;
    }
    public virtual void PoolRetrievalAction(Poolable poolable)
    {
        priorityGoals = new PriorityGoal[0];
        mainGoals = new MainGoal[0];
        ResetAgentStats();
        ResetMainGoals();
        actualGoal = null;

        if (mainCollider != null)
            mainCollider.enabled = true;

        if (GetComponent<AgentFsmAi>() != null)
            GetComponent<AgentFsmAi>().PoolRetrievalAction(poolable);
    }
    public virtual void PoolInsertionAction(Poolable poolable)
    {
        GetComponent<AgentUI>().TryPool();
        alignment = AlignmentEnum.GENERIC;
        subSpawns.ForEach(ss => ss.spawnedAgents.ForEach(sa => Poolable.TryPool(sa)));
        subSpawns.Clear();

        if (GetComponent<AIPath>() != null)
            GetComponent<AIPath>().enabled = false;

        GetComponentInChildren<ReliableOnTriggerExit>(true)?.PoolInsertionAction();
    }
    // Public (Methods) [END]
}
