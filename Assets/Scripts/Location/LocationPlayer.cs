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

            if (playerAi.humanId != HumanId.Player) GameManager.Instance.smoothCamera.ZoomInToTarget(new List<Transform> { playerAi.transform });

            if (playerAi.humanId == HumanId.Collector)
            {
                AICollector collector = playerAi as AICollector;
                collector.EquipSkinCollector((SkinRobotId)UserData.skin.GetEquippedSkin(SkinType.Robot));
            }
        }
    }
}
