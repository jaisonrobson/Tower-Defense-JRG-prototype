using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pathfinding;
using Sirenix.OdinInspector;

public class CreatureFsmAi : AgentFsmAi
{
    // Private (Variables) [START]
    private Creature creature;
    // Private (Variables) [END]
    // (Unity) Methods [START]
    protected override void OnEnable()
    {
        base.OnEnable();
        creature = agent.GetComponent<Creature>();

        currentState = new FSMStateCreatureIdle(Anim, creature, pathfinding);
    }
    protected override void Update()
    {
        base.Update();
    }
    // (Unity) Methods [END]

    // Protected (Methods) [START]
    protected override void GoToIdleState()
    {
        currentState = new FSMStateCreatureIdle(Anim, creature, pathfinding);
    }
    // Protected (Methods) [END]
}
