﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Enemy.Purin;
using TadaLib;

/// <summary>
/// プリンボスの死亡時のステート
/// </summary>

namespace Actor.Enemy.Purin
{
    partial class PurinBossController
    {
        // 死亡したときの状態
        [System.Serializable]
        private class DeadState : StateMachine<PurinBossController>.StateBase
        {
            // 死亡時の爆発のエフェクト
            [SerializeField]
            private ParticleSystem explosion_effect_;

            private bool get = false;

            // ステートの初期化
            public override void OnInit()
            {
                
            }

            // 開始時に呼ばれる
            public override void OnStart()
            {
                Time.timeScale = 0.3f;
                explosion_effect_.gameObject.SetActive(true);
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if (Timer > 2.0f && !get)
                {
                    get = true;
                    Time.timeScale = 1.0f;
                    Actor.Player.SkillManager.Instance.GainSkillPoint(1000, Parent.transform.position);
                }

                if(Timer > 7.0f)
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene("ZakkyScene");
                }
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }
        }
    }
} // namespace Actor.Enemy.Purin