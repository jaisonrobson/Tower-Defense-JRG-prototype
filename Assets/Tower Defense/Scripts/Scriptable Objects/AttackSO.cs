using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[ManageableData]
public class AttackSO : BaseOptionDataSO
{
    [VerticalGroup("Identity", PaddingTop = 5f)]

    [BoxGroup("Identity/Box", LabelText = "Identity")]
    [Required]
    public NatureSO nature;

    [BoxGroup("Identity/Box")]
    [Required]
    public FormulaSO damage;

    [BoxGroup("Identity/Box")]
    [Required]
    [Min(0f)]
    public float cooldown = 1f;

    [BoxGroup("Identity/Box")]
    [Required]
    [ProgressBar(10, 100, R = 0.3f, G = 0.8f, B = 1)]
    public int influenceOverAttackVelocity = 50;

    [BoxGroup("Identity/Box")]
    [Required]
    [ProgressBar(1.5f, 50f, R = 1f, G = 0.3f, B = 1)]
    public float minimumAttackDistance = 1.5f;

    [VerticalGroup("Animations&Sounds", PaddingTop = 7f)]
    [HorizontalGroup("Animations&Sounds/split", LabelWidth = 125, PaddingRight = 10)]

    [BoxGroup("Animations&Sounds/split/leftBox", LabelText = "Animations")]
    [PropertyTooltip("Animation when the attack spawns\n\nTo be used along with sounds")]
    [Required]
    public AnimationSO initialAnimation;

    [BoxGroup("Animations&Sounds/split/leftBox")]
    [PropertyTooltip("Animation when the attack hits the target\n\nTo be used along with sounds")]
    [Required]
    public AnimationSO finalAnimation;

    [BoxGroup("Animations&Sounds/split/rightBox", LabelText = "Sounds")]
    [PropertyTooltip("Sound when the attack spawns\n\nTo be used along with animations")]
    [Required]
    public SoundSO initialSound;

    [BoxGroup("Animations&Sounds/split/rightBox")]
    [PropertyTooltip("Sound when the attack hits the target\n\nTo be used along with animations")]
    [Required]
    public SoundSO finalSound;

    [VerticalGroup("Optional", PaddingTop = 10f)]

    [BoxGroup("Optional/Box", LabelText = "Optional")]
    [PropertyTooltip("Represents the projectile physical model prefab.\n\nCan be null as it is optional.")]
    public GameObject projectile;

    [BoxGroup("Optional/Box", LabelText = "Optional")]
    [PropertyTooltip("Represents the trail animation prefab.\n\nCan be used along with projectile or anything else.\n\nCan be null as it is optional.")]
    public GameObject trail;
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////