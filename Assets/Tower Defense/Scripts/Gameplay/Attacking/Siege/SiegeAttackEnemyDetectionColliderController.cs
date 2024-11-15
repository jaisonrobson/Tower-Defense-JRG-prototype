using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using Core.Patterns;
using Core.General;

[HideMonoScript]
public class SiegeAttackEnemyDetectionColliderController : MonoBehaviour
{
    // (Unity) Private Variables [START]
    private LayerMask affectedMasks;
    // (Unity) Private Variables [END]
    // (Unity) Methods [START]
    private void OnTriggerEnter(Collider other) => HandleAttacking(other.gameObject);
    private void OnTriggerStay(Collider other) => HandleAttacking(other.gameObject);
    private void OnEnable()
    {
        affectedMasks = LayerMask.GetMask("Creature", "Structure", "Ground");
    }
    // (Unity) Methods [END]

    // Private (Methods) [START]
    private void HandleAttacking(GameObject other)
    {
        if (other != gameObject)
        {
            
            SiegeAttackAffector raa = GetComponent<SiegeAttackAffector>();
            SiegeAttackController rac = GetComponent<SiegeAttackController>();
            
            if (!IsSelfAgentColliding(other))
            {
                if (raa != null && Utils.IsInLayerMask(other.layer, affectedMasks))
                {
                    if (rac != null && !rac.Finished)
                    {
                        if (raa.Attack.outcomePrefab != null)
                        {
                            Vector3 attackDirection = (raa.Origin - transform.position).normalized;
                            Quaternion localInitialRotation = Quaternion.LookRotation(attackDirection);
                            localInitialRotation = Quaternion.Euler(0, localInitialRotation.eulerAngles.y, 0);

                            Animating.InvokeAnimation(raa.Attack.finalAnimation, transform.position, localInitialRotation, raa.Duration);
                            AudioPlaying.InvokeSound(raa.Attack.finalSound, transform.position);



                            Attacking.InvokeOutcome(raa.Invoker, transform.position, attackDirection, raa.Alignment, raa.AffectedMasks, raa.Attack, raa.Damage);
                        }

                        rac.Finished = true;
                    }
                }
            }            
        }
    }
    private bool IsSelfAgentColliding(GameObject gobj) => Utils.IsGameObjectInsideAnother(gobj.transform, GetComponent<SiegeAttackAffector>().Invoker.transform);
    // Private (Methods) [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////