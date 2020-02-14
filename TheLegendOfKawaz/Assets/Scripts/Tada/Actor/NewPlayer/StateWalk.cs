﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor;
using Actor.NewPlayer;
using TadaLib;

/// <summary>
/// プレイヤーの歩き状態を管理するステート
/// 接地している時のみ
/// </summary>

namespace Actor.NewPlayer
{
    public partial class Player
    {
        [System.Serializable]
        private class StateWalk : StateMachine<Player>.StateBase
        {
            // ステート間で共有するデータのコピーインスタンス
            private Data data = null;

            // ステートが始まった時に呼ばれるメソッド
            public override void OnStart()
            {
                if(data == null) data = Parent.data_;

                // 歩きアニメーション開始
                data.animator.Play("Walk");

                // 移動している方向に速度を加える
                data.velocity = new Vector2(data.velocity.x, 0f);
            }

            // ステートが終了したときに呼ばれるメソッド
            public override void OnEnd()
            {

            }

            // 毎フレーム呼ばれる関数
            public override void Proc()
            {
                // ジャンプ入力ならジャンプステートへ
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    ChangeState((int)eState.Jump);
                    return;
                }

                // 地面から離れたらふぉるステートへ
                if (!data.IsGround)
                {
                    ChangeState((int)eState.Fall);
                }

                // 移動している方向に速度を加える
                float dir = 0f;
                if (Input.GetKey(KeyCode.LeftArrow)) dir = -1f;
                else if (Input.GetKey(KeyCode.RightArrow)) dir = 1f;
                else
                {
                    // 何も押していないならWait状態に
                    ChangeState((int)eState.Wait);
                    return;
                }
                if (dir < -0.5f) data.ChangeDirection(eDir.Left);
                if (dir > 0.5f) data.ChangeDirection(eDir.Right);

                ActorUtils.ProcSpeed(ref data.velocity, new Vector2(dir, 1f) * Accel, MaxAbsSpeed);
            }
        }
    }
}