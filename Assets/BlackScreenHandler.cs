using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackScreenHandler : MonoBehaviour
{
    #region Singleton Code
    private static BlackScreenHandler _instance;

    public static BlackScreenHandler Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("Attempted to Instantiate multiple BlackScreenHandler in one scene!");
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    [SerializeField]
    private CanvasGroup group;

    private void Start()
    {
        FadeIn(0, 0.5f);
    }

    public void FadeIn(float targetOpacity, float fadeRate)
    {
        StartCoroutine(IFadeIn(targetOpacity, fadeRate));
    }

    private IEnumerator IFadeIn(float targetOpacity, float fadeRate)
    {
        while (Mathf.Abs(group.alpha - targetOpacity) > 0.0001f)
        {
            group.alpha = Mathf.Lerp(group.alpha, targetOpacity, fadeRate * Time.deltaTime);
            yield return null;
        }
    }
}