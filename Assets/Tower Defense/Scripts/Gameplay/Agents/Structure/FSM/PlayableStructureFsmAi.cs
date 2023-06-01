using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableStructureFsmAi : StructureFsmAi
{
    // Private (Variables) [START]
    private PlayableStructure playableStructure;
    // Private (Variables) [END]

    // (Unity) Methods [START]
    // Start is called before the first frame update
    protected override void OnEnable()
    {
        base.OnEnable();

        playableStructure = AgentGOBJ.GetComponent<PlayableStructure>();

        currentState = new FSMStatePlayableStructureIdle(Anim, playableStructure);
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
        currentState = new FSMStatePlayableStructureIdle(Anim, playableStructure);
    }
    // Protected (Methods) [END]
}
