using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LightOrb : MonoBehaviour
{
    // The ability level this orb gives to the player on pickup.
    // If this is 0, it functions as a normal light orb for use in the first region.
    public int abilityLevelGranted;

    [SerializeField]
    private string[] pickupText;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (LightHandler.Instance != null)
            {
                LightHandler.Instance.lightOrbWithinRange = this;
            }
            Player.Instance.lightOrbWithinRange = this;
            TextManager.Instance.DisplayFloatingText("Press <b><i>E</i></b> to collect the Light Orb");
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
            TextManager.Instance.DisplayFloatingText("");
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
        TextManager.Instance.DisplayFixedText(pickupText);
        gameObject.SetActive(false);
    }
}
