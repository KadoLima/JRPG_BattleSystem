using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AnimationCycle
{
    public string name;
    public float cycleTime;
}

public class CharacterAnimationController : MonoBehaviour
{
    [Header("ANIMATION PARAMETERS")]
    [SerializeField] Animator myAnim;
    [SerializeField] protected float secondsToReachTarget = .75f;
    public float SecondsToReachTarget => secondsToReachTarget;
    [SerializeField] protected float secondsToGoBack = .45f;
    public float SecondsToGoBack => secondsToGoBack;
    [SerializeField] protected string idleAnimation;
    public string IdleAnimationName => idleAnimation;
    [SerializeField] string deadAnimation;
    public string DeadAnimationName => deadAnimation;
    [SerializeField] Transform projectileSpawnPoint;
    public Transform ProjectileSpawnPoint => projectileSpawnPoint;
    [SerializeField] ParticleSystem healingEffect;


    public void PlayAnimation(string animName) => myAnim.Play(animName);
    public void EnableAnimator() => myAnim.enabled = true;
    public void DisableAnimator() => myAnim.enabled = false;

    public void PlayHealingEffect() => healingEffect.Play();
}


