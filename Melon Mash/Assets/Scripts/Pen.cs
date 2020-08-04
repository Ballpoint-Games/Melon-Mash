using UnityEngine;

public class Pen : MonoBehaviour
{
    //Used to flip the sprite
    private SpriteRenderer m_Renderer;

    [Tooltip("The lifetime in seconds of this pen")] public float Lifetime = 3.0f;
    [Tooltip("How fast this pen moves")] public float velocity = 10.0f;

    private void Awake()
    {
        //Set up the sprite renderer
        m_Renderer = GetComponent<SpriteRenderer>();

        //Destroy this bullet after a certain number of seconds
        Destroy(gameObject, Lifetime);
    }

    private void Update()
    {
        //While still alive move in the direction this bullet is facing
        transform.Translate(Vector2.right * velocity * Time.deltaTime * (m_Renderer.flipX ? -1.0f : 1.0f));
    }
}
