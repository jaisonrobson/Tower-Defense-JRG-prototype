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
                Poolable.TryGetPoolable(
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
            case StatusEnum.BURN:
                Poolable.TryGetPoolable(
                    statusAffector.prefab,
                    (Poolable pNewStatusAffectorPoolable) =>
                    {
                        pNewStatusAffectorPoolable.GetComponent<BurnStatusAffector>().Invoker = invoker;
                        pNewStatusAffectorPoolable.GetComponent<BurnStatusAffector>().Target = target;
                        pNewStatusAffectorPoolable.GetComponent<BurnStatusAffector>().Alignment = invoker.Alignment;
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