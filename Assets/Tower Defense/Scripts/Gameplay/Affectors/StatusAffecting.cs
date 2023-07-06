using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Core.Patterns;

public static class StatusAffecting
{
    public static void InvokeStatus(StatusAffectorSO statusAffector, Agent invoker, Agent target)
    {
        if (statusAffector == null || invoker == null || target == null)
            return;

        if (target.IsStructure)
            return;

        switch (statusAffector.status.status)
        {
            case StatusEnum.FREEZE:
            case StatusEnum.BURN:
            case StatusEnum.PARALYZE:
            case StatusEnum.DROWN:
            case StatusEnum.CONFUSION:
            case StatusEnum.POISON:
            case StatusEnum.ASLEEP:
            case StatusEnum.GROUNDED:
            case StatusEnum.HEALBLOCK:
                Poolable.TryGetPoolable(
                    statusAffector.prefab,
                    (Poolable pNewStatusAffectorPoolable) =>
                    {
                        pNewStatusAffectorPoolable.GetComponent<StatusAffector>().Invoker = invoker;
                        pNewStatusAffectorPoolable.GetComponent<StatusAffector>().Target = target;
                        pNewStatusAffectorPoolable.GetComponent<StatusAffector>().Alignment = invoker.Alignment;
                    }
                );
                break;
            default:
                break;
        }
    }
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////