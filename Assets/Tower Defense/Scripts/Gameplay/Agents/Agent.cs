using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Pathfinding;
using Sirenix.OdinInspector;
using Core.Math;
using Core.Patterns;
using Core.Physics;
using Core.General;
using DestroyIt;
using System.Runtime.Serialization;
using Sirenix.Utilities;
using Unity.VisualScripting;

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

[Serializable]
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

[Serializable]
[PropertyTooltip("This structure can represent the attacks and animations origins and also the destionation, depends of what property is using it.")]
public struct AttackPositioning
{
    public AttackSO attack;
    [ChildGameObjectsOnly]
    public Transform attackPosition;
    [ChildGameObjectsOnly]
    public Transform animationPosition;
    [ReadOnly]
    public int identityCounter; // This variable validates the quantity that the same attack is present in the agent, ex: 4 bottles of toxin be throwing at 4 different destination directions (same attack happening 4 times at once). every individual toxin must be a different instance of the same attack at the array, this counter manages it.

    public AttackPositioning(AttackSO pAttack, int pIdentityCounter = 0)
    {
        attack = pAttack;
        attackPosition = null;
        animationPosition = null;
        identityCounter = pIdentityCounter;
    }
}

[Serializable]
public struct LastAttackPositioningMade
{
    public AttackSO attack;
    public int lastPositioningIdentity;

    public LastAttackPositioningMade(AttackSO pAttack, int pLastPositioningIdentity)
    {
        attack = pAttack;
        lastPositioningIdentity = pLastPositioningIdentity;
    }
}

[Serializable]
public struct LastAnimationPositioningMade
{
    public AttackSO attack;
    public int lastPositioningIdentity;

    public LastAnimationPositioningMade(AttackSO pAttack, int pLastPositioningIdentity)
    {
        attack = pAttack;
        lastPositioningIdentity = pLastPositioningIdentity;
    }
}

[HideMonoScript]
[RequireComponent(typeof(AgentUI))]
[RequireComponent(typeof(Poolable))]
[RequireComponent(typeof(Selectable))]
[RequireComponent(typeof(AgentEnemyDetectionCreator))]
[RequireComponent(typeof(AgentSelectionDisplayCreator))]
public abstract class Agent : MonoBehaviour, IPoolable
{
    // Public (Variables) [START]    
    [BoxGroup("Agent Identity")]
    [TitleGroup("Agent Identity/Main Information", Order = 1)]
    [PropertyOrder(2)]
    [Required]
    public Collider mainCollider;
    [TitleGroup("Agent Identity/Goals", Order = 99)]
    [PropertyOrder(1)]
    public AgentGoalEnum goal;
    [TitleGroup("Agent Identity/Attacking", Order = 4)]
    [PropertyOrder(1)]
    [OnInspectorInit("MaintainAttacksOrigin")]
    [ValidateInput("Validate_NotNull_Origins", "Animation or Attack origin cannot be null.")]
    public List<AttackPositioning> attacksOrigins;
    [TitleGroup("Agent Identity/Attacking", Order = 4)]
    [PropertyOrder(1)]
    [OnInspectorInit("MaintainAttacksDestinations")]
    [ValidateInput("Validate_NotNull_Destinations", "Animation or Attack destination cannot be null.")]
    public List<AttackPositioning> attacksDestinations;
    public UnityAction onReceiveDamageAction;
    // Public (Variables) [END]

