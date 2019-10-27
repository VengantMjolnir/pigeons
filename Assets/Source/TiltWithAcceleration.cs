using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltWithAcceleration : MonoBehaviour
{
    public float lerpFactor = 0.1f;
    public float pitchLimit = 10f;
    public Vector3 angleLimits;
    public Rigidbody body;

    // Update is called once per frame
    void FixedUpdate()
    {
        float pitch = (body.velocity.y / pitchLimit) * angleLimits.x;
        Debug.Log("Pitch: " + pitch);
        Vector3 angles = transform.rotation.eulerAngles;
        angles.x = pitch;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(angles), lerpFactor);
    }
}
