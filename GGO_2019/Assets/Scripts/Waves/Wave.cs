using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Wave Set", menuName = "Waves/Wave")]
public class Wave : ScriptableObject
{
    [Tooltip("The base Angle for the wave - individual elements can be offset from this angle")]
    public float m_BaseAngle;
    [Tooltip("Initial delay before stazrting the wave")]
    public float m_InitialDelay;
    [Tooltip("If true, will shuffle the Base Angle every time this Wave triggers")]
    public bool shuffleAngle = false;
    [Tooltip("Whether the wave elements in this wave fire in order or not.")]
    public bool shuffleWaves = false;
    [Tooltip("List of wave elements that compose this Wave")]
    public List<WaveElement> m_Waves;


    private int m_waveIndex;
    private WaveElement m_activeWave;
    private List<WaveElement> m_ActiveWaves = new List<WaveElement>();


    private float m_counter = 0.0f;

    private System.Action m_onWaveSetComplete;

    public void Reset(System.Action onWaveSetComplete)
    {
        m_waveIndex = -1;
        m_counter = 0.0f;
        m_ActiveWaves.Clear();
        m_ActiveWaves.AddRange(m_Waves);
        if (shuffleWaves)
        {
            m_ActiveWaves.Shuffle();
        }
        if (shuffleAngle)
        {
            m_BaseAngle = UnityEngine.Random.Range(-180, 180);
        }
        m_onWaveSetComplete = onWaveSetComplete;
    }

    public void StartNextWave()
    {

        if (m_waveIndex == -1)
        {
            m_activeWave = null;
            m_counter = m_InitialDelay;
        }
        else
        {
            m_activeWave = m_ActiveWaves[m_waveIndex];
            m_counter = m_activeWave.m_Duration;
            switch (m_activeWave.m_Type)
            {
                case WaveElement.WaveType.BasicEnemy:
                    GameManager.instance.m_BaseEnemyManager.SpawnEnemy(m_activeWave.m_Angle + m_BaseAngle);
                    break;
                case WaveElement.WaveType.Tentacle:
                    GameManager.instance.m_TentacleEnemyManager.SpawnEnemy(m_activeWave.m_Angle + m_BaseAngle);
                    break;
                case WaveElement.WaveType.Blocker:
                    GameManager.instance.m_BlockerEnemyManager.SpawnEnemy(m_activeWave.m_Angle + m_BaseAngle);
                    break;
                case WaveElement.WaveType.Spheroid:
                    GameManager.instance.m_SpheroidManager.SpawnEnemy(m_activeWave.m_Angle + m_BaseAngle);
                    break;
                default:
                    break;
            }
        }
        m_waveIndex++;

    }

    public void UpdateWave(float dt)
    {
        m_counter -= Time.deltaTime;
        if (m_counter <= 0.0f)
        {
            WaveComplete();
        }
    }

    private void WaveComplete()
    {
        if (m_waveIndex >= m_ActiveWaves.Count)
        {
            m_onWaveSetComplete?.Invoke();
        }
        else
        {
            StartNextWave();
        }

    }
}
