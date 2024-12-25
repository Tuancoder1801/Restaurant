using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float time = 0;
    public Image fill;
    public float max;

    private bool isCounting = false;

    public void Update()
    {
        if (isCounting)
        {
            time += Time.deltaTime;
            fill.fillAmount = time / max;
        }
    }

    public void StartCount()
    {       
        gameObject.SetActive(true);
        isCounting = true;
    }

    public void ResetTimer()
    {
        time = 0;
        fill.fillAmount = 0;
    }

    public void StopCount()
    {
        isCounting = false;
        gameObject.SetActive(false);
        ResetTimer();
    }

    public bool IsCookCompleted()
    {
        return time >= max;
    }
}
