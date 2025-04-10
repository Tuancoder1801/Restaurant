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
        GameManager.Instance.smoothCamera.ZoomInToTarget(new List<Transform> {goFocus.transform}, 3f);
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
            //LeanTween.scale(goUI, new Vector3(-0.017f, 0.017f, 1f), 0.5f);
            ieWaitProgress = StartCoroutine(IEWaitProgress());
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);

        if (other.tag.Equals(StaticValue.CHARACTER_NAME_TAG))
        {
            Clear();
            StopCoroutine(ieWaitProgress);
        }
    }

    private void Clear()
    {
        LeanTween.cancel(goUI);
        StopCoroutine(ieWaitProgress);
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

        GameManager.Instance.NextMap();
    }
}
