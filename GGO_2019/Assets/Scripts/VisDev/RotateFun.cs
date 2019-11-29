using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateFun : MonoBehaviour
{

    private Vector3 eulerRot;
    // Start is called before the first frame update
    void Start()
    {
        eulerRot = new Vector3(
        Random.Range(-10, 10),
        Random.Range(-10, 10),
        Random.Range(-10, 10));

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(eulerRot * Time.deltaTime);
    }
}
