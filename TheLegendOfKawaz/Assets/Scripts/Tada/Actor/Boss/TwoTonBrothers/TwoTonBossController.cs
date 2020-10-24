using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ツートンブラザーズの2人を管理するクラス
/// </summary>

namespace Actor.Enemy.TwoTon
{
    public partial class TwoTonBossController : BaseActorController
    {
        private enum eState
        {
            InitialTalk, // 開始時の会話
            BossRunning, // 兄弟ボスを実行中
            WinTalk, // 勝利時の会話
            Dead, // 死亡時
        }

        // ステートを管理して起動するクラス
        private TadaLib.StateMachine<TwoTonBossController> state_machine_;

        #region state class
        // ステート
        [SerializeField]
        private InitialTalkState initial_talk_state_;
        [SerializeField]
        private BossRunningState boss_running_state_;
        [SerializeField]
        private WinTalkState win_talk_state_;
        [SerializeField]
        private DeadState dead_state_;
# endregion

        [System.Serializable]
        private class TalkContest
        {
            [field:SerializeField]
            public Sprite img { private set; get; }
            [field: SerializeField, Multiline(3)]
            public string[] message { private set; get; }
        }

        [SerializeField]
        private BaseBossController elder_boss_;
        [SerializeField]
        private BaseBossController younger_boss_;

        [SerializeField]
        private Transform player_;

        private void Start()
        {
            state_machine_ = new TadaLib.StateMachine<TwoTonBossController>(this);

            // ステートの登録
            state_machine_.AddState((int)eState.InitialTalk, initial_talk_state_);
            state_machine_.AddState((int)eState.BossRunning, boss_running_state_);
            state_machine_.AddState((int)eState.WinTalk, win_talk_state_);
            state_machine_.AddState((int)eState.Dead, dead_state_);
            // 初期ステートの設定
            state_machine_.SetInitialState((int)eState.InitialTalk);

            // 兄弟の動きを止める
            elder_boss_.enabled = false;
            younger_boss_.enabled = false;
        }

        private void FixedUpdate()
        {
            // ステートの更新
            state_machine_.Proc();

            // 敵を倒していたら勝利のセリフを吐く
            CheckWin();
        }

        public override void Damage(int damage)
        {
            int cur_state = state_machine_.CurrentStateId;
            if (cur_state != (int)eState.Dead && cur_state != (int)eState.WinTalk)
            {
                HP = Mathf.Max(HP - damage, 0);
                if (HP == 0) Dead();
            }
        }

        private void Dead()
        {
            state_machine_.ChangeState((int)eState.Dead);
        }

        private void CheckWin()
        {
            // 敵を倒していたら勝利のセリフを吐く
            int cur_state = state_machine_.CurrentStateId;
            if (cur_state != (int)eState.Dead && cur_state != (int)eState.WinTalk &&
                player_.GetComponent<Actor.Player.Player>().IsDead())
            {
                state_machine_.ChangeState((int)eState.WinTalk);
            }
        }

        // 会話パート
        private IEnumerator Talk(List<TalkContest> contents, System.Action callback)
        {
            foreach (var c in contents)
            {
                yield return new WaitForSeconds(0.15f);

                int index = 0;

                while (index < c.message.Length)
                {
                    if (ActionInput.GetButtonDown(ActionCode.Dash)) break;

                    if (index == 0) MessageManager.OpenMessageWindow(c.message[index++], c.img);
                    else MessageManager.InitMessage(c.message[index++]);

                    yield return null;
                    yield return new WaitUntil(() => ActionInput.GetButtonDown(ActionCode.Decide) || ActionInput.GetButtonDown(ActionCode.Dash));
                }

                MessageManager.CloseMessageWindow();
            }

            callback.Invoke();
        }
    }
}