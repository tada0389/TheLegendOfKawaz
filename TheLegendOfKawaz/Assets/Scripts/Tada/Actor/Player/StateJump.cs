using System.Collections;
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

            // ステートが始まった時に呼ばれるメソッド
            public override void OnStart()
            {
                if(data == null) data = Parent.data_;

                // ジャンプアニメーション開始
                data.animator.Play("PlayerJump", 0, 0f);

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
                if (dir < -0.5f) data.ChangeDirection(eDir.Left);
                if (dir > 0.5f) data.ChangeDirection(eDir.Right);

                // 接地したらステート変更 ジャンプはじめはIsGroundがtrueになってたので一定時間が経ったら
                if (data.IsGround && Timer > 0.2f)
                {
                    // 前回のステートに応じて次のステートを決める
                    ChangeState((int)eState.Walk);
                    return;
                }

                // 壁に沿っている ただし，速度が一定以下の時
                if(data.velocity.y < wall_thr && (data.IsLeft || data.IsRight))
                {
                    ChangeState((int)eState.Wall);
                }

                // 空中ジャンプ
                if (ActionInput.GetButtonDown(ActionCode.Jump) && data.RequestArialJump())
                {
                    ChangeState((int)eState.Jump);
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

                if (Timer < jump_input_time && ActionInput.GetButton(ActionCode.Jump)) data.velocity = new Vector2(data.velocity.x, jump_power);
            }
        }
    }
}