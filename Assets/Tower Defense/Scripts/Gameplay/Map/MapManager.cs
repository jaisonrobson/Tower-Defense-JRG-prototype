using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Core.Patterns;
using System.Linq;

[HideMonoScript]
public class MapManager : Singleton<MapManager>
{
    // Private (Variables) [START]
    private GameObject catalyst;
    private GameObject generator;
    private GameObject source;
    // Private (Variables) [END]

    // Public (Variables) [START]
    public MapSO map;

    [PropertySpace(5f)]
    [Title("Map Structures")]
    [Required]
    [AssetsOnly]
    public AgentSO catalystAsset;
    [Required]
    [SceneObjectsOnly]
    public Transform catalystHook;

    [PropertySpace(10f)]
    [Required]
    [AssetsOnly]
    public AgentSO generatorAsset;
    [Required]
    [SceneObjectsOnly]
    public Transform generatorHook;

    [PropertySpace(10f)]
    [Required]
    [AssetsOnly]
    public AgentSO sourceAsset;
    [Required]
    [SceneObjectsOnly]
    public Transform sourceHook;
    // Public (Variables) [END]

    // Public (Properties) [START]
    public bool IsAnyMainStructureAlive { get { return IsStructureAlive(catalyst) || IsStructureAlive(generator) || IsStructureAlive(source); } }
    public GameObject Catalyst { get { return catalyst; } }
    public GameObject Generator { get { return generator; } }
    public GameObject Source { get { return source; } }
    // Public (Properties) [END]

    // Unity Methods [START]
    void OnEnable()
    {
        InitializeMap();
        InitializeStructures();
    }
    // Unity Methods [END]


    // Private Methods [START]
    private void InitializeMap()
    {
        string selectedMapName = PlayerPrefs.GetString("selectedMapName");
        MapSO selectedMap = map;

        MapSO[] maps = Resources.LoadAll<MapSO>("SO's/Maps");

        foreach (MapSO mp in maps)
        {
            if (mp.name == selectedMapName)
            {
                selectedMap = mp;
            }
        }

        if (selectedMap == null)
        {
            Debug.LogError("[MapManager] Selected map was not found");
        }
    }
    private void InitializeStructures()
    {
        catalyst = Poolable.TryGetPoolable(catalystAsset.prefab);
        catalyst.transform.SetPositionAndRotation(catalystHook.position, catalystHook.rotation);

        generator = Poolable.TryGetPoolable(generatorAsset.prefab);
        generator.transform.SetPositionAndRotation(generatorHook.position, generatorHook.rotation);

        source = Poolable.TryGetPoolable(sourceAsset.prefab);
        source.transform.SetPositionAndRotation(sourceHook.position, sourceHook.rotation);
    }
    // Private Methods [END]

    // Public Methods [START]
    public List<AlignmentOpponentsSO> GetNonPlayableMapTeams()
    {
        return map.alignmentsOpponents.Where(ao => ao.alignment.alignment != map.playerAlignment.alignment).ToList();
    }
    public bool IsStructureAlive(GameObject pStructure) => pStructure.activeInHierarchy && pStructure.activeSelf;
    // Public Methods [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////