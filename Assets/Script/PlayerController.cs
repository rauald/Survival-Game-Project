using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    static public bool isActivated = true;

    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private float crouchSpeed;

    private float applySpeed;

    [SerializeField]
    private float jumpForce;

    // 상태 변수
    private bool isWalk = false;
    private bool isRun = false;
    private bool isCrouch = false;
    private bool isGround = true;

    // 움직임 체크 변수
    private Vector3 lastPos;

    // 앚았을 때 얼마나 앉을지 결정하는 변수
    [SerializeField]
    private float crouchPosY;
    private float originPosY;
    private float applyCrouchPosY;

    // 땅 착지 여부
    private CapsuleCollider capsuleCollider;

    [SerializeField]
    private float lookSensitivity;                      // 민감도 | 속도

    [SerializeField]
    private float cameraRotationLimit;                  // 카메라 제한 각도
    private float currentCameraRotationX = 0f;          // X 각도 => 0도

    // 필요한 컴포넌트
    [SerializeField]
    private Camera theCamera;
    private Rigidbody myRigid;
    private GunController theGunController;
    private Crosshair theCrosshair;
    private StatusController theStatusController;


    // Start is called before the first frame update
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>();
        applySpeed = walkSpeed;
        theGunController = FindObjectOfType<GunController>();
        theCrosshair = FindObjectOfType<Crosshair>();
        theStatusController = FindObjectOfType <StatusController>();

        // 초기화
        originPosY = theCamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActivated && GameManager.canPlayerMove)
        {
            IsGround();
            TryJump();
            TryRun();
            TryCrouch();
            Move();
            MoveCheck();
            CameraRotation();
            CharacterRotation();
        }
    }

    // 앉기 시도
    private void TryCrouch()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }
    }

    // 앉기 동작
    private void Crouch()
    {
        isCrouch = !isCrouch;
        theCrosshair.CruchingAnimation(isCrouch);

        if (isCrouch)
        {
            applySpeed = crouchSpeed;
            applyCrouchPosY = crouchPosY;
        }
        else
        {
            applySpeed = walkSpeed;
            applyCrouchPosY = originPosY;
        }

        //theCamera.transform.localPosition = new Vector3(theCamera.transform.transform.localPosition.x, applyCrouchPosY, theCamera.transform.localPosition.z);
        StartCoroutine(CrouchCoroutline());
    }

    // 부드러운 동작 실행
    IEnumerator CrouchCoroutline()
    {
        float _posY = theCamera.transform.localPosition.y;
        int count = 0;

        while(_posY != applyCrouchPosY)
        {
            count++;
            _posY = Mathf.Lerp(_posY, applyCrouchPosY, 0.3f);
            theCamera.transform.localPosition = new Vector3(0, _posY, 0);
            if (count > 15) break;
            yield return null;
        }
        theCamera.transform.localPosition = new Vector3(0, applyCrouchPosY, 0f);
    }

    // 지면 체크
    private void IsGround()
    {
        // bounds = 콜리더 크기 || extents = 반값(하프) || +0.1f => 계단 or 대각선 같은 경우 땅에 안 닿을수도 있으니 여유를 더 준다
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.3f);
        theCrosshair.JumpingAnimation(!isGround);
    }

    // 점프 시도
    private void TryJump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGround && theStatusController.GetCurrentSP() > 0)
        {
            Jump();
        }
    }

    // 점프
    private void Jump()
    {
        // 앉은 상태에서 점프 시 앉은 상태 해제
        if (isCrouch) Crouch();
        theStatusController.DecreaseStamina(100);
        myRigid.velocity = transform.up * jumpForce;
    }

    // 달리기 시도
    private void TryRun()
    {
        if(Input.GetKey(KeyCode.LeftShift) && theStatusController.GetCurrentSP() > 0)
        {
            Running();
        }
        if(Input.GetKeyUp(KeyCode.LeftShift) || theStatusController.GetCurrentSP() <= 0)
        {
            RunningCancel();
        }
    }

    // 달리기 실행
    private void Running()
    {
        if (isCrouch) Crouch();

        theGunController.CancelFineSight();
        theStatusController.DecreaseStamina(10);
        isRun = true;
        theCrosshair.RunningAnimation(isRun);
        applySpeed = runSpeed;
    }
    
    // 달리기 취소
    private void RunningCancel()
    {
        isRun = false;
        theCrosshair.RunningAnimation(isRun);
        applySpeed = walkSpeed;
    }

    public bool GetRun()
    {
        return isRun;
    }

    // 움직임 실행
    private void Move()
    {
        // Horizontal => 좌, 우 화살키 (좌 : -1 || 우 : 1)
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        // Vertical   => 상, 하 화살키 (하 : -1 || 상 : 1)
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        // transform.right = > (1, 0, 0) * _moveDirX (좌 : -1 || 우 :1)
        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        // normalized 하는 이유 -> 합을 1로 만들어서 유니티 계산 속도를 빠르게 하기 위해
        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;
        
        // Time.deltaTime = > 0.016 / 1초에 60프레임 / 1초
        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
    }

    private void MoveCheck()
    {
        if(!isRun && !isCrouch && isGround)
        {
            // Distance(전 좌표, 현재 좌표) 의 거리
            if (Vector3.Distance(lastPos, transform.position) >= 0.01f) isWalk = true;
            else isWalk = false;

            theCrosshair.WalkingAnimation(isWalk);
            lastPos = transform.position;
        }
    }
    // 좌우 캐릭터 회전
    private void CharacterRotation()
    {
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
    }

    // 상하 카메라 회전
    private void CameraRotation()
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;

        // += 아래키면 위로
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

    private bool pauseCameraRotatiom = false;

    public IEnumerator TreeLookCoroutine(Vector3 _target)
    {
        pauseCameraRotatiom = true;

        Quaternion direction = Quaternion.LookRotation(_target - theCamera.transform.position);
        Vector3 eulerValue = direction.eulerAngles;
        float destionationX = eulerValue.x;

        while(Mathf.Abs(destionationX - currentCameraRotationX) >= 0.5f)
        {
            eulerValue = Quaternion.Lerp(theCamera.transform.localRotation, direction, 0.3f).eulerAngles;
            theCamera.transform.localRotation = Quaternion.Euler(eulerValue.x, 0f, 0f);
            currentCameraRotationX = theCamera.transform.localEulerAngles.x;
            yield return null;
        }

        pauseCameraRotatiom = false;
    }
}
