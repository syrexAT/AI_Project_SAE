using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RegrowPlants : MonoBehaviour
{
    public List<GameObject> currentPlantsList = new List<GameObject>();
    public int initialPlants;
    public int currentPlants;

    public bool gotPlants = false;

    public Spawner spawner;

    // Start is called before the first frame update
    void Start()
    {
        spawner = GetComponent<Spawner>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gotPlants == false)
        {
            initialPlants = GameObject.FindGameObjectsWithTag("Plant").Length;
            gotPlants = true;
        }

        currentPlants = GameObject.FindGameObjectsWithTag("Plant").Length;

        if (currentPlants <= 20)
        {
            
        }

        print(initialPlants);
        print(currentPlants);
        
    }
}
