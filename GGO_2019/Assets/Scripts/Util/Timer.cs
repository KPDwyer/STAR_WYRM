using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Timer
{

    private System.Action m_OnComplete;
    private System.Action<float> m_OnUpdate;
    private float m_TimeLeft;
    private float m_totalTime;
    private bool hasFired;

    public Timer(float duration, System.Action<float> onUpdate, System.Action onComplete)
    {
        ResetTimer(duration, onUpdate, onComplete);
    }

    public bool isActive()
    {
        return (!hasFired && m_TimeLeft > 0.0f);
    }

    public void ResetTimer(float duration, System.Action<float> onUpdate, System.Action onComplete)
    {
        m_TimeLeft = m_totalTime = duration;
        m_OnComplete = onComplete;
        m_OnUpdate = onUpdate;
        hasFired = false;
    }

    public void CancelTimer()
    {
        m_TimeLeft = m_totalTime = 0.0f;
        m_OnComplete = null;
        m_OnUpdate = null;
        hasFired = true;
    }

    public bool Update(float dt)
    {
        if (m_TimeLeft > 0)
        {
            m_TimeLeft -= dt;
            if (m_OnUpdate != null)
            {
                //0 is start, 1.0 is end
                m_OnUpdate((m_totalTime - m_TimeLeft) / m_totalTime);
            }
        }

        if (m_TimeLeft <= 0 && !hasFired)
        {
            if (m_OnComplete != null)
            {
                m_OnComplete();
            }
            m_OnComplete = null;
            hasFired = true;
        }

        return hasFired;
    }

}
