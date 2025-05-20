using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputModule : MonoBehaviour
{
    // Update is called once per frame
    [SerializeField] float speed = 5;
    [SerializeField] DynamicGravity2D gravity;
    private void Start()
    {
        IngameInputManager.instance.AddInput(KeyCode.LeftArrow, IngameInputManager.InputEventType.Hold, MoveLeft);
        IngameInputManager.instance.AddInput(KeyCode.RightArrow, IngameInputManager.InputEventType.Hold, MoveRight);
        IngameInputManager.instance.AddInput(KeyCode.UpArrow, IngameInputManager.InputEventType.Hold, MoveForward);
        IngameInputManager.instance.AddInput(KeyCode.DownArrow, IngameInputManager.InputEventType.Hold, MoveBackward);
        IngameInputManager.instance.AddInput(KeyCode.I, IngameInputManager.InputEventType.Down, OpenInventory);
        IngameInputManager.instance.AddInput(KeyCode.M, IngameInputManager.InputEventType.Down, OpenMap);
    }

    private void MoveLeft() => Move(Vector3.left);
    private void MoveRight() => Move(Vector3.right);
    private void MoveForward() => Move(Vector3.forward);
    private void MoveBackward() => Move(Vector3.back);
    private void Move(Vector3 _direction)
    {
        Vector3 t_movement = _direction * speed * Time.deltaTime;

        Vector3 t_push = gravity.UpdateCheckWall(t_movement);
        transform.localPosition += (t_movement + t_push); // 벽에 닿은 축은 0이 돼서 미끄러지듯 이동
    }

    void OpenInventory()
    {
        UIManager.instance.ShowUI<InventoryUI>(new InventoryUIData() { identifier = "InventoryUI", isAllowMultifle = false });
    }
    void OpenMap()
    {
        UIManager.instance.ShowUI<MapUI>(new UIData() { identifier = "MapUI", isAllowMultifle = false });
    }
}
