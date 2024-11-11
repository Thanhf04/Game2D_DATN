using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Addons.Physics;

public class Player : NetworkBehaviour
{
    private NetworkRigidbody2D _rigidbody2D;
    private float moveSpeed = 5f;

    private void Awake()
    {
        // Lấy NetworkRigidbody2D component
        _rigidbody2D = GetComponent<NetworkRigidbody2D>();
    }

    public override void FixedUpdateNetwork()
    {
        // Kiểm tra xem có đầu vào từ người chơi không
        if (GetInput(out NetworkInputData data))
        {
            // Chuẩn hóa hướng di chuyển
            data.direction.Normalize();

            // Tính toán vận tốc di chuyển
            Vector2 velocity = data.direction * moveSpeed;

            // Sử dụng _rigidbody2D.Rigidbody để áp dụng velocity
            _rigidbody2D.Rigidbody.velocity = velocity;
        }
    }
}
