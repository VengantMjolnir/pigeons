using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float lookLerpFactor = 0.02f;
    public float positionLerpFactor = 0.05f;

    public float defaultAngle = 0;
    public float defaultDistance = 15;
    public float defaultHeight = 10;

    [Header("Bounds Visualization Controls")]
    public bool showBoundsInGame = true;
    public bool showBoundsInEditor = true;
    public bool logBoundChanges = false;

    private Transform _transform;
    private float _angle;
    private float _height;
    private float _distance;

    private List<CameraBound> _activeBounds = new List<CameraBound>();

    private static CameraController _instance;
    public static CameraController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CameraController>();
            }
            return _instance;
        }
    }

    private void Log(string message)
    {
        if (logBoundChanges)
        {
            Debug.Log(message);
        }
    }

    public void EnteredCameraBound(CameraBound bound)
    {
        Log("Entered camera bound " + bound.name);
        if (_activeBounds.Contains(bound) == false)
        {
            _activeBounds.Add(bound);
        }
    }

    public void LeftCameraBound(CameraBound bound)
    {
        Log("Left camera bound " + bound.name);
        if (_activeBounds.Contains(bound))
        {
            _activeBounds.Remove(bound);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _transform = GetComponent<Transform>();
        _angle = defaultAngle;
        _height = defaultHeight;
        _distance = defaultDistance;
    }

    public void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        float targetAngle = defaultAngle;
        float targetDistance = defaultDistance;
        float targetHeight = defaultHeight;
        if (_activeBounds.Count > 0)
        {
            targetAngle = 0;
            targetDistance = 0;
            targetHeight = 0;
            int count = _activeBounds.Count;
            foreach (CameraBound bound in _activeBounds)
            {
                if (bound.overrideOthers)
                {
                    targetAngle = bound.angle;
                    targetDistance = bound.distance;
                    targetHeight = bound.height;
                    count = 1;
                    break;
                }
                targetAngle += bound.angle;
                targetDistance += bound.distance;
                targetHeight += bound.height;
            }
            targetAngle /= count;
            targetDistance /= count;
            targetHeight /= count;
        }

        _angle = Mathf.LerpAngle(_angle, targetAngle, lookLerpFactor);
        _distance = Mathf.Lerp(_distance, targetDistance, lookLerpFactor);
        _height = Mathf.Lerp(_height, targetHeight, lookLerpFactor);

        Vector3 pos = target.position;
        Quaternion rotation = Quaternion.Euler(0, _angle, 0);
        Vector3 forward = Vector3.forward * _distance;
        forward.y += _height;
        Vector3 dir = rotation * forward;

        _transform.position = Vector3.Lerp(_transform.position, pos + dir, positionLerpFactor);

        _transform.LookAt(target);
    }

    public void OnDrawGizmos()
    {
        if (target)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(target.position, 0.5f);
            Gizmos.DrawLine(target.position, transform.position);
        }
    }
}
