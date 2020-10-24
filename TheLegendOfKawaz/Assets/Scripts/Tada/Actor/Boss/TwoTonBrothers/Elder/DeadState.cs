using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TadaLib;

/// <summary>
/// ツートンボス(兄)の死亡時のステート
/// </summary>

namespace Actor.Enemy.TwoTon.Elder
{
    partial class TwoTonElderBossController
    {
        // 次のアクションを決めるときのステート
        [System.Serializable]
        private class DeadState : StateMachine<TwoTonElderBossController>.StateBase
        {

            // 死亡時の爆発のエフェクト
            [SerializeField]
            private ParticleSystem explosion_effect_;

            // ステートの初期化
            public override void OnInit()
            {

            }

            // 開始時に呼ばれる
            public override void OnStart()
            {
                //Parent.animator_.Play("Idle");
                explosion_effect_.gameObject.SetActive(true);

                Parent.trb_.Velocity = Vector3.zero;
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }
        }
    }
} // namespace Actor.Enemy.TwoTon.Elder