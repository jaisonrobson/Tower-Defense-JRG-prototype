using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using Core.Patterns;

/*
 * Class involved in any physics collision that mutates data through gameplay objects
 */
[HideMonoScript]
[RequireComponent(typeof(Poolable))]
public abstract class Affector : MonoBehaviour, IPoolable
{
	// Public (Properties) [START]
	public AlignmentEnum Alignment { get; set; }
	public LayerMask AffectedsMask { get; protected set; }
	// Public (Properties) [END]

	// Private (Variables) [START]
	private bool initialized = false;
    // Private (Variables) [END]

    // (Unity) Methods [START]
    protected virtual void OnEnable()
    {
        if (!initialized)
        {
            initialized = true;

            AffectedsMask = LayerMask.GetMask("Creature", "Structure");

            PoolRetrievalAction(GetComponent<Poolable>());
        }
    }
    // (Unity) Methods [END]


    // Protected (Methods) [START]
    protected bool IsAlignmentAnOpponent(AlignmentEnum pAlignment) { return AlignmentManager.instance.IsAlignmentAnOpponent(pAlignment, Alignment); }
    // Protected (Methods) [END]

    // Public (Methods) [START]
    public virtual void PoolRetrievalAction(Poolable poolable) { }
    public virtual void PoolInsertionAction(Poolable poolable)
    {
        Alignment = AlignmentEnum.GENERIC;
    }
    // Public (Methods) [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////