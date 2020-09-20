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
        //mit LoadSceneAsync das navmesh im hintergrund builden lassen
        //dann mit async.progress krieg ich eine float vom progress, die kann ich z.B. in ein ladebalken geben (ist ein float von 0-1)
        //var async = SceneManager.LoadSceneAsync(1);
        //async.progress()


    }

    // Start is called before the first frame update
    void Start()
    {
        //To give a random map on Start
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

        Debug.Log("RANDOMSEED :" + mapGen.seed);


        mapGen.GenerateMap();

        //COMMENTED BECAUSE OF LONG EDITOR LOADING TIME
        surface.BuildNavMesh();




        MeshRenderer waterPlaneMesh = waterPlaneNav.GetComponent<MeshRenderer>();
        waterPlaneMesh.enabled = false;

        MeshRenderer mapPlaneMesh = mapPlane.GetComponent<MeshRenderer>();
        mapPlaneMesh.enabled = false;
        
        //StartCoroutine(DeactivateWaterPlane());
    }

    IEnumerator DeactivateWaterPlane()
    {
        yield return new WaitForSeconds(1);
        waterPlaneNav.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public IEnumerator BuildMapAndNavMesh()
    //{
    //    mapGen.seed = Random.Range(0, 10000);

    //    mapGen.GenerateMap();

    //    //COMMENTED BECAUSE OF LONG EDITOR LOADING TIME
    //    surface.BuildNavMesh();
    //}
}
