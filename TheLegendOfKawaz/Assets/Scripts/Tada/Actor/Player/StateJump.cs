﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Player;
using TadaLib;

/// <summary>
/// プレイヤーのジャンプ状態を管理するステート
/// </summary>

namespace Actor.Player
{
    public partial class Player
    {
        [System.Serializable]
        private class StateJump : StateMachine<Player>.StateBase
        {
            // ステート間で共有するデータのコピーインスタンス
            private Data data = null;

            [SerializeField]
            private float jump_power = 0.4f;
            [SerializeField]
            private float jump_input_time = 0.25f;

            [SerializeField]
            private float wall_thr = 0.05f;

            [SerializeField]
            private AudioClip jump_se_1;
            [SerializeField]
            private AudioClip jump_se_2;

            [SerializeField]
            private float kick_wait_time_ = 0.2f;
            // 前回壁ジャンプをしていた
            private bool is_kicked_;

            // ステートが始まった時に呼ばれるメソッド
            public override void OnStart()
            {
                if (data == null) data = Parent.data_;

                // ジャンプアニメーション開始
                data.animator.Play("Jump", 0, 0f);

                // 音を鳴らす
                if (PrevStateId == (int)eState.Wall)
                {
                }
                else if (data.IsGround) data.audio.PlayOneShot(jump_se_1);
                else data.audio.PlayOneShot(jump_se_2);

                is_kicked_ = (PrevStateId == (int)eState.Wall);

                // 上向きに速度を加える
                data.velocity = new Vector2(data.velocity.x, jump_power);
            }

            // ステートが終了したときに呼ばれるメソッド
            public override void OnEnd()
            {

            }

            // 毎フレーム呼ばれる関数
            public override void Proc()
            {
                // 移動している方向に速度を加える
                float dir = ActionInput.GetAxis(AxisCode.Horizontal);
                // もし壁キック後なら一定時間は移動は固定
                if (is_kicked_ && Timer < kick_wait_time_) dir = 0f;
                if (Mathf.Abs(dir) < 0.2f) dir = 0f;
                if (dir < -0f) data.ChangeDirection(eDir.Left);
                if (dir > 0f) data.ChangeDirection(eDir.Right);

                // 接地したらステート変更 ジャンプはじめはIsGroundがtrueになってたので一定時間が経ったら
                if (data.IsGround && Timer > 0.2f)
                {
                    // 前回のステートに応じて次のステートを決める
                    ChangeState((int)eState.Walk);
                    return;
                }

                // 壁に沿っている ただし，速度が一定以下の時
                if(data.CanWallKick && data.velocity.y < wall_thr &&
                    ((data.IsLeft && ActionInput.GetButton(ButtonCode.Left)) || (data.IsRight && ActionInput.GetButton(ButtonCode.Right))))
                {
                    ChangeState((int)eState.Wall);
                    return;
                }

                // ジャンプ
                if (ActionInput.GetButtonDown(ActionCode.Jump))
                {
                    // 壁キック
                    if (data.CanWallKick && (data.IsLeft || data.IsRight))
                    {
                        ChangeState((int)eState.Wall);
                        return;
                    }

                    // 空中ジャンプ
                    if (data.RequestArialJump())
                    {
                        ChangeState((int)eState.Jump);
                        return;
                    }
                }

                // ダッシュステート
                if (ActionInput.GetButtonDown(ActionCode.Dash) && data.CanAirDash())
                {
                    ChangeState((int)eState.Dush);
                    return;
                }

                // 天井に頭がついていたら落ちる
                if (data.IsHead && data.velocity.y > 0f)
                {
                    data.velocity = new Vector2(data.velocity.x, 0f);
                }

                // ただし，頂点付近だと加速度を弱める
                float accel_rate_y = 1.0f;
                if (data.velocity.y < 0.15f) accel_rate_y = 0.5f;
                ActorUtils.ProcSpeed(ref data.velocity, new Vector2(dir, accel_rate_y) * Accel, MaxAbsSpeed);

                // ある程度の時間はジャンプボタン長押しでジャンプ飛距離を伸ばせる
                if (Timer < jump_input_time && !data.IsHead && ActionInput.GetButton(ActionCode.Jump)) data.velocity = new Vector2(data.velocity.x, jump_power);
            }
        }
    }
}