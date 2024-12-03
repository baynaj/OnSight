using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whiteboard : MonoBehaviour
{
    public Texture2D texture;
    public Color startingColor = Color.white;
    //public Vector2 texSize = new Vector2(2048, 2048);
    public Vector2 texSize = new Vector2(1024, 1024);

    void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        texture = new Texture2D((int)texSize.x, (int)texSize.y);
        rend.material.mainTexture = texture;
        SetColor();
    }

    void SetColor()
    {
        //set all of the pixels to our starting color
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                texture.SetPixel(x, y, startingColor);
            }
        }
        Debug.Log("Set color.");
    }

    //a 2 second timer function that runs a function when its done
    public IEnumerator Timer(float time, System.Action function)
    {
        yield return new WaitForSeconds(time);
        function();
    }
}
