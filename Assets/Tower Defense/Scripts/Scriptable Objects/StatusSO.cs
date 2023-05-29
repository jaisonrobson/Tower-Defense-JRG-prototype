using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[ManageableData]
public class StatusSO : BaseOptionDataSO
{
    [BoxGroup("Box1", ShowLabel = false)]
    [HorizontalGroup("Box1/split", LabelWidth = 75)]

    [VerticalGroup("Box1/split/left")]    
    [Required]
    [PreviewField(100, Alignment = ObjectFieldAlignment.Center)]
    [HideLabel]
    public Sprite image;

    [VerticalGroup("Box1/split/right")]
    [Required]
    public new string name;

    [VerticalGroup("Box1/split/right")]
    public string description;
}