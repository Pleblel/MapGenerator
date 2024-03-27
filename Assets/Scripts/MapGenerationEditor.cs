using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    System.Random prng = new System.Random(0);
    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target;

        if (DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
            {
                mapGen.GenerateMap(100001, 100001);
            }
        }

        if (GUILayout.Button("Generate"))
        {
            mapGen.GenerateMap(100001, 100001);
        }

        if (GUILayout.Button("Random seed generate"))
        {
            mapGen.GenerateMap(prng.Next(-100000, 100000), prng.Next(-100000, 100000));
        }
    }
}