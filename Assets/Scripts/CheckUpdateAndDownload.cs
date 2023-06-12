using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.UI;

// 检测更新并下载资源
public class CheckUpdateAndDownload : MonoBehaviour
{
    /// <summary>
    /// 显示下载状态和进度
    /// </summary>
    public Text updateText;

    /// <summary>
    /// 重试按钮
    /// </summary>
    public Button retryBtn;

    void Start()
    {
        retryBtn.gameObject.SetActive(false);
        retryBtn.onClick.AddListener(() =>
        {
            StartCoroutine(DoUpdateAddressadble());
        });

        // 默认自动执行一次更新检测
        StartCoroutine(DoUpdateAddressadble());
    }

    IEnumerator DoUpdateAddressadble()
    {
        AsyncOperationHandle<IResourceLocator> initHandle = Addressables.InitializeAsync(false);
        yield return initHandle;

        // 检测更新
        var checkHandle = Addressables.CheckForCatalogUpdates(false);
        yield return checkHandle;
        if (checkHandle.Status != AsyncOperationStatus.Succeeded)
        {
            OnError("CheckForCatalogUpdates Error\n" + checkHandle.OperationException.ToString());
            yield break;
        }

        if (checkHandle.Result.Count > 0)
        {
            var updateHandle = Addressables.UpdateCatalogs(checkHandle.Result, false);
            yield return updateHandle;

            if (updateHandle.Status != AsyncOperationStatus.Succeeded)
            {
                OnError("UpdateCatalogs Error\n" + updateHandle.OperationException.ToString());
                yield break;
            }

            // 更新列表迭代器
            List<IResourceLocator> locators = updateHandle.Result;
            foreach (var locator in locators)
            {
                List<object> keys = new List<object>();
                keys.AddRange(locator.Keys);
                // 获取待下载的文件总大小
                var sizeHandle = Addressables.GetDownloadSizeAsync(keys);
                yield return sizeHandle;
                if (sizeHandle.Status != AsyncOperationStatus.Succeeded)
                {
                    OnError("GetDownloadSizeAsync Error\n" + sizeHandle.OperationException.ToString());
                    yield break;
                }

                long totalDownloadSize = sizeHandle.Result;
                updateText.text = updateText.text + "\ndownload size : " + totalDownloadSize;
                Debug.Log("download size : " + totalDownloadSize);
                if (totalDownloadSize > 0)
                {
                    // 下载
                    var downloadHandle = Addressables.DownloadDependenciesAsync(keys, Addressables.MergeMode.Union, false);
                    while (!downloadHandle.IsDone)
                    {
                        if (downloadHandle.Status == AsyncOperationStatus.Failed)
                        {
                            OnError("DownloadDependenciesAsync Error\n" + downloadHandle.OperationException.ToString());
                            yield break;
                        }
                        // 下载进度
                        float percentage = downloadHandle.PercentComplete;
                        Debug.Log($"已下载: {percentage}");
                        updateText.text = updateText.text + $"\n已下载: {percentage}";
                        yield return null;
                    }
                    if (downloadHandle.Status == AsyncOperationStatus.Succeeded)
                    {
                        Debug.Log("下载完毕!");
                        updateText.text = updateText.text + "\n下载完毕";

                        
                    }

                    Addressables.Release(downloadHandle);
                }
            }

            Addressables.Release(updateHandle);


        }
        else
        {
            updateText.text = updateText.text + "\n没有检测到更新";
        }
        Addressables.Release(checkHandle);
        Addressables.Release(initHandle);
        // 进入游戏
        EnterGame();
    }

    // 异常提示
    private void OnError(string msg)
    {
        updateText.text = updateText.text + $"\n{msg}\n请重试! ";
        // 显示重试按钮
        retryBtn.gameObject.SetActive(true);
    }


    // 进入游戏
    void EnterGame()
    {
        // TODO

        Addressables.InstantiateAsync("Map");
    }
}
