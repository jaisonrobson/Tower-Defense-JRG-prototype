using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureFsmAi : AgentFsmAi
{
    // Private (Variables) [START]
    private Structure structure;
    // Private (Variables) [END]

    // (Unity) Methods [START]
    // Start is called before the first frame update
    protected override void OnEnable()
    {
        base.OnEnable();

        structure = AgentGOBJ.GetComponent<Structure>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    // (Unity) Methods [END]

    // Protected (Methods) [START]
    protected override void GoToIdleState()
    {
        //currentState = new FSMStateCreatureIdle(Anim, creature, pathfinding);
    }
    // Protected (Methods) [END]
}
