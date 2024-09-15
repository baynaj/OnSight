using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterLayer : MonoBehaviour
{

    [SerializeField] GameObject water;

    private void OnTriggerEnter(Collider other) {
        //water.gameObject.SetActive(true);
        RenderSettings.fog = true;
    }

    private void OnTriggerExit(Collider other) {
        //water.gameObject.SetActive(false);
        RenderSettings.fog = false;
    }

}
