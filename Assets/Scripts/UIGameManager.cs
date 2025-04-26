using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIGameManager : MonoBehaviour
{
    public static UIGameManager Instance;

    public Canvas canvas;
    public UIGame UIGame;
    public Joystick joystick;

    private bool isInit = false;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {   
        if(isInit) return;

        isInit = true;

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
