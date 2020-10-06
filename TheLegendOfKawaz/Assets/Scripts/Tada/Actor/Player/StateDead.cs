using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Player;
using TadaLib;

/// <summary>
/// プレイヤーの死亡状態を管理するステート
/// </summary>

namespace Actor.Player
{
    public partial class Player
    {
        [System.Serializable]
        private class StateDead : StateMachine<Player>.StateBase
        {
            // ステート間で共有するデータのコピーインスタンス
            private Data data = null;

            [SerializeField]
            private float scene_transitioin_time_ = 3.0f;

            [SerializeField]
            private bool GoNextScene = true;
            [SerializeField]
            private string next_scene_ = "ZakkyScene";

            private bool is_feed_;

            [SerializeField]
            private BaseParticle dead_eff_;

            // ステートが始まった時に呼ばれるメソッド
            public override void OnStart()
            {
                if (data == null) data = Parent.data_;

                Parent.PlayAnim("Cry");

                is_feed_ = false;

                EffectPlayer.Play(dead_eff_, Parent.transform.position, Vector3.zero, Parent.transform);

                // 死亡回数を加算する
                //Global.GlobalDataManager.AddDeathCnt();
            }

            // ステートが終了したときに呼ばれるメソッド
            public override void OnEnd()
            {

            }

            // 毎フレーム呼ばれる関数
            public override void Proc()
            {
                ActorUtils.ProcSpeed(ref data.trb.Velocity, Accel, MaxAbsSpeed);

                if (!GoNextScene) return;

                if (!is_feed_ && Timer > scene_transitioin_time_)
                {
                    KoitanLib.FadeManager.FadeIn(0.5f, next_scene_, 0);
                    is_feed_ = true;
                }
            }
        }
    }
}