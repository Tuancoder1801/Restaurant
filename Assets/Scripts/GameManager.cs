using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Player player;
    public AICustomer customer;
    public SmoothCamera smoothCamera;

    public List<Transform> spawnPosList;
    public List<Transform> startPosList;

    public Transform targetTransform;
    public Transform sitTransform;
    public Transform departurePos;

    private void Awake()
    {
        GameDataConstant.Load();

        smoothCamera.SetTarget(player.transform);

        CreateCustomer();
    }

    private void CreateCustomer()
    {

        Transform randomSpawnPoint = spawnPosList[Random.Range(0, spawnPosList.Count)];
        AICustomer cus = Instantiate(customer, randomSpawnPoint.position, randomSpawnPoint.rotation);
        cus.ChangePos(startPosList[0]);
        //cus.targetPos = targetTransform;
        cus.sittingPos = sitTransform;
        cus.departurePos = departurePos;
    }
}
