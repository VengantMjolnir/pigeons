using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdAirControl : MonoBehaviour
{
    public float MaxHeight = 20.0f;

    public float CounterGravityForceMod = 100.0f;
    public AnimationCurve CounterGravityForce;
    public AnimationCurve FlapPowerFalloff;
    public float JumpForce = 100f;
    public float JumpCooldown = 0.08f;
    public float MaxAscentVelocity = 8.0f;
    
    private Rigidbody _rigid = null;
    private bool _jumpRequested = false;
    private float _timeSinceLastJump = 0.0f;

    private void Start()
    {
        _rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump") && _timeSinceLastJump >= JumpCooldown)
        {
            _jumpRequested = true;
        }
        else if (_timeSinceLastJump < JumpCooldown)
        {
            _timeSinceLastJump += Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        float distanceFromGround = 0.0f;
        RaycastHit hitInfo;
        if(Physics.Raycast(transform.position, Vector3.down, out hitInfo))
        {
            distanceFromGround = hitInfo.distance;
        }

        Debug.Log("Velo: " + _rigid.velocity.y);

        // Only counter gravity if we're not already on the way down
        if (_rigid.velocity.y < 0)
        {
            _rigid.AddForce(Vector3.up * CounterGravityForce.Evaluate(distanceFromGround / MaxHeight) * CounterGravityForceMod * Time.deltaTime);
        }

        if (_jumpRequested)
        {
            if (distanceFromGround < MaxHeight)
            {
                _rigid.AddForce(transform.up * FlapPowerFalloff.Evaluate(distanceFromGround / MaxHeight) * JumpForce);
            }
            _jumpRequested = false;
            _timeSinceLastJump = 0.0f;
        }

        if (_rigid.velocity.y > MaxAscentVelocity)
        {
            Vector3 velo = _rigid.velocity;
            velo.y = Mathf.Min(_rigid.velocity.y, MaxAscentVelocity);
            _rigid.velocity = velo;
        }
    }
}
