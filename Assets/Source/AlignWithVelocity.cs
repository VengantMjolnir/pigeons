using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignWithVelocity : MonoBehaviour
{
    public Rigidbody body;

    // Update is called once per frame
    void Update()
    {
        if (body == null)
        {
            return;
        }

        Vector3 v = body.velocity;
        
        Quaternion look = Quaternion.LookRotation(v);
        transform.rotation = look;
    }
}
