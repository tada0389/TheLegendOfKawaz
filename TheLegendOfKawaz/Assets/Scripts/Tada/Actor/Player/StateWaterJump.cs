using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor;
using Actor.Player;
using TadaLib;

/// <summary>
/// プレイヤーの歩き状態を管理するステート
/// 接地している時のみ
/// </summary>

namespace Actor.Player
{
    public partial class Player
    {
        [System.Serializable]
        private class StateWaterJump : StateMachine<Player>.StateBase
        {
            // ステート間で共有するデータのコピーインスタンス
            private Data data = null;

            // ジャンプ力
            [SerializeField]
            private float jump_power_ = 0.1f;

            // 硬直時間
            [SerializeField]
            private float rigity_time_ = 0.1f;

            // ステートが始まった時に呼ばれるメソッド
            public override void OnStart()
            {
                if (data == null) data = Parent.data_;

                // ジャンプアニメーション開始
                data.animator.Play("Jump", 0, 0f);

                // 上向きに速度を加える
                data.velocity = new Vector2(data.velocity.x, jump_power_);
                data.velocity.y = Mathf.Min(data.velocity.y + jump_power_, MaxAbsSpeed.y);
            }

            // ステートが終了したときに呼ばれるメソッド
            public override void OnEnd()
            {
            }

            // 毎フレーム呼ばれる関数
            public override void Proc()
            {
                // 水中から出たら一定距離のジャンプ そのままStateJump
                if (!data.IsUnderWater)
                {
                    ChangeState((int)eState.Jump);
                    return;
                }

                // 一定時間たったら歩行状態に戻す
                if (Timer > rigity_time_)
                {
                    ChangeState((int)eState.WaterWalk);
                    return;
                }

                // 天井に頭がついていたら落ちる
                if (data.IsHead && data.velocity.y > 0f)
                {
                    data.velocity = new Vector2(data.velocity.x, 0f);
                }

                // 移動している方向に速度を加える
                float dir = ActionInput.GetAxis(AxisCode.Horizontal);
                if (Mathf.Abs(dir) < 0.2f) dir = 0f;
                if (dir < -0f) data.ChangeDirection(eDir.Left);
                if (dir > 0f) data.ChangeDirection(eDir.Right);

                ActorUtils.ProcSpeed(ref data.velocity, new Vector2(dir, 1f) * Accel, MaxAbsSpeed);
            }
        }
    }
}