using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IMU : MonoBehaviour
{
    private Rigidbody rb;
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    public float getPitchRotation()
    {
        return this.rb.transform.rotation.eulerAngles.x;
    }

    public float getRollRotation()
    {
        return this.rb.transform.rotation.eulerAngles.z;
    }
    public float getYawRotation()
    {
        return this.rb.transform.rotation.eulerAngles.y;
    }

    public Vector3 getVelocity()
    {
        return this.rb.velocity;
    }
}
