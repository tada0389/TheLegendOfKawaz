using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Player;
using TadaLib;

/// <summary>
/// プレイヤーが壁に密着しているときを管理するステート
/// </summary>

namespace Actor.Player
{
    public partial class Player
    {
        [System.Serializable]
        private class StateWall : StateMachine<Player>.StateBase
        {
            // ステート間で共有するデータのコピーインスタンス
            private Data data = null;

            [SerializeField]
            private Vector2 kick_power_ = new Vector2(0.2f, 0.35f);

            // ステートが始まった時に呼ばれるメソッド
            public override void OnStart()
            {
                if (data == null) data = Parent.data_;

                // アニメーション開始
                data.animator.Play("Kabe");

                // 壁に接しているほうを向く
                if (data.IsRight)
                {
                    data.ChangeDirection(eDir.Right);
                }
                else
                {
                    data.ChangeDirection(eDir.Left);
                }
            }

            // ステートが終了したときに呼ばれるメソッド
            public override void OnEnd()
            {

            }

            // 毎フレーム呼ばれる関数
            public override void Proc()
            {
                // 接地したらステート変更
                if (data.IsGround)
                {
                    ChangeState((int)eState.Walk);
                    return;
                }

                // 壁に密着していなかったら落下ステートへ
                if (!data.IsLeft && !data.IsRight)
                {
                    ChangeState((int)eState.Fall);
                    return;
                }

                // 壁ジャンプ
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    data.velocity = kick_power_;
                    if (data.IsRight) data.velocity *= -1;
                    ChangeState((int)eState.Jump);
                    // 逆向きを向く
                    data.ReverseFaceDirection();
                    return;
                }

                // 移動している方向に速度を加える
                float dir = 0f;
                if (Input.GetKey(KeyCode.LeftArrow)) dir = -1f;
                else if (Input.GetKey(KeyCode.RightArrow)) dir = 1f;

                float accel_dir = 1.0f;
                if (data.velocity.y < -MaxAbsSpeed.y) accel_dir = -1f;
                // 加速度に応じて速度を変更する
                ActorUtils.ProcSpeed(ref data.velocity, new Vector2(dir, accel_dir) * Accel, MaxAbsSpeed);
            }
        }
    }
}