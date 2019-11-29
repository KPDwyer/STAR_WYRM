using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    public float m_MinRot;
    public float m_MaxRot;
    public void Rotate()
    {
        transform.localEulerAngles = Vector3.forward * (Random.Range(m_MinRot,m_MaxRot) + 90);
    }
}
