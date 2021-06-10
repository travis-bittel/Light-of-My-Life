using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LightOrb : MonoBehaviour
{
    public GameObject uiObject;
    private void Start()
    {
        uiObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            LightHandler.Instance.lightOrbWithinRange = this;
            uiObject.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && LightHandler.Instance.lightOrbWithinRange == this)
        {
            LightHandler.Instance.lightOrbWithinRange = null;
            uiObject.SetActive(false);
        }
    }

    public void Pickup()
    {
        LightHandler.Instance.IsCarryingLight = true;
        gameObject.SetActive(false);
        Destroy(uiObject);
    }
}
