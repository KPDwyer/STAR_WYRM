using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Project References")]
    public BaseEnemy m_Prefab;

    [Header("Data")]
    public float m_PoolSize;


    private List<BaseEnemy> m_pool;
    private List<BaseEnemy> m_active;


    private void Awake()
    {
        m_pool = new List<BaseEnemy>();
        m_active = new List<BaseEnemy>();

        for (int i = 0; i < m_PoolSize; i++)
        {
            BaseEnemy e = Instantiate(m_Prefab);
            e.Despawn();
            m_pool.Add(e);
        }


    }

    public void DespawnEnemy(BaseEnemy e)
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


    public List<BaseEnemy> GetCloseEnemiesInAngleRange(float min, float max)
    {
        if (min < 0 || min >= 360)
        {
            min = min % 360;
        }
        if (max < 0 || max >= 360)
        {
            max = max % 360;
        }

        bool checkOuter = min > max;

        List<BaseEnemy> result = GetEnemiesInForeground();

        if (result.Count > 0)
        {

            for (int i = result.Count - 1; i >= 0; i--)
            {
                if (checkOuter)
                {
                    if (!(result[i].GetAngle() >= min || result[i].GetAngle() <= max))
                    {
                        result.RemoveAt(i);
                    }
                }
                else
                {
                    if (!(result[i].GetAngle() >= min && result[i].GetAngle() <= max))
                    {
                        //toss
                        result.RemoveAt(i);
                    }
                }

            }
        }


        return result;
    }

    public List<BaseEnemy> GetActiveEnemies()
    {
        return m_active;
    }

    public List<BaseEnemy> GetEnemiesInForeground()
    {
        //update to be better
        List<BaseEnemy> result = new List<BaseEnemy>();

        foreach (BaseEnemy e in m_active)
        {
            if (e.GetDepth() <= 5.0f && e.IsActive())
            {
                result.Add(e);
            }
        }
        return result;
    }

    public void SpawnEnemy(float angle)
    {
        if (m_pool.Count <= 0)
            return;             //TODO start populating the pool at 0?

        BaseEnemy e = m_pool[0];

        m_pool.RemoveAt(0);


        e.Spawn(angle, this);

        m_active.Add(e);

    }
}
