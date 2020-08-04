using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    //Input variables
    private InputManager m_Input;

    //The current list of pens
    private List<GameObject> m_Pens;

    [Tooltip("The pen")] public GameObject PenPrefab;
    [Tooltip("The maximum number of pens"), Min(0)] public int MaxPens = 3;

    private void Awake()
    {
        //Set up inputs
        m_Input = new InputManager();

        //Setup shooting variable
        m_Input.Player.Shoot.started += ctx => Shoot();

        //Create a new list of pens
        m_Pens = new List<GameObject>(MaxPens);
    }

    private void Update()
    {
        //Remove destroyed pens
        for (int i = m_Pens.Count - 1; i >= 0; i--)
        {
            if (m_Pens[i] == null)
                m_Pens.RemoveAt(i);
        }
    }

    private void Shoot()
    {
        //Spawn a new pen if possible
        if (m_Pens.Count < MaxPens)
        {
            GameObject pen = Instantiate(PenPrefab, transform.position, Quaternion.identity);
            pen.name = PenPrefab.name + " " + m_Pens.Count;
            pen.GetComponent<SpriteRenderer>().flipX = GetComponent<SpriteRenderer>().flipX;
            m_Pens.Add(pen);
        }
    }

    //Enable and disable input
    private void OnEnable()
    {
        m_Input.Enable();
    }

    private void OnDisable()
    {
        m_Input.Disable();
    }
}
