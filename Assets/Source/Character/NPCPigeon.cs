using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCPigeon : MonoBehaviour
{
    public Animator animator;
    [Tooltip("Min and Max for random idle time(in seconds)")]
    public Vector2 IdleTime = new Vector2(5f, 10f);
    public float LerpFactor = 0.01f;

    private int WALK = Animator.StringToHash("Walk");
    private int FLAP = Animator.StringToHash("Flap");
    private int PECK = Animator.StringToHash("Peck");

    private float _idleTime;
    private Vector3 _desiredDirection;

    private void Start()
    {
        _desiredDirection = transform.forward;
    }

    private void Update()
    {
        if (_idleTime < 0)
        {
            PerformIdleAction();
            _idleTime = Random.Range(IdleTime.x, IdleTime.y);
        }
        else
        {
            _idleTime -= Time.deltaTime;
        }

        float dot = Vector3.Dot(_desiredDirection, transform.forward);
        if (dot < 0.91f)
        {
            Quaternion look = Quaternion.LookRotation(_desiredDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, LerpFactor);
        }
    }

    private void PerformIdleAction()
    {
        int choice = Random.RandomRange(0, 10);
        switch (choice)
        {
            case 0:
            case 1:
                animator.Play(FLAP);
                break;
            case 2:
            case 3:
            case 4:
            case 5:
                animator.Play(PECK);
                break;
            case 6:
            case 7:
            case 8:
                animator.Play(WALK);
                break;
            case 9:
                ChangeDirection();
                break;
        }
    }

    private void ChangeDirection()
    {
        Vector2 dir = Random.insideUnitCircle.normalized;
        _desiredDirection = new Vector3(dir.x, _desiredDirection.y, dir.y);
    }
}
