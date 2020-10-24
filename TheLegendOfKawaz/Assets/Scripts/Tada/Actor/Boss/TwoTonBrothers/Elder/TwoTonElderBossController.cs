using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TadaLib;
using Bullet;

/// <summary>
/// ツートンブラザーズの弟
/// </summary>

namespace Actor.Enemy.TwoTon.Elder
{
    [RequireComponent(typeof(TadaRigidbody))]
    [RequireComponent(typeof(BulletSpawner))]
    public partial class TwoTonElderBossController : BaseBossController
    {
        // 親クラス
        [SerializeField]
        private BaseActorController two_ton_manager_;

        private enum eState
        {
            Think,
            Walk,
            Dead,
            SpinAttack1,
            SpinAttack2,
            SpinAttack3,
            SpinAttack4,
        }

        // ステートを管理して起動するクラス
        private StateMachine<TwoTonElderBossController> state_machine_;

        // 物理演算や速度を変更するやつ
        private TadaRigidbody trb_;

        // 弾を管理して発射させるやつ
        private BulletSpawner bullet_spawner_;

        // アニメータ
        private Animator animator_;

        // HP変化を監視する
        private int prev_hp_;

        #region state
        [SerializeField]
        private ThinkState think_state_;
        [SerializeField]
        private WalkState walk_state_;
        [SerializeField]
        private DeadState dead_state_;
        [SerializeField]
        private SpinAttackState1 spin_state1_;
        [SerializeField]
        private SpinAttackState2 spin_state2_;
        [SerializeField]
        private SpinAttackState3 spin_state3_;
        [SerializeField]
        private SpinAttackState4 spin_state4_;
        #endregion

        // Start is called before the first frame update
        private void Start()
        {
            trb_ = GetComponent<TadaRigidbody>();
            bullet_spawner_ = GetComponent<BulletSpawner>();
            animator_ = GetComponent<Animator>();

            state_machine_ = new StateMachine<TwoTonElderBossController>(this);

            // ステートの登録
            state_machine_.AddState((int)eState.Think, think_state_);
            state_machine_.AddState((int)eState.Walk, walk_state_);
            state_machine_.AddState((int)eState.Dead, dead_state_);
            state_machine_.AddState((int)eState.SpinAttack1, spin_state1_);
            state_machine_.AddState((int)eState.SpinAttack2, spin_state2_);
            state_machine_.AddState((int)eState.SpinAttack3, spin_state3_);
            state_machine_.AddState((int)eState.SpinAttack4, spin_state4_);
            // 初期ステートの設定
            state_machine_.SetInitialState((int)eState.Think);

            // デバッグ表示
            DebugBoxManager.Display(this).SetSize(new Vector2(500, 400)).SetOffset(new Vector2(0, 0));

            prev_hp_ = HP;
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            // ステートの更新
            state_machine_.Proc();

            CheckHPChange();
        }

        // HPの減少をチェックする
        private void CheckHPChange()
        {
            int hp_diff = prev_hp_ - HP;
            if (hp_diff == 0) return;
            // こいつにHPを実質持たせないから回復 強引すぎるから後で直す
            HP = 20;

            // ボス管理クラスにダメージを受けさせる
            two_ton_manager_.Damage(hp_diff);
        }


        // 死亡したときに呼ばれる関数 基底クラスから呼ばれる わかりにくい
        protected override void Dead()
        {
            int cur_state = state_machine_.CurrentStateId;
            if (cur_state != (int)eState.Dead)
            {
                state_machine_.ChangeState((int)eState.Dead);
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