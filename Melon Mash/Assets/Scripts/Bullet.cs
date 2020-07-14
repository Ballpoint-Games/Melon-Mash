using UnityEngine;

public class Bullet : MonoBehaviour
{
    private SpriteRenderer m_Renderer;

    [Tooltip("The lifetime in seconds of this bullet")] public float Lifetime = 3.0f;
    public float velocity = 10.0f;

    private void Awake()
    {
        m_Renderer = GetComponent<SpriteRenderer>();

        Destroy(gameObject, Lifetime);
    }

    private void Update()
    {
        transform.Translate(Vector2.right * velocity * Time.deltaTime * (m_Renderer.flipX ? -1.0f : 1.0f));
    }
}
