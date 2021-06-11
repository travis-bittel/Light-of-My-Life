using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used to display text when inside of the box (for tutorials and such)
public class TutorialBox : MonoBehaviour
{
    [SerializeField]
    private string text;

    [SerializeField]
    private bool isFixed;

    [SerializeField]
    private string[] fixedTextArray;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (isFixed)
            {
                TextManager.Instance.DisplayFixedText(Color.white, fixedTextArray);
            } else
            {
                TextManager.Instance.DisplayFloatingText(text, Color.white);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TextManager.Instance.DisplayFloatingText("", Color.white);
        }
    }
}
