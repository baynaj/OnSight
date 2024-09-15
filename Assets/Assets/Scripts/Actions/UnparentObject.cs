using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnparentObject : MonoBehaviour
{

    public void Unparent()
    {
        transform.SetParent(null, true);
    }
}
