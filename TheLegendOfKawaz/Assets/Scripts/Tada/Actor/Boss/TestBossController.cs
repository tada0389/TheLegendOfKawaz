using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TadaLib;
using Bullet;

/// <summary>
/// 全てのボスの基礎 これをコピペしてください
/// ただし，BaseBossController => 〇〇〇BossController と書き換えること
/// ファイル一つで済ませる
/// 詳しくは，テストで作ったThunderBossControllerを見てください
/// </summary>

namespace Actor.Enemy
{
    [RequireComponent(typeof(TadaRigidbody))]
    public class TestBossController : MonoBehaviour
    {
        // Bossのステート一覧
        private enum eState
        {
            Think, // 次の行動を考えるステート
            Idle, // 待機中のステート アイドリング
            Damage, // ダメージを受けたときのステート

            // 以下，任意の行動 それぞれのボスに合わせて実装する
            Shot,
            PreDash,
            Dash,
            DashEnd,
            Action5,
        }

        // 向いている方向
        private enum eDir
        {
            Left,
            Right,
        }

        // ステートマシン
        private StateMachine<TestBossController> state_machine_;

        // ステートのインスタンス
        #region state class
        [SerializeField]
        private StateThink state_think_;
        [SerializeField]
        private StateIdle state_idle_;
        [SerializeField]
        private StateDamage state_damage_;
        [SerializeField]
        private StateAction1 state_action1_;
        [SerializeField]
        private StateAction2 state_action2_;
        [SerializeField]
        private StateAction3 state_action3_;
        [SerializeField]
        private StateAction4 state_action4_;
        [SerializeField]
        private StateAction5 state_action5_;
        #endregion

        // 物理演算 trb_.Velocityをいじって移動する
        private TadaRigidbody trb_;

        // 現在見ている方向
        private float dir_ = 1f;

        // HP
        public int HP { private set; get; }

        // プレイヤーの座標
        [SerializeField]
        private Transform player_;

        private BulletSpawner bullet_spawner_;

        private void Start()
        {
            HP = 20;
            trb_ = GetComponent<TadaRigidbody>();
            bullet_spawner_ = GetComponent<BulletSpawner>();

            // ステートマシンのメモリ確保 自分自身を渡す
            state_machine_ = new StateMachine<TestBossController>(this);

            // ステートを登録
            state_machine_.AddState((int)eState.Think, state_think_);
            state_machine_.AddState((int)eState.Idle, state_idle_);
            state_machine_.AddState((int)eState.Damage, state_damage_);
            state_machine_.AddState((int)eState.Shot, state_action1_);
            state_machine_.AddState((int)eState.PreDash, state_action2_);
            state_machine_.AddState((int)eState.Dash, state_action3_);
            state_machine_.AddState((int)eState.DashEnd, state_action4_);
            state_machine_.AddState((int)eState.Action5, state_action5_);

            // 初期ステートを設定
            state_machine_.SetInitialState((int)eState.Think);

            // デバッグ表示
            DebugBoxManager.Display(this).SetSize(new Vector2(500, 400)).SetOffset(new Vector2(0, -300));
        }

        private void Update()
        {
            state_machine_.Proc();
        }

