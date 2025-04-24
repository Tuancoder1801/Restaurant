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
