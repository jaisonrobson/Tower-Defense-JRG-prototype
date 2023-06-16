using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[ManageableData]
public class StatusAffectorSO : BaseOptionDataSO
{
    [BoxGroup("Box1", ShowLabel = false)]
    [HorizontalGroup("Box1", LabelWidth = 75)]

    [VerticalGroup("Box1")]
    [Required]
    public StatusSO status;

    //FAZER TODAS AS POSSIVEIS VARIAVEIS PARA CONFIGURACOES AQUI DENTRO
    // COMO POR EX: DANO, DURACAO, TURNOS, ETC.
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////