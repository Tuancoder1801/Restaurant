using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCamera : MonoBehaviour
{
    public Vector3 offset = new Vector3(0f, 7f, -6f);

    [SerializeField] private Transform target;
    private float smoothTime;
    private Vector3 currentVelocity = Vector3.zero;

    private void Awake()
    {
        //offset = transform.position - tagert.position;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
    }
}
