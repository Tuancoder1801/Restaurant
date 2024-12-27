using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationBuild : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    private Material material;
    public float fill = 0f;
    public float max = 1f;

    private void Start()
    {
        material = spriteRenderer.material;
    }

    private void Update()
    {

        if (fill < max)
        {
            fill += Time.deltaTime;
            material.SetFloat("_Arc2", 360f - ((fill / max) * 360f));
        }
    }
}
