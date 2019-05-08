using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* The population is made up of Car_Class instances. It holds references to the simulated car, as well
   as all the variables (DNA) that the spawner uses to create the car. Car_Class also has a few functions
   regarding generating the next generation.*/
public class Car_Class {

    // This is the parent object of the car. It has no other components than a transform, and is purely
    // used to keep the cars seperate in the hierarchy.
    public GameObject CarParent;
    // Body is a reference to GameObject body
    public GameObject Body;
    // The size of the body object
    public Vector3 Size { get; set; }
    // A list of all the cars wheels. A Wheel is a class that contains a size, position and whether the wheel is symmetrical or not.
    public List<Wheel> Wheels { get; set; }
    // Instance of this cars engine. It has variables for engine speed, max motor torque and car rotation torque.
    public Engine Engine;
    // This is the cars driver
    public Driver Driver;

    // This is just a function used when instantiating a new Car_Class
    public Car_Class(GameObject carParent, GameObject body, Vector3 size, List<Wheel> wheels, Engine engine, Driver driver)

    {
        // Assign all variables
        CarParent = carParent;
        Body = body;
        Size = size;
        Wheels = wheels;
        Engine = engine;
        Driver = driver;

        // Create the DNA

        DNA.Add(carParent);
        DNA.Add(body);
        DNA.Add(size);
        DNA.Add(wheels);
        DNA.Add(engine);
        DNA.Add(driver);

        main = GameObject.FindGameObjectWithTag("Main").GetComponent<Main>();

        /*DNA.Add("CarParent", carParent);
        DNA.Add("Body", body);
        DNA.Add("Size", size);
        DNA.Add("Wheels", wheels);
        DNA.Add("Engine", engine);
        DNA.Add("Driver", driver);*/
    }

    // Variables not part of the DNA is listed under here:
    public float fitness;
    private Main main;

    // Here we will determine how far the car went, and call it "fitness". The fitness will be scaled down to 0 to 1 scale
    // later. Downscaling relies on knowing the longest distance travelled by any car, so scaling now would potentially 
    // require each car to run two functions, one two find distance, and one to scale when I know the longest distance
    // travelled. This is purely done for optimization.
    public void CalculateFitness()
    {
        fitness = Body.transform.position.x;
    }

    // The mutation function will randomly alter the car
    public void Mutate()
    {
        for(int i = 2; i < DNA.Count; i++)
        {
            if(Random.Range(0f, 1f) < main.mutationRate)
            {
                PerformMutation(i);
            }
        }

        Size = (Vector3)DNA[2];
        Wheels = (List<Wheel>)DNA[3];
        Engine = (Engine)DNA[4];
    }

