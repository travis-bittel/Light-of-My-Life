using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LightOrb : MonoBehaviour
{
    // The ability level this orb gives to the player on pickup.
    // If this is 0, it functions as a normal light orb for use in the first region.
    public int abilityLevelGranted;

    public GameObject uiObject;
    private void Start()
    {
        if (uiObject == null)
        {
            uiObject = GameObject.Find("Text/Canvas/LightText");
        }
        uiObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (LightHandler.Instance != null)
            {
                LightHandler.Instance.lightOrbWithinRange = this;
            } else
            {
                Player.Instance.lightOrbWithinRange = this;
            }
            uiObject.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Player.Instance.lightOrbWithinRange == this)
        {
            if (LightHandler.Instance != null)
            {
                LightHandler.Instance.lightOrbWithinRange = null;
            }
            Player.Instance.lightOrbWithinRange = null;
            uiObject.SetActive(false);
        }
    }

    public void Pickup()
    {
        if (abilityLevelGranted != 0)
        {
            Player.Instance.abilityLevel = abilityLevelGranted;
        } else
        {
            LightHandler.Instance.IsCarryingLight = true;
        }
        gameObject.SetActive(false);
        uiObject.SetActive(false);
    }
}
