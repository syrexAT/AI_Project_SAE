using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//takes the noise map and turn it into texture and apply the texture to a plane
public class MapDisplay : MonoBehaviour
{
    //Reference to renderer of the plane
    public Renderer textureRenderer;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public void DrawTexture(Texture2D texture)
    {
        //Apply texture to texture renderer; Texture.material is only instantitated on runtime
        textureRenderer.sharedMaterial.mainTexture = texture; //THIS IS EDITOR WORKING
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
}
