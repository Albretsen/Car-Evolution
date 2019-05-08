using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {

    [Header("Evolution variables")]
    public int carPerGeneration;
    public float percentageOfCalculatedMass;
    public float lowestMass;
    public float timeInSecondsPerGeneration;
    public float mutationRate;

    [Header("Evolution settings")]
    public bool includeDriver;

    [Header("Affected by evolution")]
    public bool wheelAmountAffectedByEvolution;
    public bool driverAffectedByEvolution;
    public bool engineAffectedByEvolution;
    public bool sizeAffectedByEvolution;

    [Header("Default Settings")]
    public float engineSpeed;
    public float maxTorque;
    public float rotationSpeed;
    public float correctionCone;
    public float accelerationCone;
    public Vector3 size;

    [Header("Randomizer Paramaters")]
    public float minCarSizeX;
    public float maxCarSizeX;
    public float minCarSizeY;
    public float maxCarSizeY;
    public float minWheelSize;
    public float maxWheelSize;
    public float minEngineSpeed;
    public float maxEngineSpeed;
    public float minMaxTorque;
    public float maxMaxTorque;
    public float minRotationSpeed;
    public float maxRotationSpeed;
    public float minSuspensionStiffness;
    public float maxSuspensionStiffness;
    public float minSuspensionDamping;
    public float maxSuspensionDamping;
    public float minCorrectionCone;
    public float maxCorrectionCone;
    public float minAccelerationCone;
    public float maxAccelerationCone;
    public int minNumberOfWheels;
    public int maxNumberOfWheels;

    [Header("Mutation paramters")]
    public float sizeMutationRange;
    public float wheelSizeMutationRange;
    public float engineSpeedMutationRange;
    public float engineMaxMotorTorqueMutationRange;
    public float engineCarRotationTorqueMutationRange;

    [Header("Scripts")]
    public Car_Spawner car_spawner;
    public Car_Population population;

    // Private variables used by this script

    // timeInSeconds is a variable that holds the amount of seconds passed since the beginning of the evolution, untill
    // the start of the last generation. If you subtract this variable from the amount of time in seconds since the 
    // beginning of the evolution, you will get the amount of seconds passed in the current generation.
    private float timeInSeconds;

    void Start()
    {
        InstantiateFirstGen();
        // NewGeneration();
    }

    // This is the core function in this evolution simulator. It is called every few seconds. (Seconds are determined by a public 
    // variable in the inspector) When called, the function will prepare and instantiate the next generation of cars.
    public void NewGeneration()
    {
        population.CalculateFitness();
        population.Generate();

        InstantiateNextGeneration();

        timeInSeconds = Time.time;
    }

    void InstantiateNextGeneration()
    {
        population.population = new List<Car_Class>();
        for (int i = 0; i < population.tempPopulation.Count; i++)
        {
            population.population.Add(car_spawner.SpawnCar(population.tempPopulation[i]));
        }
    }

    // This function will instantiate every car in the first generation. It wall also handle other parts of the initialization
    // process like adding them to the population list. 
    public void InstantiateFirstGen()
    {
        for(int i = 0; i < carPerGeneration; i++)
        {
            population.population.Add(car_spawner.SpawnCar(RandomCar()));
        }

        timeInSeconds = Time.time;
    }

    private Car_Class RandomCar()
    {
        List<Wheel> wheels = new List<Wheel>();
        for(int i = 0; i < Random.Range(minNumberOfWheels, maxNumberOfWheels); i++)
        {
            GameObject wheelGO = new GameObject("Wheel MAIN RANDOM");
            wheelGO.tag = "Delete";
            wheels.Add(new Wheel(wheelGO, new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)), new Suspension(Random.Range(minSuspensionStiffness, maxSuspensionStiffness), Random.Range(minSuspensionDamping, maxSuspensionDamping)), Random.Range(minWheelSize, maxWheelSize)));
        }

        // Create random engine variables
        if (engineAffectedByEvolution)
        {
            engineSpeed = Random.Range(minEngineSpeed, maxEngineSpeed);
            maxTorque = Random.Range(minMaxTorque, maxMaxTorque);
            rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
        }

        // Create random driver variables
        if (driverAffectedByEvolution)
        {
            correctionCone = Random.Range(minCorrectionCone, maxCorrectionCone);
            accelerationCone = Random.Range(minAccelerationCone, maxAccelerationCone);
        }

        // Create random size variables
        if (sizeAffectedByEvolution)
        {
            size.x = Random.Range(minCarSizeX, maxCarSizeX);
            size.y = Random.Range(minCarSizeY, maxCarSizeY);
        }

        // Create a Car_Class using the randomly generated variables
        return new Car_Class(
                            new GameObject("Randomized Car"), // Parent GameObject 
                            new GameObject(""), // Reference to body object
                            size, // Size of car
                            wheels, // Array of wheels
                            new Engine(engineSpeed, maxTorque, rotationSpeed), // Cars engine. Engine speed | Max torque | Rotation speed
                            new Driver(correctionCone, accelerationCone) // Driver. CorrectionCone | AccelerationCone
                            );
    }

    public void FixedUpdate()
    {
        if(Time.time - timeInSeconds >= timeInSecondsPerGeneration)
        {
            NewGeneration();
        }
    }
}
