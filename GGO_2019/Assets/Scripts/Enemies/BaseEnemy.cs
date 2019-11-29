using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    [Header("Scoring")]
    [Tooltip("Base amount the ring grows when an enemy of this type is destroyed")]
    public float m_RingGrowthForKill;
    [Space]
    protected Transform m_transform;
    protected float m_angle;
    protected float m_depth;
    protected bool m_active = false;
    protected EnemyManager m_manager;




    public virtual void Spawn(float angle, EnemyManager manager)
    {
        m_transform = this.transform;
        m_manager = manager;
        m_angle = angle;
        gameObject.SetActive(true);
        m_active = true;
        m_depth = 1000;
        GameManager.instance.m_RingManager.SetObjectOnRing(ref m_transform, (m_angle), m_depth);
    }

    public virtual void Despawn()
    {
        m_active = false;

        gameObject.SetActive(false);
    }


    public float GetDepth()
    {
        return m_depth;
    }

    public float GetAngle()
    {
        return MathHelper.LoopAngle(m_angle);
    }

    public bool IsActive()
    {
        return m_active;
    }

    public virtual bool HurtsPlayer()
    {
        return false;
    }

    public virtual void Hit()
    {

    }

    public virtual void LeapSmash()
    {

    }

    public virtual void LeapPush(float sourceAngle)
    {

    }

    protected void ScoreEnemy()
    {
        GameManager.instance.EnemyDestroyed(m_RingGrowthForKill);
    }
}