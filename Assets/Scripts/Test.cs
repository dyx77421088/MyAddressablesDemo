using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public Text text1;
    public Text text2;
    void Start()
    {
        StartCoroutine(Load("Map"));
        //StartCoroutine(Load("Player"));
    }

    IEnumerator Load(string name)
    {
        AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(name);


        while (!handle.IsDone)
        {
            Debug.Log(handle.PercentComplete);
            if (name == "Map") text1.text = name + "当前进度:" + handle.PercentComplete;
            else text2.text = name + "当前进度:" + handle.PercentComplete;
            yield return null;
        }
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            if (name == "Map") text1.text = name + "当前进度:" + handle.PercentComplete;
            else text2.text = name + "当前进度:" + handle.PercentComplete;
        }
    }
}