        // 向いている方向を変更する
        private void SetDirection(eDir dir)
        {
            // 浮動小数点型で==はあんまよくないけど・・・
            if (dir == eDir.Left && transform.localEulerAngles.y != 180f)
            {
                dir_ = -1f;
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 180f, transform.localEulerAngles.z);
            }
            // else if(data_.velocity.x > 0f)
            else if (dir == eDir.Right && transform.localEulerAngles.y != 0f)
            {
                dir_ = 1f;
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0f, transform.localEulerAngles.z);
            }
        }

        public override string ToString()
        {
            return "(" + trb_.Velocity.x.ToString("F2") + ", " + trb_.Velocity.y.ToString("F2") + ")" +
                "\nState : " + state_machine_.ToString();
        }


        // ===== 以下，ステートクラス =============================================================================

        // 説明
        // OnStart() ： ステートが切り替わったタイミングで呼ばれる
        // OnProc() ： 毎フレーム呼ばれる
        // OnEnd() ： ステートが終了するタイミングで呼ばれる

        // ChangeState(int key) ： ステートを変更するときに使う
        // Timer ： ステートが開始してからの経過時間を取得できる
        // Parent ： ボスの本体クラスにアクセスできる (private でも)
        //           例えば，Parent.trb_.Velocityをいじることで，移動ができる

        // Inspector について
        // [SerializeField] をつけて変数宣言すると，Inspectorに表示される
        // デフォルトで，加速度と移動速度の最大値などをInspectorでいじれる

        // ========================================================================================================

        // 次の行動を思考するステート
        [System.Serializable]
        private class StateThink : StateMachine<TestBossController>.StateBase
        {
            [SerializeField]
            private float think_time_ = 1.0f;

            // 開始時に呼ばれる
            public override void OnStart()
            {
                Parent.trb_.Velocity = Vector2.zero;
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if(Timer > think_time_)
                {
                    if (Random.value > 0.75f) ChangeState((int)eState.Shot);
                    else ChangeState((int)eState.PreDash);
                    return;
                }
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }
        }

        // アイドリング状態
        [System.Serializable]
        private class StateIdle : StateMachine<TestBossController>.StateBase
        {
            // 開始時に呼ばれる
            public override void OnStart()
            {

            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {

            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }
        }

        // ダメージを受けたときの状態
        [System.Serializable]
        private class StateDamage : StateMachine<TestBossController>.StateBase
        {
            // 開始時に呼ばれる
            public override void OnStart()
            {

            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {

            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }
        }

        // 行動1状態 弾を撃つ
        [System.Serializable]
        private class StateAction1 : StateMachine<TestBossController>.StateBase
        {
            // 撃つ量
            [SerializeField]
            private int shot_num_ = 5;

            [SerializeField]
            private float shot_scale_ = 1.0f;

            [SerializeField]
            private float shot_interval_ = 0.2f;

            [SerializeField]
            private int trial_num_ = 3;

            [SerializeField]
            private float delay_time_ = 0.5f;

            private int shot_cnt_ = 0;
            private int trial_cnt_ = 0;

            bool end_ = false;


            [SerializeField]
            private Vector2 shot_offset_ = new Vector2(1.0f, 0.0f);

            // 開始時に呼ばれる
            public override void OnStart()
            {
                Parent.bullet_spawner_.Init(shot_num_);
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if(!end_ && Timer > (shot_cnt_ + 1) * shot_interval_)
                {
                    ++shot_cnt_;
                    float dir =  Mathf.Sign(Parent.player_.position.x - Parent.transform.position.x);
                    Parent.SetDirection((dir < 0f) ? eDir.Left : eDir.Right);
                    Parent.bullet_spawner_.Shot(new Vector2(Parent.transform.position.x + shot_offset_.x * dir, Parent.transform.position.y 
                        + shot_offset_.y * ((shot_cnt_ % 2 == 0)? -1f : 1f)),
                        new Vector2(dir, 0f));

                    if(shot_cnt_ >= shot_num_)
                    {
                        shot_cnt_ = 0;
                        if(trial_cnt_ >= trial_num_)
                        {
                            trial_cnt_ = 0;
                            ChangeState((int)eState.Think);
                            return;
                        }
                        else
                        {
                            end_ = true;
                            ++trial_cnt_;
                        }
                    }
                }

                if(Timer > shot_num_ * shot_interval_ + delay_time_)
                {
                    ChangeState((int)eState.Shot);
                    return;
                }
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {
                end_ = false;
            }
        }

        // ダッシュの予備動作 少し後ろに下がる
        [System.Serializable]
        private class StateAction2 : StateMachine<TestBossController>.StateBase
        {
            [SerializeField]
            private float charge_time_ = 1.0f;

            // 開始時に呼ばれる
            public override void OnStart()
            {
                // 敵の方向を見る
                float dir= Mathf.Sign(Parent.player_.position.x - Parent.transform.position.x);
                Parent.SetDirection((dir < 0f) ? eDir.Left : eDir.Right);
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if (Timer > charge_time_) ChangeState((int)eState.Dash);

                ActorUtils.ProcSpeed(ref Parent.trb_.Velocity, new Vector2(-Parent.dir_, 1f) * Accel, MaxAbsSpeed);
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }
        }

        // ダッシュする
        [System.Serializable]
        private class StateAction3 : StateMachine<TestBossController>.StateBase
        {
            [SerializeField]
            private float dash_time_ = 2.0f;

            // 開始時に呼ばれる
            public override void OnStart()
            {
                Parent.trb_.Velocity = Vector2.zero;
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if (Timer > dash_time_) ChangeState((int)eState.Think);

                ActorUtils.ProcSpeed(ref Parent.trb_.Velocity, new Vector2(Parent.dir_, 1f) * Accel, MaxAbsSpeed);
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }
        }

        // 行動4状態
        [System.Serializable]
        private class StateAction4 : StateMachine<TestBossController>.StateBase
        {
            // 開始時に呼ばれる
            public override void OnStart()
            {

            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {

            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }
        }

        // 行動5状態
        [System.Serializable]
        private class StateAction5 : StateMachine<TestBossController>.StateBase
        {
            // 開始時に呼ばれる
            public override void OnStart()
            {

            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {

            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }
        }
    }
} // namespace Actor.Enemy