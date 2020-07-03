using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor;
using TadaLib;
using Bullet;

/// <summary>
// 千手観音のボススクリプト
/// </summary>

namespace Actor.Enemy.Thousand
{
    [RequireComponent(typeof(TadaRigidbody))]
    [RequireComponent(typeof(BulletSpawner))]
    public partial class ThousandBossController : BaseBossController
    {
        private enum eState
        {
            Think,
            Walk,
            Dead,
            Talk,
            WinTalk,
            ArmStretch, // 腕が伸びる
            ArmThrow, // 腕を飛ばす
            Sutras, // お経
        }

        // ステートを管理して起動するクラス
        private StateMachine<ThousandBossController> state_machine_;

        // 物理演算や速度を変更するやつ
        private TadaRigidbody trb_;

        // 弾を管理して発射させるやつ
        private BulletSpawner bullet_spawner_;

        // アニメータ
        private Animator animator_;

        [SerializeField]
        private List<int> arm_num_;

        // 手のプレハブ
        [SerializeField]
        private ArmController arm_prefab_;

        // 手たち
        private List<ArmController> arms_;

        #region state
        [SerializeField]
        private ThinkState think_state_;
        [SerializeField]
        private WalkState walk_state_;
        [SerializeField]
        private DeadState dead_state_;
        [SerializeField]
        private TalkState talk_state_;
        [SerializeField]
        private WinTalkState win_talk_state_;
        [SerializeField]
        private ArmStretchState arm_stretch_state_;
        [SerializeField]
        private ArmThrowState arm_throw_state_;
        [SerializeField]
        private SutrasState sutras_state_;
        #endregion

        // Start is called before the first frame update
        private void Start()
        {
            trb_ = GetComponent<TadaRigidbody>();
            bullet_spawner_ = GetComponent<BulletSpawner>();
            animator_ = GetComponent<Animator>();

            state_machine_ = new StateMachine<ThousandBossController>(this);

            // 手を生成する
            int boss_defeat_num = 0;
            if (Global.GlobalDataManager.EachBossDefeatCnt(Global.eBossType.Purin) >= 1) ++boss_defeat_num;
            if (Global.GlobalDataManager.EachBossDefeatCnt(Global.eBossType.VernmDrake) >= 1) ++boss_defeat_num;
            if (Global.GlobalDataManager.EachBossDefeatCnt(Global.eBossType.KawazTanBeta) >= 1) ++boss_defeat_num;

            int arm_num = arm_num_[boss_defeat_num];
            arms_ = new List<ArmController>(arm_num);
            for (int i = 0; i < arm_num; ++i)
            {
                ArmController arm = Instantiate(arm_prefab_);
                arm.Init(i, arm_num, transform, player_);
                arms_.Add(arm);
            }

            // ステートの登録
            state_machine_.AddState((int)eState.Think, think_state_);
            state_machine_.AddState((int)eState.Walk, walk_state_);
            state_machine_.AddState((int)eState.Dead, dead_state_);
            state_machine_.AddState((int)eState.Talk, talk_state_);
            state_machine_.AddState((int)eState.WinTalk, win_talk_state_);
            state_machine_.AddState((int)eState.ArmStretch, arm_stretch_state_);
            state_machine_.AddState((int)eState.ArmThrow, arm_throw_state_);
            state_machine_.AddState((int)eState.Sutras, sutras_state_);
            // 初期ステートの設定
            state_machine_.SetInitialState((int)eState.Talk);

            // デバッグ表示
            DebugBoxManager.Display(this).SetSize(new Vector2(500, 400)).SetOffset(new Vector2(0, 0));
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            // ステートの更新
            state_machine_.Proc();

            // 敵を倒していたら勝利のセリフを吐く
            CheckWin();
        }

        // 死亡したときに呼ばれる関数 基底クラスから呼ばれる わかりにくい
        protected override void Dead()
        {
            int cur_state = state_machine_.CurrentStateId;
            if (cur_state != (int)eState.Dead && cur_state != (int)eState.WinTalk)
            {
                state_machine_.ChangeState((int)eState.Dead);
            }
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

        public override string ToString()
        {
            return "(" + trb_.Velocity.x.ToString("F2") + ", " + trb_.Velocity.y.ToString("F2") + ")" +
                "\nState : " + state_machine_.ToString() +
                "\nIsGround : " + trb_.ButtomCollide.ToString() +
                "\nLeftCollide : " + trb_.LeftCollide.ToString() +
                "\nRightCollide : " + trb_.RightCollide.ToString();
        }
    }
}