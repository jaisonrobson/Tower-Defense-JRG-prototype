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
    [ReadOnly]
    [ShowInInspector]
    public List<Agent> AffectedAgents { get; set; }
    [ReadOnly]
    [ShowInInspector]
    public bool Finished { get; set; }
    // Public (Properties) [END]


    // (Unity) Methods [START]
    public void Start()
    {
        Finished = false;
        AffectedAgents = new List<Agent>();
    }
    // (Unity) Methods [END]


    // Public (Methods) [START]
    public void AddAffectedAgent(Agent agent) => AffectedAgents.Add(agent);
    public void RemoveAffectedAgent(Agent agent) => AffectedAgents.Remove(agent);
    public bool IsAgentAlreadyAffected(Agent agent) => AffectedAgents.Any(aa => aa.gameObject.GetInstanceID() == agent.gameObject.GetInstanceID());
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