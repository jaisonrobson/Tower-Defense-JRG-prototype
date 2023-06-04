using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Slider))]
[RequireComponent(typeof(Billboard))]
public class SliderController : MonoBehaviour
{
    // Private (Variables) [START]
    private Slider slider;
    private Image fill;
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    private float value = 1f;
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    private float maxValue = 1f;
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    private float minValue = 0;
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    private Color color = new Color32(255, 0, 0, 255);
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    private Transform targetToFollow;
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    private float targetHeightOffset = 0f;
    // Private (Variables) [END]

    // Public (Properties) [START]
    public float Value { get { return value; }  set { this.value = value >= minValue && value <= maxValue ? value : this.value; } }
    public float MaxValue { get { return maxValue; } set { maxValue = value; } }
    public float MinValue { get { return minValue; } set { minValue = value; } }
    public Color Color { get { return color; } set { color = value; } }
    // Public (Properties) [END]

    // (Unity) Methods [START]
    void Start()
    {
        slider = GetComponent<Slider>();
        fill = slider.fillRect.GetComponent<Image>();
    }
    void Update()
    {
        slider.minValue = minValue;
        slider.maxValue = maxValue;
        slider.value = value;
        fill.color = color;
        transform.position = new Vector3(targetToFollow.position.x, targetToFollow.position.y + targetHeightOffset, targetToFollow.position.z);
    }
    // (Unity) Methods [END]

    // Public (Methods) [START]
    public void SetTarget(Transform target) { targetToFollow = target; }
    public void SetTargetHeightOffset(float offset) { targetHeightOffset = offset + 1f; }
    // Public (Methods) [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////