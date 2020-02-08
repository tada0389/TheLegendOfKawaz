﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.NewPlayer;
using TadaLib;

/// <summary>
/// プレイヤーの落下状態(ジャンプでの落下は別)を管理するステート
/// </summary>

namespace Actor.NewPlayer
{
    public partial class Player
    {
        [System.Serializable]
        private class StateFall : StateMachine<Player>.StateBase
        {
            // ステート間で共有するデータのコピーインスタンス
            private Data data = null;

            [SerializeField]
            private Vector2 max_speed = new Vector2(0.18f, 0f);
            [SerializeField]
            private Vector2 accel = new Vector2(0.02f, 0f);
            [SerializeField]
            private float can_jump_time = 0.15f;

            private float timer;

            // ステートが始まった時に呼ばれるメソッド
            public override void OnStart()
            {
                if(data == null) data = Parent.data_;

                timer = 0.0f;

                // 落下アニメーション開始

            }

            // ステートが終了したときに呼ばれるメソッド
            public override void OnEnd()
            {

            }

            // 毎フレーム呼ばれる関数
            public override void Proc()
            {
                timer += Time.deltaTime;
                // 接地したらステート変更
                if (data.IsGround)
                {
                    // 前回のステートに応じて次のステートを決める
                    if (PrevStateId == (int)eState.Run) ChangeState((int)eState.Run);
                    else ChangeState((int)eState.Walk);
                    return;
                }

                if(timer < can_jump_time && Input.GetKeyDown(KeyCode.Space))
                {
                    ChangeState((int)eState.Jump);
                    return;
                }

                // 移動している方向に速度を加える
                float dir = 0f;
                if (Input.GetKey(KeyCode.LeftArrow)) dir = -1f;
                else if (Input.GetKey(KeyCode.RightArrow)) dir = 1f;

                ActorUtils.AddAccel(ref data.velocity, new Vector2(dir, 1f) * accel * Time.deltaTime * 60f, max_speed);
            }
        }
    }
}