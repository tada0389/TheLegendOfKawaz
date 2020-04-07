﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Enemy.Thousand;
using TadaLib;
using DG.Tweening;

/// <summary>
/// 千手観音ボスの死亡時のステート
/// </summary>

namespace Actor.Enemy.Thousand
{
    partial class ThousandBossController
    {
        // 最初の会話
        [System.Serializable]
        private class DeadState : StateMachine<ThousandBossController>.StateBase
        {
            // 死亡時の爆発のエフェクト
            [SerializeField]
            private ParticleSystem explosion_effect_;

            private bool a = false;

            // 開始時に呼ばれる
            public override void OnStart()
            {
                Parent.trb_.Velocity = Vector2.zero;
                explosion_effect_.gameObject.SetActive(true);
                if (!a) Time.timeScale = 0.3f;
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if (Timer > 2.0f && !a)
                {
                    Time.timeScale = 1.0f;
                    a = true;
                    Actor.Player.SkillManager.Instance.GainSkillPoint(1000, Parent.transform.position);
                }
                if (Timer > 6.0f)
                {
                    //UnityEngine.SceneManagement.SceneManager.LoadScene("ZakkyScene");
                }
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }
        }
    }
} // namespace Actor.Enemy.Thousand