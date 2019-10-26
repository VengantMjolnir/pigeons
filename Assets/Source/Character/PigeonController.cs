using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class PigeonController : MonoBehaviour
{
    [Header("Input Names")]
    public string HorizontalAxis = "Horizontal";
    public string VerticalAxis = "Vertical";
    public string FlapButton = "Jump";
    [Header("Ground Movement")]
    public float LateralForce = 2.5f;
    public float BobForce = 2f;
    public float BobInterval = 0.5f;
    [Tooltip("This needs to be large enought that a 'bob' still counts as on ground. Measured from the center of the object")]
    public float GroundClearance = 0.6f;
    [Header("Flight Movement")]
    public float MaxHeight = 20.0f;
    public float CounterGravityForceMod = 100.0f;
    public AnimationCurve CounterGravityForce;
    public AnimationCurve FlapPowerFalloff;
    public AnimationCurve SoaringFalloff;
    public float FlapForce = 100f;
    public float FlapCooldown = 0.08f;
    public float MaxAscentSpeed = 8.0f;
    public float MaxSoaringSpeed = 8f;
    public float HeadingLerpFactor = 0.04f;
    [Header("Dependencies")]
    public Animator animator;

    // Flight
    private bool _flapRequested = false;
    private float _timeSinceLastFlap = 0.0f;
    // Common
    private Rigidbody _rigidbody;
    private Transform _transform;
    private bool _onGround = true;
    private float _distanceFromGround;
    private Vector3 _desiredDirection;
    // Ground
    private float _bobDelay;
    private bool _walkRequested;
    // Animator states and parameters
    private int WALK = Animator.StringToHash("Walk");
    private int BOB = Animator.StringToHash("Bob");
    private int FLAP = Animator.StringToHash("Flap");
    private int ON_GROUND = Animator.StringToHash("OnGround");

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown(FlapButton) && _timeSinceLastFlap >= FlapCooldown)
        {
            _flapRequested = true;
            animator.SetTrigger(FLAP);
            _timeSinceLastFlap = 0;
        }
        else if (_timeSinceLastFlap < FlapCooldown)
        {
            _timeSinceLastFlap += Time.deltaTime;
        }

        float h = Input.GetAxis(HorizontalAxis);
        float v = Input.GetAxis(VerticalAxis);

        if (System.Math.Abs(h) < float.Epsilon && System.Math.Abs(v) < float.Epsilon)
        {
            _desiredDirection = Vector3.zero;
            return;
        }

        Vector3 forward = CameraController.Instance.transform.forward;
        Vector3 right = CameraController.Instance.transform.right;
        forward.y = 0;
        forward.Normalize();
        right.y = 0;
        right.Normalize();
        _desiredDirection = (forward * v) + (right * h);
    }

    private void FixedUpdate()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo))
        {
            _distanceFromGround = hitInfo.distance;
        }
        if (_distanceFromGround > GroundClearance)
        {
            animator.SetBool(ON_GROUND, false);
            _onGround = false;
        }
        else if (!_onGround)
        {
            animator.SetBool(ON_GROUND, true);
            _onGround = true;
        }

        // Flap!
        if (_flapRequested)
        {
            float absoluteHeight = _transform.position.y;
            if (absoluteHeight < MaxHeight)
            {
                _rigidbody.AddForce(transform.up * FlapPowerFalloff.Evaluate(absoluteHeight / MaxHeight) * FlapForce);
            }
            _flapRequested = false;
            _timeSinceLastFlap = 0.0f;
        }

        if (_onGround)
        {
            GroundUpdate();
        }
        else
        {
            FlightUpdate();
        }
    }

    private void GroundUpdate()
    {
        if (_bobDelay > 0 || _desiredDirection == Vector3.zero)
        {
            _bobDelay -= Time.deltaTime;
            return;
        }
        
        _bobDelay = BobInterval;
        _rigidbody.AddForce(_desiredDirection * LateralForce, ForceMode.Impulse);
        _rigidbody.AddForce(Vector3.up * BobForce, ForceMode.Impulse);

        animator.SetTrigger(BOB);
    }

    private void FlightUpdate()
    {
        // Only counter gravity if we're not already on the way down
        if (_rigidbody.velocity.y < 0)
        {
            _rigidbody.AddForce(Vector3.up * CounterGravityForce.Evaluate(_distanceFromGround / MaxHeight) * CounterGravityForceMod * Time.deltaTime);
        }

        if (_desiredDirection != Vector3.zero)
        {
            Vector3 desiredVelocity = _desiredDirection.normalized;
            desiredVelocity *= SoaringFalloff.Evaluate(_distanceFromGround / MaxHeight) * MaxSoaringSpeed;
            desiredVelocity = Vector3.Lerp(_rigidbody.velocity, desiredVelocity, HeadingLerpFactor);
            desiredVelocity.y = _rigidbody.velocity.y;
            _rigidbody.velocity = desiredVelocity;
        }

        if (_rigidbody.velocity.y > MaxAscentSpeed)
        {
            Vector3 velo = _rigidbody.velocity;
            velo.y = Mathf.Min(_rigidbody.velocity.y, MaxAscentSpeed);
            _rigidbody.velocity = velo;
        }
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