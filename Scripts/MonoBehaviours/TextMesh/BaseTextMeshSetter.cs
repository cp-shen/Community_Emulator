using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public abstract class BaseTextMeshSetter : MonoBehaviour
{
    private TextMesh textMesh;

    protected abstract string StringToDisplay { get; }

    protected virtual void Start() {
        textMesh = GetComponent<TextMesh>();
    }

    void Update()
    {
        textMesh.text = StringToDisplay;
    }
}
