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

    public GameObject uiObject;

    private void Start()
    {
        if (uiObject == null)
        {
            uiObject = GameObject.Find("Text/Canvas/LanternText");
        }
        uiObject.SetActive(false);
    }
    public void SetLitState(bool isLit)
    {
        this.isLit = isLit;
        if (isLit)
        {
            // Update to Lit Sprite
            Player.Instance.lastSaveLocation = transform.position;
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
            uiObject.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Player.Instance.LanternWithinRange == this)
        {
            Player.Instance.LanternWithinRange = null;
            uiObject.SetActive(false);
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
