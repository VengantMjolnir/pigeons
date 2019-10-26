using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMovement : MonoBehaviour
{
    public string horizontalAxis = "Horizontal";
    public string verticalAxis = "Vertical";
    public float force = 10f;
    public float jump = 10f;
    public float jumpInterval = 1f;

    private Rigidbody _rigidbody;
    private Transform _transform;
    private float _jumpDelay;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_jumpDelay > 0)
        {
            _jumpDelay -= Time.deltaTime;
            return;
        }

        _jumpDelay += jumpInterval;
        Vector3 forward = CameraController.Instance.transform.forward;
        Vector3 right = CameraController.Instance.transform.right;
        forward.y = 0;
        forward.Normalize();
        right.y = 0;
        right.Normalize();
        float h = Input.GetAxis(horizontalAxis);
        float v = Input.GetAxis(verticalAxis);

        if (System.Math.Abs(h) < float.Epsilon && System.Math.Abs(v) < float.Epsilon)
        {
            return;
        }

        _rigidbody.AddForce(forward * v * force, ForceMode.Impulse);
        _rigidbody.AddForce(right * h * force, ForceMode.Impulse);
        _rigidbody.AddForce(Vector3.up * jump, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        CameraBound bound = other.GetComponent<CameraBound>();
        if (bound != null)
        {
            CameraController.Instance.EnteredCameraBound(bound);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CameraBound bound = other.GetComponent<CameraBound>();
        if (bound != null)
        {
            CameraController.Instance.LeftCameraBound(bound);
        }
    }
}
