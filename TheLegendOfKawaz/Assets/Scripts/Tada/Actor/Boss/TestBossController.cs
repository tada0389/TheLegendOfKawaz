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
    public class TestBossController : BaseActorController
    {
        // Bossのステート一覧
        private enum eState
        {
            Think, // 次の行動を考えるステート
            Idle, // 待機中のステート アイドリング
            Damage, // ダメージを受けたときのステート
            Fall,

            // 以下，任意の行動 それぞれのボスに合わせて実装する
            Shot,
            PreDash,
            Dash,
            Jump,
            SuperShot,
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
        private StateFall state_fall_;
        [SerializeField]
        private StateShot state_shot_;
        [SerializeField]
        private StatePreDash state_predash_;
        [SerializeField]
        private StateDash state_dash_;
        [SerializeField]
        private StateJump state_jump_;
        [SerializeField]
        private StateSuperShot state_supershot_;
        #endregion

        // 物理演算 trb_.Velocityをいじって移動する
        private TadaRigidbody trb_;

        // 現在見ている方向
        private float dir_ = 1f;

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
            state_machine_.AddState((int)eState.Fall, state_fall_);
            state_machine_.AddState((int)eState.Shot, state_shot_);
            state_machine_.AddState((int)eState.PreDash, state_predash_);
            state_machine_.AddState((int)eState.Dash, state_dash_);
            state_machine_.AddState((int)eState.Jump, state_jump_);
            state_machine_.AddState((int)eState.SuperShot, state_supershot_);

            // 初期ステートを設定
            state_machine_.SetInitialState((int)eState.Fall);

            // デバッグ表示
            DebugBoxManager.Display(this).SetSize(new Vector2(500, 400)).SetOffset(new Vector2(0, 00));
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

        // ダメージを受ける
        public override void Damage(int damage)
        {
            HP = Mathf.Max(0, HP - damage);
            if (HP == 0) Debug.Log("Defeated");
        }

        // このボスにぶつかるとダメージを受ける
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.tag == "Player")
            {
                collider.GetComponent<BaseActorController>().Damage(3);
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
                    float value = Random.value;
                    if (value > 0.66f) ChangeState((int)eState.Shot);
                    else if (value > 0.33f) ChangeState((int)eState.PreDash);
                    else ChangeState((int)eState.Jump);
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

        // 落下状態
        [System.Serializable]
        private class StateFall : StateMachine<TestBossController>.StateBase
        {
            // 開始時に呼ばれる
            public override void OnStart()
            {

            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                // 地面に接地したら次のステートへ
                if (Parent.trb_.ButtomCollide)
                {
                    ChangeState((int)eState.Think);
                }

                ActorUtils.ProcSpeed(ref Parent.trb_.Velocity, new Vector2(Parent.dir_, 1f) * Accel, MaxAbsSpeed);
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }
        }

        // 行動1状態 弾を撃つ
        [System.Serializable]
        private class StateShot : StateMachine<TestBossController>.StateBase
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
                        new Vector2(dir, 0f), "Player");

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
        private class StatePreDash : StateMachine<TestBossController>.StateBase
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
        private class StateDash : StateMachine<TestBossController>.StateBase
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
                if (Timer > dash_time_)
                {
                    ChangeState((int)eState.Think);
                    return;
                }
                ActorUtils.ProcSpeed(ref Parent.trb_.Velocity, new Vector2(Parent.dir_, 1f) * Accel, MaxAbsSpeed);
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }
        }

        // 空中に浮遊して上昇するステート 左右に揺れながら上昇する
        [System.Serializable]
        private class StateJump : StateMachine<TestBossController>.StateBase
        {
            // 左右に揺れる周期
            [SerializeField]
            private float period_ = 0.5f;

            // 上昇する時間
            [SerializeField]
            private float duration_ = 2.0f;

            // 開始時に呼ばれる
            public override void OnStart()
            {
                // 敵の方向を見る
                float dir = Mathf.Sign(Parent.player_.position.x - Parent.transform.position.x);
                Parent.SetDirection((dir < 0f) ? eDir.Left : eDir.Right);

                // 速度をゼロにする
                Parent.trb_.Velocity = Vector2.zero;
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if(Timer > duration_)
                {
                    ChangeState((int)eState.SuperShot);
                    return;
                }

                float dir = Mathf.Sign(Mathf.Cos(Timer / period_ * Mathf.PI));
                // 上向きの正弦波
                ActorUtils.ProcSpeed(ref Parent.trb_.Velocity, new Vector2(dir, 1f) * Accel, MaxAbsSpeed);
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {
                Parent.trb_.Velocity = Vector2.zero;
            }
        }

        // 周囲に何発も弾を発射するステート
        [System.Serializable]
        private class StateSuperShot : StateMachine<TestBossController>.StateBase
        {
            [SerializeField]
            private float charge_time_ = 2.0f;

            [SerializeField]
            private float rot_interval_ = 30f;

            [SerializeField]
            private float delay_time_ = 0.25f;

            [SerializeField]
            private int shot_num_ = 5;

            private int shot_cnt_ = 0;
            private int num_;

            // 開始時に呼ばれる
            public override void OnStart()
            {
                num_ = (int)(360f / rot_interval_ + 0.5f);
                Parent.bullet_spawner_.Init(num_ * 5);
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if (Timer < charge_time_) return;

                if (Timer - charge_time_ > (shot_cnt_ + 1) * delay_time_)
                {
                    ++shot_cnt_;
                    for (int i = 0; i < num_; ++i)
                    {
                        float each_dir = i * rot_interval_;
                        if (shot_cnt_ % 2 == 0) each_dir += rot_interval_ / 2f;
                        Vector2 dir = new Vector2(Mathf.Cos(each_dir * Mathf.Deg2Rad), Mathf.Sin(each_dir * Mathf.Deg2Rad));
                        Parent.bullet_spawner_.Shot(Parent.transform.position, dir, "Player");
                    }

                    if (shot_cnt_ >= shot_num_)
                    {
                        shot_cnt_ = 0;
                        ChangeState((int)eState.Fall);
                        return;
                    }
                }
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }
        }
    }
} // namespace Actor.Enemy