using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tentacle : BaseEnemy
{
    [Tooltip("Speed the enemy approaches")]
    public float m_speed;
    public float m_pushRadius;

    void Update()
    {
        m_depth -= Time.deltaTime * m_speed;

        if (m_depth <= 5)
        {
            foreach (Enemy e in GameManager.instance.m_BaseEnemyManager.GetCloseEnemiesInAngleRange(
            m_angle - m_pushRadius,
            m_angle + m_pushRadius))
            {
                e.LeapPush(m_angle);
            }
            float f = GameManager.instance.m_PlayerControls.GetAngle();
            if (f >= m_angle - m_pushRadius &&
                f <= m_angle + m_pushRadius)
            {
                GameManager.instance.m_PlayerControls.DisablePlayer();
            }

        }

        if (m_depth <= 0)
        {
            m_depth = 0;
            m_manager.DespawnEnemy(this);
        }

        GameManager.instance.m_RingManager.SetObjectOnRing(ref m_transform, (m_angle), m_depth);

    }
}
