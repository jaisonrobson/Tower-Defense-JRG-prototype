using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Core.Patterns;
using Pathfinding;

public class FSMStateCreatureDie : FiniteStateMachine
{
    // Private (Variables) [START]
    private Animator anim;
    private Creature creature;
    private AIPath pathfinding;
    // Private (Variables) [END]

    public FSMStateCreatureDie(Animator pAnim, Creature pCreature, AIPath pPathfinding) : base()
    {
        anim = pAnim;
        creature = pCreature;
        pathfinding = pPathfinding;
        name = AgentStateEnum.DIE;
    }

    // Public (Methods) [START]
    public override void Enter()
    {
        anim.SetTrigger("isDying");

        pathfinding.canMove = false;
        pathfinding.enabled = false;

        creature.mainCollider.enabled = false;

        creature.GetComponent<CreatureFsmAi>().StartDying();

        base.Enter();
    }
    public override void Update() {}
    public override void Exit() { base.Exit(); }
    // Public (Methods) [END]

    // Private (Methods) [START]
    // Private (Methods) [END]
}
