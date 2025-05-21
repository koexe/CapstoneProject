using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputModule : MonoBehaviour
{
    [SerializeField] float speed = 5;
    [SerializeField] DynamicGravity2D gravity;

    private Vector3 moveDirection;
    private bool isInventoryKeyPressed;
    private bool isMapKeyPressed;

    private void Update()
    {
        // 입력 처리
        moveDirection = Vector3.zero;
        
        if (Input.GetKey(KeyCode.LeftArrow)) moveDirection += Vector3.left;
        if (Input.GetKey(KeyCode.RightArrow)) moveDirection += Vector3.right;
        if (Input.GetKey(KeyCode.UpArrow)) moveDirection += Vector3.forward;
        if (Input.GetKey(KeyCode.DownArrow)) moveDirection += Vector3.back;

        // 대각선 이동시 정규화
        if (moveDirection.magnitude > 0.1f)
        {
            moveDirection.Normalize();
        }

        // UI 입력 처리
        if (Input.GetKeyDown(KeyCode.I)) isInventoryKeyPressed = true;
        if (Input.GetKeyDown(KeyCode.M)) isMapKeyPressed = true;
    }

    private void FixedUpdate()
    {
        // 이동 처리
        if (moveDirection.magnitude > 0.1f)
        {
            Vector3 t_movement = moveDirection * speed * Time.fixedDeltaTime;
            Vector3 t_push = gravity.UpdateCheckWall(t_movement);

            transform.position += t_push;    // 먼저 밀어내기 적용
            transform.position += t_movement; // 그 다음 이동 적용
        }

        // UI 처리
        if (isInventoryKeyPressed)
        {
            OpenInventory();
            isInventoryKeyPressed = false;
        }
        if (isMapKeyPressed)
        {
            OpenMap();
            isMapKeyPressed = false;
        }
    }

    private void OpenInventory()
    {
        UIManager.instance.ShowUI<InventoryUI>(new InventoryUIData() { identifier = "InventoryUI", isAllowMultifle = false });
    }

    private void OpenMap()
    {
        UIManager.instance.ShowUI<MapUI>(new UIData() { identifier = "MapUI", isAllowMultifle = false });
    }
}
