using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bababa : MonoBehaviour
{
    int blendShapeCount;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public Mesh skinnedMesh;
    //float blendOne = 0f;
    //float blendTwo = 0f;
    //float blendSpeed = 1f;
    bool isBaing = false;

    void Awake()
    {
        //skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        //skinnedMesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
    }

    void Start()
    {
        //blendShapeCount = skinnedMesh.blendShapeCount;
        InvokeRepeating(nameof(SetMouthPosition), 0, 0.02f);
    }

    void Update()
    {
        //skinnedMeshRenderer.SetBlendShapeWeight(4, 100);
    }

    private void SetMouthPosition()
    {
        isBaing = !isBaing;
        
        if (isBaing)
            skinnedMeshRenderer.SetBlendShapeWeight(4, 100);
        else
            skinnedMeshRenderer.SetBlendShapeWeight(4, 0);

        Debug.Log($"isBaing: {isBaing} -- {skinnedMeshRenderer.GetBlendShapeWeight(4)}");
    }
}