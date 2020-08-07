using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{

    public enum PlayerPower
    {
        Normal,
        Poisoned,
        Midas,
    }
    [Header("Status")]
    [SerializeField] private PlayerPower m_Power = PlayerPower.Normal;
    [SerializeField] private bool isAlive = true;

    [Space]
    [Header("Movement Properties")]
    [SerializeField] private float speed = 8f;                //Player speed
    [SerializeField] private float crouchSpeedDivisor = 3f;   //Speed reduction when crouching
    [SerializeField] private float coyoteDuration = .05f;     //How long the player can jump after falling
    [SerializeField] private float maxFallSpeed = -25f;       //Max speed player can fall

    [Space]
    [Header("Jump Properties")]
    [SerializeField] private float jumpForce = 6.3f;          //Initial force of jump
    [SerializeField] private float crouchJumpBoost = 2.5f;    //Jump boost when crouching
    [SerializeField] private float hangingJumpForce = 15f;    //Force of wall hanging jumo
    [SerializeField] private float jumpHoldForce = 1.9f;      //Incremental force when jump is held
    [SerializeField] private float jumpHoldDuration = .1f;    //How long the jump key can be held

    [Space]
    [Header("Health")]
    [SerializeField] private int health = 3;

    [Space]
    [Header("Dammage")]
    [SerializeField] private float jumpImpact = 0.35f;

    [Space]
    [Header("Properties")]
    [SerializeField] private float bounciness = 0.35f;
    [SerializeField] private float poisonDuration = 3f;

    public void GetDamage (int damage)
    {
        health -= damage;
    }

    #region Getter& Setter
    public float Speed { get => speed; set => speed = value; }
    public float CrouchSpeedDivisor { get => crouchSpeedDivisor; set => crouchSpeedDivisor = value; }
    public float CoyoteDuration { get => coyoteDuration; set => coyoteDuration = value; }
    public float MaxFallSpeed { get => maxFallSpeed; set => maxFallSpeed = value; }
    public float JumpForce { get => jumpForce; set => jumpForce = value; }
    public float CrouchJumpBoost { get => crouchJumpBoost; set => crouchJumpBoost = value; }
    public float HangingJumpForce { get => hangingJumpForce; set => hangingJumpForce = value; }
    public float JumpHoldForce { get => jumpHoldForce; set => jumpHoldForce = value; }
    public float JumpHoldDuration { get => jumpHoldDuration; set => jumpHoldDuration = value; }
    public int Health { get => health; set => health = value; }
    public float JumpImpact { get => jumpImpact; set => jumpImpact = value; }
    public PlayerPower Power { get => m_Power; set => m_Power = value; }
    public float Bounciness { get => bounciness; set => bounciness = value; }
    public float PoisonDuration { get => poisonDuration; set => poisonDuration = value; }
    public bool IsAlive { get => isAlive; set => isAlive = value; }
    #endregion
}
