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

        switch (statusAffector.status.status)
        {
            case StatusEnum.FREEZE:
                GameObject newStatusAffector = Poolable.TryGetPoolable(
                    statusAffector.prefab,
                    (Poolable pNewStatusAffectorPoolable) =>
                    {
                        pNewStatusAffectorPoolable.GetComponent<FreezeStatusAffector>().Invoker = invoker;
                        pNewStatusAffectorPoolable.GetComponent<FreezeStatusAffector>().Target = target;
                        pNewStatusAffectorPoolable.GetComponent<FreezeStatusAffector>().Alignment = invoker.Alignment;
                        pNewStatusAffectorPoolable.GetComponent<FreezeStatusAffector>().UpdateAgentStats();
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