using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.NewPlayer;
using TadaLib;

/// <summary>
/// プレイヤーのジャンプ状態を管理するステート
/// </summary>

namespace Actor.NewPlayer
{
    public partial class Player
    {
        [System.Serializable]
        private class StateJump : StateMachine<Player>.StateBase
        {
            // ステート間で共有するデータのコピーインスタンス
            private Data data = null;

            [SerializeField]
            private Vector2 max_speed = new Vector2(0.18f, 0.20f);
            [SerializeField]
            private Vector2 accel = new Vector2(0.02f, -0.02f);
            [SerializeField]
            private float jump_power = 0.4f;
            [SerializeField]
            private float jump_input_time = 0.25f;

            // ジャンプしてからの経過時間
            private float timer_;

            // ステートが始まった時に呼ばれるメソッド
            public override void OnStart()
            {
                if(data == null) data = Parent.data_;

                // ジャンプアニメーション開始
                data.animator.Play("PlayerJump", 0, 0f);

                timer_ = 0.0f;

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
                timer_ += Time.deltaTime;

                // 移動している方向に速度を加える
                float dir = 0f;
                if (Input.GetKey(KeyCode.LeftArrow)) dir = -1f;
                else if (Input.GetKey(KeyCode.RightArrow)) dir = 1f;

                // ただし，頂点付近だと加速度を弱める
                float accel_rate_y = 1.0f;
                if (data.velocity.y < 0.15f) accel_rate_y = 0.5f;
                ActorUtils.AddAccel(ref data.velocity, new Vector2(dir, accel_rate_y) * accel * Time.deltaTime * 60f, max_speed);

                if (timer_ < jump_input_time && Input.GetKey(KeyCode.Space)) data.velocity = new Vector2(data.velocity.x, jump_power);

                if (data.IsLeft)
                {
                    if (data.velocity.y < 0f) data.velocity = new Vector2(data.velocity.x, -0.15f);
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        data.velocity = new Vector2(0.2f, 0.35f);
                        ChangeState((int)eState.Jump);
                        return;
                    }
                }
                if (data.IsRight)
                {
                    if (data.velocity.y < 0f) data.velocity = new Vector2(data.velocity.x, -0.15f);
                    if (Input.GetKeyDown(KeyCode.Space))
                    { 
                        data.velocity = new Vector2(-0.2f, 0.35f);
                        ChangeState((int)eState.Jump);
                        return;
                    }
                }

                // 天井に頭がついていたら落ちる
                if (data.IsHead && data.velocity.y > 0f)
                {
                    data.velocity = new Vector2(data.velocity.x, 0f);
                }

                // 接地したらステート変更 ジャンプはじめはIsGroundがtrueになってたので一定時間が経ったら
                if (data.IsGround && timer_ > 0.2f)
                {
                    // 前回のステートに応じて次のステートを決める
                    if (PrevStateId == (int)eState.Run) ChangeState((int)eState.Run);
                    else ChangeState((int)eState.Walk);
                    return;
                }
            }
        }
    }
}