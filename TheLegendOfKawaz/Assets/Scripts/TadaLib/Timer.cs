using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 時間を計測するクラス
/// </summary>

namespace TadaLib
{
    public class Timer
    {
        private float start_time_;
        private float limit_time_; // 制限時間

        public Timer(float limit_time)
        {
            limit_time_ = limit_time;
            start_time_ = Time.time;
        }

        public void TimeReset()
        {
            start_time_ = Time.time;
        }

        public bool IsTimeout()
        {
            return Time.time - start_time_ >= limit_time_;
        }

        // 時間を巻き戻す
        public void TimeReverse(float time)
        {
            start_time_ = Mathf.Min(start_time_ + time, Time.time);
        }

        public float GetTime()
        {
            return Time.time - start_time_;
        }
    }
}