using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Enemy.Purin;
using TadaLib;
using DG.Tweening;

/// <summary>
/// プリンボスの死亡時のステート
/// </summary>

namespace Actor.Enemy.Purin
{
    partial class PurinBossController
    {
        // 最初の会話
        [System.Serializable]
        private class TalkState : StateMachine<PurinBossController>.StateBase
        {
            [SerializeField]
            Sprite im;
            [SerializeField, Multiline(3)]
            private string[] message;
            private int index = 0;
            private bool isEnd;

            [SerializeField]
            private Animator name_display_ui_;

            // 開始時に呼ばれる
            public override void OnStart()
            {
                float dir = Mathf.Sign(Parent.player_.position.x - Parent.transform.position.x);
                Parent.SetDirection((dir < 0f) ? eDir.Left : eDir.Right);
                MessageManager.OpenMessageWindow(message[0], im);
                Global.GlobalPlayerInfo.ActionEnabled = false;
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                //if (SettingManager.WindowOpened()) return;

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
                MessageManager.CloseMessageWindow();
                Global.GlobalPlayerInfo.ActionEnabled = true;
                // ボス名を表示させる
                if (name_display_ui_ != null)
                {
                    name_display_ui_.gameObject.SetActive(true);
                    name_display_ui_.Play("BossText");
                }
            }

            private void EndSeq()
            {
                ChangeState((int)eState.Think);
            }

        }
    }
} // namespace Actor.Enemy.Purin