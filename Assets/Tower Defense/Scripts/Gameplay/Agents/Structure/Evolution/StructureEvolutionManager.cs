using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Core.Patterns;
using Sirenix.OdinInspector;

[System.Serializable]
public struct SubSpawnPositioning
{
    public AgentSO subSpawn;
    public List<Vector3> agentsPositions;

    public SubSpawnPositioning(AgentSO pSubSpawn)
    {
        agentsPositions = new List<Vector3>();
        subSpawn = pSubSpawn;
    }
}

[RequireComponent(typeof(StructureEvolutionController))]
[HideMonoScript]
public class StructureEvolutionManager : Singleton<StructureEvolutionManager>
{
    // Private (Variables) [START]
    [BoxGroup("Last Structure Informations")]
    [PropertyOrder(-1)]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    private PlayableStructure playableStructure;
    [BoxGroup("Last Structure Informations")]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    private AgentSO agentSO;
    [BoxGroup("Last Structure Informations")]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    private Vector3 position = new Vector3(0, 0, 0);
    [BoxGroup("Last Structure Informations")]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    private List<SubSpawnPositioning> subspawnsPositions = new List<SubSpawnPositioning>();
    [BoxGroup("Last Structure Informations")]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    private Vector3 flagPosition = new Vector3(0, 0, 0);
    [BoxGroup("Last Structure Informations")]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    private int lifePercentage = 100;
    // Private (Variables) [END]

    // Public (Properties) [START]
    public PlayableStructure PlayableStructure { get { return playableStructure; } }
    public AgentSO AgentSO { get { return agentSO; } }
    public Vector3 Position { get { return position; } }
    public List<SubSpawnPositioning> SubspawnsPositions { get { return subspawnsPositions; } }
    public Vector3 FlagPosition { get { return flagPosition; } }
    public int LifePercentage { get { return lifePercentage; } }
    // Public (Properties) [END]

    // Private (Methods) [START]
    private void EvolveStructure(PlayableStructure pPlayableStructure)
    {
        SubspawnsPositions.Clear();

        playableStructure = pPlayableStructure;
        agentSO = pPlayableStructure.agent;
        position = pPlayableStructure.transform.position;

        pPlayableStructure.SubSpawns.ForEach(
            ss => {
                SubSpawnPositioning sp = new SubSpawnPositioning(ss.subSpawn.creature);

                ss.spawnedAgents.ForEach(
                    sa => sp.agentsPositions.Add(sa.transform.position)
                );

                subspawnsPositions.Add(sp);
            }
        );

        flagPosition = pPlayableStructure.GoalFlag.position;
        lifePercentage = (int) ((pPlayableStructure.ActualHealth * 100) / pPlayableStructure.MaxHealth);

        StructureEvolutionController.instance.EvolveStructure();
    }
    // Private (Methods) [END]


    // Public (Methods) [START]
    public void EvolveStructure()
    {
        PlayableStructure ps = SelectionManager.instance.SelectedAgents.First()?.GetComponent<PlayableStructure>();
        if (ps != null)
            EvolveStructure(ps);

        SelectionManager.instance.RemoveSelectable(ps.GetComponent<Selectable>());
    }
    // Public (Methods) [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////