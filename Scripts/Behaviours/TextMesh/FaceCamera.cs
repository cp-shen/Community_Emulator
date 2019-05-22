using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform obj;
    private Transform cam;

    void Start()
    {
        obj = transform;
        cam = Camera.main.transform;
    }

    void Update()
    {
        obj.forward = cam.forward;
    }
}
