using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* TO DO
 * Make maxTorque an evolutionable trait
 * Make rotationSpeed an evolutionable trait
*/

public class Car_Controller : MonoBehaviour {

    // TEMP TEMP TEMP TEMP TEMP TEMP TEMP
    public int tempTotalAmountOfWheels;
    public int sg;
    // TEMP TEMP TEMP TEMP TEMP TEMP TEMP

    public WheelJoint2D[] wheels;

    // Engine specific variables
    public float EngineSpeed;
    public float MaxMotorTorque;
    public float CarRotationTorque;

    //Driver specific variables
    public float CorrectionCone;
    public float AccelerationCone;
    public bool hasDriver;

    private Rigidbody2D rb;

    private Main main;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        main = GameObject.Find("{ MAIN }").GetComponent<Main>();
    }

    void Accelerate(int dir, float localSpeed)
    {
        dir *= -1;
        if(localSpeed != 0)
        {
            EngineSpeed = localSpeed;
        }

        JointMotor2D motor = new JointMotor2D { motorSpeed = EngineSpeed * dir, maxMotorTorque = MaxMotorTorque };

        foreach(WheelJoint2D wheel in wheels)
        {
            wheel.useMotor = true;
            wheel.motor = motor;
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        // Handle player input
        if (!hasDriver)
        {
            float verticalInput = Input.GetAxisRaw("Vertical");
            float horizontalInput = Input.GetAxisRaw("Horizontal");

            // Handle acceleration
            if (verticalInput != 0)
            {
                Accelerate((int)verticalInput, EngineSpeed);
            }
            else
            {
                EngineOff();
            }
            // Handle car rotation
            if (horizontalInput != 0)
            {
                rb.AddTorque((horizontalInput * -1) * CarRotationTorque * Time.fixedDeltaTime);
            }
        }
        // Handle AI driving here
        else
        {
            // If driver is not simulated, just accelerate
            if (!main.includeDriver)
            {
                Accelerate(1, 0);
                return;
            }

            // Handle acceleration
            Accelerate(1, 0);
            // If checks if car has rotated towards the left
            if(transform.rotation.eulerAngles.z <= 180)
            {
                if(Map(transform.rotation.eulerAngles.z, 0, 180, 1, -1) <= AccelerationCone)
                {
                    EngineOff();
                }
            }
            // If checks if car has rotated towards the right
            if (transform.rotation.eulerAngles.z > 180)
            {
                if (Map(transform.rotation.eulerAngles.z, 180, 360, 1, -1) <= AccelerationCone)
                {
                    EngineOff();
                }
            }

            // Handle rotation
            if (transform.rotation.eulerAngles.z <= 180)
            {
                if (Map(transform.rotation.eulerAngles.z, 0, 180, 1, -1) <= CorrectionCone)
                {
                    rb.AddTorque(-1 * CarRotationTorque * Time.fixedDeltaTime);
                }
            }
            // If checks if car has rotated towards the right
            if (transform.rotation.eulerAngles.z > 180)
            {
                if (Map(transform.rotation.eulerAngles.z, 180, 360, 1, -1) <= CorrectionCone)
                {
                    rb.AddTorque(CarRotationTorque * Time.fixedDeltaTime);
                }
            }
        }
	}

    private void EngineOff()
    {
        foreach (WheelJoint2D wheel in wheels)
        {
            wheel.useMotor = false;
        }
    }

    public float Map(float value, float fromSource, float toSource, float fromTarget, float toTarget)
    {
        return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
    }
}
