using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    private InputManager m_Input;

    private List<GameObject> m_Bullets;

    public GameObject BulletPrefab;
    [Min(0)] public int MaxBullets = 3;

    private void Awake()
    {
        m_Input = new InputManager();

        m_Input.Player.Shoot.started += ctx => Shoot();

        m_Bullets = new List<GameObject>(MaxBullets);
    }

    private void Update()
    {
        for (int i = m_Bullets.Count - 1; i >= 0; i--)
        {
            if (m_Bullets[i] == null)
                m_Bullets.RemoveAt(i);
        }
    }

    private void Shoot()
    {
        if (m_Bullets.Count < MaxBullets)
        {
            GameObject bullet = Instantiate(BulletPrefab, transform.position, Quaternion.identity);
            bullet.GetComponent<SpriteRenderer>().flipX = GetComponent<SpriteRenderer>().flipX;
            m_Bullets.Add(bullet);
        }
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
