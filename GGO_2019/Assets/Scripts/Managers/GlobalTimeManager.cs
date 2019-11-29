using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalTimeManager : MonoBehaviour
{

    private bool m_TimeAdjusted = false;
    private float m_AdjustmentDuration;

    // Update is called once per frame
    void Update()
    {
        if (m_TimeAdjusted)
        {
            m_AdjustmentDuration -= Time.unscaledDeltaTime;
            if (m_AdjustmentDuration <= 0.0f)
            {
                m_TimeAdjusted = false;
                Time.timeScale = 1.0f;

            }
        }
    }

    public void HitStop()
    {
        /*
        Time.timeScale = 0.0f;
        m_TimeAdjusted = true;
        m_AdjustmentDuration = 0.05f;
        */
    }

    public void LeapStop()
    {
        /*
        Time.timeScale = 0.0f;
        m_TimeAdjusted = true;
        m_AdjustmentDuration = 0.1f;
        */
    }
}
