using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Enemy.Purin;
using TadaLib;

/// <summary>
/// プリンボスの空中移動のステート
/// </summary>

namespace Actor.Enemy.Purin
{
    partial class PurinBossController
    {
        // 空中移動のステート
        [System.Serializable]
        private class WalkState : StateMachine<PurinBossController>.StateBase
        {
            [SerializeField]
            private Vector2 speed_ = new Vector2(0.16f, 0.16f);

            [SerializeField]
            private float period_ = Mathf.PI;

            [SerializeField]
            private float move_duration_ = 2.0f;

            private float dir_;

            // ステートの初期化
            public override void OnInit()
            {

            }

            // 開始時に呼ばれる
            public override void OnStart()
            {
                // 敵の方向へ行く
                dir_ = Mathf.Sign(Parent.player_.position.x - Parent.transform.position.x);
                Parent.SetDirection((dir_ < 0f) ? eDir.Left : eDir.Right);

                Parent.trb_.Velocity = new Vector2(0f, 0f);
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                // 移動時間を超えた，壁に衝突したなら終了
                if(Timer > move_duration_ || Parent.trb_.LeftCollide || Parent.trb_.RightCollide || Parent.trb_.TopCollide)
                {
                    ChangeState((int)eState.Think);
                    return;
                }

                Parent.trb_.Velocity.y = Mathf.Sin(Mathf.PI * (Timer / period_)) * speed_.y;

                ActorUtils.ProcSpeed(ref Parent.trb_.Velocity, new Vector2(dir_, 0f) * Accel, MaxAbsSpeed);
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {
                Parent.trb_.Velocity = Vector2.zero;
            }
        }
    }
} // namespace Actor.Enemy.Purin