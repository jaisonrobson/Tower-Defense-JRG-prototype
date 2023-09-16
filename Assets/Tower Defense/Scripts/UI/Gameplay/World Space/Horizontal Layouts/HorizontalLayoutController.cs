using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using TheraBytes.BetterUi;

[HideMonoScript]
public class HorizontalLayoutController : WorldspaceInterfaceObjectController
{
    // Public (Variables) [START]
    [Min(1)]
    public int maxElements = 5;
    [Min(0)]
    public float spacingSize = 0.5f;
    [Min(1)]
    public float elementSize = 2f;
    [Required]
    [AssetsOnly]
    public GameObject horizontalLayoutElement;
    // Public (Variables) [END]

    // Protected (Variables) [START]
    protected List<GameObject> spawnedElements;
    // Protected (Variables) [END]

    // (Unity) Methods [START]
    protected override void Start()
    {
        base.Start();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        InitializeVariables();
    }

    protected override void Update()
    {
        base.Update();
    }
    // (Unity) Methods [END]


    // Private (Methods) [START]
    private void InitializeVariables()
    {
        InitializeSpawnedElementsList();
        RefreshLayoutSize();
    }
    private void InitializeSpawnedElementsList()
    {
        if (spawnedElements == null)
            spawnedElements = new List<GameObject>();
        else
        {
            spawnedElements.ForEach(element => Destroy(element));

            spawnedElements.Clear();
        }

        spawnedElements.AddRange(Enumerable.Range(0, maxElements).Select(element => {
            GameObject newElement = Instantiate(horizontalLayoutElement);

            newElement.transform.SetParent(transform);

            RecalculateLayoutElementSizeValues(newElement);

            return newElement;
        }));

        spawnedElements.ForEach(el => el.SetActive(false));
    }
    // Private (Methods) [END]

    // Protected (Methods) [START]
    protected void RefreshLayoutSize()
    {
        GetComponent<BetterAxisAlignedLayoutGroup>().spacing = spacingSize;

        Rect rect = GetComponent<RectTransform>().rect;

        rect.Set(
            rect.x,
            rect.y,
            (elementSize * maxElements) + (spacingSize * (maxElements - 1)),
            rect.height
        );
    }
    protected void RecalculateLayoutElementSizeValues(GameObject element)
    {
        element.GetComponent<BetterLayoutElement>().minWidth = elementSize;
        element.GetComponent<BetterLayoutElement>().MinWidthSizer.OptimizedSize = elementSize;
        element.GetComponent<BetterLayoutElement>().preferredWidth = elementSize;
        element.GetComponent<BetterLayoutElement>().PreferredWidthSizer.OptimizedSize = elementSize;

        element.GetComponent<BetterLayoutElement>().MinWidthSizer.OverrideLastCalculatedSize(elementSize);
        element.GetComponent<BetterLayoutElement>().PreferredWidthSizer.OverrideLastCalculatedSize(elementSize);
    }
    // Protected (Methods) [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////