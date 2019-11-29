using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Animator m_Animator;

    private readonly string ISLEAPING = "Airborn";
    private readonly string ISDISABLED = "Disabled";
    private readonly string RIGHTSIDE = "RightSide";
    private readonly string ATTACKTRIGGER = "Attack";


    public void Attack(bool Right)
    {
        if(m_Animator != null)
        {
            m_Animator.SetBool(RIGHTSIDE, Right);
            m_Animator.SetTrigger(ATTACKTRIGGER);
        }

    }

    public void StartLeap()
    {
        if (m_Animator != null)
        {
            m_Animator.SetBool(ISLEAPING, true);
        }
    }

    public void StopLeap()
    {
        if (m_Animator != null)
        {
            m_Animator.SetBool(ISLEAPING, false);
        }
    }

    public void BecomeUnplayable()
    {
        if (m_Animator != null)
        {
            m_Animator.SetBool(ISDISABLED, true);
        }
    }

    public void BecomePlayable()
    {
        if (m_Animator != null)
        {
            m_Animator.SetBool(ISDISABLED, false);
        }
    }
}
