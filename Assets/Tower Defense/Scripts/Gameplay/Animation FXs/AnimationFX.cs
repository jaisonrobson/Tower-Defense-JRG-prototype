using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using Core.Patterns;
using Unity.VisualScripting;

[RequireComponent(typeof(ParticleSystem))]
[RequireComponent(typeof(Poolable))]
[HideMonoScript]
public class AnimationFX : MonoBehaviour, IPoolable
{
    // Private (Variables) [START]
    private ParticleSystem mainSystem;
    private List<ParticleSystem> secondarySystems;
    private float startingTime;
    private float chainedAnimationDelay;
    private bool isRunning;
    private bool didPlayedSystems;
    private bool didStartedChainedAnimation;
    private bool didPlayedChainedAnimation;
    private bool isTrailAnimation;
    // Private (Variables) [END]

    // Public (Properties) [START]
    public AnimationSO AnimationSO { get; set; }
    public float Duration { get; set; }
    public GameObject ObjectToFollow { get; set; }
    // Public (Properties) [END]

    // (Unity) Methods [START]
    protected virtual void Start()
    {
    }
    protected virtual void Update()
    {
        if (isRunning)
        {
            HandleParticleSystemsDelayedPlaying();
            HandleChainedAnimationInvoking();
            HandleAnimationFinishing();
            HandleTrailAnimationObjectFollowing();
        }
    }
    // (Unity) Methods [END]

    // Private (Methods) [START]
    private void ResetVariables()
    {
        isRunning = false;
        isTrailAnimation = false;
        didPlayedSystems = false;
        didStartedChainedAnimation = false;
        didPlayedChainedAnimation = false;
        startingTime = 0f;
        chainedAnimationDelay = 0f;
        Duration = 0f;
        AnimationSO = null;
        ObjectToFollow = null;

        RefreshSystemsVariables();
        StopAllParticleSystems();
    }
    private void RefreshSystemsVariables()
    {
        mainSystem = GetComponent<ParticleSystem>();

        secondarySystems = GetComponentsInChildren<ParticleSystem>().ToList();
        secondarySystems.Remove(mainSystem);
    }
    private void UpdateParticleSystems()
    {
        UpdateParticleSystemsDuration();
    }
    private void UpdateParticleSystemsDuration()
    {
        var main = mainSystem.main;

        main.duration = Duration;

        secondarySystems.ForEach(ps => {
            var main = ps.main;

            main.duration = Duration;
        });
    }
    private void StopAllParticleSystems() => mainSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    private void PlayAllParticleSystems() => mainSystem.Play(true);
    private void HandleParticleSystemsDelayedPlaying()
    {
        if (Time.time > (startingTime + AnimationSO.delay) && !didPlayedSystems)
        {
            didPlayedSystems = true;

            PlayAllParticleSystems();
        }
    }
    private void HandleChainedAnimationInvoking()
    {
        if (AnimationSO.chainedAnimation != null)
        {
            if (didPlayedSystems && !mainSystem.isPlaying)
            {
                if (!didStartedChainedAnimation)
                {
                    didStartedChainedAnimation = true;
                    chainedAnimationDelay = Time.time;
                }
                else if (Time.time > (chainedAnimationDelay + AnimationSO.delayBeforeChainedAnimation) && !didPlayedChainedAnimation)
                {
                    Animating.InvokeAnimation(AnimationSO, transform.position, transform.rotation, Duration);

                    didPlayedChainedAnimation = true;
                }
            }
        }
    }
    private void HandleAnimationFinishing()
    {
        if (didPlayedSystems && !mainSystem.isPlaying)
        {
            if (AnimationSO != null)
            {
                if (didPlayedChainedAnimation)
                    Poolable.TryPool(gameObject);
            }
            else
                Poolable.TryPool(gameObject);
        }
    }
    private void HandleTrailAnimationObjectFollowing()
    {
        if (isTrailAnimation && ObjectToFollow != null && ObjectToFollow.activeInHierarchy)
        {
            transform.position = ObjectToFollow.transform.position;
        }
        else if (isTrailAnimation && ObjectToFollow != null && !ObjectToFollow.activeInHierarchy)
        {
            Poolable.TryPool(gameObject);
        }
    }
    // Private (Methods) [END]

    // Public (Methods) [START]
    public void StartAnimation(AnimationSO pAnimSO, float pDuration, GameObject pObjectToFollow = null, bool pIsTrailAnimation = false)
    {
        ResetVariables();

        AnimationSO = pAnimSO;
        Duration = pDuration;
        startingTime = Time.time;
        ObjectToFollow = pObjectToFollow;
        isTrailAnimation = pIsTrailAnimation;

        UpdateParticleSystems();

        isRunning = true;
    }
    public virtual void PoolRetrievalAction(Poolable poolable)
    {
    }
    public virtual void PoolInsertionAction(Poolable poolable)
    {
        ResetVariables();
    }
    // Public (Methods) [END]
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////