using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sprintSpeedMultiplier = 1.5f;
    public float sprintStamina = 100f;
    public float sprintStaminaDrainRate = 10f;
    public float sprintStaminaRegenRate = 5f;
    public float jumpForce = 5f;
    public float gravity = 9.8f;

    private bool isJumping;
    private bool isSprinting;
    private CharacterController characterController;
    private Transform cameraTransform;
    private float verticalVelocity;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);
        movement = cameraTransform.TransformDirection(movement);
        movement.y = 0f;
        movement.Normalize();

        if (isSprinting && sprintStamina <= 0f)
        {
            isSprinting = false;
        }

        if (!isSprinting && sprintStamina >= 100f)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                isSprinting = true;
            }
        }

        if (isSprinting && sprintStamina > 0f)
        {
            movement *= moveSpeed * sprintSpeedMultiplier;
            sprintStamina -= sprintStaminaDrainRate * Time.deltaTime;

            if (sprintStamina <= 0f)
            {
                sprintStamina = 0f;
                isSprinting = false;
            }
        }
        else
        {
            movement *= moveSpeed;
            isSprinting = false;
            sprintStamina += sprintStaminaRegenRate * Time.deltaTime;
            sprintStamina = Mathf.Clamp(sprintStamina, 0f, 100f);
        }

        // Apply gravity
        if (characterController.isGrounded)
        {
            verticalVelocity = -gravity * Time.deltaTime;
            isJumping = false;

            // Check for jump input
            if (Input.GetButtonDown("Jump"))
            {
                Jump();
            }
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        movement += Vector3.up * verticalVelocity;

        characterController.Move(movement * Time.deltaTime);
    }

    private void Jump()
    {
        isJumping = true;
        verticalVelocity = jumpForce;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
        }
    }
}
