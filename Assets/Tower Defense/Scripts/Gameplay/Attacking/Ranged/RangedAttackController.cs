using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using Core.Patterns;
using Core.Math;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(RangedAttackEnemyDetectionColliderController))]
[HideMonoScript]
public class RangedAttackController : AttackController
{
    /*
     * Armazenar informacoes relevantes ao movimento do projetil aqui, exemplo: velocidade, direcao
     * Verificar como deve ser feito a movimentacao do projetil
     */

    // Private (Properties) [START]
    private float StartTime { get; set; }
    // Private (Properties) [END]

    // (Unity) Methods [START]
    private void OnEnable()
    {
        StartTime = Time.fixedTime;
    }
    private void FixedUpdate()
    {
        //Fazer um metodo aqui pra verificar se o tipo de viagem do ataque e linear ou em arco, e usar lerp ou slerp de acordo com o mesmo
        RangedAttackAffector raa = GetComponent<RangedAttackAffector>();

        transform.position = Slerp.EvaluateSlerpPointsVector3(raa.Origin, raa.Destination, 1, StartTime, raa.Duration);
    }
    // (Unity) Methods [END]

    // Public (Methods) [START]
    public override void PoolRetrievalAction(Poolable poolable)
    {
        base.PoolRetrievalAction(poolable);
    }
    public override void PoolInsertionAction(Poolable poolable)
    {
        base.PoolInsertionAction(poolable);
    }
    // Public (Methods) [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////