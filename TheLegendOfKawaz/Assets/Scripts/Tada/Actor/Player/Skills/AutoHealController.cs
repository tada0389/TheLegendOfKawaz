using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TadaLib;

/// <summary>
/// 自動回復のエフェクト等に関するスクリプト
/// </summary>

namespace Actor.Player
{
    public class AutoHealController : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem always_eff_;
        [SerializeField]
        private ParticleSystem once_eff_;

        private Timer timer_;

        // エフェクトを出現させる
        public void Init(float heal_interval)
        {
            always_eff_.gameObject.SetActive(true);
            once_eff_.gameObject.SetActive(true);

            timer_ = new Timer(heal_interval);
        }

        // 回復の一時的なエフェクトを出す
        public void PlayHealEffect()
        {
            once_eff_.Play();
            timer_.TimeReset();
        }

        // 回復できるか
        public bool CanHeal()
        {
            return timer_.IsTimeout();
        }
    }
}