using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PopupMart : BasePopup
{
    public Transform content;
    public UIMartItem martItem;

    private void OnEnable()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        List<MapData> maps = GameDataConstant.maps;
        int currentMap = UserData.map.currentMapIndex;

        for (int i = 0; i < maps.Count; i++)
        {
            MapData map = maps[i];
            UIMartItem item = Instantiate(martItem, content);
            item.Load(map);

            bool isUnlocked = UserData.map.unlockedMapIndexes.Contains(i);
            bool isCompleted = UserData.map.completedMapIndexes.Contains(i);
            bool isCurrent = (i == currentMap);

            if (!isUnlocked)
            {
                item.txtlock.gameObject.SetActive(true);
                item.txtLocation.gameObject.SetActive(false);
                item.btnGoTo.gameObject.SetActive(false);
            }

            if (isCurrent)
            {
                item.txtLocation.gameObject.SetActive(true);
                item.txtlock.gameObject.SetActive(false);
                item.btnGoTo.gameObject.SetActive(false);
            }
            else
            {
                if (isCompleted || isUnlocked)
                {
                    item.btnGoTo.gameObject.SetActive(true);
                    item.txtLocation.gameObject.SetActive(false);
                    item.txtlock.gameObject.SetActive(false);

                    int capturedIndex = i;
                    item.btnGoTo.onClick.AddListener(() =>
                    {
                        UserData.map.currentMapIndex = capturedIndex;
                        UserData.Save();

                        UIGameManager.Instance.gameObject.SetActive(false);
                        SceneManager.LoadScene("Start");
                    });
                }
            }
        }
    }
}
