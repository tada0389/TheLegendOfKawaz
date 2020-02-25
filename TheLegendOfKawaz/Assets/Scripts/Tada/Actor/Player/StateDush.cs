﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Player;
using TadaLib;

/// <summary>
/// プレイヤーのダッシュ状態を管理するステート
/// 地上ダッシュと空中ダッシュでステートわければよかった
/// </summary>

namespace Actor.Player
{
    public partial class Player
    {
        [System.Serializable]
        private class StateDush : StateMachine<Player>.StateBase
        {
            // ステート間で共有するデータのコピーインスタンス
            private Data data = null;

            [SerializeField]
            private float dush_time_ = 1.0f;

            [SerializeField]
            private float dush_speed_ = 0.4f;

            private bool is_air_dash_;

            // ステートが始まった時に呼ばれるメソッド
            public override void OnStart()
            {
                if (data == null) data = Parent.data_;

                // 待機アニメーション開始
                data.animator.Play("Dash");
                data.animator.SetBool("isDash", true);

                is_air_dash_ = !data.IsGround;

                data.velocity.x = (data.Dir == eDir.Left) ? -dush_speed_ : dush_speed_;
                data.velocity.y = 0f;
            }

            // ステートが終了したときに呼ばれるメソッド
            public override void OnEnd()
            {
                data.animator.SetBool("isDash", false);
                // 急に落ちないように少し上昇する
                if(!data.IsGround) data.velocity.y += 0.1f;
            }

            // 毎フレーム呼ばれる関数
            public override void Proc()
            {
                // 壁に当たったら終了
                if(data.IsLeft && data.Dir == eDir.Left)
                {
                    if (data.IsGround) ChangeState((int)eState.Walk);
                    else ChangeState((int)eState.Fall);
                    return;
                }
                if(data.IsRight && data.Dir == eDir.Right)
                {
                    if (data.IsGround) ChangeState((int)eState.Walk);
                    else ChangeState((int)eState.Fall);
                    return;
                }

                // ジャンプ
                if (data.IsGround && ActionInput.GetButtonDown(ActionCode.Jump))
                {
                    ChangeState((int)eState.Jump);
                    return;
                }

                // 時間経過でステート以降
                if(Timer > dush_time_)
                {
                    if (data.IsGround) ChangeState((int)eState.Walk);
                    else ChangeState((int)eState.Fall);
                    return;
                }

                if (!is_air_dash_) ActorUtils.ProcSpeed(ref data.velocity, new Vector2(0f, Accel.y), MaxAbsSpeed);
                data.velocity.x = (data.Dir == eDir.Left) ? -dush_speed_ : dush_speed_;
                //ActorUtils.ProcSpeed(ref data.velocity, new Vector2(Mathf.Sign(-data.velocity.x), 1f) * Accel, MaxAbsSpeed);
            }
        }
    }
}