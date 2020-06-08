using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering.PostProcessing;

public class MiniGamePostProcessing : MonoBehaviour
{
    [SerializeField]
    PostProcessVolume postProcesser;
    // Start is called before the first frame update
    void Start()
    {
        
        //FadeManager.FadeOut(1f, null);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ExplotionLight()
    {
        DOTween.CompleteAll();
        postProcesser.weight = 0.5f;
        // 数値の変更
        DOTween.To(
            () => postProcesser.weight,          // 何を対象にするのか
            num => postProcesser.weight = num,   // 値の更新
            0.1f,                  // 最終的な値
            1f                  // アニメーション時間
        );
    }
}
