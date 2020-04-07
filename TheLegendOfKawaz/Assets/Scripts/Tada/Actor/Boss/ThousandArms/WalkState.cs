using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Enemy.Thousand;
using TadaLib;

/// <summary>
/// 千手観音ボスの歩行ステート
/// </summary>

namespace Actor.Enemy.Thousand
{
    partial class ThousandBossController
    {
        // 左右に移動するステート プレイヤーのほうに近づく
        [System.Serializable]
        private class WalkState : StateMachine<ThousandBossController>.StateBase
        {
            [SerializeField]
            private float move_duration_ = 1.0f;

            // ステートの初期化
            public override void OnInit()
            {

            }

            // 開始時に呼ばれる
            public override void OnStart()
            {
                Parent.dir_ = Mathf.Sign(Parent.player_.position.x - Parent.transform.position.x);

            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if (Timer > move_duration_)
                {
                    ChangeState((int)eState.Think);
                    return;
                }

                float dir = (Timer > move_duration_ * 3f / 4f)? -Parent.dir_ : Parent.dir_;
                ActorUtils.ProcSpeed(ref Parent.trb_.Velocity, Accel * new Vector2(dir, 0f), MaxAbsSpeed);
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {
                Parent.trb_.Velocity.x = 0f;
            }
        }
    }
} // namespace Actor.Enemy.Purin