using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dummy1 : MonoBehaviour
{
    private CharacterController _characterController;
    private Vector3 _direction;
    public float ForwardSpeed;
    public float MaxSpeed;

    private int _desiredLane = 1; // 0: Kiri, 1: Tengah, 2: Kanan
    public float LaneDistance;
    public float LaneChangeSpeed;

    public float JumpForce;
    public float MaxJumpForce;

    public float Gravity = -10;
    private bool isJumping = false;

    private Animator _animator;

    public float NormalHeight;
    public float SlideHeight;
    private float _originalHeight;
    private float _originalCenterY;

     private bool isSliding = false;

    private Vector3 _targetPosition;
    private Vector3 _initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _initialPosition = transform.position;
        _originalHeight = _characterController.height;
        _originalCenterY = _characterController.center.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (_characterController.isGrounded && isJumping)
        {
            isJumping = false;
            ReturnToRunningAnimation();
        }

        HandleKeyboardInput();
    }

    private void FixedUpdate()
    {
        Vector3 currentPosition = transform.position;
        Vector3 nextPosition = Vector3.Lerp(currentPosition, _targetPosition, LaneChangeSpeed * Time.deltaTime);
        Vector3 moveDirection = nextPosition - currentPosition;

        _characterController.Move(moveDirection + _direction * Time.deltaTime);
    }
    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            OnSwipeLeft();
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            OnSwipeRight();
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            OnSwipeUp();
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            OnSwipeDown();
    }

    private void IncreaseSpeed()
    {
        if (ForwardSpeed < MaxSpeed)
            ForwardSpeed += 0.1f * Time.deltaTime;

        if (JumpForce < MaxJumpForce)
            JumpForce += 0.01f * Time.deltaTime;
    }

    void OnSwipeLeft()
    {
        if (PlayerManager.IsGameStarted && !PlayerManager.GameOver)
        {
            _desiredLane = Mathf.Max(0, _desiredLane - 1);
        }
    }

    void OnSwipeRight()
    {
        if (PlayerManager.IsGameStarted && !PlayerManager.GameOver)
        {
            _desiredLane = Mathf.Min(2, _desiredLane + 1);
        }
    }

    void OnSwipeUp()
    {
        if (PlayerManager.IsGameStarted && !PlayerManager.GameOver)
            Jump();
    }

    void OnSwipeDown()
    {
        if (PlayerManager.IsGameStarted && !PlayerManager.GameOver && !isSliding)
        {
            _animator.SetTrigger("Slide");
            StartCoroutine(SlideCoroutine());
        }
    }

    private void Jump()
    {
        if (_characterController.isGrounded)
        {
            _direction.y = JumpForce;
            isJumping = true;
            _animator.SetTrigger("Jump");
        }
    }

    private void ReturnToRunningAnimation()
    {
        _targetPosition.z += transform.position.z - _initialPosition.z;
    }

    private IEnumerator SlideCoroutine()
    {
        isSliding = true;
        _characterController.height = SlideHeight;
        _characterController.center = new Vector3(_characterController.center.x, 0.4f, _characterController.center.z);
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        _characterController.height = _originalHeight;
        _characterController.center = new Vector3(_characterController.center.x, _originalCenterY, _characterController.center.z);
        isSliding = false;
    }

}
