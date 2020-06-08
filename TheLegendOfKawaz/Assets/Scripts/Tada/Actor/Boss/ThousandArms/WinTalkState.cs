using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Enemy.Purin;
using TadaLib;
using DG.Tweening;
using KoitanLib;

/// <summary>
/// 千手観音がプレイヤーを倒した時に流れるセリフ
/// </summary>

namespace Actor.Enemy.Thousand
{
    partial class ThousandBossController { 
        // プレイヤー撃墜後のセリフ
        [System.Serializable]
        private class WinTalkState : StateMachine<ThousandBossController>.StateBase
        {
            [SerializeField]
            Sprite im;
            [SerializeField, Multiline(3)]
            private string[] message;
            private int index = 0;
            private bool isEnd;
            private bool open_window_ = false;
            [SerializeField]
            private float talk_wait_time_ = 1.5f;

            // 開始時に呼ばれる
            public override void OnStart()
            {
                float dir = Mathf.Sign(Parent.player_.position.x - Parent.transform.position.x);
                Parent.SetDirection((dir < 0f) ? eDir.Left : eDir.Right);
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                // セリフを出してすぐにスキップされるのを防ぐ (ジャンプとかダッシュキーを押すと進んでしまうため)
                if (Timer > talk_wait_time_ / 2f)
                {
                    if (!open_window_)
                    {
                        MessageManager.OpenMessageWindow(message[0], im);
                        open_window_ = true;
                    }
                }

                // 死亡アニメーションが終わるまでまつ
                if (Timer < talk_wait_time_) return;

                if (!isEnd)
                {
                    if (!open_window_)
                    {
                        MessageManager.OpenMessageWindow(message[0], im);
                        open_window_ = true;
                    }

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
            }

            private void EndSeq()
            {
                isEnd = true;
                MessageManager.CloseMessageWindow();
                FadeManager.FadeIn(0.5f, "ZakkyScene");
            }
        }
    }
} // namespace Actor.Enemy.Purin