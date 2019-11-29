using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spheroid : BaseEnemy
{
    [Header("refs")]
    public Transform m_RingTransform;
    public Transform m_Vis;
    public Renderer m_Renderer;

    [Header("Project Refs")]
    public Material m_HurtMat;
    public Material m_DefaultMat;

    [Header("Temp Data")]
    public float m_GapWidth;
    public AnimationCurve m_ScaleOverTime;

    [Tooltip("Speed the enemy approaches")]
    public float m_speed;
    [Tooltip("Rotation Speed (deg/second)")]
    public float m_rotationSpeed;

    public AnimationCurve m_normalizedMinBoundsScalar;
    public AnimationCurve m_normalizedMaxBoundsScalar;

    private float m_DeadCounter = 0.0f;


    public override void Spawn(float angle, EnemyManager manager)
    {
        //everything from base except ring placement
        m_transform = this.transform;
        m_manager = manager;
        m_angle = MathHelper.LoopAngle(angle);
        gameObject.SetActive(true);
        m_active = true;
        m_depth = 1000;
        m_Renderer.material = m_DefaultMat;



        m_transform.position = Vector3.forward * m_depth;
        m_RingTransform.localEulerAngles = new Vector3(0, 0, m_angle);

    }

    private void Update()
    {
        if (m_active)
        {
            //check for existing front Spheroid.
            List<BaseEnemy> b = m_manager.GetActiveEnemies();
            if (b.Count > 0)
            {
                if (b[0] != this)
                {
                    return;
                }
            }


            m_depth -= Time.deltaTime * m_speed;

            if (m_depth <= 0)
            {
                m_depth = 0;
            }

            m_angle += m_rotationSpeed * Time.deltaTime;
            m_RingTransform.localEulerAngles = new Vector3(0, 0, m_angle);
            m_Vis.localScale = Vector3.one * m_ScaleOverTime.Evaluate(GameManager.instance.m_RingManager.GetRadiusInterpolant());

            m_transform.position = Vector3.forward * m_depth;
        }
        else
        {
            m_DeadCounter -= Time.deltaTime;
            if (m_DeadCounter <= 0.0f)
            {
                m_manager.DespawnEnemy(this);
            }
        }

    }

    private void OnDrawGizmosSelected()
    {
        Debug.DrawLine(transform.position, MathHelper.DegreeToVector2(m_angle - m_GapWidth) * 100, Color.white);
        Debug.DrawLine(transform.position, MathHelper.DegreeToVector2(m_angle + m_GapWidth) * 100, Color.white);

        Debug.DrawLine(transform.position, MathHelper.DegreeToVector2(m_angle + 180 - m_GapWidth) * 100, Color.white);
        Debug.DrawLine(transform.position, MathHelper.DegreeToVector2(m_angle + 180 + m_GapWidth) * 100, Color.white);
        /*
        float calc = m_normalizedMaxBoundsScalar.Evaluate(GameManager.instance.m_RingManager.GetRadiusInterpolant()) -
        m_normalizedMinBoundsScalar.Evaluate(GameManager.instance.m_RingManager.GetRadiusInterpolant());
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, GameManager.instance.m_RingManager.m_Radius * calc);
        */

    }

    public bool IsPlayerInRange(float normalizedPos)
    {
        if (normalizedPos >= m_normalizedMinBoundsScalar.Evaluate(GameManager.instance.m_RingManager.GetRadiusInterpolant())
                &&
                normalizedPos <= m_normalizedMaxBoundsScalar.Evaluate(GameManager.instance.m_RingManager.GetRadiusInterpolant()))
        {
            return true;
        }
        return false;
    }

    public override void Hit()
    {
        base.Hit();
        GameManager.instance.m_HitCircleFXManager.SpawnFX(transform.position, m_angle);
        BecomeInactive(true);
    }

    private void BecomeInactive(bool score)
    {
        m_Renderer.material = m_HurtMat;
        m_DeadCounter = 1.0f;
        m_active = false;
        if (score)
        {
            ScoreEnemy();
        }
    }

    public bool IsPlayerInGap(float pAngle)
    {

        //will be 0-360
        if (pAngle <= Mathf.NegativeInfinity)
        {
            return false;
        }

        if (MathHelper.CheckAngleInRange(pAngle, m_angle - m_GapWidth, m_angle + m_GapWidth))
        {
            return true;
        }

        if (MathHelper.CheckAngleInRange(pAngle + 180, m_angle - m_GapWidth, m_angle + m_GapWidth))
        {
            return true;
        }

        return false;

    }


}
