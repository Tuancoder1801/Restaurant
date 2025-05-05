using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    public Image imgProgress;

    private string nameMap;

    private void Start()
    {
        Screen.SetResolution(540, 810, false);

        LeanTween.value(gameObject, (value) =>
        {
            imgProgress.fillAmount = value;
        }, 0f, 1f, 1.5f).setOnComplete(() =>
        {
            imgProgress.fillAmount = 1f;
        });
        StartCoroutine(IEWaitNextScene());
    }

    public IEnumerator IEWaitNextScene()
    {
        yield return new WaitForEndOfFrame();

        yield return new WaitUntil(() => GameData.Instance != null);

        yield return new WaitUntil(() => imgProgress.fillAmount >= 1);

        nameMap = GameData.Instance.GetNameMap();

        SceneManager.LoadScene(nameMap);
    }
}
