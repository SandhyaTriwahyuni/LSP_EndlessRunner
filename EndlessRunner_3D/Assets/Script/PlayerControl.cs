using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerControl : MonoBehaviour
{
    private CharacterController _characterController;
    private Vector3 _direction;
    public float ForwardSpeed;
    public float MaxSpeed;

    private int _desiredLane = 1; // 0: Kiri, 1: Tengah, 2: Kanan
    public float LaneDistance;
    public float LaneChangeSpeed;

    private Vector3 _targetPosition;
    private Vector3 _initialPosition;

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

    // PowerUp
    //public GameObject bulletPrefab;
    //public Transform bulletSpawnPoint;
    //public float bulletSpeed = 20f;
    //public int bulletsToShoot = 8;

    //Distance
    public TextMeshProUGUI distanceTextGO; // UI Text untuk menampilkan jarak
    public TextMeshProUGUI distanceTextWin;
    private Vector3 lastPosition; // Posisi terakhir pemain
    private float distanceTraveled; // Jarak yang sudah ditempuh

    public HealthManager HealthManager;
    public ScoreSystem ScoreSystem;
    public PlayerManager PlayerManager;

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _initialPosition = transform.position;
        _originalHeight = _characterController.height;
        _originalCenterY = _characterController.center.y;

        HealthManager = FindObjectOfType<HealthManager>();
        PlayerManager = FindObjectOfType<PlayerManager>();

        //bulletSpawnPoint.parent = transform;

        lastPosition = transform.position; // Menginisialisasi posisi awal
        distanceTraveled = 0f; // Mengatur jarak awal ke 0
    }

    void Update()
    {
        // Periksa apakah game sudah dimulai dan belum game over
        if (PlayerManager.IsGameStarted && !PlayerManager.GameOver)
        {
            _animator.SetBool("isStarted", true);

            //IncreaseSpeed
            if (ForwardSpeed < MaxSpeed)
            {
                ForwardSpeed += 0.1f * Time.deltaTime;
            }

            if (JumpForce < MaxJumpForce)
            {
                JumpForce += 0.01f * Time.deltaTime;
            }

            _direction.z = ForwardSpeed;
            _direction.y += Gravity * Time.deltaTime;

            if (_characterController.isGrounded && isJumping)
            {
                isJumping = false;
                ReturnToRunningAnimation();
            }

            // Memeriksa input dari keyboard
            HandleKeyboardInput();

            // Menghitung posisi target berdasarkan jalur yang diinginkan
            Vector3 lanePosition = transform.position.z * transform.forward + transform.position.y * transform.up;

            if (_desiredLane == 0)
            {
                lanePosition += Vector3.left * LaneDistance;
            }
            else if (_desiredLane == 2)
            {
                lanePosition += Vector3.right * LaneDistance;
            }

            _targetPosition = new Vector3(lanePosition.x, transform.position.y, transform.position.z);
        }
        else
        {
            // Game over atau belum dimulai, hentikan pergerakan
            _direction.z = 0;

            // Start game with Enter key
            if (Input.GetKeyDown(KeyCode.Return) && !PlayerManager.IsGameStarted)
            {
                PlayerManager.IsGameStarted = true;
                Destroy(PlayerManager.StartingText);
            }

        }

        float distanceThisFrame = Vector3.Distance(lastPosition, transform.position);
        distanceTraveled += distanceThisFrame; // Menambahkan jarak baru ke total
        lastPosition = transform.position; // Memperbarui posisi terakhir

        // Menampilkan jarak di UI
        distanceTextGO.text = "Distance: " + distanceTraveled.ToString("F2") + " m";
        distanceTextWin.text = "Distance: " + distanceTraveled.ToString("F2") + " m";
    }

    private void FixedUpdate()
    {
        if (PlayerManager.IsGameStarted && !PlayerManager.GameOver)
        {
            Vector3 currentPosition = transform.position;
            Vector3 nextPosition = Vector3.Lerp(currentPosition, _targetPosition, LaneChangeSpeed * Time.deltaTime);
            Vector3 moveDirection = nextPosition - currentPosition;

            _characterController.Move(moveDirection + _direction * Time.deltaTime);
        }
    }

    // Menangani input keyboard
    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            OnSwipeLeft();
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            OnSwipeRight();
        }
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            OnSwipeUp();
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            OnSwipeDown();
        }
    }

    // Tindakan ketika input kiri
    void OnSwipeLeft()
    {
        if (PlayerManager.IsGameStarted && !PlayerManager.GameOver)
        {
            _desiredLane--;
            if (_desiredLane < 0)
                _desiredLane = 0;
        }
    }

    // Tindakan ketika input kanan
    void OnSwipeRight()
    {
        if (PlayerManager.IsGameStarted && !PlayerManager.GameOver)
        {
            _desiredLane++;
            if (_desiredLane > 2)
                _desiredLane = 2;
        }
    }

    // Tindakan ketika input atas (untuk melompat)
    void OnSwipeUp()
    {
        if (PlayerManager.IsGameStarted && !PlayerManager.GameOver)
        {
            Jump();
        }
    }

    // Tindakan ketika input bawah (untuk slide)
    void OnSwipeDown()
    {
        if (PlayerManager.IsGameStarted && !PlayerManager.GameOver)
        {
            if (!isSliding)
            {
                _animator.SetTrigger("Slide");
                StartCoroutine(SlideCoroutine());
            }
        }
    }

    private void Jump()
    {
        if (PlayerManager.IsGameStarted && !PlayerManager.GameOver)
        {
            if (_characterController.isGrounded)
            {
                _direction.y = JumpForce;
                isJumping = true;
                _animator.SetTrigger("Jump");
            }
        }
    }

    private void ReturnToRunningAnimation()
    {
        _targetPosition.z += transform.position.z - _initialPosition.z;
    }

    private IEnumerator SlideCoroutine()
    {
        if (PlayerManager.IsGameStarted && !PlayerManager.GameOver)
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

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.tag == "Obstacle")
        {
            if (PlayerManager.IsGameStarted && !PlayerManager.GameOver)
            {
                HealthManager.ObstacleHit();
                ScoreSystem.ObstacleHit();
                _animator.SetTrigger("Stumble");
                Destroy(hit.gameObject);
            }
        }
    }

    //public void ActivatePowerUp()
    //{
    //    if (PlayerManager.IsGameStarted && !PlayerManager.GameOver)
    //    {
    //        StartCoroutine(ShootBullets());
    //        SoundManager.Instance.PlaySound3D("Shoot", transform.position);
    //    }
    //}

    //private IEnumerator ShootBullets()
    //{
    //    for (int i = 0; i < bulletsToShoot; i++)
    //    {
    //        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
    //        Rigidbody rb = bullet.GetComponent<Rigidbody>();
    //        rb.velocity = bulletSpawnPoint.forward * bulletSpeed;

    //        yield return new WaitForSeconds(0.1f);
    //    }
    //}
}
