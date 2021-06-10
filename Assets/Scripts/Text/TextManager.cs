using System.Collections;
using System.Collections.Generic;
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

    //public GameObject uiObject;
    public Text textObject;
    public Button button;
    private int sentenceNum;
    // Update is called once per frame
    void Start()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(NextSentence);
        }
    }
    //thinking that uiObject could be a sentence within the collection
    public void DisplayFloatingText(string text)
    {
        textObject.text = text;
        textObject.enabled = true;
    }
    public void HideText()
    {
        textObject.enabled = false;
    }
    public void DisplayFixedText(string[] sentences)
    {
        while (sentenceNum < sentences.Length)
        {
            textObject.text = sentences[sentenceNum];
            textObject.enabled = true;
        }
    }
    //scrolls to the next sentence in sentences array
    private void NextSentence()
    {
        //hides current text
        HideText();
        //increments to next sentence
        sentenceNum++;
    }
}
