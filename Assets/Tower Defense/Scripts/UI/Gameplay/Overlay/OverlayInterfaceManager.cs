using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Core.Patterns;
using Sirenix.OdinInspector;

[RequireComponent(typeof(OverlayInterfaceController))]
[HideMonoScript]
public class OverlayInterfaceManager : Singleton<OverlayInterfaceManager>
{
    // Public (Variables) [START]
    [Required]
    [SceneObjectsOnly]
    public Canvas canvas;
    [BoxGroup("Interface Objects")]
    [Title("Panel 2")]
    [Required]
    [SceneObjectsOnly]
    public GameObject panel_2_1;
    [BoxGroup("Interface Objects")]
    [Required]
    [SceneObjectsOnly]
    public GameObject panel_2_2;
    [BoxGroup("Interface Objects")]
    [Title("Panel 3")]
    [Required]
    [SceneObjectsOnly]
    public GameObject panel_3_2_1;
    // Public (Variables) [END]


    // Public (Methods) [START]
    public bool IsOverUI()
    {
        PointerEventData m_PointerEventData = new PointerEventData(EventSystem.current);
        m_PointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();

        canvas.GetComponent<GraphicRaycaster>().Raycast(m_PointerEventData, results);

        return results.Count > 0;
    }
    public void OpenStructureEvolutionPanel()
    {
        panel_3_2_1.SetActive(true);
    }
    // Public (Methods) [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////