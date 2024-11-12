using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public Vector3 direction;  // Player's movement direction (x, y, z for 3D or x, y for 2D)
    public bool isJumping;     // Whether the jump button (Space) is pressed
    public bool isAttacking;   // Whether the attack button is pressed
    public bool isRolling;     // Whether the roll button is pressed (new input for roll)
}