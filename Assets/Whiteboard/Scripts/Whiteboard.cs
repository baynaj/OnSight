using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whiteboard : MonoBehaviour
{
    public Texture2D texture;
    public Vector2 texSize = new Vector2(2048, 2048);

    void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        texture = new Texture2D((int)texSize.x, (int)texSize.y);
        rend.material.mainTexture = texture;
    }



}
