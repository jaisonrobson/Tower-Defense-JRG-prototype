using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Core.Patterns;

public static class Attacking
{
	public static void InvokeAttack(AttackSO attack, Agent invoker, Agent target)
    {
        if (attack == null || invoker == null || target == null)
            return;

        GameObject newAttack = Poolable.TryGetPoolable(
            attack.prefab,
            (Poolable pNewAttackPoolable) => {
                pNewAttackPoolable.gameObject.GetComponent<Affector>().Alignment = invoker.Alignment;
                pNewAttackPoolable.gameObject.GetComponent<AttackAffector>().Attack = attack;
                pNewAttackPoolable.gameObject.GetComponent<AttackAffector>().Duration = invoker.CalculateAttackVelocity(attack);

                switch (attack.type)
                {
                    case AttackTypeEnum.RANGED:
                        pNewAttackPoolable.gameObject.GetComponent<RangedAttackAffector>().Origin = invoker.GetAttackOriginOfAttack(attack).attackOrigin.position;
                        pNewAttackPoolable.gameObject.GetComponent<RangedAttackAffector>().Destination = target.transform.position;
                        break;
                    case AttackTypeEnum.MELEE:
                        Vector3 localOrigin = invoker.GetAttackOriginOfAttack(attack).attackOrigin.position;
                        Vector3 direction = (localOrigin - target.transform.position).normalized;

                        pNewAttackPoolable.gameObject.GetComponent<MeleeAttackAffector>().Origin = localOrigin;
                        pNewAttackPoolable.gameObject.GetComponent<MeleeAttackAffector>().InitialRotation = Quaternion.LookRotation(direction);
                        break;
                    case AttackTypeEnum.IMMEDIATE:
                        //CRIAR O IMMEDIATE AQUI
                        break;
                }
            }
        );
    }

}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////