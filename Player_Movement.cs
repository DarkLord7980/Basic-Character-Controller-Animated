using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player_Movement : MonoBehaviour
{
    public Transform groundCheck;
    bool runningPressed;

    CharacterController characterController;
    Vector3 velocity;
    Vector3 playerInput;

    public bool isGrounded;
    public LayerMask Ground;
    public float speed;
    public float walkSpeed;
    public float runSpeed;
    public float gravity = -9.81f;
    public float reducedGravity = -2f;
    public float radius = 0.2f;
    public float jumpHeight = 2f;
    float? lastGrounded;
    float? jumpButtonPressedTime;

    public float jumpButtonGracePeriod;

    Animator player_anim;

    [Header("Animations")]
    public float accelaration;
    public float deaccelaration;
    float x;
    float y;
    bool isJumping;
    bool isFalling;
    bool onGround;

    // Start is called before the first frame update
    void Awake()
    {
        characterController = GetComponent<CharacterController>();   
        player_anim = GetComponent<Animator>();  
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, radius, Ground);
        runningPressed = Input.GetKey(KeyCode.LeftShift);

        MyInput();
        AddGravity();
        HandleAnimations();
    }

    void MyInput()
    {
        playerInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
        speed = runningPressed ? runSpeed : walkSpeed;

        // this line is used in order to move correctly according to local pos and not according to world pos
        Vector3 moveVector = transform.TransformDirection(playerInput);

        characterController.Move(moveVector * speed * Time.deltaTime);

        if (isGrounded)
        {
            lastGrounded = Time.time;
            onGround = true;
            player_anim.SetBool("isGrounded", onGround);
            isJumping = false;
            player_anim.SetBool("isJumping", isJumping);
            isFalling = false;
            player_anim.SetBool("isFalling", isJumping);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
                isJumping = true;
                player_anim.SetBool("isJumping", isJumping);
            }

        }
        else
        {
            onGround = false;
            player_anim.SetBool("isGrounded", onGround);

            isFalling = true;
            player_anim.SetBool("isFalling", isFalling);
        }
    }


    void AddGravity()
    {
        if (isGrounded  && velocity.y < 0)
            velocity.y = reducedGravity * Time.deltaTime;
        else
            velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);
    }


    void HandleAnimations()
    {        

        float maximumRunVel = 2f;
        float maximumWalkVel = 1f;

        float currentMaxVelocity = runningPressed ? maximumRunVel : maximumWalkVel;

        if (isGrounded)
        {
            if (playerInput.x > 0 && x < currentMaxVelocity)
            {
                x += Time.deltaTime * accelaration;
            }

            if (playerInput.x < 0 && x > -currentMaxVelocity)
            {
                x -= Time.deltaTime * deaccelaration;
            }

            if (playerInput.z > 0 && y < currentMaxVelocity)
            {
                y += Time.deltaTime * accelaration;
            }

            if (playerInput.z > 0 && y > currentMaxVelocity)
            {
                y -= Time.deltaTime * deaccelaration;
            }

            if (playerInput.z < 0 && y > -currentMaxVelocity)
            {
                y -= Time.deltaTime * deaccelaration;
            }

        }else 
        {
            isFalling = true;
            player_anim.SetBool("isFalling", isFalling);
        }

        ResetAnimation();
         
        player_anim.SetFloat("XAxis", x);
        player_anim.SetFloat("YAxis", y);
    }

    void ResetAnimation()
    {
        if (playerInput.x == 0)
        {
            if (x > 0)
            {
                x -= Time.deltaTime * deaccelaration;
                if (x < 0)
                    x = 0f;
            }
            else if (x < 0)
            {
                x += Time.deltaTime * accelaration;
                if (x > 0)
                    x = 0f;
            }
        }

        if (playerInput.z == 0)
        {
            if (y > 0)
            {
                y -= Time.deltaTime * deaccelaration;
                if (y < 0)
                    y = 0f;
            }

            else if (y <= 0)
            {
                y += Time.deltaTime * accelaration;
                if (y > 0)
                    y = 0f;
            }
        }
    }

}
