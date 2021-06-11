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

    [SerializeField]
    private Color color;

    [SerializeField]
    private bool isEndgame;

    public void SetLitState(bool isLit)
    {
        if (isEndgame)
        {
            StartCoroutine(Endgame());
        }
        this.isLit = isLit;
        if (isLit)
        {
            // Update to Lit Sprite
            Player.Instance.lastSaveLocation = transform.position;
            if (lightingText != null && lightingText.Length != 0)
            {
                TextManager.Instance.DisplayFixedText(color, lightingText);
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

    private IEnumerator Endgame()
    {
        BlackScreenHandler.Instance.FadeIn(1, 2);
        yield return new WaitForSeconds(2);
        TextManager.Instance.DisplayFixedText(color, "Welcome home, Lumo.<br><b>THE END<b>");
        TextManager.Instance.fixedText.alignment = TMPro.TextAlignmentOptions.Center;
        TextManager.Instance.fixedText.fontSize = 76;
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
                    TextManager.Instance.DisplayFloatingText("Press <b><i>E</i></b> to light the Lantern", Color.white);
                }
                else
                {
                    TextManager.Instance.DisplayFloatingText("Your Light is not strong enough to light the Lantern!", Color.white);
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Player.Instance.LanternWithinRange == this)
        {
            Player.Instance.LanternWithinRange = null;
            TextManager.Instance.DisplayFloatingText("", Color.white);
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
