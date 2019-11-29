using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCinematic : MonoBehaviour
{
    public Camera m_MainCam;
    public Camera m_IntroCam;

    public Vector3 m_IntroStart;
    public Vector3 m_IntroEnd;
    public AnimationCurve m_curve;

    // Start is called before the first frame update
    void Start()
    {
        m_MainCam.enabled = false;
        m_IntroCam.enabled = true;
        GameManager.instance.m_TimerManager.AddTimer(
            5.0f,
            (t) =>
            {
                m_IntroCam.transform.position =
                Vector3.Lerp(m_IntroStart, m_IntroEnd, m_curve.Evaluate(t));
            },
            () =>
            {
                m_IntroCam.transform.position = m_IntroEnd;
                m_IntroCam.enabled = false;
                m_MainCam.enabled = true;
                GameManager.instance.m_UIManager.StartCountdown();
            });
    }


}
