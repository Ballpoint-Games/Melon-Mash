using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
public class PlayerMovement : MonoBehaviour
{
    private InputManager m_Input;
    private Vector2 m_Movement;
    private bool m_Jump, m_JumpLast;
    private bool m_Switch;

    private CharacterController2D m_Controller;
    private Vector2 m_Velocity;

    private SpriteRenderer m_Renderer;

    private float m_RunningAcceleration, m_RunningDeceleration;
    private float m_SkateboardingAcceleration, m_SkateboardingDeceleration;

    private float m_JumpVelocity;
    private float m_TimeSinceLastGround, m_LastJumpTime;

    [Header("Movement Options")]
    [Min(0.0001f)] public float RunningAccelerationTime = 0.1f;
    [Min(0.0001f)] public float RunningDecelerationTime = 0.05f;
    [Min(0.0001f)] public float MaxRunningSpeed = 3.5f;
    [Min(0.0001f)] public float SkateboardingAccelerationTime = 1.1f;
    [Min(0.0001f)] public float SkateboardingDecelerationTime = 0.8f;
    [Min(0.0001f)] public float MaxSkateboardingSpeed = 10.0f;
    public PlayerMovementMode MovementMode = PlayerMovementMode.Running;

    [Space]

    [Header("Jumping Options")]
    public float JumpHeight = 3.0f;
    public float CoyoteTime = 0.1f;
    public float JumpTime = 0.1f;

    [Space]

    [Header("Gravity Options")]
    [Range(0.0f, 2.0f)] public float GravityScale = 1.0f;
    [Range(0.0f, 5.0f)] public float LowJumpMultiplier = 2.5f;
    [Range(0.0f, 5.0f)] public float FallMultiplier = 2.5f;

    private void Awake()
    {
        m_Input = new InputManager();

        m_Input.Player.Movement.performed += ctx => m_Movement = ctx.ReadValue<Vector2>();
        m_Input.Player.Movement.canceled += ctx => m_Movement = Vector2.zero;

        m_Input.Player.Jump.started += ctx => m_Jump = ctx.ReadValueAsButton();
        m_Input.Player.Jump.canceled += ctx => m_Jump = false;

        m_Input.Player.Switch.started += ctx => m_Switch = true;

        m_Controller = GetComponent<CharacterController2D>();
        m_Velocity = m_Controller.velocity;

        m_Renderer = GetComponent<SpriteRenderer>();

        m_RunningAcceleration = MaxRunningSpeed / RunningAccelerationTime;
        m_RunningDeceleration = MaxRunningSpeed / RunningDecelerationTime;

        m_SkateboardingAcceleration = MaxSkateboardingSpeed / SkateboardingAccelerationTime;
        m_SkateboardingDeceleration = MaxSkateboardingSpeed / SkateboardingDecelerationTime;

        m_JumpVelocity = Mathf.Sqrt(Mathf.Abs(Physics2D.gravity.y * GravityScale * JumpHeight * 2));
        m_TimeSinceLastGround = CoyoteTime;
        m_LastJumpTime = JumpTime;
    }

    private void Update()
    {
        float gravity = GravityScale;

        if (m_Controller.collisionState.left || m_Controller.collisionState.right)
            m_Velocity.x = 0.0f;

        if (m_Controller.collisionState.above || m_Controller.collisionState.below)
            m_Velocity.y = 0.0f;

        switch (MovementMode)
        {
            case PlayerMovementMode.Running:
                if (m_Switch)
                {
                    MovementMode = PlayerMovementMode.Skateboarding;
                    break;
                }

                Run();
                break;

            case PlayerMovementMode.Skateboarding:
                if (m_Switch)
                {
                    MovementMode = PlayerMovementMode.Running;
                    break;
                }

                Skateboard();
                break;
        }

        if (m_Jump && !m_JumpLast)
            m_LastJumpTime = 0.0f;
        else
            m_LastJumpTime += Time.deltaTime;

        if (m_Controller.isGrounded)
        {
            m_TimeSinceLastGround = 0.0f;
            m_Velocity.y = 0.0f;
        }
        else
        {
            if (m_Velocity.y < 0)
                gravity *= FallMultiplier;
            else if (m_Velocity.y > 0 && !m_Jump)
                gravity *= LowJumpMultiplier;

            m_TimeSinceLastGround += Time.deltaTime;
        }

        if (m_LastJumpTime <= JumpTime && m_TimeSinceLastGround <= CoyoteTime)
            m_Velocity.y = m_JumpVelocity;

        m_Velocity.y += Physics2D.gravity.y * Time.deltaTime * gravity;
        m_Controller.Move(m_Velocity * Time.deltaTime);

        if (m_Velocity.x > 0)
            m_Renderer.flipX = false;
        else if (m_Velocity.x < 0)
            m_Renderer.flipX = true;
    }

    private void LateUpdate()
    {
        m_JumpLast = m_Jump;
        m_Switch = false;
    }

    private void Run()
    {
        float movement = m_Movement.x != 0.0f ? Mathf.Sign(m_Movement.x) : 0.0f;

        if (movement != 0.0f && Mathf.Abs(m_Velocity.x) < MaxRunningSpeed)
            m_Velocity.x += m_RunningAcceleration * Time.deltaTime * movement;
        else
        {
            if (m_Velocity.x > m_RunningDeceleration * Time.deltaTime)
                m_Velocity.x -= m_RunningDeceleration * Time.deltaTime;
            else if (m_Velocity.x < -m_RunningDeceleration * Time.deltaTime)
                m_Velocity.x += m_RunningDeceleration * Time.deltaTime;
            else
                m_Velocity.x = 0.0f;
        }

        Mathf.Clamp(m_Velocity.x, -MaxRunningSpeed, MaxRunningSpeed);
    }

    private void Skateboard()
    {
        float movement = m_Movement.x != 0.0f ? Mathf.Sign(m_Movement.x) : 0.0f;

        if (movement != 0.0f && Mathf.Abs(m_Velocity.x) < MaxSkateboardingSpeed)
            m_Velocity.x += m_SkateboardingAcceleration * Time.deltaTime * movement;
        else
        {
            if (m_Velocity.x > m_SkateboardingDeceleration * Time.deltaTime)
                m_Velocity.x -= m_SkateboardingDeceleration * Time.deltaTime;
            else if (m_Velocity.x < -m_SkateboardingDeceleration * Time.deltaTime)
                m_Velocity.x += m_SkateboardingDeceleration * Time.deltaTime;
            else
                m_Velocity.x = 0.0f;
        }

        Mathf.Clamp(m_Velocity.x, -MaxSkateboardingSpeed, MaxSkateboardingSpeed);
    }

    private void OnEnable()
    {
        m_Input.Enable();
    }

    private void OnDisable()
    {
        m_Input.Disable();
    }
}

public enum PlayerMovementMode
{
    Running,
    Skateboarding
}