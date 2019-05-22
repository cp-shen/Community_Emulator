using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RandomMove : MonoBehaviour
{
    [SerializeField] private float speed = 10f;

    private Rigidbody rgbd;

    /// <summary>
    /// normalized vector2 to indicate horizontal move direction
    /// </summary>
    private Vector2 horDirection;

    void Start() {
        rgbd = GetComponent<Rigidbody>();
        horDirection = Vector2.zero;

        SetRandomMoveDirection();
    }

    private void FixedUpdate() {
        horDirection = horDirection.normalized;
        rgbd.velocity = speed * new Vector3(horDirection.x, 0, horDirection.y);
    }

    private void SetRandomMoveDirection() {
        horDirection = Random.insideUnitCircle;
    }

    /// <summary>
    /// change move direction when hit something
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision) {
        Vector3 normal = collision.contacts[0].normal;
        horDirection = new Vector2(normal.x, normal.z);
    }
}
