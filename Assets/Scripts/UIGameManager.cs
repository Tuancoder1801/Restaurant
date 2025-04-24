using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameManager : MonoBehaviour
{
    public static UIGameManager Instance;

    public Canvas canvas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Camera cam = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
            if (cam != null)
            {
                Camera uiCamera = cam;

                if (canvas != null)
                {
                    canvas.worldCamera = uiCamera;
                    
                }   
            }         
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
