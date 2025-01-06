using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Player player;
    public SmoothCamera smoothCamera;

    private void Awake()
    {
        GameDataConstant.Load();

        smoothCamera.SetTarget(player.transform);
    }
}
