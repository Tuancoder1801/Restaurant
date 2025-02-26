using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationPlayer : LocationBase
{
    public AICharacter playerAi;

    private void OnEnable()
    {
        if (playerAi != null)
        {
            playerAi.SetIdleTransform(transform);
            playerAi.gameObject.SetActive(true);
        }
    }
}
