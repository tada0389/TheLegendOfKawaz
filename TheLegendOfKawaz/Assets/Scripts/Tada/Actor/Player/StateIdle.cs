﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Player;
using TadaLib;

/// <summary>
/// プレイヤーのアイドリング状態を管理するステート
/// </summary>

namespace Actor.Player
{
    public partial class Player
    {
        [System.Serializable]
        private class StateIdle : StateMachine<Player>.StateBase
        {
            // ステート間で共有するデータのコピーインスタンス
            private Data data = null;

            // ステートが始まった時に呼ばれるメソッド
            public override void OnStart()
            {
                if(data == null) data = Parent.data_;

                // 待機アニメーション開始
                //Parent.PlayAnim("Idle");

                // 速度をゼロにする
                //data.velocity = Vector2.zero;

                // がくつかないために下向きに速度を加える
                data.trb.Velocity.y = -MaxAbsSpeed.y;
            }

            // ステートが終了したときに呼ばれるメソッド
            public override void OnEnd()
            {
                // 下向き速度を戻す
                data.trb.Velocity.y = 0f;
            }

            // 毎フレーム呼ばれる関数
            public override void Proc()
            {
                // ダッシュステート
                if (data.CanGroundDash() && Parent.input_.GetButtonDown(ActionCode.Dash))
                {
                    data.DashCalled();
                    ChangeState((int)eState.Dush);
                    return;
                }

                // ジャンプ入力ならジャンプステートへ
                if (Parent.input_.GetButtonDown(ActionCode.Jump))
                {
                    ChangeState((int)eState.Jump);
                    return;
                }

                // 地面から離れたらふぉるステートへ
                if (!data.IsGround)
                {
                    ChangeState((int)eState.Fall);
                    return;
                }

                // 左右に押したら歩くステートに変更
                if (Mathf.Abs(Parent.input_.GetAxis(AxisCode.Horizontal)) > 0.2f)
                {
                    ChangeState((int)eState.Walk);
                    return;
                }


                // 地面の摩擦係数によって速度減衰を早める
                ActorUtils.ProcSpeed(ref data.trb.Velocity, Vector2.zero, MaxAbsSpeed, data.GroundFriction);
                if (data.GroundFriction < 0.25f) return;
                ActorUtils.ProcSpeed(ref data.trb.Velocity, Vector2.zero, MaxAbsSpeed, data.GroundFriction);
                if (data.GroundFriction < 0.5f) return;
                ActorUtils.ProcSpeed(ref data.trb.Velocity, Vector2.zero, MaxAbsSpeed, data.GroundFriction);
            }
        }
    }
}