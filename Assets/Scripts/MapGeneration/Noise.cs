using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise //static bc it will only have this instance; no monobehaviour bc we wont apply it on any object
{
    //Method for generating a noisemap and return falues from 0-1 --> 2D Array

    #region Octave/lacunarity/persistance explanation
    /*Imagine a mountain
    First octave (y = amplitude, x = frequency) defines the main outline of the mountain
    Second octave defines the boulders
    Third octave defines the small rocks
    So more detailed with each subsequent octave
    To Control this we create lacunarity = the frequency of octaves
    as each octave increases in detail its influence should diminish!
    the smaller the rock the less effect it should have on the outline of the mountain!!
    --> so we create Persistance, we set the amplitude of octaves
    persistance value 0-1 lets us control how rapidly the amplitude decreases with each octave
    With doing all of this the mountain is realistiaclly good looking*/
    #endregion

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        //seed to get the same map when we want
        System.Random prng = new System.Random(seed);
        //we want each octave to be sampled from a different location, so Vector2 array
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x; //+ offset to scroll through noise with our own offset value
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (scale <= 0)
        {
            scale = 0.0001f; //weil wenn 0 dann dividerd durch 0!
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        //to not zoom into top right when setting noise scale
        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0; //instead of setting noiseMap directly to perlinValue, we want to increase the noiseHeight by the perlin Value of each octave

                for (int i = 0; i < octaves; i++)
                {
                    //At which point well sampling our height values from
                    //will give us the same value when we only use the int values --> deswegen float scale --> for no int values
                    float sampleX = (x-halfWidth) / scale * frequency + octaveOffsets[i].x; //the higher the frequency the further apart the sample points will be --> the height values will change more rapidly
                    float sampleY = (y-halfHeight) / scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1; //is only 0-1 but to get interesting values we want some values to be negative
                    //noiseMap[x, y] = perlinValue;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance; //decreses each octave as its 0-1
                    frequency *= lacunarity; //increases each octaves as lacunarity should be greater than 1
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;

            }
        }

        //normalise so all is 0-1 again
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                //Loop through all of the noisemap again
                //for each value in the noiseMap we want to set that equal to InverseLerp(minNoiseheight, maxNoiseHeight, current noise Map value)
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]); //InverseLerp returns value 0-1, if noiseMapValue is equal to minNoiseHeight = 0, equal to maxNoiseHeight = 1, halfway 0.5 usw.
            }
        }

        return noiseMap;
    }
}
