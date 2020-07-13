using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
public class PlayerMovement : MonoBehaviour
{
    private InputManager m_Input;
    private Vector2 m_Movement;

    private CharacterController2D m_Controller;
    private Vector2 m_Velocity;

    [Header("Movement Options")]
    public float RunningSpeed = 3.5f;
    public PlayerMovementMode MovementMode = PlayerMovementMode.Running;

    [Space]

    [Header("Gravity Options")]
    [Range(0.0f, 2.0f)] public float GravityScale = 1.0f;

    private void Awake()
    {
        m_Input = new InputManager();

        m_Input.Player.Movement.performed += ctx => m_Movement = ctx.ReadValue<Vector2>();
        m_Input.Player.Movement.canceled += ctx => m_Movement = Vector2.zero;

        m_Controller = GetComponent<CharacterController2D>();
        m_Velocity = m_Controller.velocity;
    }

    private void Update()
    {
        if (m_Controller.collisionState.left || m_Controller.collisionState.right)
            m_Velocity.x = 0.0f;

        if (m_Controller.collisionState.above || m_Controller.collisionState.below)
            m_Velocity.y = 0.0f;

        switch (MovementMode)
        {
            case PlayerMovementMode.Running:
                Run();
                break;

            case PlayerMovementMode.Skateboarding:
                //TODO Implement Skateboarding
                Debug.LogWarning("Skateboarding is not implemented yet!");
                MovementMode = PlayerMovementMode.Running;
                break;
        }

        if (m_Controller.isGrounded)
        {
            m_Velocity.y = 0.0f;
        }

        m_Velocity += Physics2D.gravity * Time.deltaTime * GravityScale;
        m_Controller.Move(m_Velocity * Time.deltaTime);
    }

    private void Run()
    {
        m_Velocity.x = (m_Movement.x != 0 ? Mathf.Sign(m_Movement.x) : 0.0f) * RunningSpeed;
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