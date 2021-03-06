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
        private class StateDashJump : StateMachine<Player>.StateBase
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

            // ステートが始まった時に呼ばれるメソッド
            public override void OnStart()
            {
                if (data == null) data = Parent.data_;

                // ジャンプアニメーション開始
                Parent.PlayAnim("Jump", eAnimType.Restart);

                // 音を鳴らす
                if (PrevStateId == (int)eState.Wall)
                {
                }
                else if (data.IsGround) data.audio.PlayOneShot(jump_se_1);
                else data.audio.PlayOneShot(jump_se_2);

                // 上向きに速度を加える
                data.trb.Velocity.y = jump_power;
                // 地上でダッシュジャンプすると急加速 Celesteみたいな
                //if(data.IsGround) data.velocity.x *= 1.5f;
                // 計算式変更
                float x = Parent.dash_remain_time_;
                if (x > 0.75f) x = 1.0f;
                x = Mathf.Sqrt(x);
                //Debug.Log(x);
                if (data.IsGround) data.trb.Velocity.x *= (1.0f + x * 0.5f);
            }

            // ステートが終了したときに呼ばれるメソッド
            public override void OnEnd()
            {

            }

            // 毎フレーム呼ばれる関数
            public override void Proc()
            {
                // 水中に落ちた
                if (data.IsUnderWater)
                {
                    ChangeState((int)eState.WaterWalk);
                    return;
                }

                // 移動している方向に速度を加える
                float dir = Parent.input_.GetAxis(AxisCode.Horizontal);
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
                if (data.CanWallKick && data.trb.Velocity.y < wall_thr &&
                    ((data.IsLeft && Parent.input_.GetButton(ButtonCode.Left)) || (data.IsRight && Parent.input_.GetButton(ButtonCode.Right))))
                {
                    ChangeState((int)eState.Wall);
                }

                // 空中ジャンプ
                if (data.RequestArialJump() && Parent.input_.GetButtonDown(ActionCode.Jump))
                {
                    data.ArialJumpCalled();
                    ChangeState((int)eState.Jump);
                    return;
                }

                // ダッシュステート
                if (data.CanGroundDash() && Parent.input_.GetButtonDown(ActionCode.Dash))
                {
                    data.DashCalled();
                    ChangeState((int)eState.Dush);
                    return;
                }

                // ダッシュステート
                if (data.CanAirDash() && Parent.input_.GetButtonDown(ActionCode.Dash))
                {
                    data.AirDashCalled();
                    ChangeState((int)eState.Dush);
                    return;
                }

                // 天井に頭がついていたら落ちる
                if (data.IsHead && data.trb.Velocity.y > 0f)
                {
                    data.trb.Velocity = new Vector2(data.trb.Velocity.x, 0f);
                }

                // 壁に当たってるなら速度ゼロ
                if ((data.trb.Velocity.x > 0f && data.IsRight) || (data.trb.Velocity.x < 0f && data.IsLeft)) data.trb.Velocity.x = 0f;

                // ただし，頂点付近だと加速度を弱める
                float accel_rate_y = 1.0f;
                if (data.trb.Velocity.y < 0.15f) accel_rate_y = 0.5f;
                float can_handle = (Mathf.Abs(data.trb.Velocity.x) < Parent.cant_handle_velocity_shr_) ? 1.0f : 0.0f;
                ActorUtils.ProcSpeed(ref data.trb.Velocity, new Vector2(dir * can_handle, accel_rate_y) * Accel, MaxAbsSpeed, 0.72f * Parent.air_friction_);

                // ある程度の時間はジャンプボタン長押しでジャンプ飛距離を伸ばせる
                if (Timer < jump_input_time && !data.IsHead && Parent.input_.GetButton(ActionCode.Jump)) data.trb.Velocity = new Vector2(data.trb.Velocity.x, jump_power);
            }
        }
    }
}