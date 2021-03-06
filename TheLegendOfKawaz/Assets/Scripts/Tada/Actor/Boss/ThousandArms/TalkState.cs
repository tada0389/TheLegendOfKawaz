﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Enemy.Thousand;
using TadaLib;
using DG.Tweening;

/// <summary>
/// 千手観音ボスの死亡時のステート
/// </summary>

namespace Actor.Enemy.Thousand
{
    partial class ThousandBossController
    {
        // 最初の会話
        [System.Serializable]
        private class TalkState : StateMachine<ThousandBossController>.StateBase
        {
            [SerializeField]
            Sprite im;
            [SerializeField, Multiline(3)]
            private string[] message;
            private int index = 0;
            private bool isEnd;

            private bool not_open_ = true;

            [SerializeField]
            private Animator name_display_ui_;

            // 開始時に呼ばれる
            public override void OnStart()
            {
                float dir = Mathf.Sign(Parent.player_.position.x - Parent.transform.position.x);
                Parent.SetDirection((dir < 0f) ? eDir.Left : eDir.Right);


                Global.GlobalPlayerInfo.ActionEnabled = false;
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if (Timer < 0.1f) return;

                if (not_open_)
                {
                    MessageManager.OpenMessageWindow(message[0], im);
                    not_open_ = false;
                }

                if (!isEnd)
                {
                    if (ActionInput.GetButtonDown(ActionCode.Decide))
                    {
                        index++;
                        if (index < message.Length)
                        {
                            MessageManager.InitMessage(message[index]);
                        }
                        else
                        {
                            EndSeq();
                        }
                    }

                    if (ActionInput.GetButtonDown(ActionCode.Dash))
                    {
                        EndSeq();
                    }
                }
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {
                // ボス名を表示させる
                if (name_display_ui_ != null)
                {
                    name_display_ui_.gameObject.SetActive(true);
                    name_display_ui_.Play("BossText");
                }
                MessageManager.CloseMessageWindow();
                Global.GlobalPlayerInfo.ActionEnabled = true;
            }

            private void EndSeq()
            {
                ChangeState((int)eState.Think);
            }

        }
    }
} // namespace Actor.Enemy.Thousand