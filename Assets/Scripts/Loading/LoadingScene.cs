using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    private AsyncOperation async;

    //public UnityEvent<float> Onloading;
    public Image imgProgress;

    private string nameMap;

    private void Start()
    {
        LeanTween.value(gameObject, (value) =>
        {
            imgProgress.fillAmount = value;
        }, 0f, 1f, 1.5f).setOnComplete(() =>
        {
            imgProgress.fillAmount = 1f;
        });
        async = SceneManager.LoadSceneAsync("DataHolder", LoadSceneMode.Additive);
        StartCoroutine(IEWaitNextScene());
    }

    private void Update()
    {
        
    }

    public IEnumerator IEWaitNextScene()
    {
        yield return new WaitForEndOfFrame();

        yield return new WaitUntil(() => GameData.Instance != null);

        yield return new WaitUntil(() => imgProgress.fillAmount == 1);

        nameMap = GameData.Instance.GetNameMap();

        SceneManager.LoadScene(nameMap);
    }
}
