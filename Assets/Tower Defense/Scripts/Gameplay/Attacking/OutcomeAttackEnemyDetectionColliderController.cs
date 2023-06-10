using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using Core.Patterns;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
[HideMonoScript]
public class OutcomeAttackEnemyDetectionColliderController : MonoBehaviour, IPoolable
{
    // Public (Variables) [START]
    [PropertyTooltip("The duration/existance of the collision before it gets disabled")]
    public float duration = 1f;
    [Range(0, 100)]
    [PropertyTooltip("The percentage the calculation will take account on the damage value before passing the value to the enemy")]
    public int damagePercentage = 100;
    // Public (Variables) [END]

    // Private (Variables) [START]
    private List<GameObject> affectedAgents;
    private float startTime;
    // Private (Variables) [END]

    // Public (Properties) [START]
    public bool Finished { get; private set; }
    // Public (Properties) [END]

    // (Unity) Methods [START]
    private void OnEnable()
    {
        ResetVariables();
    }
    private void Update()
    {
        HandleExistanceByTime();
    }
    private void OnTriggerEnter(Collider other) => HandleAttacking(other.gameObject);
    private void OnTriggerStay(Collider other) => HandleAttacking(other.gameObject);
    // (Unity) Methods [END]

    // Private (Methods) [START]
    private void HandleExistanceByTime()
    {
        if (Time.time > (startTime + duration))
        {
            gameObject.SetActive(false);

            Finished = true;
        }
    }
    private void HandleAttacking(GameObject other)
    {
        //MAKE THE ATTACKING LOGIC HERE
        //THE DAMAGE MUST BE DONE ACCORDING TO THE DAMAGE PERCENTAGE CONFIGURED ON THIS COMPONENT
        //RESET ALL LOCAL VARIABLES BEFORE DEACTIVATING
        //DEACTIVATE THE GAMEOBJECT WHEN THE DURATION EXPIRES
        //REFACTOR OUTCOME TO HAVE AN AFFECTOR INSTEAD OF RECEIVING ALL VARIABLES DIRECTLY BY METHOD
        /*
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
                    if (raa.IsAlignmentAnOpponent(enemy.Alignment))
                    {
                        RangedAttackController rac = GetComponent<RangedAttackController>();

                        if (rac != null && !rac.Finished)
                        {
                            if (raa.Attack.outcomePrefab != null)
                            {
                                Attacking.InvokeOutcome(transform.position, raa.Alignment, raa.AffectedsMask, raa.Attack, raa.Damage);
                            }
                            else if (!rac.IsAgentAlreadyAffected(enemy))
                            {
                                rac.AddAffectedAgent(enemy);

                                enemy.OnReceiveDamage(raa.Alignment, raa.Damage, raa.Attack);
                            }

                            rac.Finished = true;
                        }
                    }
                }
            }
        }
        */
    }
    private void ResetVariables()
    {
        if (affectedAgents == null)
            affectedAgents = new List<GameObject>();
        else
            affectedAgents.Clear();

        startTime = Time.time;
    }
    // Private (Methods) [END]

    // Public (Methods) [START]
    public void StartOutcomeCollider()
    {
        gameObject.SetActive(true);
    }
    public virtual void PoolRetrievalAction(Poolable poolable)
    {
        gameObject.SetActive(false);

        ResetVariables();
    }

    public virtual void PoolInsertionAction(Poolable poolable)
    {
        gameObject.SetActive(false);

        ResetVariables();
    }
    // Public (Methods) [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////