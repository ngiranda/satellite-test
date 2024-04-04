using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Orbit : MonoBehaviour
{
    [SerializeField]
    private GameObject target;

    [SerializeField]
    private Vector3 axis;

    [SerializeField]
    private float degreePerSecond;
    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target.transform);
        transform.RotateAround(target.transform.position, axis, degreePerSecond * Time.deltaTime);
    }
}
