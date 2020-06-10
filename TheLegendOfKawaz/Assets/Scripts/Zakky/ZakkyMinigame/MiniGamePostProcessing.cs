using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering.PostProcessing;

public class MiniGamePostProcessing : MonoBehaviour
{
    [SerializeField]
    PostProcessVolume postProcesser;

    private DG.Tweening.Core.TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> tween_;

    public void ExplotionLight()
    {
        tween_.Kill();
        postProcesser.weight = 0.5f;
        // 数値の変更
        tween_ = DOTween.To(
            () => postProcesser.weight,          // 何を対象にするのか
            num => postProcesser.weight = num,   // 値の更新
            0.1f,                  // 最終的な値
            1f                  // アニメーション時間
        );
    }
}
