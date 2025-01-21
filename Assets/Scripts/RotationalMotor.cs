using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationalMotor : MonoBehaviour
{
    [SerializeField] private float motor_rpm;
    [SerializeField] private float current; // [-inf, inf] denotes current and polarity

    void Start()
    {
        
    }

    void Update()
    {
        this.applyPropellerForces();
        this.rotatePropeller();
        this.current = calculateCurrent(this.motor_rpm);
    }

    private void applyPropellerForces()
    {
        Rigidbody drone = this.GetComponentInParent<Rigidbody>();

        drone.AddForceAtPosition(new Vector3(0, calculateThrust(this.motor_rpm) * 0.135f, 0), this.transform.position);
    }
    private void rotatePropeller()
    {
        float rps = this.motor_rpm * Time.deltaTime / 60f;
        this.transform.Rotate(new Vector3(0, 360*rps, 0));
    }

    protected float calculateRPM(float current)
    {
        return (1948.9f * Mathf.Log(current)) + 4255.7f; // based on a log best fit on datasheet data
    }

    protected float calculateCurrent(float RPM)
    {
        return Mathf.Exp((RPM-4255.7f)/1948.9f);
    }

    private float calculateThrust(float rpm)
    {
        double thrust = Math.Clamp((2D*Math.Pow(10D, -8D) * Math.Pow((double)this.motor_rpm, 2D)) - (4D*Math.Pow(10D, -5D) * (double)this.motor_rpm), 0D, double.PositiveInfinity); // kgf
        return (float)thrust;
    }
}
