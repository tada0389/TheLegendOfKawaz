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

            private float not_ground_time_;

            // ステートが始まった時に呼ばれるメソッド
            public override void OnStart()
            {
                if(data == null) data = Parent.data_;

                // 歩きアニメーション開始
                //data.animator.Play("Walk");
                data.animator.SetBool("isWalk", true);

                // 移動している方向に速度を加える
                data.velocity = new Vector2(data.velocity.x, -0.02f);

                not_ground_time_ = 0.0f;

                // がくつかないために下向きに速度を加える
                data.velocity.y = -MaxAbsSpeed.y;
            }

            // ステートが終了したときに呼ばれるメソッド
            public override void OnEnd()
            {
                data.animator.SetBool("isWalk", false);
                // 下向きの速度をもとにもどす (がくつかないために下向きに速度を加えていた)
                data.velocity.y = 0f;
            }

            // 毎フレーム呼ばれる関数
            public override void Proc()
            {
                // ジャンプ入力ならジャンプステートへ
                if (Parent.input_.GetButtonDown(ActionCode.Jump))
                {
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

                // 地面から離れたらふぉるステートへ
                if (!data.IsGround) not_ground_time_ += Time.deltaTime;
                else not_ground_time_ = 0.0f;

                // 壁に当たってるなら速度ゼロ
                if ((data.velocity.x > 0f && data.IsRight) || (data.velocity.x < 0f && data.IsLeft)) data.velocity.x = 0f;

                // 移動している方向に速度を加える
                float dir = Parent.input_.GetAxis(AxisCode.Horizontal);
                if (Mathf.Abs(dir) < 0.2f) dir = 0f;
                if (dir == 0.0f) { // 良くない
                    // 何も押していないならWait状態に
                    ChangeState((int)eState.Wait);
                    return;
                }
                if (dir < -0f) data.ChangeDirection(eDir.Left);
                if (dir > 0f) data.ChangeDirection(eDir.Right);

                ActorUtils.ProcSpeed(ref data.velocity, new Vector2(dir, 1f) * Accel * data.GroundFriction, MaxAbsSpeed * (1 / (Mathf.Max(0.5f, data.GroundFriction))));

                if (not_ground_time_ > 0.0f)
                {
                    ChangeState((int)eState.Fall);
                    return;
                }
            }
        }
    }
}