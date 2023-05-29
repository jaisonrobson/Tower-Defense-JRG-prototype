using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Core.Patterns;
using System.Linq;

[HideMonoScript]
public class MapManager : Singleton<MapManager>
{
    public MapSO map;

    [PropertySpace(5f)]
    [Title("Map Structures")]
    [Required]
    [AssetsOnly]
    public Agent catalystAsset;
    [Required]
    [SceneObjectsOnly]
    public Transform catalystHook;

    [PropertySpace(10f)]
    [Required]
    [AssetsOnly]
    public Agent generatorAsset;
    [Required]
    [SceneObjectsOnly]
    public Transform generatorHook;

    [PropertySpace(10f)]
    [Required]
    [AssetsOnly]
    public Agent sourceAsset;
    [Required]
    [SceneObjectsOnly]
    public Transform sourceHook;

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
        catalystAsset = Poolable.TryGetPoolable(catalystAsset.gameObject).GetComponent<Agent>();
        catalystAsset.transform.SetPositionAndRotation(catalystHook.position, catalystHook.rotation);

        generatorAsset = Poolable.TryGetPoolable(generatorAsset.gameObject).GetComponent<Agent>();
        generatorAsset.transform.SetPositionAndRotation(generatorHook.position, generatorHook.rotation);

        sourceAsset = Poolable.TryGetPoolable(sourceAsset.gameObject).GetComponent<Agent>();
        sourceAsset.transform.SetPositionAndRotation(sourceHook.position, sourceHook.rotation);
    }
    // Private Methods [END]

    // Public Methods [START]
    public List<AlignmentOpponentsSO> GetNonPlayableMapTeams()
    {
        return map.alignmentsOpponents.Where(ao => ao.alignment.alignment != map.playerAlignment.alignment).ToList();
    }
    // Public Methods [END]
}