    // Private (Variables) [START]
    private bool firstInitialization = true;
    [TitleGroup("Agent Identity/Main Information")]
    [PropertyOrder(-1)]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    private AlignmentEnum alignment;
    [TitleGroup("Agent Identity/Stats", Order = 2)]
    [PropertyOrder(3)]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    [LabelText("Health")]
    [GUIColor(1f, 0.5411765f, 0.5411765f, 1f)]
    [ProgressBar(0f, 100f, MaxGetter = "MaxHealth", R = 1f, G = 0.54f, B = 0.54f)]
    private float actualHealth = 0f;
    private float maxHealth = 0f;
    [TitleGroup("Agent Identity/Stats")]
    [PropertyOrder(1)]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    [GUIColor(0.7215f, 0.5137f, 0.9215f, 1f)]
    private float damage = 0f;
    [TitleGroup("Agent Identity/Stats")]
    [PropertyOrder(4)]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    [ProgressBar(3f, 10f, 0f, 0.8f, 0f)]
    [GUIColor(0f, 0.8f, 0f, 1f)]
    private float velocity = 0f;
    [TitleGroup("Agent Identity/Stats")]
    [PropertyOrder(5)]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    [ProgressBar(0.3f, 5f, 0.3f, 0.8f, 1f)]
    [GUIColor(0.3f, 0.8f, 1f, 1f)]
    private float attackVelocity = 0f;
    private int experienceToEvolve = 0;
    [TitleGroup("Agent Identity/Stats")]
    [PropertyOrder(2)]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    [LabelText("Experience")]
    [GUIColor(1f, 0.8f, 0.4f, 1f)]
    [ProgressBar(0, 100, MaxGetter = "ExperienceToEvolve", R = 1f, G = 0.8f, B = 0.4f)]
    private int actualExperience = 0;
    [TitleGroup("Agent Identity/Stats")]
    [PropertyOrder(6)]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    [ProgressBar(3f, 50f, 0.8f, 0.8f, 0.8f)]
    [GUIColor(0.8f, 0.8f, 0.8f, 1f)]
    private float visibilityArea = 0f;
    [TitleGroup("Agent Identity/Stats")]
    [PropertyOrder(7)]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    [ProgressBar(1.5f, 50f, 1f, 0.3f, 1f)]
    [GUIColor(1f, 0.3f, 1f, 1f)]
    private float attackRange = 0f;
    [TitleGroup("Agent Identity/Stats")]
    [PropertyOrder(8)]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    [GUIColor(0.61f, 0.73f, 0.33f, 1f)]
    [MinMaxSlider(0, 100)]
    private Vector2Int evasion = new Vector2Int(0, 0);
    [TitleGroup("Agent Identity/Spawning", Order = 5)]
    [PropertyOrder(1)]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    private Agent master;
    /// <summary>
    /// The main goals are determined by the goals enum which are interpreted on the start function
    /// </summary>
    [TitleGroup("Agent Identity/Goals")]
    [PropertyOrder(3)]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    private MainGoal[] mainGoals = new MainGoal[0];
    /// <summary>
    /// Priority goals are enemy agents and structures found in the path of this agent.
    /// </summary>
    [TitleGroup("Agent Identity/Goals")]
    [PropertyOrder(4)]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    private PriorityGoal[] priorityGoals = new PriorityGoal[0];
    [TitleGroup("Agent Identity/Spawning")]
    [PropertyOrder(3)]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    private List<SubSpawn> subSpawns;
    [TitleGroup("Agent Identity/Affectors", Order = 3)]
    [PropertyOrder(9)]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    private List<StatusAffector> affectingStatuses;
    [TitleGroup("Agent Identity/Affectors")]
    [PropertyOrder(10)]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    private List<bool> isMovementPrevented;
    [TitleGroup("Agent Identity/Affectors")]
    [PropertyOrder(11)]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    private List<bool> isAttackPrevented;
    [TitleGroup("Agent Identity/Affectors")]
    [PropertyOrder(12)]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    private List<LastAttackPositioningMade> lastAttackPositioningsMade;
    [TitleGroup("Agent Identity/Affectors")]
    [PropertyOrder(13)]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    private List<LastAttackPositioningMade> lastAnimationPositioningsMade;
    // Private (Variables) [END]

    // Protected (Variables) [START]
    [TitleGroup("Agent Identity/Goals")]
    [PropertyOrder(2)]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    protected Agent actualGoal;
    // Protected (Variables) [END]

