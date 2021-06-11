using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used to display text when inside of the box (for tutorials and such)
public class TutorialBox : MonoBehaviour
{
    [SerializeField]
    private string text;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TextManager.Instance.DisplayFloatingText(text);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TextManager.Instance.DisplayFloatingText("");
        }
    }
}
