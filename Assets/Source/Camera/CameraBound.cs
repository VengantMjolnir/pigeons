using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraBound : MonoBehaviour
{
    public float distance = 10f;
    public float height = 4f;
    public float angle = 0f;
	public bool overrideOthers = false;
    [HideInInspector]
    public float weight = 0f;

    private MeshRenderer _mesh;
    private Collider _collider;

    public void Start()
    {
        _mesh = GetComponent<MeshRenderer>();
        _collider = GetComponent<Collider>();
    }

    public void Update()
    {
        bool showBounds = Application.isPlaying ? CameraController.Instance.showBoundsInGame : CameraController.Instance.showBoundsInEditor;
        if (showBounds != _mesh.enabled)
        {
            _mesh.enabled = showBounds;
        }
    }

    public new Collider collider { get { return _collider; } }

    public float RoughRadius
    {
        get
        {
            if (collider is BoxCollider)
            {
                BoxCollider box = collider as BoxCollider;
                Vector3 size = box.bounds.extents;
                //size.y = 0;
                return size.magnitude;
            }
            else if (collider is SphereCollider)
            {
                SphereCollider sphere = collider as SphereCollider;
                return sphere.radius;
            }
            else
            {
                Vector3 size = _mesh.bounds.extents;
                //size.y = 0;
                return size.magnitude;
            }
            
        }
    }
}
