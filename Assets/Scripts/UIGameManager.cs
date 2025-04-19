using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameManager : MonoBehaviour
{
    public static UIGameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
