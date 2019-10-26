using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBound : MonoBehaviour
{
    public float distance = 10f;
    public float height = 4f;
    public float angle = 0f;

    private MeshRenderer _mesh;

    public void Start()
    {
        _mesh = GetComponent<MeshRenderer>();
    }

    public void Update()
    {
        bool showBounds = CameraController.Instance.showBounds;
        if (showBounds != _mesh.enabled)
        {
            _mesh.enabled = showBounds;
        }
    }
}
