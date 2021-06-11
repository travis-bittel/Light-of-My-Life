using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Lantern : MonoBehaviour
{
    [SerializeField]
    private bool isLit;
    [SerializeField]
    private LinkedObject[] linkedObjects;

    private Light2D light2D;
    private SpriteMask mask;

    [SerializeField]
    private string[] lightingText;

    public void SetLitState(bool isLit)
    {
        this.isLit = isLit;
        if (isLit)
        {
            // Update to Lit Sprite
            Player.Instance.lastSaveLocation = transform.position;
            if (lightingText != null && lightingText.Length != 0)
            {
                TextManager.Instance.DisplayFixedText(lightingText);
            }
        } else
        {
            // Update to Unlit Sprite
        }

        if (light2D == null)
        {
            light2D = GetComponent<Light2D>();
        }
        if (mask == null)
        {
            mask = GetComponentInChildren<SpriteMask>();
        }
        if (mask != null)
        {
            mask.enabled = isLit;
        }
        light2D.enabled = isLit;

        if (linkedObjects != null)
        {
            foreach (LinkedObject obj in linkedObjects)
            {
                switch (obj.behavior)
                {
                    case LanternBehaviour.Enable:
                        obj.gameObject.SetActive(true);
                        break;
                    case LanternBehaviour.Disable:
                        obj.gameObject.SetActive(false);
                        break;
                    case LanternBehaviour.Unlock:
                        Gate gate = obj.gameObject.GetComponent<Gate>();
                        if (gate == null)
                        {
                            Debug.LogError("Lantern attempted to unlock an object without a Gate Component");
                        } else
                        {
                            obj.gameObject.GetComponent<Gate>().AdjustUnlockProgress(1);
                        }
                        break;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player.Instance.LanternWithinRange = this;
            if (!isLit)
            {
                if ((LightHandler.Instance != null && LightHandler.Instance.IsCarryingLight) || Player.Instance.abilityLevel >= 1)
                {
                    TextManager.Instance.DisplayFloatingText("Press <b><i>E</i></b> to light the Lantern");
                }
                else
                {
                    TextManager.Instance.DisplayFloatingText("Your Light is not strong enough to light the Lantern!");
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Player.Instance.LanternWithinRange == this)
        {
            Player.Instance.LanternWithinRange = null;
            TextManager.Instance.DisplayFloatingText("");
        }
    }
}

[System.Serializable]
public struct LinkedObject
{
    public GameObject gameObject;
    //public bool enableObjectWhenLit; // Else disable it
    public LanternBehaviour behavior;
}

public enum LanternBehaviour
{
    Enable,
    Disable,
    Unlock
}
