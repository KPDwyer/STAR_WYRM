using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveElement
{

    public enum WaveType
    {
        Timer = 0,
        BasicEnemy = 1,
        Tentacle = 2,
        Blocker = 3,
        Spheroid = 4
    }

    public WaveType m_Type;

    public float m_Angle;
    public float m_Duration;

}
