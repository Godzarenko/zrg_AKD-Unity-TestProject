using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_UsePrompt : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void StaticReinit()
    {
        //domain reset
    }

    public static UI_UsePrompt Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<UI_UsePrompt>(FindObjectsInactive.Include);
            }
            return _instance;
        }
    }
    static UI_UsePrompt _instance;

    public TMPro.TextMeshProUGUI useText;
    public UnityEngine.UI.Image useImage;

    public void ShowPrompt(string text, Sprite icon)
    {
        gameObject.SetActive(true);
        useText.text = text;
        useImage.sprite = icon;
    }
    public void HidePrompt()
    {
        gameObject.SetActive(false);
    }
}
