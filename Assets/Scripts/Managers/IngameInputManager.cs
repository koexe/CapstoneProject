using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameInputManager : MonoBehaviour
{
    public static IngameInputManager instance;

    public enum InputEventType { Down, Hold, Up }

    public class InputBinding
    {
        public KeyCode key;
        public InputEventType eventType;
        public Action action;
        public Func<bool> condition; // ex) GameManager.Instance.IsIngame
    }

    private readonly List<InputBinding> bindings = new List<InputBinding>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        foreach (var binding in bindings)
        {
            if (binding.condition != null && !binding.condition.Invoke())
                continue;

            switch (binding.eventType)
            {
                case InputEventType.Down:
                    if (Input.GetKeyDown(binding.key)) binding.action?.Invoke();
                    break;
                case InputEventType.Hold:
                    if (Input.GetKey(binding.key)) binding.action?.Invoke();
                    break;
                case InputEventType.Up:
                    if (Input.GetKeyUp(binding.key)) binding.action?.Invoke();
                    break;
            }
        }
    }

    public void AddInput(KeyCode _key, InputEventType _type, Action _action, Func<bool> _condition = null)
    {
        bindings.Add(new InputBinding
        {
            key = _key,
            eventType = _type,
            action = _action,
            condition = _condition ?? (() => true) // 조건 없으면 항상 허용
        });
    }
}
