using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Population : MonoBehaviour {

    public List<Car_Class> population = new List<Car_Class>();
    public List<Car_Class> tempPopulation = new List<Car_Class>();

    public GenerationContainer generationsContainer = new GenerationContainer();

    // Save stats here:
    string statsPath = "";

    void Awake()
    {
        statsPath = Path.Combine(Application.persistentDataPath, "gameData.json");
    }

    // CalculateFitness will tell every single car to find their own fitness. 
    public void CalculateFitness()
    {
        for(int i = 0; i < population.Count; i++)
        {
            population[i].CalculateFitness();
        }
    }

    public int generation = 0;
    public float highestFitness;

    public void Generate()
    {
        generation++;
        highestFitness = 0;
        for (int i = 0; i < population.Count; i++)
        {
            if(population[i].fitness > highestFitness) { highestFitness = population[i].fitness; }
        }

        CollectStats();

        List<Car_Class> newPopulation = new List<Car_Class>();
        for(int i = 0; i < population.Count; i++)
        {
            Car_Class partnerA = AcceptReject();
            Car_Class partnerB = AcceptReject();
            Car_Class child = partnerA.CrossOver(partnerB);
            child.Mutate();
            newPopulation.Add(child);
        }
        tempPopulation = newPopulation;
        DestroyPreviousGeneration();
        population = newPopulation;
    }

    void CollectStats()
    {
        Generation gen = new Generation();

        gen.generationNumber = generation;
        gen.longestDistance = highestFitness;
        gen.averageDistance = AverageDistance();
        gen.averageCarSize = AverageCarSize();
        gen.averageEngineSpeed = AverageEngineSpeed();
        gen.averageMaxMotorTorque = AverageMaxMotorTorque();

        generationsContainer.generations.Add(gen);

        string json = JsonUtility.ToJson(generationsContainer);

        File.WriteAllText(statsPath, json);
    }

    float AverageEngineSpeed()
    {
        float engineSpeed = 0;
        for (int i = 0; i < population.Count; i++)
        {
            engineSpeed += population[i].Engine.EngineSpeed;
        }
        engineSpeed = engineSpeed / population.Count;
        return engineSpeed;
    }

    float AverageMaxMotorTorque()
    {
        float maxMotorTorque = 0;
        for (int i = 0; i < population.Count; i++)
        {
            maxMotorTorque += population[i].Engine.MaxMotorTorque;
        }
        maxMotorTorque = maxMotorTorque / population.Count;
        return maxMotorTorque;
    }

    float AverageDistance()
    {
        float averageDistance = 0;
        for(int i = 0; i < population.Count; i++)
        {
            averageDistance += population[i].fitness;
        }
        averageDistance = averageDistance / population.Count;
        return averageDistance;
    }

    float AverageCarSize()
    {
        float averageCarSize = 0;
        for (int i = 0; i < population.Count; i++)
        {
            averageCarSize += population[i].Size.x * population[i].Size.y;
        }
        averageCarSize = averageCarSize / population.Count;
        return averageCarSize;
    }

    void DestroyPreviousGeneration()
    {
        for (int i = population.Count - 1; i >= 0; i--)
        {
            Destroy(population[i].CarParent);
        }
        GameObject[] deletables = GameObject.FindGameObjectsWithTag("Delete");
        for (int i = deletables.Length - 1; i >= 0; i--)
        {
            Destroy(deletables[i]);
        }
    }

    // This function will return a Car_Class, but it will disproportionally favor Car_Classes with higher fitness.
    // It means the best cars are more likely to have their genes make up the next generation of cars.
    private Car_Class AcceptReject()
    {
        int useProtection = 0;
        while (true)
        {
            int index = UnityEngine.Random.Range(0, population.Count - 1);
            float r = UnityEngine.Random.Range(0f, highestFitness);
            Car_Class partner = population[index];
            if(r < partner.fitness) { return partner; }
            useProtection++;
            if(useProtection > 10000) { print("HAD TO USE PROTECTIOn"); return null; }
        }
    }
}

[Serializable]
public class GenerationContainer
{
    public List<Generation> generations;
}

[Serializable]
public class Generation
{
    public int generationNumber;
    public float longestDistance;
    public float averageDistance;
    public float averageCarSize;
    public float averageEngineSpeed;
    public float averageMaxMotorTorque;

}
