﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToVelocityXZ : MonoBehaviour
{
    public float lerpFactor = 0.1f;
    public float velocityThreshold = 1f;
    public Rigidbody body;

    // Update is called once per frame
    void Update()
    {
        Vector3 v = body.velocity;
        v.y = 0;
        if (v.sqrMagnitude > velocityThreshold)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(v), lerpFactor);
        }
    }
}