using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : BaseEnemy
{
    [Header("Local Refs")]
    public Renderer m_renderer;

    [Header("Project Refs")]
    public Material m_DefaultMat;
    public Material m_HurtMat;

    [Header("Data")]
    [Tooltip("How long the enemy lives on the edge before disappearing")]
    public float m_EdgeDuration;
    [Tooltip("when the entity gets pushed, how much they slow down per second")]
    public float m_angleDrag;
    [Tooltip("How strongly the enemy gets pushed (from leap/smash)")]
    public float m_PushPower;
    [Tooltip("Speed the enemy approaches")]
    public float m_speed;




    private float m_edgeCounter;

    private float m_angleVelocity;


    //called by manager
    public override void Spawn(float angle, EnemyManager manager)
    {
        base.Spawn(angle, manager);
        m_renderer.material = m_DefaultMat;
        m_edgeCounter = m_EdgeDuration;
    }

    public override void Hit()
    {
        base.Hit();
        GameManager.instance.m_HitCircleFXManager.SpawnFX(transform.position, m_angle);
        BecomeInactive(true);
    }

    public override void LeapSmash()
    {
        BecomeInactive(true);
        base.LeapSmash();
    }

    public override void LeapPush(float sourceAngle)
    {
        base.LeapPush(sourceAngle);
        if (sourceAngle - m_angle > 180)
            m_angle += 360;
        m_angleVelocity = (m_angle) > (sourceAngle) ? m_PushPower : -m_PushPower;
        BecomeInactive(true);
    }

    private void BecomeInactive(bool score)
    {
        m_renderer.material = m_HurtMat;
        m_edgeCounter = 1.0f;
        m_active = false;
        if (score)
        {
            ScoreEnemy();
        }
    }

    void Update()
    {
        m_depth -= Time.deltaTime * m_speed;
        if (m_depth <= 0)
        {
            m_depth = 0;

            //update loop for edge
            m_edgeCounter -= Time.deltaTime;
            if (m_edgeCounter <= 0)
            {
                if (m_active)
                {
                    BecomeInactive(false);
                }
                else
                {
                    m_manager.DespawnEnemy(this);
                }
            }
        }

        if (Mathf.Abs(m_angleVelocity) > 0.1f)
        {
            m_angle += m_angleVelocity * Time.deltaTime;
            if (m_angleVelocity > 0)
            {
                m_angleVelocity -= m_angleDrag * Time.deltaTime;
            }
            else
            {
                m_angleVelocity += m_angleDrag * Time.deltaTime;
            }
        }

        GameManager.instance.m_RingManager.SetObjectOnRing(ref m_transform, (m_angle), m_depth);
    }
}
