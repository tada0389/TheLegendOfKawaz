using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// タイムスケールを変更できるクラス
/// </summary>

namespace TadaLib
{
    public class TimerScaleChanger : MonoBehaviour
    {
        // タイムスケールをduration(s)の間scaleに変更する
        public static void Change(float scale, float duration)
        {
            Time.timeScale = scale;

            // 新しいタイムスケールを考慮した継続時間
            float new_duration = duration / scale;
        }
    }
}