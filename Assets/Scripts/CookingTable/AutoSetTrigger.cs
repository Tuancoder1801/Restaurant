using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSetTrigger : MonoBehaviour
{
    List<GameObject> goEnters = new List<GameObject>();

    void OnEnable()
    {
        GetComponent<Collider>().isTrigger = true;
        StartCoroutine(IEWaitEnable());
    }

    void OnTriggerEnter(Collider other)
    {
        if (goEnters == null) return;
        if (other.GetComponent<Character>() != null)
        {
            goEnters.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (goEnters == null) return;
        if (goEnters.Contains(other.gameObject))
        {
            goEnters.Remove(other.gameObject);
            CheckSetTrigger();
        }
    }

    public IEnumerator IEWaitEnable()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(1f);
        CheckSetTrigger();
    }

    private void CheckSetTrigger()
    {
        if (goEnters == null || goEnters.Count <= 0)
        {
            GetComponent<Collider>().isTrigger = false;
            goEnters = null;
        }
    }
}
