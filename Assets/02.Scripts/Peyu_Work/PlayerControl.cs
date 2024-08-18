using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class PlayerControl : MonoBehaviour
{
    // Player ����
    [SerializeField]
    private float walkSpeed = 3.0f;
    [SerializeField]
    private float sprintSpeed = 6.0f;

    private bool _hasAnimator;
    private int _animIDIsMove;
    private int _animIDIsRun;

    private Transform _playerHeadTr;

    // Camera ����
    [SerializeField]
    private float lookSensitivity;
    [SerializeField]
    private float cameraRotationLimit;
    private float _currentCameraRotationX;

    // Components
    [SerializeField]
    private GameObject playerCamera;
    private Rigidbody _characterRigid;
    private Animator _animator;

    void Start()
    {
        _hasAnimator = TryGetComponent(out _animator);

        _characterRigid = GetComponent<Rigidbody>();

        AssignAnimationIDs();

        if (_hasAnimator)
        {
            _playerHeadTr = _animator.GetBoneTransform(HumanBodyBones.Head);    // Head ���� Transform ��������
        }
    }

    void Update()
    {
        _hasAnimator = TryGetComponent(out _animator);

        playerCamera.transform.position = _playerHeadTr.transform.position + _playerHeadTr.transform.up * 0.1f;

        Move();
        CameraRotation();
        ChracterRotation();
    }

    void LateUpdate()
    {
        HeadBoneRotation();
    }

    /// <summary>
    /// �ִϸ����� �Ķ���� �ؽø� string ���·� �������� �Լ�
    /// </summary>
    private void AssignAnimationIDs()
    {
        _animIDIsMove = Animator.StringToHash("IsMove");
        _animIDIsRun = Animator.StringToHash("IsRun");
    }

    /// <summary>
    /// �÷��̾��� �⺻ �̵��� ���� �Լ�
    /// </summary>
    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        if ((_moveHorizontal + _moveVertical) != Vector3.zero)
        {
            Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized;

            bool _isRun = Input.GetKey(KeyCode.LeftShift) ? true : false;
            float _targetSpeed = _isRun ? sprintSpeed : walkSpeed;
            _velocity *= _targetSpeed;

            _characterRigid.MovePosition(transform.position + _velocity * Time.deltaTime);

            if (_hasAnimator)
            {
                _animator.SetBool(_animIDIsMove, true);     // Move ���·� ��ȯ
                _animator.SetBool(_animIDIsRun, _isRun);    // _isRun ���� ���� Run �Ķ���� on/off
            }
        }
        else
        {
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDIsMove, false);    // Idle ���·� ��ȯ
                _animator.SetBool(_animIDIsRun, false);     // Run �Ķ���� off
            }
        }
    }

    /// <summary>
    /// ī�޶��� ���Ʒ� ���� ���濡 ���� �Լ�
    /// </summary>
    private void CameraRotation()
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;

        _currentCameraRotationX -= _cameraRotationX;
        _currentCameraRotationX = Mathf.Clamp(_currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        playerCamera.transform.localEulerAngles = new Vector3(_currentCameraRotationX, 0f, 0f);
    }

    private void HeadBoneRotation()
    {
        Vector3 HeadDir = playerCamera.transform.position + playerCamera.transform.forward * 10.0f;
        _playerHeadTr.LookAt(HeadDir);
    }

    /// <summary>
    /// ĳ������ �¿� ȸ���� ���� �Լ�
    /// </summary>
    private void ChracterRotation()
    {
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        _characterRigid.MoveRotation(_characterRigid.rotation * Quaternion.Euler(_characterRotationY)); // ���ʹϾ� * ���ʹϾ�
    }
}
