using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightController : MonoBehaviour
{
    public RotationalMotor[] rotationalMotors;
    private IMU imu; // InertialMeasurementUnit

    private PIDController pitchPIDController;
    private PIDController rollPIDController;
    private PIDController yawPIDController;
    private PIDController velocityPIDController;

    private float deltaTime;
    private float baseThrust = 0f;

    [SerializeField] float desiredPitch;
    [SerializeField] float desiredRoll;
    [SerializeField] float desiredYaw;
    [SerializeField] float desiredVelocity;

    [SerializeField] float[] pidvalues;

    void Start()
    {
        this.rotationalMotors = this.GetComponentsInChildren<RotationalMotor>();
        this.imu = this.GetComponent<IMU>();

        this.pitchPIDController = new PIDController(pidvalues[0], pidvalues[1], pidvalues[2]);
        this.rollPIDController = new PIDController(pidvalues[0], pidvalues[1], pidvalues[2]);
        this.yawPIDController = new PIDController(0f, 0f, 0f);
        this.velocityPIDController = new PIDController(0f, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        this.deltaTime = Time.deltaTime;

        float pitchDelta = this.pitchPIDController.Compute(this.desiredPitch, this.imu.getPitchRotation(), this.deltaTime);
                pitchDelta = Mathf.Sign(pitchDelta) * Mathf.Pow(pitchDelta, 2f);
        float rollDelta = this.rollPIDController.Compute(this.desiredRoll, this.imu.getRollRotation(), this.deltaTime);
                rollDelta = Mathf.Sign(rollDelta) * Mathf.Pow(rollDelta, 2f);
        float yawDelta = this.yawPIDController.Compute(this.desiredYaw, this.imu.getYawRotation(), this.deltaTime);
                yawDelta = Mathf.Sign(yawDelta) * Mathf.Pow(yawDelta, 2f);
        float thrustDelta = this.velocityPIDController.Compute(this.desiredVelocity, this.imu.getVelocity().magnitude, this.deltaTime);
                thrustDelta = Mathf.Sign(thrustDelta) * Mathf.Pow(thrustDelta, 2f);

        //Debug.Log(pitchDelta);

        this.adjustMotorSuppliedCurrent(pitchDelta, rollDelta, yawDelta, thrustDelta);
    }

    private void adjustPropellerSpin(float pitchDelta, float rollDelta, float yawDelta, float thrustDelta)
    {
        this.rotationalMotors[0].adjustRPM(0, -pitchDelta + rollDelta); // FL
        this.rotationalMotors[1].adjustRPM(0, -pitchDelta - rollDelta); // AC FR
        this.rotationalMotors[2].adjustRPM(0, pitchDelta + rollDelta); // BL
        this.rotationalMotors[3].adjustRPM(0, pitchDelta - rollDelta); // AC BR
    }

    private void adjustMotorSuppliedCurrent(float pitchDelta, float rollDelta, float yawDelta, float thrustDelta)
    {
        this.rotationalMotors[0].adjustCurrent(0, -pitchDelta - rollDelta); // FL
        this.rotationalMotors[1].adjustCurrent(0, -pitchDelta + rollDelta); // AC FR
        this.rotationalMotors[2].adjustCurrent(0, pitchDelta - rollDelta); // BL
        this.rotationalMotors[3].adjustCurrent(0, pitchDelta + rollDelta); // AC BR
    }
}

public class PIDController
{
    public float Kp, Ki, Kd;
    private float previousError = 0f;
    private float integralError = 0f;

    public PIDController(float kp, float ki, float kd)
    {
        Kp = kp;
        Ki = ki;
        Kd = kd;
    }

    public float Compute(float setpoint, float actualValue, float deltaTime)
    {
        float error = setpoint - actualValue;
        float proportional = Kp * error;

        integralError += error * deltaTime;
        float integral = Ki * integralError;

        float derivative = Kd * (error - previousError) / deltaTime;

        previousError = error;

        return proportional + integral + derivative;
    }
}
