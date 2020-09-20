using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//takes the noise map and turn it into texture and apply the texture to a plane
public class MapDisplay : MonoBehaviour
{
    public Renderer textureRenderer;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public MeshCollider meshCollider;

    public void DrawTexture(Texture2D texture)
    {
        //Apply texture to texture renderer; Texture.material is only instantitated on runtime
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshCollider.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
}
