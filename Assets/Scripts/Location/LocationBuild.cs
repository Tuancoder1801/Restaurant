using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationBuild : LocationBase
{
    public SpriteRenderer spriteRenderer;

    private Material material;
    private bool isWarning;
    private Transform tranPlayer;
    private IEnumerator ieWaitTakeMoney;

    private void OnEnable()
    {
        material = spriteRenderer.material;
    }

    private void OnDisable()
    {
        Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(StaticValue.CHARACTER_NAME_TAG))
        {
            tranPlayer = other.transform;
            if (ieWaitTakeMoney != null) StopCoroutine(ieWaitTakeMoney);
            ieWaitTakeMoney = LoadFill(); // Gán coroutine
            StartCoroutine(ieWaitTakeMoney);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals(StaticValue.CHARACTER_NAME_TAG))
        {
            tranPlayer = null;
            material.SetFloat("_Arc2", 360f);
            spriteRenderer.material = material;

            if (ieWaitTakeMoney != null) StopCoroutine(ieWaitTakeMoney);
        }
    }

    private IEnumerator LoadFill()
    {
        float fill = 0f;
        float max = 1f;
        float duration = 2f; 
        float elapsed = 0f;

        while (elapsed < duration)
        {
            fill = Mathf.Lerp(0f, max, elapsed / duration); 
            material.SetFloat("_Arc2", 360f - ((fill / max) * 360f));
            spriteRenderer.material = material;

            elapsed += Time.deltaTime;
            yield return null; 
        }

        //material.SetFloat("_Arc2", 0f);
        //spriteRenderer.material = material;

        GameManager.Instance.OnBuildCompleted(this);
    }

    private void Clear()
    {
        material.SetFloat("_Arc2", 360f);
        spriteRenderer.material = material;
    }
}
