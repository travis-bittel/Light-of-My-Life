using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    #region Singleton Code
    private static TextManager _instance;

    public static TextManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("Attempted to Instantiate multiple Textmanagers in one scene!");
            Destroy(this.gameObject);
            DontDestroyOnLoad(this); // Canvas should persist between levels
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

    [SerializeField]
    private TMP_Text floatingText;
    [SerializeField]
    private TMP_Text fixedText;

    [SerializeField]
    private int paragraphIndex;
    public string[] currentParagraph;

    void Start()
    {
        if (floatingText == null)
        {
            floatingText = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<TMP_Text>();
        }
        if (floatingText == null)
        {
            Debug.LogError("Unable to get TMP_Text component from Player object!");
        }
        if (fixedText == null)
        {
            Debug.LogError("FixedText was null! Remember to assign it in the inspector!");
        }
    }
    // Pass in an empty string to hide the text
    public void DisplayFloatingText(string text)
    {
        floatingText.text = text;
    }
    public void DisplayFixedText(params string[] paragraph)
    {
        currentParagraph = paragraph;
        paragraphIndex = 0;
        if (currentParagraph == null)
        {
            fixedText.text = null;
        } else
        {
            fixedText.text = paragraph[0];
            Player.Instance.canMove = false;
        }
    }

    // Scrolls to the next sentence in the currentParagraph
    // This is triggered using Unity's Fancy New Input System(tm)
    public void NextSentence()
    {
        paragraphIndex++;
        if (paragraphIndex < currentParagraph.Length)
        {
            fixedText.text = currentParagraph[paragraphIndex];
        } else
        {
            currentParagraph = null;
            paragraphIndex = 0;
            fixedText.text = "";
            Player.Instance.canMove = true;
        }
    }
}
