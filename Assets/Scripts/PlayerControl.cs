using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    public float speed = 5f;
    public float sprintSpeed = 10f;
    [SerializeField] private float jumpForce = 5f;
    //[SerializeField] private float jumpAcceleration = 5f;
    private bool isGrounded;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;
    private Vector3 jumpVelocity;
    private AudioSource audioSource;
    public AudioClip sprintSound;
    public AudioClip jumpSound; // ジャンプの音
    private bool isSprinting;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        Vector3 move = new Vector3(x, 0);
        Move(move);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        //ApplyJumpAcceleration();
        CheckGround();
        HandleSprinting();
    }

    private void Move(Vector3 direction)
    {
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (right * direction.x).normalized;
        float currentSpeed = isSprinting ? sprintSpeed : speed;

        rb.MovePosition(rb.position + moveDirection * currentSpeed * Time.deltaTime);
    }

    private void Jump()
    {
        jumpVelocity = Vector3.up * jumpForce;
        rb.velocity = new Vector3(rb.velocity.x, jumpVelocity.y, rb.velocity.z);
        audioSource.PlayOneShot(jumpSound); // ジャンプ音を再生
    }

    //private void ApplyJumpAcceleration()
    //{
    //    if (!isGrounded)
    //    {
    //        // 下方向の加速
    //        if (rb.velocity.y < 0)
    //        {
    //            rb.AddForce(Vector3.down * (jumpAcceleration / 2), ForceMode.Acceleration);
    //        }
    //        // 上方向の加速（力を半分にする）
    //        else if (rb.velocity.y > 0)
    //        {
    //            rb.AddForce(Vector3.up * (jumpAcceleration / 2), ForceMode.Acceleration);
    //        }
    //    }
    //}

    private void CheckGround()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }

    private void HandleSprinting()
    {
        bool sprintInput = Input.GetKey(KeyCode.LeftShift) || Input.GetButton("Fire1"); ;
        if (sprintInput && !isSprinting)
        {
            isSprinting = true;
            audioSource.PlayOneShot(sprintSound);
        }
        else if (!sprintInput && isSprinting)
        {
            isSprinting = false;
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("goal"))
    //    {
    //        SceneManager.LoadScene("ClearScene");
    //    }
    //}
}
