using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;

public class AddressablesUtils : MonoBehaviour
{
    private static AddressablesUtils instance;
    private AddressablesUtils() { }
    public static AddressablesUtils Instance
    {
        get
        {
            instance ??= new GameObject("addressablesUtils").AddComponent<AddressablesUtils>();
            return instance;
        }
    }


    public void GetUpdateSize(Action<long, List<IResourceLocator>> updateSizeAction)
    {
        StartCoroutine(GetSize(updateSizeAction));
    }

    IEnumerator GetSize(Action<long, List<IResourceLocator>> updateSizeAction)
    {
        
        Debug.Log("正在检测更新大小");
        var init = Addressables.InitializeAsync(false); // 初始化
        yield return init;
        var checkForCatalogUpdate = Addressables.CheckForCatalogUpdates(false); // 检测更新
        yield return checkForCatalogUpdate;

        long sizeSum = 0;
        List<IResourceLocator> resources = null;
        if (checkForCatalogUpdate.Status == AsyncOperationStatus.Succeeded)
        {
            List<string> results = checkForCatalogUpdate.Result;
            if (results != null && results.Count > 0)
            {
                //Debug.Log("检测完成，等待更新");
                foreach (var item in results)
                {
                    Debug.Log(item);
                }
                var updateHandle = Addressables.UpdateCatalogs(results, false); // 更新本地的catlog
                yield return updateHandle;
                resources = updateHandle.Result;

                foreach (var item in resources)
                {
                    var keys = new List<object>();
                    keys.AddRange(item.Keys);

                    var size = Addressables.GetDownloadSizeAsync(keys);
                    yield return size;
                    Debug.Log("大小是" + size.Result);
                    sizeSum += size.Result;
                }
                

                Addressables.Release(updateHandle);
            }
            else
            {
                Debug.Log("无需更新");
            }

        }
        else
        {
            Debug.Log("检测失败");
        }

        // 执行回调
        updateSizeAction(sizeSum, resources);
        Addressables.Release(checkForCatalogUpdate);
        Addressables.Release(init);
    }

    public void StartDownLoadUpdate(List<IResourceLocator> resources, Action<MyAddressablesInfoModel> updateInfo)
    {
        StartCoroutine(DownLoadUpdate(resources, updateInfo));
    }

    IEnumerator DownLoadUpdate(List<IResourceLocator> resources, Action<MyAddressablesInfoModel> updateInfo)
    {
        MyAddressablesInfoModel myAddressablesInfoModel = new MyAddressablesInfoModel();
        if (resources == null) yield break;
        foreach (var item in resources)
        {
            var keys = new List<object>();
            keys.AddRange(item.Keys);

            var size = Addressables.GetDownloadSizeAsync(keys);
            yield return size;
            Debug.Log("大小是" + size.Result);

            myAddressablesInfoModel.Size = size.Result;
            if (size.Result > 0)
            {
                var download = Addressables.DownloadDependenciesAsync(keys, Addressables.MergeMode.Union, false);
                float speedCount = 0;
                long lastSpeed = 0, speed = 0;
                Debug.Log("下载");
                while (!download.IsDone)
                {
                    Debug.Log("进度条++++>" + download.GetDownloadStatus().Percent);
                    myAddressablesInfoModel.DownloadProgress = download.GetDownloadStatus().Percent;
                    myAddressablesInfoModel.DownloadedSize = download.GetDownloadStatus().DownloadedBytes;
                    speedCount += Time.deltaTime;

                    if (speedCount > 0.5f)
                    {
                        speed = download.GetDownloadStatus().DownloadedBytes;
                        myAddressablesInfoModel.DownloadSpeed = (long)((speed - lastSpeed) / speedCount);
                        lastSpeed = speed;
                        speedCount = 0;
                    }
                    updateInfo(myAddressablesInfoModel);
                    yield return null;
                }
                yield return download;
                if (download.Status == AsyncOperationStatus.Succeeded)
                {
                    myAddressablesInfoModel.DownloadProgress = 1;
                    updateInfo(myAddressablesInfoModel);
                    Debug.Log("下载完成了！！");
                }
                Addressables.Release(download);
            }

        }

    }
}
