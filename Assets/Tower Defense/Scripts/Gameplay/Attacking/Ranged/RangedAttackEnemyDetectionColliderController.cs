using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using Core.Patterns;

[HideMonoScript]
public class RangedAttackEnemyDetectionColliderController : MonoBehaviour
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
            RangedAttackAffector raa = GetComponent<RangedAttackAffector>();

            if (raa != null && raa.IsInLayerMask(other.layer))
            {
                Agent enemy = other.GetComponentInChildren<Agent>();

                if (enemy == null)
                    enemy = other.GetComponentInParent<Agent>();

                if (enemy != null)
                {
                    if (raa.IsAlignmentAnOpponent(enemy.Alignment) || enemy.IsAgentUnderStatusConfusion)
                    {
                        RangedAttackController rac = GetComponent<RangedAttackController>();

                        if (rac != null && !rac.Finished)
                        {
                            if (raa.Attack.outcomePrefab != null)
                            {
                                Attacking.InvokeOutcome(raa.Invoker, transform.position, transform.forward, raa.Alignment, raa.AffectedMasks, raa.Attack, raa.Damage);
                            }
                            else if (!rac.IsAgentAlreadyAffected(enemy))
                            {
                                rac.AddAffectedAgent(enemy);

                                enemy.OnReceiveDamageByDirectAttack(raa.Alignment, raa.Damage, raa.Attack, raa.Invoker);

                                raa.Attack.onHitProbabilityStatusAffectors
                                    .ToList()
                                    .ForEach(sap => StatusAffecting.TryInvokeStatus(sap, raa.Invoker, enemy));

                                Quaternion localInitialRotation = Quaternion.LookRotation((raa.Origin - enemy.transform.position).normalized);
                                localInitialRotation = Quaternion.Euler(0, localInitialRotation.eulerAngles.y, 0);

                                Animating.InvokeAnimation(raa.Attack.finalAnimation, enemy.transform.position, localInitialRotation, raa.Duration);
                                AudioPlaying.InvokeSound(raa.Attack.finalSound, enemy.transform.position);
                            }

                            rac.Finished = true;
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