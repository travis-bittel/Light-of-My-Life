using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LightOrb : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            LightHandler.Instance.lightOrbWithinRange = this;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && LightHandler.Instance.lightOrbWithinRange == this)
        {
            LightHandler.Instance.lightOrbWithinRange = null;
        }
    }

    public void Pickup()
    {
        LightHandler.Instance.IsCarryingLight = true;
        gameObject.SetActive(false);
    }
}
