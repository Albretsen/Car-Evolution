using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Spawner : MonoBehaviour {

    [Header("Scripts")]
    public Main main;

    [Header("Prefabs")]
    public GameObject bodyPrefab;
    public GameObject wheelPrefab;

	void Start () {
        /*
        // This is how you create a car class
        Car_Class defaultCar = new Car_Class(
                                                new GameObject("Default Car"), // Parent GameObject 
                                                new GameObject(""), // Reference to body object
                                                new Vector3(0.5f, 0.5f, 0), // Size of car
                                                new List<Wheel> { new Wheel(new GameObject("Wheel"), new Vector2(-1, -1), new Suspension(5, 0.5f), 1), new Wheel(new GameObject("Wheel"), new Vector2(1, -1), new Suspension(5, 0.5f), 1) }, // Array of wheels
                                                new Engine(1000f, 10000f, 1000f), // Cars engine. Engine speed | Max torque | Rotation speed
                                                new Driver(0f,0f) // Driver. CorrectionCone | AccelerationCone
                                                );

        Car_Class randomizedCar = new Car_Class(
                                                new GameObject("Randomized Car"), // Parent GameObject 
                                                new GameObject(""), // Reference to body object
                                                new Vector3(0.5f, 0.5f, 0), // Size of car
                                                new List<Wheel> { new Wheel(new GameObject("Wheel"), new Vector2(-1, -1), new Suspension(5, 0.5f), 1), new Wheel(new GameObject("Wheel"), new Vector2(1, -1), new Suspension(5, 0.5f), 1) }, // Array of wheels
                                                new Engine(1000f, 10000f, 1000f), // Cars engine. Engine speed | Max torque | Rotation speed
                                                new Driver(0f, 0f) // Driver. CorrectionCone | AccelerationCone
                                                );

        // Spawning a car
        SpawnCar(randomizedCar);*/
	}

    private Quaternion emptyQuaternion = new Quaternion(0, 0, 0, 0);
    private Vector3 emptyVector3 = new Vector3(0, 0, 0);
	
    // This function is responsible for spawning a car based on a Car_Class input. It will return a Car_Class
    // with references to the spawned objects of that car.
    public Car_Class SpawnCar(Car_Class car)
    {
        // If wheel amount is not affected by evolution, make sure every car has 2 wheels
        if (!main.wheelAmountAffectedByEvolution)
        {
            car.Wheels = car.Wheels.GetRange(0, 2);
            for(int i = 0; i < 2; i++)
            {
                if(car.Wheels.Count <= 1)
                {
                    car.Wheels.Add(new Wheel(new GameObject("Wheel WTF"), new Vector2(-1, -1), new Suspension(5, 0.5f), 1));
                }
            }
        }

        // Create a parent object that all other objects can be listed under in the hierarchy
        string carParentName = car.CarParent.name;
        Destroy(car.CarParent);
        if(carParentName == "") { carParentName = "Car"; }
        car.CarParent = new GameObject(carParentName);
        car.DNA[0] = car.CarParent;

        // Create the car body
        Destroy(car.Body);
        car.Body = Instantiate(bodyPrefab, emptyVector3, emptyQuaternion, car.CarParent.transform);
        car.DNA[1] = car.Body;
        car.Body.transform.localScale = car.Size;

        // Make sure the body has a body tag, so we can exclude it from colliding with other cars.
        car.Body.layer = 8;

        // Determine the cars weight based on scale
        float mass = car.Size.x * car.Size.y * 100 * main.percentageOfCalculatedMass;
        if(mass < main.lowestMass) { mass = main.lowestMass; }
        car.Body.GetComponent<Rigidbody2D>().mass = mass;

        // Initialize the car controller component on the car body
        Car_Controller car_controller = car.Body.GetComponent<Car_Controller>();

        // Setup the engine
        car_controller.EngineSpeed = car.Engine.EngineSpeed;
        car_controller.MaxMotorTorque = car.Engine.MaxMotorTorque;
        car_controller.CarRotationTorque = car.Engine.CarRotationTorque;

        // Prepare the car driver
        car_controller.CorrectionCone = car.Driver.CorrectionCone;
        car_controller.AccelerationCone = car.Driver.AccelerationCone;
        if(car.Driver.CorrectionCone != 0 || car.Driver.AccelerationCone != 0)
        {
            car_controller.hasDriver = true;
        }

        // Add the wheels
        float carLengthX = car.Body.GetComponent<SpriteRenderer>().bounds.size.x * (1 / car.Size.x) / 2;
        float carLengthY = car.Body.GetComponent<SpriteRenderer>().bounds.size.y * (1 / car.Size.y) / 2;

        foreach (Wheel wheel in car.Wheels)
        {
            // Get the position and instantiate the wheel object
            Vector3 wheelPos = new Vector3(Map(wheel.Position.x, -1, 1, carLengthX * -1, carLengthX), Map(wheel.Position.y, -1, 1, carLengthY * -1, carLengthY), 0);
            wheel.WheelGO = Instantiate(wheelPrefab, wheelPos, emptyQuaternion, car.CarParent.transform);

            // Add the wheel join
            WheelJoint2D wheelJoint = car.Body.AddComponent<WheelJoint2D>();
            wheelJoint.connectedBody = wheel.WheelGO.GetComponent<Rigidbody2D>();
            wheelJoint.anchor = wheelPos;

            // Set wheels graphical size
            wheel.WheelGO.GetComponent<SpriteRenderer>().size = new Vector3(wheel.SizeX, wheel.SizeX);

            // Add wheel to the car tag, to avoid cars colliding with each other
            wheel.WheelGO.layer = 8;

            // Setup the suspension
            wheelJoint.suspension = new JointSuspension2D { frequency = wheel.Suspension.Stiffness, dampingRatio = wheel.Suspension.DampingRatio, angle = 90};
        }
        car.Body.GetComponent<Car_Controller>().wheels = car.Body.GetComponents<WheelJoint2D>();
        car.DNA[3] = car.Wheels;

        return car;
    }

    public float Map(float value, float fromSource, float toSource, float fromTarget, float toTarget)
    {
        return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
    }

}
