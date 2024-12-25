using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Player player;
    public SmoothCamera smoothCamera;

    private void Awake()
    {
        smoothCamera.SetTarget(player.transform);
    }
}
