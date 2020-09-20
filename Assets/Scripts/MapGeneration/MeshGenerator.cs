using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, int levelOfDetail)
    {
        int width = heightMap.GetLength(0); 
        int height = heightMap.GetLength(1);
        float topLeftX = (width - 1) / -2f; //left
        float topLeftZ = (height - 1) / 2f; //right

        int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2; //if levelofdetail is 0 we want the meshSimplifciation to be 1
        int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;

        MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);
        int vertexIndex = 0; // at the end of each loop increment it by 1 so we can keep track of where we are in the array

        for (int y = 0; y < height; y+=meshSimplificationIncrement)
        {
            for (int x = 0; x < width; x+=meshSimplificationIncrement)
            {
                //Create vertices
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x,y]) * heightMultiplier, topLeftZ - y); //for the y -> we pass in the value of the heightmap at the current point, for Z the y
                //the mesh should be perfectly centered on the screen, so z.B. der punkt in der mitte muss 0 haben, der links -1, der rechts +1
                //um den linken (-1) zu bekommen muss man x = (width - 1) / 2 == 3-1 = 2 = 2/-2 = -1  --> topLeftX

                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                //set the triangles, looping through the map z.B: wenn wir vertex 0 sind setzen wir beide triangles für den square, das heisst wir brauchen nicht die ganz rechten und die unteren
                if (x < width - 1 && y < height - 1) //ignoring the right and bottom edge vertices of the map
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                    meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }
        return meshData;
    }
}

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs; //for texturing the mesh

    int triangleIndex;//keep track of the current index of triangle array

    //Constructor
    public MeshData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight]; //tell each vertex where it is in relation to the rest the map in percentage to the x and y axis
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
        //3x3 map hat 9 vertices
        //Meshes sind triangles, also braucht man int array wo each set of 3 int points to the vertices which make up the triangle
    }
    public void AddTriangle(int a, int b, int c) //3 points-- each vertex of the triangle
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    //Getting the mesh from the meshdata
    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals(); //for lighting
        return mesh;
    }
}
