using System;
using UnityEngine;
using UnityEngine.UI;

public class DialogUI : MonoBehaviour
{
    public Text info;
    public Action onOkClick;
    public void SetActive(bool active = true)
    {
        this.gameObject.SetActive(active);
    }
    public void SetText(string text)
    {
        info.text = text;
    }
    public void OnOkButton()
    {
        onOkClick();
        
        Destroy(this.gameObject);
    }

    public void OnNoButton()
    {

    }
}
