using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Tooltip("Currently active WavePool")]
    public WavePool m_WavePool;

    private float m_duration = 0.5f;
    private float m_counter;
    private Wave m_Set;

    private bool m_initiated = false;

    public void BeginGame()
    {
        m_initiated = true;

        m_counter = m_duration;
        m_WavePool.Reset();
        SetNewWaveSet();
    }

    public void Start()
    {
        /*
        m_initiated = true;

        m_counter = m_duration;
        m_WavePool.Reset();
        SetNewWaveSet();
        */
    }

    private void SetNewWaveSet()
    {
        m_Set = m_WavePool.GetNextWave();
        m_Set.Reset(WaveSetComplete);
    }

    void Update()
    {
        if (m_initiated)
        {
            m_Set.UpdateWave(Time.deltaTime);
        }
    }

    private void WaveSetComplete()
    {
        SetNewWaveSet();
    }

}
