using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    [Header("<color=#6A89A7>Animation</color>")]
    [SerializeField] private string _areaAtkName = "onAreaAttack";
    [SerializeField] private string _atkName = "onAttack";
    [SerializeField] private string _pierceAtkName = "onPierceAttack";
    [SerializeField] private string _isGroundName = "isGrounded";
    [SerializeField] private string _isMovName = "isMoving";
    [SerializeField] private string _jmpName = "onJump";
    [SerializeField] private string _xName = "xAxis";
    [SerializeField] private string _zName = "zAxis";

    [Header("<color=#6A89A7>Behaviours</color>")]
    [SerializeField] private int _atkDmg = 20;
    [SerializeField] private Transform _atkOrigin;
    [SerializeField] private Transform _intOrigin;

    [Header("<color=#6A89A7>Camera</color>")]
    [SerializeField] private Transform _camTarget;

    public Transform GetCamTarget { get { return _camTarget; } }

    [Header("<color=#6A89A7>Inputs</color>")]
    [SerializeField] private KeyCode _areaAtkKey = KeyCode.Mouse2;
    [SerializeField] private KeyCode _atkKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode _pierceAtkKey = KeyCode.Mouse1;
    [SerializeField] private KeyCode _interactKey = KeyCode.F;
    [SerializeField] private KeyCode _jumpKey = KeyCode.Space;

    [Header("<color=#6A89A7>Physics</color>")]
    [SerializeField] private float _areaAtkRad = 2.5f;
    [SerializeField] private float _atkDist = 1.5f;
    [SerializeField] private LayerMask _atkMask;
    [SerializeField] private float _blockDistCheck = 1.0f;
    [SerializeField] private LayerMask _blockMask;
    [SerializeField] private float _intRayDist = 1.0f;
    [SerializeField] private LayerMask _intMask;
    [SerializeField] private float _jmpRayOffset = 0.125f;
    [SerializeField] private float _jmpRayDist = 0.5f;
    [SerializeField] private LayerMask _jmpMask;
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _movSpeed = 3.5f;
    [SerializeField] private float _pierceAtkDist = 15.0f;

    private Vector3 _dir = new(), _blockDir = new(), _jmpOrigin = new();
    private Vector3 _camFowardFix = new(), _camRightFix = new(), _dirFix = new();

    private Animator _anim;
    private Rigidbody _rb;
    private Transform _camTransform;

    private Ray _atkRay, _blockRay, _intRay, _jmpRay, _pierceAtkRay;
    private RaycastHit _atkHit, _intHit;

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
        _camTransform = Camera.main.transform;

        _anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        _dir.x = Input.GetAxis("Horizontal");
        _dir.z = Input.GetAxis("Vertical");

        _anim.SetFloat(_xName, _dir.x);
        _anim.SetFloat(_zName, _dir.z);
        _anim.SetBool(_isMovName, _dir.x != 0 || _dir.z != 0);
        _anim.SetBool(_isGroundName, IsGrounded());

        if (Input.GetKeyDown(_atkKey))
        {
            _anim.SetTrigger(_atkName);
        }
        else if (Input.GetKeyUp(_areaAtkKey))
        {
            _anim.SetTrigger(_areaAtkName);
        }
        else if (Input.GetKeyDown(_pierceAtkKey))
        {
            _anim.SetTrigger(_pierceAtkName);
        }

        if (Input.GetKeyDown(_interactKey))
        {
            Interact();
        }

        if (Input.GetKeyDown(_jumpKey) && IsGrounded())
        {
            Jump();
        }
    }    

    private void FixedUpdate()
    {
        if((_dir.x != 0 || _dir.z != 0) && !IsBlocked(_dir.x, _dir.z))
        {
            Movement(_dir);
        }
    }

    private void Interact()
    {
        _intRay = new Ray(_intOrigin.position, transform.forward);

        if(Physics.SphereCast(_intRay, 0.25f, out _intHit, _intRayDist, _intMask))
        {
            if(_intHit.collider.TryGetComponent(out IInteractable intObj))
            {
                intObj.OnInteract();
            }
        }
    }

    private void Jump()
    {
        _anim.SetTrigger(_jmpName);
        _rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
    }

    private void Movement(Vector3 dir)
    {
        if(dir.sqrMagnitude != 0.0f)
        {
            _camFowardFix = _camTransform.forward;
            _camRightFix = _camTransform.right;

            _camFowardFix.y = 0.0f;
            _camRightFix.y = 0.0f;

            Rotate(_camFowardFix);

            _dirFix = (_camRightFix * dir.x + _camFowardFix * dir.z).normalized;

            _rb.MovePosition(transform.position + _dirFix * _movSpeed * Time.fixedDeltaTime);
        }
    }

    private void Rotate(Vector3 dir)
    {
        transform.forward = dir;
    }

    public void AreaAttack(int dmg = 0)
    {
        Collider[] hitObjs = Physics.OverlapSphere(transform.position, _areaAtkRad);

        foreach(Collider obj in hitObjs)
        {
            if (obj.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(_atkDmg * 5);
            }
        }
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

    public void PierceAttack(int dmg = 0)
    {
        _pierceAtkRay = new Ray(_atkOrigin.position, transform.forward);

        RaycastHit[] hitObjs = Physics.RaycastAll(_pierceAtkRay, _pierceAtkDist, _atkMask);

        foreach(RaycastHit obj in hitObjs)
        {
            if(obj.collider.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(_atkDmg * 2);
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
