using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkRotate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    private void FixedUpdate() {
        transform.Rotate(0.0f, -0.1f, 0.0f);
    }
}
