using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    public MapGenerator mapGen;
    public NavMeshSurface surface;
    public GameObject waterPlaneNav;
    public GameObject mapPlane;

    private void Awake()
    {
        if (PlayerPrefExtension.GetBool("randomSeedBool") == true)
        {
            mapGen.seed = Random.Range(0, 10000);
        }
        else if (PlayerPrefExtension.GetBool("randomSeedBool") == false && PlayerPrefs.GetInt("customSeedInput") != 0)
        {
            int customSeed;
            customSeed = PlayerPrefs.GetInt("customSeedInput");
            mapGen.seed = customSeed;
            Debug.Log("CUSTOMSEED :" + customSeed);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("RANDOMSEED :" + mapGen.seed);
        mapGen.GenerateMap();

        surface.BuildNavMesh();

        MeshRenderer waterPlaneMesh = waterPlaneNav.GetComponent<MeshRenderer>();
        waterPlaneMesh.enabled = false;

        MeshRenderer mapPlaneMesh = mapPlane.GetComponent<MeshRenderer>();
        mapPlaneMesh.enabled = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
    }
}
