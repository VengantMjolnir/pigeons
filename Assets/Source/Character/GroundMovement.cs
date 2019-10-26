using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMovement : MonoBehaviour
{
    public string horizontalAxis = "Horizontal";
    public string verticalAxis = "Vertical";
    public float force = 10f;

    private Rigidbody _rigidbody;
    private Transform _transform;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis(horizontalAxis);
        float v = Input.GetAxis(verticalAxis);

        if (h == 0 && v == 0)
        {
            return;
        }

        _rigidbody.AddForce(_transform.forward * v * force);
    }
}
