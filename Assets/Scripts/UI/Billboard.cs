using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private BillboardType billboardType;

    public enum BillboardType { LookAtCamera, CameraForwad};

    private void LateUpdate()
    {
        switch (billboardType)
        {
            case BillboardType.LookAtCamera:
                Vector3 direction = Camera.main.transform.position - transform.position;
                transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180, 0);
                break;
            case BillboardType.CameraForwad:
                transform.forward = -Camera.main.transform.forward;
                break;
        }

    }
}
