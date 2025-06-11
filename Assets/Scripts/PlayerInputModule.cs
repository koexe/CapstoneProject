using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using DG.Tweening;

public class PlayerInputModule : MonoBehaviour
{
    [SerializeField] float speed = 5;
    [SerializeField] DynamicGravity2D gravity;

    [SerializeField] SkeletonAnimation skeletonAnimation;

    [SerializeField] AnimationReferenceAsset idle;
    [SerializeField] AnimationReferenceAsset walk;

    [SerializeField] GameObject encounterUI;

    private Vector3 moveDirection;
    private bool isFacingRight = true;

    public void SetEncounterUI(bool _isActive) => this.encounterUI.SetActive(_isActive);

    public void ShowEncounterUI() 
    {
        if (encounterUI != null)
        {
            // UI의 원래 위치 저장
            Transform uiTransform = encounterUI.GetComponent<Transform>();
            Vector3 originalPosition = uiTransform.localPosition;
            
            uiTransform.localPosition = new Vector3(originalPosition.x, originalPosition.y - 1f, originalPosition.z);
            
            // UI 활성화
            encounterUI.SetActive(true);
            
            // DOTween으로 원래 위치로 부드럽게 이동
            uiTransform.DOLocalMoveY(originalPosition.y, 0.5f)
                      .SetEase(Ease.OutBack)
                      .SetUpdate(true); // 시간 스케일에 영향받지 않도록 설정
        }
    }

    private void Start()
    {
        // 방향키 입력 설정
        IngameInputManager.instance.AddInput(KeyCode.LeftArrow, IngameInputManager.InputEventType.Hold, () =>
        {
            moveDirection += Vector3.left;
            if (GameManager.instance.GetGameState() == GameState.Field && isFacingRight)
            {
                isFacingRight = false;
                skeletonAnimation.skeleton.ScaleX = -1;
            }
        });
        IngameInputManager.instance.AddInput(KeyCode.RightArrow, IngameInputManager.InputEventType.Hold, () =>
        {
            moveDirection += Vector3.right;
            if (GameManager.instance.GetGameState() == GameState.Field && !isFacingRight)
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
