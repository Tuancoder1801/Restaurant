using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationNextMap : LocationBase
{
    public GameObject goFxSmoke;
    public GameObject goArea;
    public GameObject goUI;
    public GameObject goFocus;
    public Image imgProgress;

    public Animator animCar;

    public float timeProgress;

    private Coroutine ieWaitProgress;

    private void OnEnable()
    {
        int currentMap = UserData.map.currentMapIndex;
        if (!UserData.map.IsBuildUnlocked(currentMap, buildIndex))
        {
            GameManager.Instance.smoothCamera.ZoomInToTarget(new List<Transform> { goFocus.transform }, 3f);
        }
    }

    private void OnDisable()
    {
        Clear();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.tag.Equals(StaticValue.CHARACTER_NAME_TAG))
        {
            ieWaitProgress = StartCoroutine(IEWaitProgress());
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);

        if (other.tag.Equals(StaticValue.CHARACTER_NAME_TAG))
        {
            Clear();
            if (ieWaitProgress != null)
            {
                StopCoroutine(ieWaitProgress);
                ieWaitProgress = null;
            }
        }
    }

    private void Clear()
    {
        LeanTween.cancel(goUI);
        if (ieWaitProgress != null)
        {
            StopCoroutine(ieWaitProgress);
            ieWaitProgress = null;
        }
        LeanTween.scale(goUI, new Vector3(-0.015f, 0.015f, 1f), 0.5f);
        imgProgress.fillAmount = 0;
    }

    public IEnumerator IEWaitProgress()
    {
        float time = 0;
        while(time < timeProgress)
        {
            time += Time.deltaTime;
            imgProgress.fillAmount = time/timeProgress;
            yield return null;
        }

        imgProgress.fillAmount = 1;

        animCar.SetTrigger("move");
        goUI.SetActive(false);
        goArea.SetActive(false);
        goFxSmoke.SetActive(true);
        GameManager.Instance.player.gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);

        UIGameManager.Instance.gameObject.SetActive(false);
        UserData.CompleteCurrentMap();
        GameManager.Instance.NextMap();
    }
}
