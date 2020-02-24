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
        private class StateWalk : StateMachine<Player>.StateBase
        {
            // ステート間で共有するデータのコピーインスタンス
            private Data data = null;

            // ステートが始まった時に呼ばれるメソッド
            public override void OnStart()
            {
                if(data == null) data = Parent.data_;

                // 歩きアニメーション開始
                //data.animator.Play("Walk");
                data.animator.SetBool("isWalk", true);

                // 移動している方向に速度を加える
                data.velocity = new Vector2(data.velocity.x, 0f);
            }

            // ステートが終了したときに呼ばれるメソッド
            public override void OnEnd()
            {
                data.animator.SetBool("isWalk", false);
            }

            // 毎フレーム呼ばれる関数
            public override void Proc()
            {
                // ジャンプ入力ならジャンプステートへ
                if (ActionInput.GetButtonDown(ActionCode.Jump))
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
                float dir = ActionInput.GetAxis(AxisCode.Horizontal);
                if(dir == 0.0f) { // 良くない
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