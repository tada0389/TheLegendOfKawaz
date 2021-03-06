﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Player;
using TadaLib;

/// <summary>
/// プレイヤーのダメージ状態を管理するステート
/// </summary>

namespace Actor.Player
{
    public partial class Player
    {
        [System.Serializable]
        private class StateDamage : StateMachine<Player>.StateBase
        {
            // ステート間で共有するデータのコピーインスタンス
            private Data data = null;

            [SerializeField]
            private Vector2 power_ = new Vector2(0.5f, 1.0f);

            // 硬直時間
            [SerializeField]
            private float rigidity_time_ = 1.0f;

           // [SerializeField]
           // private BaseParticle damage_eff_;

            // ステートが始まった時に呼ばれるメソッド
            public override void OnStart()
            {
                if (data == null) data = Parent.data_;

                // 待機アニメーション開始
                if (data.IsGround) Parent.PlayAnim("Damage2");
                else Parent.PlayAnim("Damage");

                // 速度ダメージを受けた方向に飛ぶ
                //data.trb.Velocity.x += power_.x;
                //if(data.trb.Velocity.y < power_.y) data.trb.Velocity.y += power_.y;

                // カメラを揺らす
                CameraSpace.CameraShaker.Shake(0.20f, 0.15f, 0.03f);

                // 時間をスローに
                TadaLib.TimeScaler.Instance.RequestChange(0.5f, 0.025f);

                //EffectPlayer.Play(damage_eff_, Parent.transform.position, Vector3.zero, Parent.transform);
            }

            // ステートが終了したときに呼ばれるメソッド
            public override void OnEnd()
            {

            }

            // 毎フレーム呼ばれる関数
            public override void Proc()
            {
                if (Timer >= rigidity_time_)
                {
                    // 接地してたらステート変更
                    if (data.IsGround)
                    {
                        ChangeState((int)eState.Wait);
                    }
                    else
                    {
                        ChangeState((int)eState.Fall);
                    }
                    return;
                }

                float friction = (data.IsGround) ? data.GroundFriction : Parent.air_friction_;
                ActorUtils.ProcSpeed(ref data.trb.Velocity, Accel, MaxAbsSpeed, friction);
            }
        }
    }
}