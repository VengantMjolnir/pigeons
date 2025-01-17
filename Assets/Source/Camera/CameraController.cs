﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float lookLerpFactor = 0.02f;
    public float positionLerpFactor = 0.05f;
    public LayerMask raycastMask;

    [Header("Default Bounds Values")]
    public float defaultAngle = 0;
    public float defaultDistance = 15;
    public float defaultHeight = 10;

    [Header("Bounds Visualization Controls")]
    public bool showBoundsInGame = true;
    public bool showBoundsInEditor = true;
    public bool logBoundChanges = false;
    public bool useDistanceBlending = false;

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
            float totalWeight = 0f;
            int count = _activeBounds.Count;
            Vector3 start = target.position;
            Vector3 end, outDir;
            RaycastHit[] hitInfoList = new RaycastHit[_activeBounds.Count];
            foreach (CameraBound bound in _activeBounds)
            {
                if (bound.overrideOthers || count == 1)
                {
                    targetAngle = bound.angle;
                    targetDistance = bound.distance;
                    targetHeight = bound.height;
                    count = 1;
                    break;
                }
                if (useDistanceBlending)
                {
                    end = bound.transform.position;
                    //start.y = end.y;
                    outDir = (start - end).normalized * bound.RoughRadius;
                    Ray ray = new Ray(start + outDir, -outDir);
                    int hitCount = Physics.RaycastNonAlloc(ray, hitInfoList, outDir.magnitude, raycastMask.value);
                    bool foundDistance = false;
                    if (hitCount > 0)
                    {
                        for (int i = 0; i < hitCount; ++i)
                        {
                            RaycastHit hitInfo = hitInfoList[i];
                            if (hitInfo.collider == bound.collider)
                            {
                                float distance = Vector3.Distance(start, hitInfo.point);
                                bound.weight = distance;
                                foundDistance = true;
                            }
                        }
                    }

                    if (!foundDistance)
                    {
                        bound.weight = 0f;
                    }
                }
                else
                {
                    bound.weight = 1f;
                }
                totalWeight += bound.weight;
            }

            if (count > 1)
            {
                if (totalWeight < 1f)
                {
                    totalWeight = 1f;
                }
                foreach (CameraBound bound in _activeBounds)
                {
                    bound.weight /= totalWeight;
                    targetAngle += bound.angle * bound.weight;
                    targetDistance += bound.distance * bound.weight;
                    targetHeight += bound.height * bound.weight;
                }
            }
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
