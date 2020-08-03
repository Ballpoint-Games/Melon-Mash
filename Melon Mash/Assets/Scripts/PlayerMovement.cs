using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
public class PlayerMovement : MonoBehaviour
{
    //Input Variables
    private InputManager m_Input;
    private Vector2 m_Movement;
    private bool m_Jump, m_JumpLast;
    private bool m_Switch;

    //Movement variables
    private CharacterController2D m_Controller;
    private Vector2 m_Velocity;

    //Used to flip the sprite if the velocity changes sign
    private SpriteRenderer m_Renderer;

    //Used to add movement curves and make the character feel organic
    private float m_RunningAcceleration, m_RunningDeceleration;
    private float m_SkateboardingAcceleration, m_SkateboardingDeceleration;

    //The velocity used to jump
    private float m_JumpVelocity;
    //Used the add coyote time and last jump time, which help improve game feel.
    private float m_TimeSinceLastGround, m_LastJumpTime;

    [Header("Movement Options")]
    [Min(0.0001f), Tooltip("How long it takes the player to accelerate to max speed from idle while running")] public float RunningAccelerationTime = 0.1f;
    [Min(0.0001f), Tooltip("How long it takes the player to decelerate to idle from max speed while running")] public float RunningDecelerationTime = 0.05f;
    [Min(0.0001f), Tooltip("The max speed while running")] public float MaxRunningSpeed = 3.5f;
    [Min(0.0001f), Tooltip("How long it takes the player to accelerate to max speed from idle while skateboarding")] public float SkateboardingAccelerationTime = 1.1f;
    [Min(0.0001f), Tooltip("How long it takes the player to decelerate to idle from max speed while skateboarding")] public float SkateboardingDecelerationTime = 0.8f;
    [Min(0.0001f), Tooltip("The max speed while skateboarding")] public float MaxSkateboardingSpeed = 10.0f;
    public PlayerMovementMode MovementMode = PlayerMovementMode.Running;

    [Space]

    [Header("Jumping Options")]
    [Min(0.0001f), Tooltip("The maximum jump height")] public float JumpHeight = 3.0f;
    [Min(0.0001f), Tooltip("How late the player can still press jump after leaving the ground")] public float CoyoteTime = 0.1f;
    [Min(0.0001f), Tooltip("How early the player can press jump before hitting the ground")] public float JumpTime = 0.1f;

    [Space]

    [Header("Gravity Options")]
    [Range(0.0f, 2.0f), Tooltip("How much gravity affects this object")] public float GravityScale = 1.0f;
    [Range(0.0f, 5.0f), Tooltip("How much extra gravity affects this object when rising and releasing the jump button")] public float LowJumpMultiplier = 2.5f;
    [Range(0.0f, 5.0f), Tooltip("How much extra gravity affects this object when falling")] public float FallMultiplier = 2.5f;

    private void Awake()
    {
        //Set up inputs
        m_Input = new InputManager();

        //Set up movement input variables
        m_Input.Player.Movement.performed += ctx => m_Movement = ctx.ReadValue<Vector2>();
        m_Input.Player.Movement.canceled += ctx => m_Movement = Vector2.zero;

        //Set up jumping input variables
        m_Input.Player.Jump.started += ctx => m_Jump = ctx.ReadValueAsButton();
        m_Input.Player.Jump.canceled += ctx => m_Jump = false;

        //Set up movement switching variables
        m_Input.Player.Switch.started += ctx => m_Switch = true;

        //Set up the character controller
        m_Controller = GetComponent<CharacterController2D>();
        m_Velocity = m_Controller.velocity;

        //Set up the sprite renderer
        m_Renderer = GetComponent<SpriteRenderer>();

        //Set up acceleration and deceleration amounts
        m_RunningAcceleration = MaxRunningSpeed / RunningAccelerationTime;
        m_RunningDeceleration = MaxRunningSpeed / RunningDecelerationTime;

        m_SkateboardingAcceleration = MaxSkateboardingSpeed / SkateboardingAccelerationTime;
        m_SkateboardingDeceleration = MaxSkateboardingSpeed / SkateboardingDecelerationTime;

        //Set the jump velocity using the following equation: v^2 = 2ax, where v is the final velocity, 2 is the acceleration, and x is the change in distance
        m_JumpVelocity = Mathf.Sqrt(Mathf.Abs(Physics2D.gravity.y * GravityScale * JumpHeight * 2));
        
        //Prevent the player from jumping right when spawning
        m_TimeSinceLastGround = CoyoteTime;
        m_LastJumpTime = JumpTime;
    }

