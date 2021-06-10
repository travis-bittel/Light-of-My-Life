using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightOrb : MonoBehaviour
{
    // The ability level this orb gives to the player on pickup.
    // If this is 0, it functions as a normal light orb for use in the first region.
    public int abilityLevelGranted;

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
        if (abilityLevelGranted != 0)
        {
            Player.Instance.abilityLevel = abilityLevelGranted;
        } else
        {
            LightHandler.Instance.IsCarryingLight = true;
        }
        gameObject.SetActive(false);
    }
}
