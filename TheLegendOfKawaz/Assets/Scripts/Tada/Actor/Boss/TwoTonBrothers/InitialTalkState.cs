using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Enemy.Thousand;
using TadaLib;
using DG.Tweening;

/// <summary>
/// ツートンボスの対戦時の最初の会話
/// </summary>

namespace Actor.Enemy.TwoTon
{
    partial class TwoTonBossController
    {
        // 最初の会話
        [System.Serializable]
        private class InitialTalkState : StateMachine<TwoTonBossController>.StateBase
        {
            [SerializeField]
            private List<TalkContest> talk_contests;

            // 開始時に呼ばれる
            public override void OnStart()
            {
                Global.GlobalPlayerInfo.ActionEnabled = false;

                Parent.StartCoroutine(Parent.Talk(talk_contests, TalkEnd));
            }


            // 終了時に呼ばれる
            public override void OnEnd()
            {
                Global.GlobalPlayerInfo.ActionEnabled = true;
            }

            private void TalkEnd()
            {
                ChangeState((int)eState.BossRunning);
            }
        }
    }
} // namespace Actor.Enemy.TwoTon