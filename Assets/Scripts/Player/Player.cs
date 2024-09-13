using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    [Header("<color=#6A89A7>Animation</color>")]
    [SerializeField] private string _atkName = "onAttack";
    [SerializeField] private string _isGroundName = "isGrounded";
    [SerializeField] private string _isMovName = "isMoving";
    [SerializeField] private string _jmpName = "onJump";
    [SerializeField] private string _xName = "xAxis";
    [SerializeField] private string _zName = "zAxis";

    [Header("<color=#6A89A7>Behaviours</color>")]
    [SerializeField] private int _atkDmg = 20;
    [SerializeField] private Transform _atkOrigin;

    [Header("<color=#6A89A7>Inputs</color>")]
    [SerializeField] private KeyCode _atkKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode _jumpKey = KeyCode.Space;

    [Header("<color=#6A89A7>Physics</color>")]
    [SerializeField] private float _atkDist = 1.5f;
    [SerializeField] private LayerMask _atkMask;
    [SerializeField] private float _blockDistCheck = 1.0f;
    [SerializeField] private LayerMask _blockMask;
    [SerializeField] private float _jmpRayOffset = 0.125f;
    [SerializeField] private float _jmpRayDist = 0.5f;
    [SerializeField] private LayerMask _jmpMask;
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _movSpeed = 3.5f;

    private float _xAxis = 0f, _zAxis = 0f;
    private Vector3 _dir = new(), _blockDir = new(), _jmpOrigin = new();

    private Animator _anim;
    private Rigidbody _rb;

    private Ray _atkRay, _blockRay, _jmpRay;
    private RaycastHit _atkHit;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotation;

        //_rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        //_rb.angularDrag = 1f;

        GameManager.Instance.Player = this;
    }

    private void Start()
    {
        _anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        _xAxis = Input.GetAxis("Horizontal");
        _zAxis = Input.GetAxis("Vertical");

        _anim.SetFloat(_xName, _xAxis);
        _anim.SetFloat(_zName, _zAxis);
        _anim.SetBool(_isMovName, _xAxis != 0 || _zAxis != 0);
        _anim.SetBool(_isGroundName, IsGrounded());

        if (Input.GetKeyDown(_atkKey))
        {
            _anim.SetTrigger(_atkName);
        }

        if (Input.GetKeyDown(_jumpKey) && IsGrounded())
        {
            Jump();
        }
    }    

    private void FixedUpdate()
    {
        if((_xAxis != 0 || _zAxis != 0) && !IsBlocked(_xAxis, _zAxis))
        {
            Movement(_xAxis, _zAxis);
        }
    }

    private void Jump()
    {
        _anim.SetTrigger(_jmpName);
        _rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
    }

    private void Movement(float x, float z)
    {
        _dir = (transform.right * x + transform.forward * z).normalized;

        //_rb.velocity = _dir * _movSpeed;
        //_rb.AddForce(_dir * _movSpeed, ForceMode.Force);

        _rb.MovePosition(transform.position + _dir * _movSpeed * Time.fixedDeltaTime);
    }

    public void Attack(int dmg = 0)
    {
        _atkRay = new Ray(_atkOrigin.position, transform.forward);

        if(Physics.Raycast(_atkRay, out _atkHit, _atkDist, _atkMask))
        {
            if(_atkHit.collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.TakeDamage(_atkDmg);
            }
        }
    }

    private bool IsBlocked(float x, float z)
    {
        _blockDir = (transform.right * x + transform.forward * z);

        _blockRay = new Ray(transform.position, _blockDir);

        return Physics.Raycast(_blockRay, _blockDistCheck, _blockMask);
    }

    private bool IsGrounded()
    {
        _jmpOrigin = new Vector3(transform.position.x, transform.position.y + _jmpRayOffset, transform.position.z);

        _jmpRay = new Ray(_jmpOrigin, -transform.up);

        return Physics.Raycast(_jmpRay, _jmpRayDist, _jmpMask);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, _blockDir);
    }
}
