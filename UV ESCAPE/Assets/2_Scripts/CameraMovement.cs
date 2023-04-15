using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform ObjToFollow; // 따라갈 오브젝트
    public float followSpeed = 10f; // 따라가는 속도
    public float sensitivity = 100f; // 마우스 감도
    public float clampAngle = 70f; // 제한 각도

    // 마우스 인풋 받을 함수
    private float rotX;
    private float rotY;

    public Transform realCamera; // 카메라 정보
    public Vector3 dirNormalized; // 방향백터
    public Vector3 finalDir; // 최종 방향
    public float minDistance; // 최소 거리
    public float maxDistance; // 최대 거리
    public float fppDistance; // 1인칭 거리
    public float finalDistance; // 최종 거리
    public float smoothness = 10f;

    private bool _playerPov = false;
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        rotX = transform.localRotation.eulerAngles.x;
        rotY = transform.localRotation.eulerAngles.y;

        dirNormalized = realCamera.localPosition.normalized;
        finalDistance = realCamera.localPosition.magnitude;
    }

    
    void Update()
    {
        // 마우스 인풋 받고 넣기
        rotX += -(Input.GetAxis("Mouse Y")) * sensitivity * Time.deltaTime;
        rotY += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;

        // 각도 제한 시켜주기
        rotX = Mathf.Clamp(rotX, - clampAngle, clampAngle);
        
        Quaternion rot = Quaternion.Euler(rotX, rotY, 0);
        transform.rotation = rot;

        InputKey();
    }

    void LateUpdate()
    {
        // 오브젝트 따라가기
        transform.position = Vector3.MoveTowards(transform.position, ObjToFollow.position, followSpeed * Time.deltaTime);

        // 로컬 좌표 -> 월드 좌표
        finalDir = transform.TransformPoint(dirNormalized * maxDistance);
        
        // 물체 확인
        RaycastHit hit;

        if (_playerPov == true)
        {
            // Linecast로 종료지점 확인
            if(Physics.Linecast(transform.position, finalDir, out hit))
            {
                finalDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
            }
            else
            {
                finalDistance = maxDistance;
            }
        }
        else
        {
            finalDistance = fppDistance;
        }
        realCamera.localPosition = Vector3.Lerp(realCamera.localPosition, dirNormalized * finalDistance,
            Time.deltaTime * smoothness);
    }


    private int PovCnt;
    private void InputKey()
    {
        if (Input.GetKeyDown(KeyCode.V) && PovCnt == 0)
        {
            _playerPov = true;
            PovCnt++;
            Debug.Log("VVV");
        }
        else if (Input.GetKeyDown(KeyCode.V) && PovCnt == 1)
        {
            _playerPov = false;
            PovCnt = 0;
            Debug.Log("ReVVV");
        }
    }
}