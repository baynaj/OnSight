using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


[RequireComponent(typeof(Toggle))]
public class ToggleEvent : MonoBehaviour
{
    private Toggle toggle;

    public UnityEvent onValueTrue = null;
    public UnityEvent onValueFalse = null;


    private void Start()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(delegate { ToggleValueChanged(toggle); });
    }

    void ToggleValueChanged(Toggle toggle)
    {
        if (toggle.isOn)
        {
            onValueTrue.Invoke();
        }
        else
        {
            onValueFalse.Invoke();
        }
    }
}
