    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;

[ManageableData]
public class AnimationSO : BaseOptionDataSO
{
    [BoxGroup("Box1", showLabel: false)]
    [PropertyTooltip("The prefab with particle animations or anything else that does the animation.")]
    [Required]
    public GameObject prefab;

    [PropertyRange(0f, 10f)]
    [BoxGroup("Box1")]
    [PropertyTooltip("Time until the animation starts.")]
    public float delay = 0f;

    [VerticalGroup("vertical", PaddingTop = 5f)]

    [BoxGroup("vertical/Box2", LabelText = "Optional")]
    [PropertyTooltip("If not null, this chained animation is to be called after the execution of this own.")]
    public AnimationSO chainedAnimation;
}