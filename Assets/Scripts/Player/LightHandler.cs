using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

// Handles light mechanics in the first region. Should not be attached to the Player object afterwards.
public class LightHandler : MonoBehaviour
{
    #region Singleton Code
    private static LightHandler _instance;

    public static LightHandler Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("Attempted to Instantiate multiple LightHandlers in one scene!");
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void OnDestroy()
    {
        if (this == _instance) { _instance = null; }
    }
    #endregion

    public Lantern lanternWithinRange; // The lantern the player is currently able to interact with

    public LightOrb lightOrbWithinRange;

    private bool isCarryingLight;
    public bool IsCarryingLight
    {
        get { return isCarryingLight; }
        set
        {
            if (light2D == null)
            {
                light2D = GetComponentInChildren<Light2D>();
            }
            if (mask == null)
            {
                mask = GetComponentInChildren<SpriteMask>();
            }

            isCarryingLight = value;
            /*if (isCarryingLight)
            {
                light2D.intensity = 2;
                mask.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            } else
            {
                light2D.intensity = 1;
                mask.transform.localScale = new Vector3(1f, 1f, 1f);
            }*/
        }
    }

    private Light2D light2D;
    private SpriteMask mask;

    public void OnInterract()
    {
        if (lanternWithinRange != null)
        {
            if (isCarryingLight)
            {
                lanternWithinRange.SetLitState(true);
                IsCarryingLight = false; // Player can only light one lantern between light pickups until they get stronger light
                TextManager.Instance.DisplayFloatingText("", Color.white);
            }
        }
        if (lightOrbWithinRange != null)
        {
            if (isCarryingLight)
            {
                // You are already carrying light!
            } else
            {
                lightOrbWithinRange.Pickup();
                TextManager.Instance.DisplayFloatingText("", Color.white);
            }
        }
    }
}
