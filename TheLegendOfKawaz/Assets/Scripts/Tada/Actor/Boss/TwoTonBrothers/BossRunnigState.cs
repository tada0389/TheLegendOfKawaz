using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Enemy.Thousand;
using TadaLib;
using DG.Tweening;

/// <summary>
/// ツートンボスが生存し動いているときのステート
/// </summary>

namespace Actor.Enemy.TwoTon
{
    partial class TwoTonBossController
    {
        [System.Serializable]
        private class BossRunningState : StateMachine<TwoTonBossController>.StateBase
        {
            // 開始時に呼ばれる
            public override void OnStart()
            {
                // ボスを動かす
                Parent.younger_boss_.enabled = true;
                Parent.elder_boss_.enabled = true;
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {
                //// ボスを死亡させる
                //Parent.younger_boss_.Damage(1000);
                //Parent.elder_boss_.Damage(1000);
            }
        }
    }
} // namespace Actor.Enemy.TwoTon