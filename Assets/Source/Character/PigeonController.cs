using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigeonController : MonoBehaviour
{
    [Header("Input Names")]
    public string HorizontalAxis = "Horizontal";
    public string VerticalAxis = "Vertical";
    public string FlapButton = "Jump";
    public string PeckButton = "Fire1";
    public string PoopButton = "Fire2";
    public string DiveButton = "Fire3";
    [Header("Ground Movement")]
    public float LateralForce = 2.5f;
    public float BobForce = 2f;
    public float BobInterval = 0.5f;
    public float PeckDuration = 1.5f;
    [Tooltip("Min and Max for random idle time(in seconds)")]
    public Vector2 IdleTime = new Vector2(5f, 10f);
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
    public Transform visual;
    [Header("Actions")]
    public List<Transform> UIPoopObjects;
    public GameObject poojectile;
    public float PooPower = 1;
    public float PopForceBurst = 10f;
    public float PopStretchTime = 0.5f;
    public AnimationCurve PopStretchCurve;
    public float RechargeTime = 0.75f;

    // Flight
    private bool _flapRequested = false;
    private float _timeSinceLastFlap = 0.0f;
    // Common
    private Rigidbody _rigidbody;
    private Transform _transform;
    private bool _onGround = true;
    private float _distanceFromGround;
    private Vector3 _desiredDirection;
    private bool _isStretching = false;
    private float _rechargeTimer;
    private int _topPoopIndex;
    // Ground
    private float _bobDelay;
    private bool _walkRequested;
    private float _idleTime;
    private float _idleAccumulator;
    // Animator states and parameters
    private int WALK = Animator.StringToHash("Walk");
    private int BOB = Animator.StringToHash("Bob");
    private int FLAP = Animator.StringToHash("Flap");
    private int ON_GROUND = Animator.StringToHash("OnGround");
    private int PECK = Animator.StringToHash("Peck");

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();

        ResetIdle(true);
        _topPoopIndex = UIPoopObjects.Count - 1;
        _rechargeTimer = RechargeTime;
    }

    private void ResetIdle(bool pickNewTime = false)
    {
        _idleAccumulator = 0f;
        if (pickNewTime)
        {
            _idleTime = Random.Range(IdleTime.x, IdleTime.y);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_topPoopIndex < UIPoopObjects.Count - 1)
        {
            if (_rechargeTimer > 0f)
            {
                _rechargeTimer -= Time.deltaTime;
            }
            else
            {
                _topPoopIndex++;
                UIPoopObjects[_topPoopIndex].gameObject.SetActive(true);
                _rechargeTimer = RechargeTime;
            }
        }

        bool idle = true;
        if (Input.GetButtonDown(FlapButton))
        {
            animator.Play(FLAP);
            if (_timeSinceLastFlap >= FlapCooldown)
            {
                _flapRequested = true;
                _timeSinceLastFlap = 0;
            }
            idle = false;
        }
        else if (_timeSinceLastFlap < FlapCooldown)
        {
            _timeSinceLastFlap += Time.deltaTime;
        }

        if (!_onGround && Input.GetButtonDown(PoopButton))
        {
            if (_topPoopIndex >= 0)
            {
                PoopAction(_rigidbody.velocity / 2f);
                UIPoopObjects[_topPoopIndex].gameObject.SetActive(false);
                _topPoopIndex--;
                _rechargeTimer = RechargeTime;
            }
        }

        if (Input.GetButtonDown(DiveButton) && !_onGround)
        {
            Dash();
        }

        float h = Input.GetAxis(HorizontalAxis);
        float v = Input.GetAxis(VerticalAxis);

        if (_onGround && Input.GetButtonDown(PeckButton))
        {
            PeckAction();
            idle = false;
        }

        if (System.Math.Abs(h) > float.Epsilon || System.Math.Abs(v) > float.Epsilon)
        {
            Vector3 forward = CameraController.Instance.transform.forward;
            Vector3 right = CameraController.Instance.transform.right;
            forward.y = 0;
            forward.Normalize();
            right.y = 0;
            right.Normalize();
            _desiredDirection = (forward * v) + (right * h);
            idle = false;
        }
        else
        {
            _desiredDirection = Vector3.zero;
        }

        if (idle && _onGround)
        {
            _idleAccumulator += Time.deltaTime;
            if (_idleAccumulator > _idleTime)
            {
                PerformIdleAction();
                ResetIdle(true);
            }
        }
        else
        {
            if (!_onGround)
            {
                _idleTime = 1f;
            }
            ResetIdle();
        }
    }

    private void PoopAction(Vector3 initialVelocity)
    {
        GameObject poop = Instantiate(poojectile, transform.position + Vector3.down * 0.5f, Quaternion.identity);
        Rigidbody pooBody = poop.GetComponent<Rigidbody>();
        if (pooBody)
        {
            pooBody.velocity = initialVelocity;
            pooBody.AddForce(Vector3.down * PooPower, ForceMode.Impulse);
        }
    }

    private void PeckAction()
    {
        Debug.Log("Peck! Peck!");
        animator.SetTrigger(PECK);
        _bobDelay = PeckDuration;
    }

    private void PerformIdleAction()
    {
        int choice = Random.Range(0, 3);
        Debug.Log("Idle Action! " + choice);
        switch (choice)
        {
            case 0:
                PeckAction();
                break;
            case 1:
            case 2:
                Vector2 dir = Random.insideUnitCircle;
                _desiredDirection.x = dir.x;
                _desiredDirection.z = dir.y;
                _desiredDirection.y = 0;
                _desiredDirection.Normalize();
                Debug.Log("Idle bob!" + _desiredDirection.ToString());
                break;
        }
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

        if (_desiredDirection == Vector3.zero)
        {
            Vector3 desiredVelocity = _rigidbody.velocity.normalized;
            {
                _desiredDirection = desiredVelocity;
            }
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

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("I Hit something! " + collision.gameObject.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        CameraBound bound = other.GetComponent<CameraBound>();
        if (bound != null)
        {
            CameraController.Instance.EnteredCameraBound(bound);
        }
        else if (other.gameObject.CompareTag("Balloon"))
        {
            Vector3 dir = other.transform.position - _transform.position;
            dir.Normalize();
            float dot = Vector3.Dot(dir, visual.forward);
            var speed = _rigidbody.velocity.magnitude;

            BalloonController balloon = other.GetComponent<BalloonController>();

            if (balloon && dot > balloon.PopCoefficient && speed > balloon.RequiredSpeed)
            {
                balloon.Pop();
            }
            else
            {
                _rigidbody.AddForce(dir * -PopForceBurst, ForceMode.Impulse);
            }
        }

        Collectible collectible = other.GetComponent<Collectible>();
        if (collectible != null)
        {
            collectible.Pop();
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

    private void Dash()
    {
        _rigidbody.AddForce(visual.forward * PopForceBurst, ForceMode.Impulse);
        StartCoroutine(PopStretchRoutine());
    }

    private IEnumerator PopStretchRoutine()
    {
        if (!_isStretching)
        {
            _isStretching = true;
            WaitForEndOfFrame wait = new WaitForEndOfFrame();
            Vector3 originalScale = visual.localScale;
            Vector3 scale = originalScale;

            bool pooped = false;
            float time = Time.time;
            float targetTime = time + PopStretchTime;
            while (time < targetTime)
            {
                float t = 1.0f - (targetTime - time) / PopStretchTime;
                visual.localScale = scale;
                float s = PopStretchCurve.Evaluate(t);
                if (s > 0.5 && !pooped)
                {
                    pooped = true;
                    PoopAction(_rigidbody.velocity / -8f);
                }
                scale.z = s * originalScale.z;
                scale.x = (1f / s) * originalScale.x;
                scale.y = (1f / s) * originalScale.y;
                visual.localScale = scale;
                yield return wait;
                time = Time.time;
            }
            visual.localScale = originalScale;
            _isStretching = false;
        }
    }
}
