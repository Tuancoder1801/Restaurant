using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubLocationMachine : MonoBehaviour
{
    public LocationMachine machine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(StaticValue.CHARACTER_NAME_TAG))
        {
            Debug.Log("OnTriggerEnter: GameConfig.TAG_PLAYER");
            machine.AddChef(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals(StaticValue.CHARACTER_NAME_TAG))
        {
            Debug.Log("OnTriggerExit: GameConfig.TAG_PLAYER");
            machine.RemoveChef(other.gameObject);
        }
    }
}
