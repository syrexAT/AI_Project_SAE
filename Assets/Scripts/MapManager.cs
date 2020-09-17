using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    public MapGenerator mapGen;
    public NavMeshSurface surface;

    // Start is called before the first frame update
    void Start()
    {
        //mit LoadSceneAsync das navmesh im hintergrund builden lassen
        //dann mit async.progress krieg ich eine float vom progress, die kann ich z.B. in ein ladebalken geben (ist ein float von 0-1)
        //var async = SceneManager.LoadSceneAsync(1);
        //async.progress()
        mapGen.GenerateMap();
        surface.BuildNavMesh();


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
