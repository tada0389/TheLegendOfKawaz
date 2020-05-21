using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// カメラを揺らすためのクラス
/// メインカメラが MultipleTargetCamera をアタッチされている場合に実行できる
/// </summary>

namespace CameraSpace
{
    public class CameraShaker : MonoBehaviour
    {
        /// <summary>
        /// カメラを揺らす関数
        /// メインカメラが MultipleTargetCamera をアタッチされている場合に実行できる
        /// </summary>
        /// <param name="power">揺らす力の強さ</param>
        /// <param name="duration">揺らす時間の長さ</param>
        /// <param name="shake_interval">揺れの細かさ</param>
        public static void Shake(float power = 1.0f, float duration = 1.0f, float shake_interval = 0.06f)
        {
            Camera camera = Camera.main;
            if (camera == null) return;

            var camera_ctrl = camera.GetComponent<MultipleTargetCamera>();
            if (camera_ctrl == null) return;

            camera_ctrl.Shake(power, duration, shake_interval);
        }
    }
}