using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour
{
    [Header("Project References")]
    public FX m_Prefab;

    [Header("Data")]
    public float m_PoolSize;

    private List<FX> m_pool;
    private List<FX> m_active;

    private void Awake()
    {
        m_pool = new List<FX>();
        m_active = new List<FX>();

        for (int i = 0; i < m_PoolSize; i++)
        {
            FX e = Instantiate(m_Prefab);
            e.Despawn();
            m_pool.Add(e);
        }


    }

    public void DespawnFX(FX e)
    {
        if (m_active.Contains(e))
        {
            m_active.Remove(e);
            e.Despawn();
            m_pool.Add(e);
        }
        else
        {
            //TODO what do

        }
    }

    public void SpawnFX(Vector3 pos, float angle)
    {
        if (m_pool.Count <= 0)
            return;

        FX e = m_pool[0];

        m_pool.RemoveAt(0);


        e.Spawn(pos, angle, this);

        m_active.Add(e);
    }

    public void SpawnFX(Vector3 pos)
    {
        if (m_pool.Count <= 0)
            return;

        FX e = m_pool[0];

        m_pool.RemoveAt(0);


        e.Spawn(pos, this);

        m_active.Add(e);

    }

}
