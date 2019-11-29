using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FX : MonoBehaviour
{
    [Header("VisDev")]
    public UnityEvent m_OnSpawn = new UnityEvent();

    [Header("Data")]
    public float m_Duration;


    private float m_counter;
    private FXManager m_manager;

    public void Spawn(Vector3 pos, float angle, FXManager manager)
    {
        transform.eulerAngles = Vector3.forward * (angle + 90);
        Spawn(pos, manager);
    }

    public void Spawn(Vector3 pos, FXManager manager)
    {
        transform.position = pos;
        m_manager = manager;
        gameObject.SetActive(true);
        m_counter = m_Duration;
        m_OnSpawn?.Invoke();
    }

    public void Despawn()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        m_counter -= Time.deltaTime;
        if (m_counter <= 0)
        {
            m_manager.DespawnFX(this);
        }
    }
}
