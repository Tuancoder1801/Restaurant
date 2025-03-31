using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SmoothCamera : MonoBehaviour
{
    public Camera cam;
    public Transform tranTarget;
    public Vector3 distance;

    public float normalSize = 10f;
    public float zoomInSize = 6f;
    public float zoomOutSize = 14f;

    public float speedFollowCamera = 0f;

    public bool allowZoom;

    private float timeMoveCamera = 0.5f;
    private Transform mainTarget;
    private IEnumerator ieWaitZoomLocation;

    private List<CamZoomConfig> zoomConfigs = new List<CamZoomConfig>();

    private void Start()
    {
        MoveCamera();

        mainTarget = tranTarget;
    }

    private void Update()
    {
        MoveCamera();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void MoveCamera()
    {
        if (tranTarget == null)
        {
            return;
        }

        if (speedFollowCamera > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, tranTarget.position + distance, speedFollowCamera * Time.deltaTime);
        }
        else
        {
            transform.position = tranTarget.position + distance;
        }
    }

    public void ZoomInToTarget(List<Transform> trans, float timeDelay = 1f)
    {
        Debug.Log("ZoomInToTarget");
        if (!allowZoom) return;
        Debug.Log("ZoomInToTarget allowZoom");

        foreach (var item in trans)
        {
            zoomConfigs.Add(new CamZoomConfig
            {
                trans = item,
                isZoomIn = true,
                timeDelay = timeDelay
            });
        }

        if (ieWaitZoomLocation == null)
        {
            ieWaitZoomLocation = IEWaitZoomLocation();
            StartCoroutine(ieWaitZoomLocation);
        }
    }

    public void ZoomOutToTarget(List<Transform> trans, float timeDelay = 1f)
    {
        if (!allowZoom) return;

        foreach (var item in trans)
        {
            zoomConfigs.Add(new CamZoomConfig
            {
                trans = item,
                isZoomIn = false,
                timeDelay = timeDelay
            });
        }

        if (ieWaitZoomLocation == null)
        {
            ieWaitZoomLocation = IEWaitZoomLocation();
            StartCoroutine(ieWaitZoomLocation);
        }
    }

    private IEnumerator IEWaitZoomLocation()
    {
        yield return new WaitForEndOfFrame();

        float timeMove = timeMoveCamera;
        while (zoomConfigs.Count > 0)
        {
            var config = zoomConfigs[0];
            zoomConfigs.RemoveAt(0);

            tranTarget = config.trans;

            float dis = Vector3.Distance(transform.position, tranTarget.position + distance);
            speedFollowCamera = dis / timeMoveCamera;

            timeMove = timeMoveCamera;
            if (speedFollowCamera < 10)
            {
                speedFollowCamera = 10;
                timeMove = dis / speedFollowCamera;

                if (timeMove <= 0f) timeMove = timeMoveCamera;
            }

            LeanTween.cancel(gameObject);
            LeanTween.value(gameObject, (value) => {
                cam.orthographicSize = value;
                //camui.orthographicSize = value;
            }, cam.orthographicSize, config.isZoomIn ? zoomInSize : zoomOutSize, timeMove);

            yield return new WaitUntil(() => Vector3.Distance(tranTarget.position + distance, transform.position) < 0.2f);
            yield return new WaitForSeconds(config.timeDelay);
        }

        tranTarget = mainTarget;
        float dis2 = Vector3.Distance(transform.position, tranTarget.position + distance);
        speedFollowCamera = dis2 / timeMoveCamera;

        timeMove = timeMoveCamera;
        if (speedFollowCamera < 10)
        {
            speedFollowCamera = 10;
            timeMove = dis2 / speedFollowCamera;

            if (timeMove <= 0f) timeMove = timeMoveCamera;
        }

        LeanTween.value(gameObject, (value) => {
            cam.orthographicSize = value;
            //camui.orthographicSize = value;
        }, cam.orthographicSize, normalSize, timeMove);

        yield return new WaitUntil(() => Vector3.Distance(tranTarget.position + distance, transform.position) < 0.2f);
        if (zoomConfigs.Count > 0)
        {
            ieWaitZoomLocation = IEWaitZoomLocation();
            StartCoroutine(ieWaitZoomLocation);
        }
        else
        {
            speedFollowCamera = 0;
            ieWaitZoomLocation = null;
        }
    }
}

public class CamZoomConfig
{
    public Transform trans;
    public bool isZoomIn;
    public float timeDelay;
}
