using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateInfoAndProgressUI : MonoBehaviour
{
    public Text updateInfo;
    public Text updateSpeedText;
    public Slider updateSlider;
    public GameObject loading;


    private void Start()
    {
        loading.transform.DOScale(Vector3.one * 1.1f, 1f).SetLoops(-1, LoopType.Yoyo);
    }
    public void SetUpdateInfo(string text)
    {
        updateInfo.text = text;
    }

    /// <summary>
    /// 更新显示的速度
    /// </summary>
    /// <param name="speed"></param>
    /// <param name="downSize">已经下载的大小</param>
    /// <param name="size">最大下载的大小</param>
    public void SetUpdateSpeedText(string speed, string downSize, string size)
    {
        updateSpeedText.text = $"{speed}/S {downSize}/{size}";
    }

    public void SetUpdateSpeedText(long speed, long downSize, long size)
    {
        updateSpeedText.text = $"{DataUtils.GetDataSize(speed)}/S {DataUtils.GetDataSize(downSize)}/{DataUtils.GetDataSize(size)}";
    }


    public void SetSlider(float progress)
    {
        updateSlider.value = progress;
    }
}
