using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The driver is an important part of the cars success. It controls when the car can accelerate and how to rotate the car
public class Driver
{

    // Correction cone is a variable that affects when the driver decides to rotate the car. This value
    // can go from -1 to 1. A value of 0 will mean the driver does not correct the cars rotation before it has reached 90
    // degrees in either direction. A value of 0.5 means the driver corrects the cars rotation when it's over 45 degrees
    // in either direction. A value of -1 means the driver will not correct the rotation. A value of 1 means the driver
    // will constantly try to keep the car level.
    public float CorrectionCone;
    // This value works like the correction cone. It also goes from -1 to 1, each value corresponds to the same degrees of car
    // rotation. The difference is that the driver will let of the gas if the car has rotated towards the left. This means
    // the car will get time to settle down again without wheelying. If the car is outside the cone, but has rotated towards
    // the right, the driver will still accelerate. This will attempt to get the car out of a nose wheely.
    public float AccelerationCone;

    public Driver(float correctionCone, float accelerationCone)
    {
        CorrectionCone = correctionCone;
        AccelerationCone = accelerationCone;
    }

}

// This class holds information about the cars suspension. This includes stiffness and damping ratio. Stiffness is how much
// slack/give there is in the suspension. It can go from 0 to 1*10^6. Damping ration is the bounciness. 0 means a lot of
// movement in the suspension, while 1 means virtually no movement in the suspension.
public class Suspension
{
    public float Stiffness;
    public float DampingRatio;

    public Suspension(float stiffness, float dampingRatio)
    {
        Stiffness = stiffness;
        DampingRatio = dampingRatio;
    }
}

// The engine contains variables for the engine rotation speed, max motor torque and car rotation torque
public class Engine
{

    public float EngineSpeed;
    public float MaxMotorTorque;
    public float CarRotationTorque;

    public Engine(float engineSpeed, float maxMotorTorque, float carRotationTorque)
    {
        EngineSpeed = engineSpeed;
        MaxMotorTorque = maxMotorTorque;
        CarRotationTorque = carRotationTorque;
    }

}

// The Wheel class contains a position, sizeX and a bool for whether the wheel is symmetrical or not
public class Wheel
{

    public GameObject WheelGO;
    // Both the x and y value for the position can go from -1 to 1. x = -1 would be the edge of the far left side of the car. y = -1 would be the edge of the bottom side.
    public Vector2 Position;
    public Suspension Suspension;
    public float SizeX;

    public Wheel(GameObject wheelGO, Vector2 position, Suspension suspension, float sizeX)
    {
        WheelGO = wheelGO;
        Position = position;
        Suspension = suspension;
        SizeX = sizeX;
    }

}
