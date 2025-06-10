using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Spine;
using Spine.Unity;
using UnityEngine;

public class SpineModelController : MonoBehaviour
{
    [SerializeField] SkeletonAnimation skeletonAnimation;
    [SerializeField] AnimationReferenceAsset idle, walk, meleeAttack, magicAttack, hit, die;
    [SerializeField] float attackTiming;
    public void Initialization(
        SkeletonDataAsset _skeletonAnimation,
        AnimationReferenceAsset _idle,
        AnimationReferenceAsset _attack,
        AnimationReferenceAsset _hit)
    {
        this.skeletonAnimation.skeletonDataAsset = _skeletonAnimation;
        this.idle = _idle;
        this.meleeAttack = _attack;
        this.hit = _hit;
        this.skeletonAnimation.Initialize(true);
    }


    public async UniTask PlayAnimationAsync(AnimationType _type, float _effectTiming = 0.0f, System.Action onEffect = null)
    {
        AnimationReferenceAsset t_animName = _type switch
        {
            AnimationType.idle => this.idle,
            AnimationType.walk => this.walk,
            AnimationType.meleeAttack => this.meleeAttack,
            AnimationType.magicAttack => this.magicAttack,
            AnimationType.hit => this.hit,
            AnimationType.die => this.die,
            _ => this.idle
        };

        bool t_loop = (_type == AnimationType.idle || _type == AnimationType.walk);
        TrackEntry t_entry = this.skeletonAnimation.AnimationState.SetAnimation(0, t_animName, t_loop);

        if (_type == AnimationType.meleeAttack && onEffect != null)
        {
            // 예: 0.3초 후에 이펙트 발동
            await UniTask.Delay(TimeSpan.FromSeconds(_effectTiming));
            onEffect?.Invoke();
        }

        // 루프 애니메이션이면 기다릴 필요 없음
        if (!t_loop)
        {
            var tcs = new UniTaskCompletionSource();

            t_entry.Complete += _ => tcs.TrySetResult();

            await tcs.Task;
        }
    }
    public void PlayAnimation(AnimationType _type)
    {
        AnimationReferenceAsset t_animName = _type switch
        {
            AnimationType.idle => this.idle,
            AnimationType.walk => this.walk,
            AnimationType.meleeAttack => this.meleeAttack,
            AnimationType.magicAttack => this.magicAttack,
            AnimationType.hit => this.hit,
            AnimationType.die => this.die,
            _ => this.idle
        };

        bool t_loop = (_type == AnimationType.idle || _type == AnimationType.walk);

        TrackEntry t_entry = this.skeletonAnimation.AnimationState.SetAnimation(0, t_animName, t_loop);
    }

}
public enum AnimationType
{
    idle,
    walk,
    meleeAttack,
    magicAttack,
    hit,
    die,
}