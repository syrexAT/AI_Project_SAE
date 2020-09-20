using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//To generate map in editor with a button
[CustomEditor (typeof (MapGenerator))]
public class MapGeneratorEditor : Editor
{
    //public override void OnInspectorGUI()
    //{
    //    //Reference to map generator

    //    MapGenerator mapGen = (MapGenerator)target; //Target is the object which the custom editor is inspecting

    //    if (DrawDefaultInspector()) //if any value was changed
    //    {
    //        if (mapGen.autoUpdate)
    //        {
    //            mapGen.GenerateMap();
    //        }
    //    }

    //    //add button
    //    if (GUILayout.Button("Generate"))
    //    {
    //        mapGen.GenerateMap();
    //    }
    //}
}
