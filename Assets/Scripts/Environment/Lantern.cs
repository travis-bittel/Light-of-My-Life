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
                obj.gameObject.SetActive(obj.enableObjectWhenLit == isLit); // I wrote out a truth table to figure out this condition lmao
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
    public bool enableObjectWhenLit; // Else disable it
}
