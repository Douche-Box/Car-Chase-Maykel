using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipCar : MonoBehaviour
{
    Rigidbody rb;
    float lastTimeChecked;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (transform.up.y > 0.5f || rb.velocity.magnitude > 1)
        {
            lastTimeChecked = Time.time;
        }

        if (Time.time > lastTimeChecked + 3)
        {
            FlipCar();
        }
    }

    /// <summary>
    /// Flip the car
    /// </summary>
    void FlipCar()
    {
        transform.position += Vector3.up;
        transform.rotation = Quaternion.LookRotation(transform.forward);
    }
}
