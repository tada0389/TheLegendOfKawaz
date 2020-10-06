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

            [SerializeField]
            private BaseParticle kick_effect_;

            // ステートが始まった時に呼ばれるメソッド
            public override void OnStart()
            {
                if (data == null) data = Parent.data_;

                // 空中ジャンプ数をリセット
                data.ResetArialJump();

                // アニメーション開始
                Parent.PlayAnim("isWall", eAnimType.SetBoolTrue);
                //Parent.PlayAnim("Wall");

                // 壁に接していないほうを向く
                if (data.IsRight)
                {
                    data.ChangeDirection(eDir.Left);
                }
                else
                {
                    data.ChangeDirection(eDir.Right);
                }

                // 即ジャンプする場合もある (落下状態から壁ジャンプなど)
                if (Parent.input_.GetButtonDown(ActionCode.Jump))
                {
                    WallKick();
                    return;
                }
            }

            // ステートが終了したときに呼ばれるメソッド
            public override void OnEnd()
            {
                Parent.PlayAnim("isWall", eAnimType.SetBoolFalse);
            }

            // 毎フレーム呼ばれる関数
            public override void Proc()
            {
                // 壁に接しているほうを押さなかったら落下する
                if(!(data.IsLeft && Parent.input_.GetButton(ButtonCode.Left) || data.IsRight && Parent.input_.GetButton(ButtonCode.Right))){
                    ChangeState((int)eState.Fall);
                    return;
                }

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
                if (Parent.input_.GetButtonDown(ActionCode.Jump))
                {
                    WallKick();
                    return;
                }

                // ダッシュステート
                if (Parent.input_.GetButtonDown(ActionCode.Dash))
                {
                    //data.ReverseFaceDirection();
                    ChangeState((int)eState.Dush);
                    return;
                }

                // 移動している方向に速度を加える
                float dir = Parent.input_.GetAxis(AxisCode.Horizontal);

                float accel_dir = 1.0f;
                if (data.trb.Velocity.y < -MaxAbsSpeed.y) accel_dir = -1f;
                // 加速度に応じて速度を変更する
                ActorUtils.ProcSpeed(ref data.trb.Velocity, new Vector2(dir, accel_dir) * Accel, MaxAbsSpeed);
            }

            private void WallKick()
            {
                EffectPlayer.Play(kick_effect_, data.transform.position + new Vector3((data.Dir == eDir.Left) ? 1.0f : -1.0f, 0f, 0f), new Vector2((data.Dir == eDir.Left) ? -1.0f : 1.0f, 0f));

                data.trb.Velocity = kick_power_;
                if (data.IsRight) data.trb.Velocity *= -1;
                ChangeState((int)eState.Jump);
                // ダッシュ回数リセット
                data.ResetDash();
            }
        }
    }
}