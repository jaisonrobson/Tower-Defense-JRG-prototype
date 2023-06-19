using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[ManageableData]
public class StatusAffectorSO : BaseOptionDataSO
{
    [BoxGroup("Box1", ShowLabel = false)]
    [Required]
    public StatusSO status;

    [BoxGroup("Box1")]
    [Required]
    [PropertyTooltip("The total duration of the affector.")]
    [PropertyRange(1, 180)]
    public int duration = 1;

    [BoxGroup("Box1")]
    [Required]
    [PropertyTooltip("The damage percentage each turn will deal to the affected.")]
    [PropertyRange(0, 100)]
    public int damage = 0;

    [BoxGroup("Box1")]
    [Required]
    [PropertyTooltip("The number of turns will be divided by the total duration equally.")]
    [PropertyRange(1, 50)]
    public int turnsQuantity = 1;

    [BoxGroup("Box1")]
    [PropertyTooltip("Any kind of special condition that needs to be met by the status.")]
    [PropertyRange(0, 50)]
    public int specialCondition = 0;
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////