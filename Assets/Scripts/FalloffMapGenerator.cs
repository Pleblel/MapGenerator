using System;
using UnityEngine;

public class FalloffMapGenerator : MonoBehaviour
{
    public static float[,] GenerateFalloffMap(int size)
    {
        float[,] map = new float[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;
                float r = Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2));

                float value = Mathf.Max(Mathf.Abs(r), Mathf.Abs(r));
                map[i, j] = value;
            }
        }

        return map;
    }
}