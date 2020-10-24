using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Enemy.Thousand;
using TadaLib;
using DG.Tweening;

/// <summary>
/// ツートンボスが勝利したときの会話
/// </summary>

namespace Actor.Enemy.TwoTon
{
    partial class TwoTonBossController
    {
        // 最初の会話
        [System.Serializable]
        private class WinTalkState : StateMachine<TwoTonBossController>.StateBase
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
                KoitanLib.FadeManager.FadeIn(0.5f, "ZakkyScene");
            }
        }
    }
} // namespace Actor.Enemy.TwoTon