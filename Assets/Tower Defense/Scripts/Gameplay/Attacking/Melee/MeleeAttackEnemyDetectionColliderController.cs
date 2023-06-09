using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using Core.Patterns;
using Core.General;

[HideMonoScript]
public class MeleeAttackEnemyDetectionColliderController : MonoBehaviour
{
    // (Unity) Methods [START]
    private void OnTriggerEnter(Collider other) => HandleAttacking(other.gameObject);
    private void OnTriggerStay(Collider other) => HandleAttacking(other.gameObject);
    // (Unity) Methods [END]


    // Private (Methods) [START]
    private void HandleAttacking(GameObject other)
    {
        if (other != gameObject)
        {
            MeleeAttackAffector maa = GetComponent<MeleeAttackAffector>();

            if (maa != null && maa.IsInLayerMask(other.layer))
            {
                Agent enemy = other.GetComponentInChildren<Agent>();

                if(enemy == null)
                    enemy = other.GetComponentInParent<Agent>();

                if (enemy != null)
                {
                    if (maa.IsAlignmentAnOpponent(enemy.Alignment))
                    {
                        MeleeAttackController mac = GetComponent<MeleeAttackController>();

                        if (mac != null && !mac.IsAgentAlreadyAffected(enemy))
                        {
                            mac.AddAffectedAgent(enemy);

                            enemy.OnReceiveDamage(maa.Alignment, maa.Damage, maa.Attack);
                        }
                    }
                }
            }
        }
    }
    // Private (Methods) [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////
