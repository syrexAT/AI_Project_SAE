using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
    //Method to create a texture out of a one dimensional color map
    public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }

    //Method to get a texture based on 2D heightmap
    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        int width = heightMap.GetLength(0); //For the first dimension in the 2D Array
        int height = heightMap.GetLength(1); //For the second dimension in the 2D Array


        //Set color of each of the pixels
        //Faster to first generate an array of all the colors of all the pixels and set them at once
        Color[] colorMap = new Color[width * height];

        //Loop through all the values in our noise map
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                //Mulitplying y with width gives us the index of the row we are on and to get the column + x
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }

        return TextureFromColorMap(colorMap, width, height);

    }
}
