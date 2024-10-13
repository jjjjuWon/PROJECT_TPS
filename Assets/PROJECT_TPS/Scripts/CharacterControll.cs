using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    private float horizontalInput;

    private float verticalInput;

    private Vector3 movement;

    private float verticalSpeed;
    public float yVelocity = 0;
    public float jumpSpeed = 10f;
    public float gravity = -20f;

    private Vector3 velocity;

    [Header("Settings")]
    [SerializeField] private float characterMovementSpeed;
    [SerializeField] private float rotationSpeed;

    [Header("Physics")]
    [SerializeField] private float checkSphereRadius;
    [SerializeField] private Vector3 checkSphereOffset;
    [SerializeField] private LayerMask groundLayer;

    private CameraControll cameraController;

    private Animator characterAnimator;

    private CharacterController characterController;

    private Quaternion desiredRotation;

    private Vector3 desiredMovementDir;

    private void Start()
    {
        cameraController = Camera.main.GetComponent<CameraControll>();
        characterAnimator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        var movementSpeed = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        movement = new Vector3(horizontalInput, 0.0f, verticalInput).normalized;
        desiredMovementDir = cameraController.YRotation * movement;

        if (characterAnimator != null)
        {
            bool isMoving = horizontalInput != 0 || verticalInput != 0;
            characterAnimator.SetBool("isWalking", isMoving);
        }

        if (characterController.isGrounded)
        {
            yVelocity = 0;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                yVelocity = jumpSpeed;
                // TODO: Jump Trigger 미구현
                if (characterAnimator != null)
                {
                    characterAnimator.SetTrigger("isJump");
                }
            }
        }

        if (IsGrounded())
        {
            verticalSpeed = -0.5f;
        }
        else
        {
            verticalSpeed += Physics.gravity.y * Time.deltaTime;
        }

        velocity = desiredMovementDir * characterMovementSpeed;
        velocity.y = verticalSpeed;

        if (movementSpeed > 0.0f)
        {
            desiredRotation = Quaternion.LookRotation(desiredMovementDir);
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation
            , rotationSpeed * Time.deltaTime);

        characterAnimator.SetFloat("MovementSpeed", movementSpeed, 0.2f, Time.deltaTime);

        yVelocity += (gravity * Time.deltaTime);
        velocity.y = yVelocity;
        characterController.Move(velocity * Time.deltaTime);
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(transform.TransformPoint(checkSphereOffset), checkSphereRadius, groundLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.3f);
        Gizmos.DrawSphere(transform.TransformPoint(checkSphereOffset), checkSphereRadius);
    }
}
