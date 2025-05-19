using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputModule : MonoBehaviour
{
    // Update is called once per frame
    [SerializeField] float speed = 10;
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

    private void MoveLeft()
    {
        Vector3 t_pos = transform.localPosition;
        Vector3 t_movement = Vector3.zero;

        t_movement.x = -Time.deltaTime * speed;

        Vector3 t_push = this.gravity.UpdateCheckWall(t_movement);
        t_pos += (t_movement + t_push);

        transform.localPosition = t_pos;
    }

    private void MoveRight()
    {
        Vector3 t_pos = transform.localPosition;
        Vector3 t_movement = Vector3.zero;

        t_movement.x = Time.deltaTime * speed;

        Vector3 t_push = this.gravity.UpdateCheckWall(t_movement);
        t_pos += (t_movement + t_push);

        transform.localPosition = t_pos;
    }

    private void MoveForward()
    {
        Vector3 t_pos = transform.localPosition;
        Vector3 t_movement = Vector3.zero;

        t_movement.z = Time.deltaTime * speed;

        Vector3 t_push = this.gravity.UpdateCheckWall(t_movement);
        t_pos += (t_movement + t_push);

        transform.localPosition = t_pos;
    }

    private void MoveBackward()
    {
        Vector3 t_pos = transform.localPosition;
        Vector3 t_movement = Vector3.zero;

        t_movement.z = -Time.deltaTime * speed;

        Vector3 t_push = this.gravity.UpdateCheckWall(t_movement);
        t_pos += (t_movement + t_push);

        transform.localPosition = t_pos;
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
