using UnityEngine;

public class MapDisplay : MonoBehaviour
{

    public Renderer textureRender;

    public void DrawTexture(Texture2D texture)
    {

        textureRender.sharedMaterial.mainTexture = texture;
    }

}