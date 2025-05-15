using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class SpineModelController : MonoBehaviour
{
    [SerializeField] SkeletonAnimation skeletonAnimation;
    [SerializeField] AnimationReferenceAsset run, idle, jump;

    public void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            skeletonAnimation.AnimationState.SetAnimation(0, run, true);
        }
        if (Input.GetKey(KeyCode.S))
        {
            skeletonAnimation.AnimationState.SetAnimation(0, idle, true);
        }

        if (Input.GetKey(KeyCode.D))
        {
            skeletonAnimation.AnimationState.SetAnimation(0, jump, true);
        }

    }

}
