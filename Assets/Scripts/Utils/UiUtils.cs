using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiUtils : MonoBehaviour
{
    private static UiUtils instance;
    private GameObject canvas;
    public static UiUtils Instance
    {
        get
        {
            return instance ??= new GameObject("UiManager").AddComponent<UiUtils>();
        }
    }

    public GameObject Canvas
    {
        get { return canvas ??= GameObject.Find("Canvas"); }
    }
}
