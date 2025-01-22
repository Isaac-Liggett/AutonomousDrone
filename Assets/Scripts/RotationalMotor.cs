using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum direction
{
    clockwise,
    anticlockwise
}
public class RotationalMotor : MonoBehaviour
{
    [SerializeField] private float motor_rpm;
    [SerializeField] private float current; // [-inf, inf] denotes current and polarity
    [SerializeField] direction motor_direction = direction.clockwise;
    [SerializeField] public float forceGenerated;

    private float baseRPM = 9000f;
    private float baseCurrent = 0f;

    public void adjustRPM(float adjustmentBaseThrust, float adjustmentRPM)
    {
        this.baseRPM += adjustmentBaseThrust;
        if(this.baseRPM < 0f)
        {
            this.baseRPM = 0f;
        }
        this.motor_rpm = this.baseRPM + adjustmentRPM;
        if(this.motor_rpm < 0f)
        {
            this.motor_rpm = 0f;
        }
    }

    public void adjustCurrent(float adjustmentBaseCurrent, float adjustmentCurrent)
    {
        this.baseCurrent += adjustmentBaseCurrent;
        if(this.baseCurrent < 0f)
        {
            this.baseCurrent = 0f;
        }

        this.current = this.baseCurrent + adjustmentCurrent;
        if (this.current < 0f)
        {
            this.current = 0f;
        }
    }

    void Update()
    {
        this.motor_rpm = this.calculateRPM(this.current);
        this.applyPropellerForces();
        this.rotatePropeller();
    }

    private void applyPropellerForces()
    {
        Rigidbody drone = this.GetComponentInParent<Rigidbody>();

        drone.AddForceAtPosition(new Vector3(0, calculateThrust(this.motor_rpm) * 0.135f, 0), this.transform.position);
        drone.AddTorque(new Vector3(0f, calculateTorque(this.motor_rpm), 0f), ForceMode.Force);
    }
    private void rotatePropeller()
    {
        float rps = this.motor_rpm * Time.deltaTime / 60f;

        if (this.motor_direction == direction.anticlockwise ) { }
        this.transform.Rotate(new Vector3(0, 360*rps, 0));
    }

    protected float calculateRPM(float current)
    {
        return Mathf.Clamp((1948.9f * Mathf.Log(current)) + 4255.7f, 0f, float.PositiveInfinity); // based on a log best fit on datasheet data
    }

    protected float calculateCurrent(float RPM)
    {
        return Mathf.Exp((RPM-4255.7f)/1948.9f);
    }

    protected float calculateThrust(float rpm)
    {
        double thrust = Math.Clamp((2D*Math.Pow(10D, -8D) * Math.Pow((double)rpm, 2D)) - (4D*Math.Pow(10D, -5D) * (double)rpm), 0D, double.PositiveInfinity); // kgf
        return (float)thrust;
    }

    protected float calculateTorque(float rpm)
    {
        double torque = ((3D * Math.Pow(10, -9)) * Math.Pow((double)rpm, 2D)) + ((2D * Math.Pow(10D, -5D)) * ((double)Mathf.Abs(rpm)));
        if(this.motor_direction == direction.clockwise)
        {
            return (float)torque;
        }
        else
        {
            return -(float)torque;
        }
    }
}
