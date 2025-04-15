using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubLocationMachine : MonoBehaviour
{
    public LocationMachine machine;

    public GameObject fxIndexCook;

    private void OnEnable()
    {
        fxIndexCook.gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(StaticValue.CHARACTER_NAME_TAG))
        {
            Debug.Log("OnTriggerEnter: GameConfig.TAG_PLAYER");
            machine.AddChef(other.gameObject);
            fxIndexCook.gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals(StaticValue.CHARACTER_NAME_TAG))
        {
            Debug.Log("OnTriggerExit: GameConfig.TAG_PLAYER");
            machine.RemoveChef(other.gameObject);
            fxIndexCook.gameObject.SetActive(true);
        }
    }
}
