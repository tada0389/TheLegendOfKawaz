using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Time.timeScaleを変更させるクラス
/// 複数の要求に対応できる
/// 現在の要求のうち，最もタイムスケールが小さいものを優先する
/// 速度を加速させたい場合はどうしよう
/// </summary>

namespace TadaLib
{
    public class TimeScaler : TadaLib.SingletonMonoBehaviour<TimeScaler>
    {
        private TadaLib.MultiSet<float> values_;

        protected override void Awake()
        {
            base.Awake();
            values_ = new MultiSet<float>();

            // シーンが切り替わったらタイムスケールを1にする
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += ActiveSceneChanged;
        }

        private void LateUpdate()
        {
            if (values_.Count() == 0) Time.timeScale = 1.0f;
            else Time.timeScale = values_.ElementAt(0);
            Time.fixedDeltaTime = 0.01f * Time.timeScale;
        }

        /// <summary>
        /// タイムスケールの変更を申請する
        /// 期間を指定してその間だけ変更する (かもしれない)
        /// </summary>
        /// <param name="time_scale">タイムスケール</param>
        /// <param name="duration">長さ</param>
        public void RequestChange(float time_scale, float duration)
        {
            StartCoroutine(DOChange(time_scale, duration));
        }

        /// <summary>
        /// タイムスケールの変更を申請する
        /// ただし，期間は指定せず，DismissRequestが呼ばれるまで続く
        /// </summary>
        /// <param name="time_scale">タイムスケール</param>
        public void RequestChange(float time_scale)
        {
            values_.Insert(time_scale);
        }

        /// <summary>
        /// 申請したタイムスケールを破棄する
        /// </summary>
        /// <param name="time_scale">前回に登録したタイムスケール</param>
        public void DismissRequest(float time_scale)
        {
            values_.Remove(time_scale);
        }

        private IEnumerator DOChange(float time_scale, float duration)
        {
            values_.Insert(time_scale);
            yield return new WaitForSeconds(duration);
            values_.Remove(time_scale);
        }

        // シーンが切り替わったら今までの申請をなくす
        private void ActiveSceneChanged(Scene cur_scene, Scene next_scene)
        {
            values_.Clear();
        }
    }
}