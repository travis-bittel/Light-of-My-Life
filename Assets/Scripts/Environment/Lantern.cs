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

    public void SetLitState(bool isLit)
    {
        this.isLit = isLit;
        if (isLit)
        {
            // Update to Lit Sprite

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
        mask.enabled = isLit;
        light2D.enabled = isLit;

        if (linkedObjects == null)
        {
            Debug.LogWarning("Lantern had no linked objects. Make sure to assign a linked object in the inspector.");
        } else
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
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Player.Instance.LanternWithinRange == this)
        {
            Player.Instance.LanternWithinRange = null;
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
