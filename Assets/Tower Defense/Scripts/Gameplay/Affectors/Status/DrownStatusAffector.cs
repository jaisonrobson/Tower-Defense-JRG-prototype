using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using Pathfinding;

[HideMonoScript]
public class DrownStatusAffector : StatusAffector
{
    // Private (Variables) [START]
    private float minimumYPosition = 0f;
    private float maximumYPosition = 0f;
    private float startAnimationTime = 0f;
    // Private (Variables) [END]

    // Private (Properties) [START]
    private float BC_AttackVelocity { get; set; }
    private float BC_Velocity { get; set; }
    // Private (Properties) [END]

    // Private (Methods) [START]
    private void ResetProperties()
    {
        BC_Velocity = 0f;
        BC_AttackVelocity = 0f;
        minimumYPosition = 0f;
        maximumYPosition = 0f;
        startAnimationTime = 0f;
    }
    // Private (Methods) [END]

    // (Unity) Methods [START]
    protected override void Update()
    {
        base.Update();

        if (Target != null && Target.GetComponent<AIPath>() != null && !Target.IsDead)
        {
            Vector3 newPos = Target.transform.position;

            float t = (Time.time - startAnimationTime) / 2f;

            if (t > 1f)
            {
                float localMin = minimumYPosition;
                minimumYPosition = maximumYPosition;
                maximumYPosition = localMin;
                startAnimationTime = Time.time;
                t = 0f;
            }

            newPos.y = Mathf.SmoothStep(minimumYPosition, maximumYPosition, t);

            Target.GetComponent<AIPath>().Teleport(newPos);
            Target.GetComponent<AIPath>().destination = Target.transform.position;
        }
    }
    // (Unity) Methods [END]

    // Protected (Methods) [START]
    protected override void ExecuteTurnActions()
    {
        Target.OnReceiveDamage(Alignment, Damage, statusAffectorSO);
    }
    protected override void InitializeStatusActions()
    {
        BC_Velocity = Target.Velocity;
        BC_AttackVelocity = Target.AttackVelocity;
        minimumYPosition = Target.transform.position.y;
        maximumYPosition = Target.transform.position.y + 3f;
        startAnimationTime = Time.time;

        Target.UpdateAgentVelocity(0f);
        Target.UpdateAgentAttackVelocity(0f);
    }
    protected override void FinishStatusActions()
    {
        Target.UpdateAgentVelocity(BC_Velocity);
        Target.UpdateAgentAttackVelocity(BC_AttackVelocity);

        ResetProperties();
    }
    // Protected (Methods) [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////