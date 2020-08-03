using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    //Input variables
    private InputManager m_Input;

    //The current list of bullets
    private List<GameObject> m_Bullets;

    [Tooltip("The default bullet sprite")] public GameObject BulletPrefab;
    [Tooltip("The maximum number of bullets"), Min(0)] public int MaxBullets = 3;

    private void Awake()
    {
        //Set up inputs
        m_Input = new InputManager();

        //Setup shooting variable
        m_Input.Player.Shoot.started += ctx => Shoot();

        //Create a new list of bullets
        m_Bullets = new List<GameObject>(MaxBullets);
    }

    private void Update()
    {
        //Remove destroyed bullets
        for (int i = m_Bullets.Count - 1; i >= 0; i--)
        {
            if (m_Bullets[i] == null)
                m_Bullets.RemoveAt(i);
        }
    }

    private void Shoot()
    {
        //Spawn a new bullet if possible
        if (m_Bullets.Count < MaxBullets)
        {
            GameObject bullet = Instantiate(BulletPrefab, transform.position, Quaternion.identity);
            bullet.GetComponent<SpriteRenderer>().flipX = GetComponent<SpriteRenderer>().flipX;
            m_Bullets.Add(bullet);
        }
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
