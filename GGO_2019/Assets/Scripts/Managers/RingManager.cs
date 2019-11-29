using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RingManager : MonoBehaviour
{
    [Header("Data")]
    [Tooltip("The Starting (and current) radius of the play area")]
    public float m_Radius;
    [Tooltip("A multiplier on the player's velocity as the play area shrinks")]
    public AnimationCurve m_PlayerVelocityScalar;
    [Tooltip("Multiplied by delta time every frame to shrink the radius")]
    public float m_ShrinkRate;
    [Tooltip("How Fast the Ring Grows (per second) when the user earns some radius")]
    public float m_GrowthRate;


    [Header("References")]
    public Camera m_Camera;
    public Transform m_RingTransform;

    private float m_maxRadius = 75;
    private float m_minRadius = 10;
    private float m_MaxCameraDistance = -140;
    private float m_MinCameraDistance = -30;

    private float m_GrowthRemaining = 0;

    private bool m_Initiated = false;

    public void BeginGame()
    {
        m_Initiated = true;
    }

    private void Start()
    {
        UpdateRingState();
        UpdateCamera();
        UpdateRingUIData();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Initiated)
        {
            UpdateRingState();
            UpdateCamera();
            UpdateRingUIData();
        }
    }

    public void SetObjectOnRing(ref Transform t, float angle, float depth)
    {
        t.position = GetPositionForRingAngle(angle, depth);
        t.eulerAngles = Vector3.forward * (angle + 90);
    }

    public Vector3 GetPositionForRingAngle(float angle, float depth)
    {
        Vector3 result = MathHelper.DegreeToVector2(angle) * m_Radius;
        result += Vector3.forward * depth;
        return result;

    }

    /// <summary>
    /// Gets the radius interpolant - basically 0.0f - 1.0f where a lower number means a bigger
    /// radius.
    /// </summary>
    /// <returns>The radius interpolant.</returns>
    public float GetRadiusInterpolant()
    {
        return Mathf.InverseLerp(m_maxRadius, m_minRadius, m_Radius);
    }

    public float GetVelocityScalar()
    {
        return m_PlayerVelocityScalar.Evaluate(GetRadiusInterpolant());
    }

    public void GrowRing(float amount)
    {
        m_GrowthRemaining += amount;
    }

    private void UpdateRingState()
    {
        if (m_GrowthRemaining > 0)
        {
            m_GrowthRemaining -= (Time.deltaTime * m_GrowthRate);
            m_Radius += (Time.deltaTime * m_GrowthRate);
        }
        else
        {
            m_Radius -= Time.deltaTime * m_ShrinkRate;
        }
        if (m_Radius <= m_minRadius)
        {
            GameManager.instance.EndGame();
        }
        m_Radius = Mathf.Clamp(m_Radius, m_minRadius, m_maxRadius);
        Vector3 scale = new Vector3(m_Radius, m_Radius, 1);
        m_RingTransform.localScale = scale;
    }

    private void UpdateRingUIData()
    {
        GameManager.instance.m_UIManager.SetHealthFill(
            1.0f - Mathf.InverseLerp(m_maxRadius, m_minRadius, m_Radius),
            1.0f - Mathf.InverseLerp(m_maxRadius, m_minRadius, m_Radius + m_GrowthRemaining)
            );
    }



    private void UpdateCamera()
    {
        float interpolant = GetRadiusInterpolant();
        m_Camera.transform.position = Vector3.forward * Mathf.Lerp(m_MaxCameraDistance, m_MinCameraDistance, interpolant);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(Vector3.zero, m_Radius);

    }
}
