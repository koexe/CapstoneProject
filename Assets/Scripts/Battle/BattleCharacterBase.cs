using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCharacterBase : MonoBehaviour
{
    int hp;
    public int speed;
    [SerializeField] SpriteRenderer skin;
    [SerializeField] Animator animator;

    public void Attack()
    {
        this.animator.Play("Attack");
    }
    public void Hit()
    {
        this.animator.Play("Hit");
    }
    public void Idle()
    {
        this.animator.Play("Idle");
    }
}
