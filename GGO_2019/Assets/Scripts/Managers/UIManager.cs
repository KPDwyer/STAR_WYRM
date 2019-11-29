using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Refs")]
    public Image m_HealthBarMain;
    public Image m_HealthBarDelta;
    public Transform m_RedTransform;
    public Text m_CountdownText;
    public Transform m_GameOverObject;
    public GameObject m_EndText;

    [Header("VisDev")]
    public float m_RedSpikeDuration;
    public AnimationCurve m_RedSpikeScalar;
    [Space]
    public float m_GameOverDuration;



    private float m_actualRedScale;
    private float m_redSpikeCounter;

    private float m_counter;
    private bool m_counting = false;

    private Vector3 m_gameOverPos;

    public void StartCountdown()
    {

        m_counter = 5.4f;
        m_counting = true;
        m_EndText.SetActive(false);
        m_gameOverPos = m_GameOverObject.localPosition;

    }

    public void SetHealthFill(float amount, float delta)
    {
        m_HealthBarMain.fillAmount = amount;
        m_HealthBarDelta.fillAmount = delta;
    }

    public void RedSpike(float duration)
    {
        m_RedSpikeDuration = duration;
        m_redSpikeCounter = m_RedSpikeDuration;

    }

    [ContextMenu("GO")]
    public void ShowGameOver(System.Action endCall)
    {
        Timer timer = GameManager.instance.m_TimerManager.AddTimer(
            m_GameOverDuration,
            (t) =>
            {
                m_GameOverObject.transform.localPosition = Vector3.Lerp(
                    m_gameOverPos,
                    Vector3.zero,
                    t);
            },
            () =>
            {
                m_GameOverObject.transform.localPosition = Vector3.zero;
                m_EndText.SetActive(true);
                endCall?.Invoke();
            });
    }

    private void Update()
    {
        if (m_counting)
        {
            m_counter -= Time.deltaTime;
            int count = Mathf.RoundToInt(m_counter);
            m_CountdownText.text = count.ToString();
            if (count <= 0)
            {
                GameManager.instance.BeginGame();
                m_CountdownText.gameObject.SetActive(false);
                m_counting = false;
            }
        }


        m_actualRedScale = m_RedSpikeScalar.Evaluate(GameManager.instance.m_RingManager.GetRadiusInterpolant());

        //lerp from actual to target based on counter i guess.
        if (m_redSpikeCounter >= 0.0f)
        {
            //1->0
            float interpolant = m_redSpikeCounter / m_RedSpikeDuration;

            m_actualRedScale = Mathf.Lerp(m_actualRedScale, 1.0f, interpolant);

            m_redSpikeCounter -= Time.deltaTime;
        }

        m_RedTransform.localScale = Vector3.one * m_actualRedScale;
    }
}
