using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToVelocityXZ : MonoBehaviour
{
    public float lerpFactor = 0.1f;
    public float velocityThreshold = 1f;
    public float rollAmount = -0.1f;
    public Rigidbody body;

    // Update is called once per frame
    void Update()
    {
        Vector3 v = body.velocity;
        v.y = 0;
        if (v.sqrMagnitude > velocityThreshold)
        {
            Quaternion look = Quaternion.LookRotation(v);
            float angle = Vector3.SignedAngle(transform.forward, v, Vector3.up);
            Debug.Log("Turning: " + angle);
            Quaternion roll = Quaternion.Euler(0, 0, angle * rollAmount);
            look *= roll;
            transform.rotation = Quaternion.Slerp(transform.rotation, look, lerpFactor);
        }
    }
}