    // This function will alter the cars DNA. Each case has to be handmade to fit each type
    // of DNA in the DNA list.
    private void PerformMutation(int i)
    {
        switch (i)
        {
            case 2:
                if (main.sizeAffectedByEvolution)
                {
                    Vector3 temp = (Vector3)DNA[2];
                   //Debug.Log("1: " + temp.x + " | " + temp.y);
                    float x = Mathf.Clamp(temp.x + Random.Range(-main.sizeMutationRange, main.sizeMutationRange), main.minCarSizeX, main.maxCarSizeX);
                    float y = Mathf.Clamp(temp.y + Random.Range(-main.sizeMutationRange, main.sizeMutationRange), main.minCarSizeY, main.minCarSizeY);
                    temp = new Vector3(x, y, temp.z);
                    //Debug.Log("2: " + temp.x + " | " + temp.y);
                    DNA[2] = temp;
                    Vector3 temp2 = (Vector3)DNA[2];
                    //Debug.Log("3: " + temp2.x + " | " + temp2.y);
                }
                break;
            case 3:
                List<Wheel> tempWheels = (List<Wheel>)DNA[3];
                // 30% chance to change wheel amount. 70% chance to change wheel size
                if (Random.Range(0f, 1f) < 0.3f)
                {
                    if (!main.wheelAmountAffectedByEvolution)
                    {
                        if (Random.Range(0f, 1f) < 0.5f) { tempWheels.RemoveAt(tempWheels.Count - 1); }
                        else
                        {
                            tempWheels.Add(new Wheel(new GameObject("Wheel"), new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)), new Suspension(Random.Range(main.minSuspensionStiffness, main.maxSuspensionStiffness), Random.Range(main.minSuspensionDamping, main.maxSuspensionDamping)), Random.Range(main.minWheelSize, main.maxWheelSize)));
                        }
                    }
                }
                else
                {
                    int x = Random.Range(0, tempWheels.Count);
                    tempWheels[x].SizeX += Random.Range(-main.wheelSizeMutationRange, main.wheelSizeMutationRange);
                }
                DNA[3] = tempWheels;
                break;
            case 4:
                if (main.engineAffectedByEvolution)
                {
                    Engine engine = (Engine)DNA[4];
                    // 50% to change speed. 25% change to change max engine torque, 25% to change car rotation torque.
                    int randInt = Random.Range(0, 3);
                    switch (randInt)
                    {
                        case 0:
                            engine.EngineSpeed = Mathf.Clamp(Random.Range(-main.engineSpeedMutationRange, main.engineSpeedMutationRange) + engine.EngineSpeed, main.minEngineSpeed, main.maxEngineSpeed);
                            break;
                        case 1:
                            engine.EngineSpeed = Mathf.Clamp(Random.Range(-main.engineSpeedMutationRange, main.engineSpeedMutationRange) + engine.EngineSpeed, main.minEngineSpeed, main.maxEngineSpeed);
                            break;
                        case 2:
                            engine.MaxMotorTorque = Mathf.Clamp(Random.Range(-main.engineMaxMotorTorqueMutationRange, main.engineMaxMotorTorqueMutationRange) + engine.MaxMotorTorque, main.minMaxTorque, main.maxMaxTorque);
                            break;
                        case 3:
                            engine.CarRotationTorque = Mathf.Clamp(Random.Range(-main.engineCarRotationTorqueMutationRange, main.engineCarRotationTorqueMutationRange) + engine.CarRotationTorque, main.minRotationSpeed, main.maxRotationSpeed);
                            break;
                        default:
                            Debug.Log("You shouldn't every see this, but printing just in case");
                            break;
                    }
                }
                DNA[4] = Engine;
                break;
            case 5:
                if (main.driverAffectedByEvolution)
                {
                    // Perform mutation
                }
                break;
            default:
                Debug.Log("Car_Class function 'PerformMutation' fucked up somehow");
                break;
        }
    }

    // This function will randomly combine the genes of two Car_Classes. 
    public Car_Class CrossOver(Car_Class partner)
    {
        GameObject parent = new GameObject("ParentDel");
        GameObject body = new GameObject("BodyDel");

        parent.tag = "Delete";
        body.tag = "Delete";

        Car_Class child = new Car_Class(
                                        parent, // Parent GameObject 
                                        body, // Reference to body object
                                        new Vector3(0.5f, 0.5f, 0), // Size of car
                                        new List<Wheel> { }, // Array of wheels
                                        new Engine(1000f, 10000f, 1000f), // Cars engine. Engine speed | Max torque | Rotation speed
                                        new Driver(0f, 0f) // Driver. CorrectionCone | AccelerationCone
                                        );

        int midPoint = Random.Range(0, DNA.Count);

        for(int i = 0; i < DNA.Count; i++)
        {
            if (i > midPoint)
            {
                child.DNA[i] = DNA[i];
            }
            else
            {
                child.DNA[i] = partner.DNA[i];
            }
        }

        List<Wheel> wheels = (List<Wheel>)child.DNA[3];
        List<object> temp = child.DNA;
        child = new Car_Class(
                              (GameObject)child.DNA[0], // Parent GameObject 
                              (GameObject)child.DNA[1], // Reference to body object
                              (Vector3)child.DNA[2], // Size of car
                              wheels, // Array of wheels
                              (Engine)child.DNA[4], // Cars engine. Engine speed | Max torque | Rotation speed
                              (Driver)child.DNA[5] // Driver. CorrectionCone | AccelerationCone
                              );

        return child;
    }

    // The DNA list contains every variable that is affected by the evolution
    public List<object> DNA = new List<object>();

}
