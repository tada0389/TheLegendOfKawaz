using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor;
using Actor.Enemy;
using TadaLib;

/// <summary>
/// プレイヤーの歩き状態を管理するステート
/// 接地している時のみ
/// </summary>

namespace Actor.Enemy.Boss1
{
    public partial class ThunderBossController
    {
        [System.Serializable]
        private class StateIdle : StateMachine<BaseBossController>.StateBase
        {

            // ステートが始まった時に呼ばれるメソッド
            public override void OnStart()
            {
            }

            // ステートが終了したときに呼ばれるメソッド
            public override void OnEnd()
            {

            }

            // 毎フレーム呼ばれる関数
            public override void Proc()
            {

            }
        }
    }
}