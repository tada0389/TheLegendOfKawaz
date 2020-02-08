using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.NewPlayer;
using TadaLib;

/// <summary>
/// プレイヤーが壁に密着しているときを管理するステート
/// </summary>

namespace Actor.NewPlayer
{
    public partial class Player
    {
        [System.Serializable]
        private class StateWall : StateMachine<Player>.StateBase
        {
            // ステート間で共有するデータのコピーインスタンス
            private Data data = null;

            [SerializeField]
            private Vector2 max_speed = new Vector2(0.18f, 0.05f);
            [SerializeField]
            private Vector2 accel = new Vector2(0.01f, -0.015f);

            // ステートが始まった時に呼ばれるメソッド
            public override void OnStart()
            {
                if (data == null) data = Parent.data_;

            }

            // ステートが終了したときに呼ばれるメソッド
            public override void OnEnd()
            {

            }

            // 毎フレーム呼ばれる関数
            public override void Proc()
            {
                // 壁に密着していなかったら落下ステートへ
                if(!data.IsLeft && !data.IsRight)
                {
                    ChangeState((int)eState.Fall);
                    return;
                }

                if (data.IsLeft)
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        data.velocity = new Vector2(0.2f, 0.35f);
                        ChangeState((int)eState.Jump);
                        return;
                    }
                }
                if (data.IsRight)
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        data.velocity = new Vector2(-0.2f, 0.35f);
                        ChangeState((int)eState.Jump);
                        return;
                    }
                }

                // 移動している方向に速度を加える
                float dir = 0f;
                if (Input.GetKey(KeyCode.LeftArrow)) dir = -1f;
                else if (Input.GetKey(KeyCode.RightArrow)) dir = 1f;

                ActorUtils.AddAccel(ref data.velocity, new Vector2(dir, 1f) * accel * Time.deltaTime * 60f, max_speed);

                // 接地したらステート変更
                if (data.IsGround)
                {
                    ChangeState((int)eState.Walk);
                    return;
                }
            }
        }
    }
}