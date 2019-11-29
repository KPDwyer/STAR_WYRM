using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerControls : MonoBehaviour
{
    [Header("Data - Ring")]
    [Tooltip("The maximum velocity (at max-sized play area) in Degrees/sec ")]
    public float m_MaxVelocity;
    [Tooltip("The upper Acceleration (at max play area) in Degrees/sec")]
    public float m_MaxAcceleration;
    [Tooltip("The rate that acceleration increases per second")]
    public float m_AccelerationRate;
    [Tooltip("multiplied by the current velocity when there is no input")]
    public float m_Dampening;
    [Tooltip("Enemies whose angles are lesser than this value from the player will get slashed")]
    public float m_AngleDistanceSlash;
    [Tooltip("duration the player is disabled when `hit`")]
    public float m_disabledDuration;

    [Header("Data - Leaping")]
    [Tooltip("How long the leap lasts (Y axis) as the play area shrinks (x axis)")]
    public AnimationCurve m_LeapDurationCurve;
    [Tooltip("Distance from player (degrees) after landing that gets SMASHED")]
    public float m_AngleDistanceLeapSmash;
    [Tooltip("Distance from player (degrees) after landing that gets PUSHED")]
    public float m_AngleDistanceLeapPush;

    [Header("Vis Dev")]
    public Material m_defaultMaterial;
    public Material m_disabledMaterial;

    [Header("Optional Animation Hookups")]
    public UnityEvent OnLeapStarted = new UnityEvent();
    public UnityEvent OnLeapEnded = new UnityEvent();
    public UnityEvent OnBecomeUnplayable = new UnityEvent();
    public UnityEvent OnBecomePlayable = new UnityEvent();
    public BoolEvent OnAttack = new BoolEvent();

    [Header("References")]
    public Transform m_PlayerVisTransform;
    public List<Renderer> m_Renderers;


    private enum PlayerState
    {
        Uninitiated = 0,
        OnRing = 1,
        Leaping = 2

    }

    private PlayerState m_State = PlayerState.Uninitiated;

    private Transform m_transform;

    private float m_angle;
    private float m_velocity;
    private float m_acceleration;

    private float m_LeapCounter;
    private float m_LeapDuration;
    private Vector3 m_TargetPosition;
    private Vector3 m_StartingPosition;

    private bool m_disabled;
    private float m_disabledCounter = 0.0f;

    private List<BaseEnemy> m_cachedEnemies;

    void Awake()
    {
        m_transform = this.transform;
        m_cachedEnemies = new List<BaseEnemy>();
    }

    private void Start()
    {
        m_angle = 0;
        m_disabled = false;
        m_disabledCounter = 0.0f;
        foreach (Renderer r in m_Renderers)
        {
            r.material = m_defaultMaterial;
        }
    }

    public void BeginGame()
    {
        m_State = PlayerState.OnRing;

    }

    public void EndGame()
    {
        m_State = PlayerState.Uninitiated;
    }

    // Update is called once per frame
    void Update()
    {
        m_angle = MathHelper.LoopAngle(m_angle);

        switch (m_State)
        {
            case PlayerState.OnRing:
                if (UpdateOnRing())
                {
                    UpdateRingCombat();
                }
                break;
            case PlayerState.Leaping:
                UpdateLeaping();

                break;
            default:
                break;
        }
    }

    public float GetAngle()
    {
        if (m_disabled)
        {
            return Mathf.NegativeInfinity;
        }
        return MathHelper.LoopAngle(m_angle);
    }

    public void DisablePlayer()
    {
        if (!m_disabled)
        {
            OnBecomeUnplayable?.Invoke();
            m_acceleration = 0.0f;
            m_disabled = true;
            m_disabledCounter = m_disabledDuration;
            GameManager.instance.m_UIManager.RedSpike(m_disabledDuration);
            foreach (Renderer r in m_Renderers)
            {
                r.material = m_disabledMaterial;
            }
        }
    }

    private void StartLeaping()
    {
        OnLeapStarted?.Invoke();
        m_State = PlayerState.Leaping;
        m_StartingPosition = transform.position;
        m_angle += 180;
        m_TargetPosition = GameManager.instance.m_RingManager.GetPositionForRingAngle((m_angle), 0);
        m_LeapCounter = 0.0f;
        m_velocity = 0.0f;
        m_acceleration = 0.0f;
        m_LeapDuration = m_LeapDurationCurve.Evaluate(GameManager.instance.m_RingManager.GetRadiusInterpolant());
    }

    private void LeapingBounce()
    {
        m_StartingPosition = m_TargetPosition;
        m_angle += 180;
        m_TargetPosition = GameManager.instance.m_RingManager.GetPositionForRingAngle((m_angle), 0);
        m_LeapCounter = m_LeapDuration - m_LeapCounter;
    }

    private void UpdateLeaping()
    {
        m_LeapCounter += Time.deltaTime;
        if (m_LeapCounter >= m_LeapDuration)
        {
            LeapLand();
        }
        else
        {
            float t = m_LeapCounter / m_LeapDuration;
            float scale = 0.0f;
            if (t < 0.5f)
            {
                scale = t;
            }
            else
            {
                scale = Mathf.Abs(t - 1.0f);
            }
            m_PlayerVisTransform.localScale = Vector3.one + (Vector3.up * scale * 8.0f);
            transform.position = Vector3.Lerp(m_StartingPosition, m_TargetPosition, t);

            //disabled mid-leap means you're bouncing back.
            if (!m_disabled)
            {
                List<BaseEnemy> middle = GameManager.instance.m_SpheroidManager.GetEnemiesInForeground();
                //if there is a baddie in the middle
                if (middle.Count > 0)
                {
                    Spheroid s = middle[0] as Spheroid;
                    //if we are in the middle
                    if (s.IsPlayerInRange(t))
                    {
                        m_disabled = true;
                        if (s.IsPlayerInGap(m_angle))
                        {
                            s.Hit();
                        }
                        else
                        {
                            LeapingBounce();
                        }
                    }
                }
            }
        }

    }

    private bool UpdateOnRing()
    {
        if (m_disabled)
        {
            m_velocity *= m_Dampening;
            m_disabledCounter -= Time.deltaTime;
            if (m_disabledCounter <= 0.0f)
            {
                m_disabled = false;
                OnBecomePlayable?.Invoke();

                foreach (Renderer r in m_Renderers)
                {
                    r.material = m_defaultMaterial;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            StartLeaping();
            return false;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (m_velocity > 0.0f)
                m_velocity *= m_Dampening;
            if (m_acceleration > 0.0f)
                m_acceleration = 0;
            m_acceleration -= Time.deltaTime * m_AccelerationRate;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            if (m_velocity < 0.0f)
                m_velocity *= m_Dampening;
            if (m_acceleration < 0.0f)
                m_acceleration = 0;
            m_acceleration += Time.deltaTime * m_AccelerationRate;
        }
        else
        {
            m_acceleration = 0;
            m_velocity *= m_Dampening;
        }

        m_acceleration = Mathf.Clamp(m_acceleration, -m_MaxAcceleration, m_MaxAcceleration);

        m_velocity += (m_acceleration * m_MaxAcceleration) * Time.deltaTime;

        m_velocity = Mathf.Clamp(m_velocity, -m_MaxVelocity, m_MaxVelocity);

        m_angle += m_velocity * GameManager.instance.m_RingManager.GetVelocityScalar() * Time.deltaTime;

        //place on ring
        GameManager.instance.m_RingManager.SetObjectOnRing(ref m_transform, m_angle, 0);
        return !m_disabled;
    }

    private void LeapLand()
    {
        OnLeapEnded?.Invoke();
        m_disabled = false;
        m_PlayerVisTransform.localScale = Vector3.one;
        transform.position = GameManager.instance.m_RingManager.GetPositionForRingAngle((m_angle), 0);
        GameManager.instance.m_SmashCircleFXManager.SpawnFX(transform.position, m_angle);


        m_angle = MathHelper.LoopAngle(m_angle);
        bool hit = false;

        m_cachedEnemies.Clear();

        m_cachedEnemies = GameManager.instance.m_BaseEnemyManager.GetCloseEnemiesInAngleRange(
            m_angle - m_AngleDistanceLeapSmash,
            m_angle + m_AngleDistanceLeapSmash);
        m_cachedEnemies.AddRange(
        GameManager.instance.m_BlockerEnemyManager.GetCloseEnemiesInAngleRange(
            m_angle - m_AngleDistanceLeapSmash,
            m_angle + m_AngleDistanceLeapSmash));

        foreach (BaseEnemy e in m_cachedEnemies)
        {
            hit = true;
            e.LeapSmash();
        }

        m_cachedEnemies.Clear();

        m_cachedEnemies = GameManager.instance.m_BaseEnemyManager.GetCloseEnemiesInAngleRange(
            m_angle - m_AngleDistanceLeapPush,
            m_angle + m_AngleDistanceLeapPush);
        m_cachedEnemies.AddRange(
        GameManager.instance.m_BlockerEnemyManager.GetCloseEnemiesInAngleRange(
            m_angle - m_AngleDistanceLeapPush,
            m_angle + m_AngleDistanceLeapPush));

        foreach (BaseEnemy e in m_cachedEnemies)
        {
            e.LeapPush(m_angle);
        }


        if (hit)
        {
            GameManager.instance.m_GlobalTimeManager.LeapStop();
        }

        m_State = PlayerState.OnRing;

    }

    private void UpdateRingCombat()
    {
        m_angle = MathHelper.LoopAngle(m_angle);
        bool hurt = false;
        foreach (Blocker b in GameManager.instance.m_BlockerEnemyManager.GetCloseEnemiesInAngleRange(
                m_angle - 5,
                m_angle + 5))
        {
            hurt = true;
            float sourceAngle = b.GetAngle();
            MathHelper.LoopAngle(sourceAngle);
            MathHelper.LoopAngle(m_angle);
            if (sourceAngle - m_angle > 180)
                m_angle += 360;
            m_velocity = m_angle > sourceAngle ? 50.0f : -50.0f;
            DisablePlayer();

        }

        bool hit = false;
        if (!hurt)
        {
            foreach (Enemy e in GameManager.instance.m_BaseEnemyManager.GetCloseEnemiesInAngleRange(
                m_angle - m_AngleDistanceSlash,
                m_angle + m_AngleDistanceSlash))
            {
                hit = true;
                OnAttack.Invoke(e.GetAngle() > m_angle);
                e.Hit();
            }
        }
        if (hit || hurt)
        {
            GameManager.instance.m_GlobalTimeManager.HitStop();
        }
    }


}
