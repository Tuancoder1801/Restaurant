using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    public static LoadingUI Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI loadingCountingTxt;
    [SerializeField] private Image loadingfilled;

    private Coroutine fillCoroutine;

    public void UpdateUI(float loadingProgress)
    {   
        if (loadingCountingTxt)
        {
            loadingCountingTxt.text = $"Loading...{(loadingProgress * 100).ToString("f0")}%";
        }

        if (loadingfilled)
        {
            if (fillCoroutine != null)
                StopCoroutine(fillCoroutine); // Dừng coroutine cũ nếu có

            fillCoroutine = StartCoroutine(SmoothFill(loadingProgress));
        }
    }

    private IEnumerator SmoothFill(float targetFill)
    {
        float startFill = loadingfilled.fillAmount;
        float elapsedTime = 0f;
        float duration = 0.5f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            loadingfilled.fillAmount = Mathf.Lerp(startFill, targetFill, elapsedTime / duration);
            yield return null;
        }

        loadingfilled.fillAmount = targetFill;
    }

    public bool IsFull()
    {
        return Mathf.Approximately(loadingfilled.fillAmount, 1f);
    }
}
