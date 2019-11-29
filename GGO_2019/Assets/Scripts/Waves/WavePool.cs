using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Wave Pool", menuName = "Waves/Pool")]
public class WavePool : ScriptableObject
{
    public bool Shuffle;

    public List<Wave> m_Waves;

    private int m_index = 0;
    private List<Wave> m_activeWaves = new List<Wave>();

    public void Reset()
    {
        m_index = 0;
        m_activeWaves.Clear();
        m_activeWaves.AddRange(m_Waves);
        if (Shuffle)
        {
            m_activeWaves.Shuffle();
        }
    }

    public Wave GetNextWave()
    {
        if (m_index >= m_activeWaves.Count)
        {
            //for now we loop
            Reset();
        }

        Wave result = m_activeWaves[m_index];
        m_index++;
        return result;
    }


}

