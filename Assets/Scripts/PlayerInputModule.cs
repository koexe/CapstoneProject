using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class PlayerInputModule : MonoBehaviour
{
    [SerializeField] float speed = 5;
    [SerializeField] DynamicGravity2D gravity;

    [SerializeField] SkeletonAnimation skeletonAnimation;

    [SerializeField] AnimationReferenceAsset idle;
    [SerializeField] AnimationReferenceAsset walk;


    private Vector3 moveDirection;
    private bool isFacingRight = true;

    private void Start()
    {
        // 방향키 입력 설정
        IngameInputManager.instance.AddInput(KeyCode.LeftArrow, IngameInputManager.InputEventType.Hold, () =>
        {
            moveDirection += Vector3.left;
            if (isFacingRight)
            {
                isFacingRight = false;
                skeletonAnimation.skeleton.ScaleX = -1;
            }
        });
        IngameInputManager.instance.AddInput(KeyCode.RightArrow, IngameInputManager.InputEventType.Hold, () =>
        {
            moveDirection += Vector3.right;
            if (!isFacingRight)
            {
                isFacingRight = true;
                skeletonAnimation.skeleton.ScaleX = 1;
            }
        });
        IngameInputManager.instance.AddInput(KeyCode.UpArrow, IngameInputManager.InputEventType.Hold, () => moveDirection += Vector3.forward);
        IngameInputManager.instance.AddInput(KeyCode.DownArrow, IngameInputManager.InputEventType.Hold, () => moveDirection += Vector3.back);

        // UI 입력 설정
        IngameInputManager.instance.AddInput(KeyCode.I, IngameInputManager.InputEventType.Down, () => OpenInventory());
        IngameInputManager.instance.AddInput(KeyCode.M, IngameInputManager.InputEventType.Down, () => OpenMap());

        // 초기 애니메이션 설정
        SetAnimation(idle, true);
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.GetGameState() != GameState.Field)
        {
            SetAnimation(idle, true);
            return;
        }
        // 이동 처리
        if (moveDirection.magnitude > 0.1f)
        {
            moveDirection.Normalize();
            Vector3 t_movement = moveDirection * speed * Time.fixedDeltaTime;
            Vector3 t_push = gravity.UpdateCheckWall(t_movement);

            transform.position += t_push;    // 먼저 밀어내기 적용
            transform.position += t_movement; // 그 다음 이동 적용

            // 걷기 애니메이션 재생
            SetAnimation(walk, true);
            moveDirection = Vector3.zero;
        }
        else
        {
            // 정지 시 대기 애니메이션 재생
            SetAnimation(idle, true);
        }


    }
    private void OnDisable()
    {
        Debug.Log("꺼짐");
    }
    private void SetAnimation(AnimationReferenceAsset anim, bool loop)
    {
        if (skeletonAnimation.AnimationName != anim.name)
        {
            skeletonAnimation.AnimationState.SetAnimation(0, anim, loop);
        }
    }

    private void OpenInventory()
    {
        if (GameManager.instance.GetGameState() == GameState.Field)
        {
            UIManager.instance.ShowUI<InventoryUI>(new InventoryUIData() { identifier = "InventoryUI", isAllowMultifle = false });
        }
    }

    private void OpenMap()
    {
        if (GameManager.instance.GetGameState() == GameState.Field)
        {
            UIManager.instance.ShowUI<MapUI>(new UIData() { identifier = "MapUI", isAllowMultifle = false });
        }
    }
}
