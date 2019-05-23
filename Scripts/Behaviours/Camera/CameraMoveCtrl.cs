using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoveCtrl : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private float rotSpeed = 5f;

    private float rotX = 0f;
    private float rotY = 0f;
    private Vector3 initPosition;

    void Start() {
        initPosition = transform.position;
    }

    public void ResetPosition() {
        transform.position = initPosition;
    }

    public void ResetRotation() {
        rotX = rotY = 0f;
    }

    /// <summary>
    /// Get move vector from keyboard input
    /// multiply it with speed and deltaTime
    /// then perform movement by transform
    /// </summary>
    private void MoveByKeys() {
        float moveX = Input.GetAxis("Horizontal") * moveSpeed;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed;

        Vector3 move = new Vector3(moveX, 0, moveZ);
        move = Vector3.ClampMagnitude(move, moveSpeed);
        move *= Time.deltaTime;

        transform.Translate(move, Space.Self);
    }

    private void RotateByMouse() {
        rotX += Input.GetAxis("Mouse Y") * (-rotSpeed) * Time.deltaTime;
        rotY += Input.GetAxis("Mouse X") * rotSpeed * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -89f, 89f);
        rotY = Mathf.Clamp(rotY, -180f, 180f);

        // rotate around world y then local x
        transform.rotation = Quaternion.AngleAxis(rotX, transform.right) * Quaternion.AngleAxis(rotY, Vector3.up) * Quaternion.identity;
    }

    void LateUpdate() {
        MoveByKeys();
    }

    private void OnDisable() {
        ResetPosition();
        ResetRotation();
    }
}
