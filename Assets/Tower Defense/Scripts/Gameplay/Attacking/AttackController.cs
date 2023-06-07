using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using Core.Patterns;

[HideMonoScript]
public abstract class AttackController : MonoBehaviour, IPoolable
{
    // Public (Properties) [START]
    public List<GameObject> AffectedAgents { get; set; }
    public bool Finished { get; set; }
    // Public (Properties) [END]


    // (Unity) Methods [START]
    public void Start()
    {
        Finished = false;
        AffectedAgents = new List<GameObject>();
    }
    // (Unity) Methods [END]


    // Public (Methods) [START]
    public virtual void PoolRetrievalAction(Poolable poolable)
    {
        
    }
    public virtual void PoolInsertionAction(Poolable poolable)
    {
        AffectedAgents.Clear();
        Finished = false;
    }
    // Public (Methods) [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////