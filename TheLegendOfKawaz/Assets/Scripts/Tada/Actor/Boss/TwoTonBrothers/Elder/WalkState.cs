using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TadaLib;

/// <summary>
/// ツートンボス(兄)の移動時のステート
/// </summary>

namespace Actor.Enemy.TwoTon.Elder
{
    partial class TwoTonElderBossController
    {
        // 次のアクションを決めるときのステート
        [System.Serializable]
        private class WalkState : StateMachine<TwoTonElderBossController>.StateBase
        {
            // ステートの初期化
            public override void OnInit()
            {

            }

            // 開始時に呼ばれる
            public override void OnStart()
            {
                //Parent.animator_.Play("Idle");
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }
        }
    }
} // namespace Actor.Enemy.TwoTon.Elder