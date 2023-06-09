using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using Core.Patterns;

[HideMonoScript]
public class MeleeAttackEnemyDetectionColliderController : MonoBehaviour
{
    // (Unity) Methods [START]
    void OnCollisionStay(Collision collisionInfo)
    {
        //Brincar com o AffectedAgents do MeleeAttackController aqui dentro
        //E tambem o Finished
        //Chamar o onReceiveDamage aqui tambem caso encontre um agente.
        // Debug-draw all contact points and normals
        foreach (ContactPoint contact in collisionInfo.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal * 10, Color.red);
        }
    }
    // (Unity) Methods [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////
