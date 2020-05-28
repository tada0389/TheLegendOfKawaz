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
            }

            private void EndSeq()
            {
                ChangeState((int)eState.Think);
                Global.GlobalPlayerInfo.ActionEnabled = true;
            }

        }
    }
} // namespace Actor.Enemy.Purin