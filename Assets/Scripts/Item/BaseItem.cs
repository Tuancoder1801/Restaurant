using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BaseItem : MonoBehaviour
{
    public ItemId itemId;
    public float height;

    private void Start()
    {
        float height = gameObject.GetComponentInChildren<MeshRenderer>().bounds.size.y;
        Debug.Log("Chiều cao của vật thể là: " + height);
    }
}