    // Public (Properties) [START]
    public float ActualHealth { get { return actualHealth; } set { actualHealth = (value >= 0 && value <= maxHealth) ? value : actualHealth; } }
    public float MaxHealth { get { return maxHealth; } }
    public float Damage { get { return damage; } }
    public float Velocity { get { return velocity; } }
    public float AttackVelocity { get { return attackVelocity; } }
    public int ExperienceToEvolve { get { return experienceToEvolve; } }
    public float ActualExperience { get { return actualExperience; } }
    public float VisibilityArea { get { return visibilityArea; } }
    public float AttackRange { get { return attackRange; } }
    public Vector2Int Evasion { get { return evasion; } }
    public Agent ActualGoal { get { FilterActualGoal(); return actualGoal; } set { actualGoal = value; } }
    public AlignmentEnum Alignment { get { return alignment; } set { alignment = value; } }
    public List<SubSpawn> SubSpawns { get { return subSpawns; } set { subSpawns = value; } }
    public List<PriorityGoal> PriorityGoals { get { FilterPriorityGoals(); return priorityGoals.ToList(); } }
    public List<MainGoal> MainGoals { get { FilterMainGoals(); return mainGoals.ToList(); } }
    public Agent Master { get { return master; } set { master = value; } }
    public bool IsDead { get { return actualHealth <= 0f; } }
    public bool IsMovementPrevented { get { return isMovementPrevented.Count > 0; } }
    public bool IsAttackPrevented { get { return isAttackPrevented.Count > 0; } }
    public bool IsAgentUnderStatusParalyze { get { return affectingStatuses.Any(sa => sa.statusAffectorSO.status.status == StatusEnum.PARALYZE); } }
    public bool IsAgentUnderStatusDrown { get { return affectingStatuses.Any(sa => sa.statusAffectorSO.status.status == StatusEnum.DROWN); } }
    public bool IsAgentUnderStatusConfusion { get { return affectingStatuses.Any(sa => sa.statusAffectorSO.status.status == StatusEnum.CONFUSION); } }
    public bool IsAgentUnderStatusAsleep { get { return affectingStatuses.Any(sa => sa.statusAffectorSO.status.status == StatusEnum.ASLEEP); } }
    public bool IsAgentUnderStatusGrounded { get { return affectingStatuses.Any(sa => sa.statusAffectorSO.status.status == StatusEnum.GROUNDED); } }
    public bool IsAgentUnderStatusHealblock { get { return affectingStatuses.Any(sa => sa.statusAffectorSO.status.status == StatusEnum.HEALBLOCK); } }
    public bool IsAgentUnderStatusTaunt { get { return affectingStatuses.Any(sa => sa.statusAffectorSO.status.status == StatusEnum.TAUNT); } }
    public bool IsCreature { get { return GetAgent().type == AgentTypeEnum.CREATURE; } }
    public bool IsStructure { get { return GetAgent().type == AgentTypeEnum.STRUCTURE; } }
    public int OnDieExperience { get { return actualExperience + GetAgent().experienceOnDie; } }
    public bool CanEvolve { get { return actualExperience == experienceToEvolve && GetAgent() != null && GetAgent().evolutionTree != null && GetAgent().evolutionTree.Count > 0; } }
    public List<StatusEnum> NotAffectingStatuses { get { return Enumerable.Range(0, Enum.GetValues(typeof(StatusEnum)).Length).ToList().Where(status => affectingStatuses.All(sa => sa.statusAffectorSO.status.status != (StatusEnum)status)).Select(element => (StatusEnum)element).ToList(); } }
    public List<StatusEnum> AffectingStatuses { get { return affectingStatuses.Select(sa => sa.statusAffectorSO.status.status).ToList(); } }
    // Public (Properties) [END]

    // (Unity) Methods [START]
    protected virtual void OnEnable()
    {
        if (firstInitialization)
        {
            PoolRetrievalAction(GetComponent<Poolable>());

            firstInitialization = false;
        }
    }
    protected virtual void Start()
    {
    }
    protected virtual void Update()
    {
    }
    // (Unity) Methods [END]

