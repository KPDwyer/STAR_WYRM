using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    [SerializeField]
    public List<Timer> m_Timers = new List<Timer>();
    private Queue<Timer> m_DeadTimers = new Queue<Timer>();
    private List<Timer> m_DirtyIndices = new List<Timer>();

    public Timer AddTimer(float duration, System.Action<float> onUpdate, System.Action onComplete)
    {
        Timer t;
        if (m_DeadTimers.Count > 0)
        {
            t = m_DeadTimers.Dequeue();
            t.ResetTimer(duration, onUpdate, onComplete);
        }
        else
        {
            t = new Timer(duration, onUpdate, onComplete);
        }


        m_Timers.Add(t);

        return t;

    }

    void LateUpdate()
    {
        m_DirtyIndices.Clear();

        for (int i = 0; i < m_Timers.Count; i++)
        {
            if (m_Timers[i].Update(Time.deltaTime))
            {
                m_DeadTimers.Enqueue(m_Timers[i]);
                m_DirtyIndices.Add(m_Timers[i]);
            }
        }

        for (int i = 0; i < m_DirtyIndices.Count; i++)
        {

            m_Timers.Remove(m_DirtyIndices[i]);
        }

    }
}
