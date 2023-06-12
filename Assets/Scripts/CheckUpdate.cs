using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CheckUpdate : MonoBehaviour
{
    
    void Start()
    {
        StartCoroutine(CheckUpdateForCatLog());
    }
    List<object> keys;
    IEnumerator CheckUpdateForCatLog()
    {
        var init = Addressables.InitializeAsync(false); // 初始化
        yield return init;

        var checkForCatalogUpdate =  Addressables.CheckForCatalogUpdates(false); // 检测更新
        yield return checkForCatalogUpdate;

        if (checkForCatalogUpdate.Status == AsyncOperationStatus.Succeeded)
        {
            List<string> results = checkForCatalogUpdate.Result;
            if (results != null && results.Count > 0)
            {
                Debug.Log("检测完成，等待更新");
                var updateHandle = Addressables.UpdateCatalogs(results, false); // 更新本地的catlog
                yield return updateHandle;
                List<IResourceLocator> resources = updateHandle.Result;

                foreach (var item in resources)
                {
                    keys = new List<object>();
                    keys.AddRange(item.Keys);
                    
                    var size = Addressables.GetDownloadSizeAsync(keys);
                    Debug.Log("大小是" + size.Result);

                    if (size.Result > 0)
                    {
                        var download = Addressables.DownloadDependenciesAsync(keys, Addressables.MergeMode.Union, false);
                        Debug.Log("下载");
                        while (!download.IsDone)
                        {
                            Debug.Log("进度条++++>" + download.PercentComplete);
                            yield return null;
                        }
                        yield return download;
                        if (download.Status == AsyncOperationStatus.Succeeded)
                        {
                            Debug.Log("下载完成了！！");
                        }
                        Addressables.Release(download);
                    }

                }

                Addressables.Release(updateHandle);
            }
            else
            {
                Debug.Log("不能更新");
            }
            
        }
        else
        {
            Debug.Log("检测失败");
        }

        Addressables.Release(checkForCatalogUpdate);
        Addressables.Release(init);
    }
}