    private void Update()
    {
        //Used for crisp jumping
        float gravity = GravityScale;

        //Cancel velocity when colliding with wall
        if (m_Controller.collisionState.left || m_Controller.collisionState.right)
            m_Velocity.x = 0.0f;

        if (m_Controller.collisionState.above || m_Controller.collisionState.below)
            m_Velocity.y = 0.0f;

        //Change the movement method depending on the current movement mode
        switch (MovementMode)
        {
            case PlayerMovementMode.Running:
                //Switch to Skateboarding
                if (m_Switch)
                {
                    MovementMode = PlayerMovementMode.Skateboarding;
                    break;
                }

                //Run
                Run();
                break;

            case PlayerMovementMode.Skateboarding:
                //Switch to Running
                if (m_Switch)
                {
                    MovementMode = PlayerMovementMode.Running;
                    break;
                }

                //Skateboard
                Skateboard();
                break;
        }

        //Set up last jump time
        if (m_Jump && !m_JumpLast)
            m_LastJumpTime = 0.0f;
        else
            m_LastJumpTime += Time.deltaTime;

        //Check if the player is grounded and prevent them from gaining momentum
        if (m_Controller.isGrounded)
        {
            m_TimeSinceLastGround = 0.0f;
            m_Velocity.y = 0.0f;
        }
        else
        {
            //Check if the player is falling
            if (m_Velocity.y < 0)
                gravity *= FallMultiplier;
            //Check if the player isn't holding the jump button
            else if (m_Velocity.y > 0 && !m_Jump)
                gravity *= LowJumpMultiplier;

            m_TimeSinceLastGround += Time.deltaTime;
        }

        //Check for jumping
        if (m_LastJumpTime <= JumpTime && m_TimeSinceLastGround <= CoyoteTime)
            m_Velocity.y = m_JumpVelocity;

        //Add gravity
        m_Velocity.y += Physics2D.gravity.y * Time.deltaTime * gravity;

        //Move the player
        m_Controller.Move(m_Velocity * Time.deltaTime);

        //Flip the sprite depending on the sign
        if (m_Velocity.x > 0)
            m_Renderer.flipX = false;
        else if (m_Velocity.x < 0)
            m_Renderer.flipX = true;
    }

    private void LateUpdate()
    {
        //Reset input variables
        m_JumpLast = m_Jump;
        m_Switch = false;
    }

    private void Run()
    {
        //Get the current movement input
        float movement = m_Movement.x != 0.0f ? Mathf.Sign(m_Movement.x) : 0.0f;

        //Accelerate
        if (movement != 0.0f && Mathf.Abs(m_Velocity.x) < MaxRunningSpeed)
            m_Velocity.x += m_RunningAcceleration * Time.deltaTime * movement;
        else
        {
            //Decelerate
            if (m_Velocity.x > m_RunningDeceleration * Time.deltaTime)
                m_Velocity.x -= m_RunningDeceleration * Time.deltaTime;
            else if (m_Velocity.x < -m_RunningDeceleration * Time.deltaTime)
                m_Velocity.x += m_RunningDeceleration * Time.deltaTime;
            //Stop
            else
                m_Velocity.x = 0.0f;
        }

        //Clamp the velocity to the max speed
        Mathf.Clamp(m_Velocity.x, -MaxRunningSpeed, MaxRunningSpeed);
    }

    private void Skateboard()
    {
        //Get the current movement input
        float movement = m_Movement.x != 0.0f ? Mathf.Sign(m_Movement.x) : 0.0f;

        //Accelerate
        if (movement != 0.0f && Mathf.Abs(m_Velocity.x) < MaxSkateboardingSpeed)
            m_Velocity.x += m_SkateboardingAcceleration * Time.deltaTime * movement;
        else
        {
            //Decelerate
            if (m_Velocity.x > m_SkateboardingDeceleration * Time.deltaTime)
                m_Velocity.x -= m_SkateboardingDeceleration * Time.deltaTime;
            else if (m_Velocity.x < -m_SkateboardingDeceleration * Time.deltaTime)
                m_Velocity.x += m_SkateboardingDeceleration * Time.deltaTime;
            //Stop
            else
                m_Velocity.x = 0.0f;
        }

        //Clamp the velocity to the max speed
        Mathf.Clamp(m_Velocity.x, -MaxSkateboardingSpeed, MaxSkateboardingSpeed);
    }


    //Enable and disable movement
    private void OnEnable()
    {
        m_Input.Enable();
    }

    private void OnDisable()
    {
        m_Input.Disable();
    }
}

//Available movement modes
public enum PlayerMovementMode
{
    Running,
    Skateboarding
}