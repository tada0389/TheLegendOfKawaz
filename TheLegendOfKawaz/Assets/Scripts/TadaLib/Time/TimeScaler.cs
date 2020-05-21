using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Time.timeScaleを変更させるクラス
/// 複数の要求に対応できる
/// </summary>

namespace TadaLib
{
    public class TimeScaler : TadaLib.SingletonMonoBehaviour<TimeScaler>
    {
        private TadaLib.MultiSet<float> values_;

        private void Awake()
        {
            base.Awake();
            values_ = new MultiSet<float>();
            values_.Insert(1.0f);
        }

        private void LateUpdate()
        {
            Time.timeScale = values_.ElementAt(0);
        }

        /// <summary>
        /// タイムスケールの変更を申請する
        /// </summary>
        /// <param name="time_scale">タイムスケール</param>
        /// <param name="duration">長さ</param>
        public void RequestChange(float time_scale, float duration)
        {
            StartCoroutine(DOChange(time_scale, duration));
        }

        private IEnumerator DOChange(float time_scale, float duration)
        {
            values_.Insert(time_scale);
            yield return new WaitForSeconds(duration);
            values_.Remove(time_scale);
        }
    }
}