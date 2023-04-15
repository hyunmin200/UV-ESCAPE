using System;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    public enum PlayerState
    {
        idle,
        walk,
        run
    }

    private Animator _animator;
    private Camera _camera;
    private CharacterController _controller;
    private PlayerState _state;

    private float _x;
    private float _z;

    private Vector3 moveDirection;
    public float walkSpeed = 5f; // 걷기 속도
    public float runSpeed = 8f; // 달리기 속도
    public float finalSpeed; // 최종 속도

    public bool toggleCameraRotation; // 둘러보기

    public float smoothness = 10f;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _camera = Camera.main;
        _controller = this.GetComponent<CharacterController>();
        _state = PlayerState.idle;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            toggleCameraRotation = true; // 둘러보기 활성화
        }
        else
        {
            toggleCameraRotation = false; // 둘러보기 비활성화
        }

        if (Input.GetKey(KeyCode.LeftShift) && moveDirection != new Vector3(0, 0, 0))
        {
            _state = PlayerState.run; // 달리기
        }
        else if (moveDirection != new Vector3(0, 0, 0))
        {
            _state = PlayerState.walk;
        }
        else
        {
            _state = PlayerState.idle; // 걷기
        }

        InputMovement();
    }

    private void LateUpdate()
    {
        if (toggleCameraRotation != true)
        {
            Vector3 playerRotate = Vector3.Scale(_camera.transform.forward, new Vector3(1, 0, 1));

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate),
                Time.deltaTime * smoothness);
        }
    }

    void InputMovement()
    {
        // 캐릭터 속도 변환
        if (_state == PlayerState.run) finalSpeed = runSpeed;
        else if (_state == PlayerState.walk) finalSpeed = walkSpeed;
        else if (_state == PlayerState.idle) finalSpeed = 0;

        // 캐릭터 이동 값 받기
        _x = Input.GetAxisRaw("Horizontal");
        _z = Input.GetAxisRaw("Vertical");

        // 로컬 방향을 월드 방향으로 변환
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // 캐릭터 방향
        moveDirection = forward * _z + right * _x;

        _controller.Move(moveDirection.normalized * (finalSpeed * Time.deltaTime));

        float percent = 0f;
        if (_state == PlayerState.run)
        {
            percent = 1; //* moveDirection.magnitude;
        }
        else if (_state == PlayerState.walk)
        {
            percent = 0.5f; //* moveDirection.magnitude;
        }

        _animator.SetFloat("Blend", percent, 0.1f, Time.deltaTime);
    }
}