    // Private (Methods) [START]
    private void ResetAgentStats()
    {
        actualHealth = GetAgent().health;
        maxHealth = GetAgent().health;
        damage = GetAgent().damage;
        velocity = GetAgent().velocity;
        attackVelocity = GetAgent().attackVelocity;
        experienceToEvolve = GetAgent().experienceToEvolve;
        actualExperience = 0;
        visibilityArea = GetAgent().visibilityArea;
        attackRange = GetAgent().attackRange;
        evasion = GetAgent().evasion;
        subSpawns = new List<SubSpawn>(GetAgent().subspawns.ToList().Select(ssSO => new SubSpawn(ssSO)).ToList());

        if (affectingStatuses == null)
            affectingStatuses = new List<StatusAffector>();
        else
            affectingStatuses.Clear();
    }
    private void ResetMainGoals()
    {
        switch (goal)
        {
            case AgentGoalEnum.CORESTRUCTURES:
                MapManager.instance.PlayerMainEntities.ForEach(pme => AddMainGoal(pme.GetComponent<Agent>()));

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
    private void ResetPreventionLists()
    {
        if (isMovementPrevented != null)
            isMovementPrevented.Clear();
        else
            isMovementPrevented = new List<bool>();

        if (isAttackPrevented != null)
            isAttackPrevented.Clear();
        else
            isAttackPrevented = new List<bool>();
    }
    private void ResetLastAttackPositioningsMade()
    {
        if (lastAttackPositioningsMade == null)
            lastAttackPositioningsMade = new List<LastAttackPositioningMade>();
        else
        {
            lastAttackPositioningsMade.Clear();

            AgentSO aso = GetAgent();

            aso.attacks.Distinct().ToList().ForEach(at => {
                lastAttackPositioningsMade.Add(new LastAttackPositioningMade(at, 1));
            });
        }
    }
    private void ResetLastAnimationPositioningsMade()
    {
        if (lastAnimationPositioningsMade == null)
            lastAnimationPositioningsMade = new List<LastAttackPositioningMade>();
        else
        {
            lastAnimationPositioningsMade.Clear();

            AgentSO aso = GetAgent();

            aso.attacks.Distinct().ToList().ForEach(at => {
                lastAnimationPositioningsMade.Add(new LastAttackPositioningMade(at, 1));
            });
        }
    }
    private void ResetIndependentAssets()
    {
        ResetDestructibles();
    }
    private void RefreshDestructibles()
    {
        List<Destructible> destructibles = GetComponentsInChildren<Destructible>().ToList();

        destructibles.ForEach(dt =>
        {
            dt.TotalHitPoints = MaxHealth;
            dt.CurrentHitPoints = ActualHealth;
        });
    }
    private void ResetDestructibles()
    {
        List<Destructible> destructibles = GetComponentsInChildren<Destructible>(true).ToList();

        destructibles.ForEach(dt =>
        {
            dt.gameObject.layer = LayerMask.NameToLayer("Structure");
            dt.TotalHitPoints = MaxHealth;
            dt.CurrentHitPoints = MaxHealth;

            dt.destroyedPrefabParent
                .GetComponentsInChildren<Transform>(true)
                .Where(t => t != dt.destroyedPrefabParent.transform)
                .ToList()
                .ForEach(debrisToDestroy => Destroy(debrisToDestroy.gameObject));

            dt.gameObject.SetActive(true);
            dt.ResetDamageLevel();
        });
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
    private void OnReceiveDamage(float value, Agent pDealer)
    {
        float healthBefore = actualHealth;

        actualHealth = Mathf.Clamp(actualHealth - value, 0f, MaxHealth);

        if (Mathf.Equals(actualHealth, 0f) && healthBefore > 0f)
            pDealer?.OnReceiveExperience(OnDieExperience);

        GetComponent<AgentUI>().GenerateFloatingText(-value);

        onReceiveDamageAction?.Invoke();

        RefreshDestructibles();
    }
    // Private (Methods) [END]

    // Public (Methods) [START]
    public void DoSpawnFXs()
    {
        Animating.InvokeAnimation(GetAgent().animations.GetValueOrDefault("spawn"), transform.position, transform.rotation);
        AudioPlaying.InvokeSound(GetAgent().sounds.GetValueOrDefault("spawn"), transform.position);
    }
    public void DoDeathFXs(float pDuration = 1f)
    {
        Animating.InvokeAnimation(GetAgent().animations.GetValueOrDefault("death"), transform.position, transform.rotation, pDuration);
        AudioPlaying.InvokeSound(GetAgent().sounds.GetValueOrDefault("death"), transform.position);
    }
    public void DoEvolutionFXs()
    {
        if (GetAgent().animations.GetValueOrDefault("evolution") != null)
            Animating.InvokeAnimation(GetAgent().animations.GetValueOrDefault("evolution"), transform.position, transform.rotation);

        if (GetAgent().sounds.GetValueOrDefault("evolution") != null)
            AudioPlaying.InvokeSound(GetAgent().sounds.GetValueOrDefault("evolution"), transform.position);
    }
    public void AddMovementPrevention() => isMovementPrevented.Add(true);
    public void RemoveMovementPrevention()
    {
        if (isMovementPrevented?.Count > 0)
            isMovementPrevented?.RemoveAt(0);
    }
    public void AddAttackPrevention() => isAttackPrevented.Add(true);
    public void RemoveAttackPrevention()
    {
        if (isAttackPrevented?.Count > 0)
            isAttackPrevented?.RemoveAt(0);
    }
    public void UpdateAgentVelocity(float newVelocity) => velocity = newVelocity;
    public void UpdateAgentAttackVelocity(float newAttackVelocity) => attackVelocity = newAttackVelocity;
    public bool IsStatusAlreadyAffectingAgent(StatusAffector sa) => affectingStatuses.Contains(sa) || affectingStatuses.Any(afs => afs.statusAffectorSO.status == sa.statusAffectorSO.status);
    public bool IsStatusAlreadyAffectingAgent(StatusAffectorSO sa) => affectingStatuses.Any(afs => afs.statusAffectorSO.status == sa.status);
    public bool IsAgentImmuneToStatus(StatusEnum status) => GetAgent().statusImmunities.Any(si => si.status == status);
    public void AddAffectingStatus(StatusAffector sa) => affectingStatuses.Add(sa);
    public void RemoveAffectingStatus(StatusAffector sa) => affectingStatuses.Remove(sa);
    public abstract AgentSO GetAgent();
    public void OnInsertSubSpawnPoolableAgent(Poolable agent)
    {
        SubSpawns.ForEach(ss => {
            if (ss.spawnedAgents.Any(sa => sa == agent.gameObject))
                ss.spawnedAgents.Remove(agent.gameObject);
        });
    }
    public void PoolAgent()
    {
        if (Master != null)
            Master.OnInsertSubSpawnPoolableAgent(GetComponent<Poolable>());

        Poolable.TryPool(gameObject);
    }
    public virtual void HandleDeath()
    {
        if (ActualHealth <= 0f)
        {
            PoolAgent();
        }
    }
    public Vector3 GetAgentColliderBoundsInitialPosition(Transform newAgent)
    {
        return new Vector3(mainCollider.bounds.min.x - (mainCollider.bounds.extents.x * 0.5f), newAgent.localScale.y + 0.5f, mainCollider.bounds.min.z - (mainCollider.bounds.extents.z * 0.5f));
    }
    public float CalculateAttackVelocityPerSecond(AttackSO pAttack) => 1f / CalculateAttackVelocity(pAttack);
    public float CalculateAttackVelocity(AttackSO pAttack) =>
        Mathf.Clamp(
            (AttackVelocity * ((float)pAttack.influenceOverAttackVelocity / 100)),
            0.03f,
            5f
        );
    public AttackPositioning GetActualAttackOrigin(AttackSO pAttack) => attacksOrigins.Where(ao => ao.attack == pAttack && ao.identityCounter == lastAttackPositioningsMade.Find(lao => lao.attack == ao.attack).lastPositioningIdentity).First();
    public AttackPositioning GetActualAnimationOrigin(AttackSO pAttack) => attacksOrigins.Where(ao => ao.attack == pAttack && ao.identityCounter == lastAnimationPositioningsMade.Find(lao => lao.attack == ao.attack).lastPositioningIdentity).First();
    public AttackPositioning GetActualAttackDestination(AttackSO pAttack) => attacksDestinations.Where(ao => ao.attack == pAttack && ao.identityCounter == lastAttackPositioningsMade.Find(lao => lao.attack == ao.attack).lastPositioningIdentity).First();
    public AttackPositioning GetActualAnimationDestination(AttackSO pAttack) => attacksDestinations.Where(ao => ao.attack == pAttack && ao.identityCounter == lastAnimationPositioningsMade.Find(lao => lao.attack == ao.attack).lastPositioningIdentity).First();
    public void FinishActualAttack(AttackSO pAttack)
    {
        AgentSO aso = GetAgent();

        lastAttackPositioningsMade = Utils.UpdateValueInStructList(lastAttackPositioningsMade, lapm => {
            if (lapm.attack == pAttack)
                if (lapm.lastPositioningIdentity == aso.attacks.Where(atk => atk == lapm.attack).ToList().Count)
                    lapm.lastPositioningIdentity = 1;
                else
                    lapm.lastPositioningIdentity++;

            return lapm;
        }).ToList();

        lastAnimationPositioningsMade = Utils.UpdateValueInStructList(lastAnimationPositioningsMade, lapm => {
            if (lapm.attack == pAttack)
                if (lapm.lastPositioningIdentity == aso.attacks.Where(atk => atk == lapm.attack).ToList().Count)
                    lapm.lastPositioningIdentity = 1;
                else
                    lapm.lastPositioningIdentity++;

            return lapm;
        }).ToList();
    }
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
        newPriorityGoal.ignoreBattle = GetAgent().isPlayable && TryToEvade();

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
    public bool OnReceiveDamageByDirectAttack(AlignmentEnum dealerAlignment, float dealerDamage, AttackSO dealerAttack, Agent pDealer)
    {
        bool result = false;

        if (!AlignmentManager.instance.IsAlignmentAnOpponent(dealerAlignment, alignment) && !IsAgentUnderStatusConfusion)
            return result;

        float rawValue = dealerDamage;

        if ((!TryToEvade() || IsAgentUnderStatusConfusion) && rawValue > 0f)
        {
            float finalValue;

            finalValue = dealerAttack.formula.CalculateFormulaFromValue(rawValue);
            finalValue *= WeaknessesManager.instance.GetWeakness(dealerAttack.nature, GetAgent().nature);

            OnReceiveDamage(finalValue, pDealer);

            result = true;
        }

        return result;
    }
    public bool OnReceiveDamageByStatus(AlignmentEnum dealerAlignment, float dealerDamage, StatusAffectorSO dealerStatus, Agent pDealer)
    {
        bool result = false;

        if (!AlignmentManager.instance.IsAlignmentAnOpponent(dealerAlignment, alignment) && !IsAgentUnderStatusConfusion)
            return result;

        float rawValue = dealerDamage;

        if (rawValue > 0f)
        {
            float finalValue;

            finalValue = rawValue;
            finalValue *= WeaknessesManager.instance.GetWeakness(dealerStatus.status.nature, GetAgent().nature);

            OnReceiveDamage(finalValue, pDealer);

            result = true;
        }

        return result;
    }
    public void OnReceiveExperience(int newExperience)
    {
        if (Master != null)
            Master?.OnReceiveExperience(newExperience);
        else
        {
            int experienceToReceive = newExperience;

            if (Alignment == AlignmentManager.instance.PlayerAlignment.alignment)
            {
                experienceToReceive = Mathf.FloorToInt(experienceToReceive / 2);

                experienceToReceive = experienceToReceive == 0 ? 1 : experienceToReceive;

                PlayerManager.instance.IncreasePoints(experienceToReceive);
            }

            actualExperience = Mathf.Clamp(actualExperience + experienceToReceive, actualExperience, ExperienceToEvolve);
        }
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
    public virtual void PoolRetrievalAction(Poolable poolable)
    {
        priorityGoals = new PriorityGoal[0];
        mainGoals = new MainGoal[0];
        ResetAgentStats();
        ResetMainGoals();
        ResetPreventionLists();
        ResetLastAttackPositioningsMade();
        ResetLastAnimationPositioningsMade();
        actualGoal = null;
        onReceiveDamageAction = null;

        if (mainCollider != null)
            mainCollider.enabled = true;

        if (GetComponent<AgentFsmAi>() != null)
            GetComponent<AgentFsmAi>().PoolRetrievalAction(poolable);

        ResetIndependentAssets();
    }
    public virtual void PoolInsertionAction(Poolable poolable)
    {
        GetComponent<AgentUI>().TryPool();
        
        subSpawns.ForEach(ss => ss.spawnedAgents.ForEach(sa => Poolable.TryPool(sa)));
        subSpawns.Clear();

        if (GetComponent<AIPath>() != null)
            GetComponent<AIPath>().enabled = false;

        GetComponentInChildren<ReliableOnTriggerExit>(true)?.PoolInsertionAction();

        alignment = AlignmentEnum.GENERIC;
    }
    // Public (Methods) [END]

    // (Validation) Methods [START]
    private void MaintainAttacksOrigin()
    {
        if (attacksOrigins == null)
            attacksOrigins = new List<AttackPositioning>();

        AgentSO agent = GetAgent();

        if (agent != null)
        {
            agent.attacks.ForEach(a =>
            {
                List<AttackPositioning> aos = attacksOrigins.Where(ao => ao.attack == a).ToList();
                int timesAttackAppear = aos.Count;
                int sameAttackRepetitionsCount = agent.attacks.FindAll(atk => atk == a).Count;
                List<int> missingNumbers = Enumerable.Range(1, sameAttackRepetitionsCount).Where(num => !aos.Any(ao => ao.identityCounter == num)).ToList();
                int identityNumber = missingNumbers.Count > 0 ? missingNumbers.ElementAt(0) : sameAttackRepetitionsCount;


                if (timesAttackAppear < sameAttackRepetitionsCount)
                    attacksOrigins.Add(new AttackPositioning(a, identityNumber));
            });
        }
    }
    private bool Validate_NotNull_Origins() {
        MaintainAttacksOrigin();

        return attacksOrigins != null && attacksOrigins.Count > 0 ? !attacksOrigins.Any(ao => ao.attackPosition == null || ao.animationPosition == null) : true;
    }
    private void MaintainAttacksDestinations()
    {
        if (attacksDestinations == null)
            attacksDestinations = new List<AttackPositioning>();

        AgentSO agent = GetAgent();

        if (agent != null)
        {
            // ONLY SIEGE ATTACKS NEEDS DESTINATION COORDINATES at the moment.
            agent.attacks.Where(a => a.type == AttackTypeEnum.SIEGE).ToList().ForEach(a =>
            {
                List<AttackPositioning> ads = attacksDestinations.Where(ad => ad.attack == a).ToList();
                int timesAttackAppear = ads.Count;
                int sameAttackRepetitionsCount = agent.attacks.FindAll(atk => atk == a).Count;
                List<int> missingNumbers = Enumerable.Range(1, sameAttackRepetitionsCount).Where(num => !ads.Any(ad => ad.identityCounter == num)).ToList();
                int identityNumber = missingNumbers.Count > 0 ? missingNumbers.ElementAt(0) : sameAttackRepetitionsCount;

                if (timesAttackAppear < sameAttackRepetitionsCount)
                    attacksDestinations.Add(new AttackPositioning(a, identityNumber));
            });
        }
    }
    private bool Validate_NotNull_Destinations()
    {
        MaintainAttacksDestinations();

        return attacksDestinations != null && attacksDestinations.Count > 0 ? !attacksDestinations.Any(ao => ao.attackPosition == null || ao.animationPosition == null) : true;
    }
    // (Validation) Methods [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////