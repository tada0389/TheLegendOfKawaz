using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TadaLib;
using Bullet;

/// <summary>
/// ロックマンゼロ 減らくリウスのパクリ
/// </summary>

namespace Actor.Enemy
{
    [RequireComponent(typeof(TadaRigidbody))]
    public class KabtBossController : BaseActorController
    {
        // Bossのステート一覧
        private enum eState
        {
            Think, // 次の行動を考えるステート
            Idle, // 待機中のステート アイドリング
            Fall, // 落下中のステート
            Damage, // ダメージを受けたときのステート
            Dead, // 死亡したときのステート

            Tackle1, // ビートタックル 準備
            Tackle2, // ビートタックル 移動
            Tackle3, // ビートタックル 衝突後
            Anchor, // ビートアンカー
            PlasmaMini, // ビートプラズマ (小)
            PlasmaBig, // ビートプラズマ (大)
        }

        // 向いている方向
        private enum eDir
        {
            Left,
            Right,
        }

        // ステートマシン
        private StateMachine<KabtBossController> state_machine_;

        // ステートのインスタンス
        #region state class
        [SerializeField]
        private StateThink state_think_;
        [SerializeField]
        private StateIdle state_idle_;
        [SerializeField]
        private StateFall state_fall_;
        [SerializeField]
        private StateDamage state_damage_;
        [SerializeField]
        private StateDead state_dead_;
        [SerializeField]
        private StateTackle1 state_tackle1_;
        [SerializeField]
        private StateTackle2 state_tackle2_;
        [SerializeField]
        private StateTackle3 state_tackle3_;
        [SerializeField]
        private StateAnchor state_anchor_;
        [SerializeField]
        private StatePlasmaMini state_plasma_mini_;
        [SerializeField]
        private StatePlasmaBig state_plasma_big_;
        #endregion

        // 物理演算 trb_.Velocityをいじって移動する
        private TadaRigidbody trb_;

        // 現在見ている方向
        private float dir_ = 1f;

        // プレイヤーの座標
        [SerializeField]
        private Transform player_;

        // 死亡時の爆発のエフェクト
        [SerializeField]
        private ParticleSystem explosion_effect_;

        private BulletSpawner bullet_spawner_;
        private bool bullet_inited_;

        private void Start()
        {
            HP = 20;

            trb_ = GetComponent<TadaRigidbody>();
            bullet_spawner_ = GetComponent<BulletSpawner>();
            bullet_inited_ = false;

            // ステートマシンのメモリ確保 自分自身を渡す
            state_machine_ = new StateMachine<KabtBossController>(this);

            // ステートを登録
            state_machine_.AddState((int)eState.Think, state_think_);
            state_machine_.AddState((int)eState.Idle, state_idle_);
            state_machine_.AddState((int)eState.Fall, state_fall_);
            state_machine_.AddState((int)eState.Damage, state_damage_);
            state_machine_.AddState((int)eState.Dead, state_dead_);
            state_machine_.AddState((int)eState.Tackle1, state_tackle1_);
            state_machine_.AddState((int)eState.Tackle2, state_tackle2_);
            state_machine_.AddState((int)eState.Tackle3, state_tackle3_);
            state_machine_.AddState((int)eState.Anchor, state_anchor_);
            state_machine_.AddState((int)eState.PlasmaMini, state_plasma_mini_);
            state_machine_.AddState((int)eState.PlasmaBig, state_plasma_big_);

            // 初期ステートを設定
            state_machine_.SetInitialState((int)eState.Fall);

            // デバッグ表示
            DebugBoxManager.Display(this).SetSize(new Vector2(500, 400)).SetOffset(new Vector2(0, 0));
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
            if (HP == 0)
            {
                explosion_effect_.gameObject.SetActive(true);
                state_machine_.ChangeState((int)eState.Dead);
                Debug.Log("Defeated");
            }
        }

        // このボスにぶつかるとダメージを受ける
        private void OnTriggerStay2D(Collider2D collider)
        {
            if (collider.tag == "Player")
            {
                collider.GetComponent<BaseActorController>().Damage(3);
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
        private class StateThink : StateMachine<KabtBossController>.StateBase
        {
            [SerializeField]
            private float think_time_ = 1.0f;

            // 開始時に呼ばれる
            public override void OnStart()
            {

            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if(Timer > think_time_)
                {
                    DecideNextAction();
                    return;
                }
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }

            // 次の行動を決める
            private void DecideNextAction()
            {
                float value = Random.value;

                if(value < 0.1f)
                {
                    ChangeState((int)eState.Tackle1);
                }
                else
                {
                    ChangeState((int)eState.PlasmaMini);
                }
            }
        }

        // アイドリング状態
        [System.Serializable]
        private class StateIdle : StateMachine<KabtBossController>.StateBase
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
        private class StateFall : StateMachine<KabtBossController>.StateBase
        {
            // 開始時に呼ばれる
            public override void OnStart()
            {

            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if (Parent.trb_.ButtomCollide)
                {
                    ChangeState((int)eState.Think);
                    return;
                }

                ActorUtils.ProcSpeed(ref Parent.trb_.Velocity, Accel, MaxAbsSpeed);
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }
        }

        // ダメージを受けたときの状態
        [System.Serializable]
        private class StateDamage : StateMachine<KabtBossController>.StateBase
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

        // 死亡したときのの状態
        [System.Serializable]
        private class StateDead : StateMachine<KabtBossController>.StateBase
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

        // 行動1状態
        [System.Serializable]
        private class StateTackle1 : StateMachine<KabtBossController>.StateBase
        {
            [SerializeField]
            private float jump_time_ = 0.5f;

            // 開始時に呼ばれる
            public override void OnStart()
            {
                // 速度をリセット
                Parent.trb_.Velocity = Vector2.zero;

                // 敵の方向をむく
                float dir = Mathf.Sign(Parent.player_.position.x - Parent.transform.position.x);
                Parent.SetDirection((dir < 0f) ? eDir.Left : eDir.Right);
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if(Timer > jump_time_)
                {
                    ChangeState((int)eState.Tackle2);
                    return;
                }

                ActorUtils.ProcSpeed(ref Parent.trb_.Velocity, new Vector2(Parent.dir_, 1f) * Accel, MaxAbsSpeed);
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }
        }
        // 行動1状態
        [System.Serializable]
        private class StateTackle2 : StateMachine<KabtBossController>.StateBase
        {
            [SerializeField]
            private float wait_time_ = 0.5f;

            // 開始時に呼ばれる
            public override void OnStart()
            {
                // 速度をリセット
                Parent.trb_.Velocity = Vector2.zero;

                // 敵の方向をむく
                float dir = Mathf.Sign(Parent.player_.position.x - Parent.transform.position.x);
                Parent.SetDirection((dir < 0f) ? eDir.Left : eDir.Right);
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if (Timer < wait_time_) return;

                // 壁にぶつかったら次のステートへ
                if(Parent.trb_.LeftCollide || Parent.trb_.RightCollide)
                {
                    ChangeState((int)eState.Tackle3);
                    return;
                }

                ActorUtils.ProcSpeed(ref Parent.trb_.Velocity, new Vector2(Parent.dir_, 1f) * Accel, MaxAbsSpeed);
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }
        }
        // 行動1状態
        [System.Serializable]
        private class StateTackle3 : StateMachine<KabtBossController>.StateBase
        {
            [SerializeField]
            private Vector2 power_ = new Vector2(-0.2f, 0.2f);

            [SerializeField]
            private float rigidy_time_ = 1.0f;

            // 開始時に呼ばれる
            public override void OnStart()
            {
                // dirの反対側に飛ぶ
                Parent.trb_.Velocity = power_ * new Vector2(Parent.dir_, 1f);
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if(Timer > rigidy_time_)
                {
                    ChangeState((int)eState.Fall);
                    return;
                }

                ActorUtils.ProcSpeed(ref Parent.trb_.Velocity, new Vector2(Parent.dir_, 1f) * Accel, MaxAbsSpeed);
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {
                // 速度をリセット
                Parent.trb_.Velocity = Vector2.zero;
            }
        }


        // 行動2状態
        [System.Serializable]
        private class StateAnchor : StateMachine<KabtBossController>.StateBase
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

        // 行動3状態
        [System.Serializable]
        private class StatePlasmaMini : StateMachine<KabtBossController>.StateBase
        {
            [SerializeField]
            private int shot_num_ = 5;

            [SerializeField]
            private float delay_ = 0.2f;

            [SerializeField]
            private Vector2 shot_offset_ = new Vector2(0.2f, 0.2f);

            private int shot_cnt_;

            // 開始時に呼ばれる
            public override void OnStart()
            {
                shot_cnt_ = 0;

                // その場でとどまる
                Parent.trb_.Velocity = Vector2.zero;

                // 弾のプーリングの初期化
                if (!Parent.bullet_inited_) Parent.bullet_spawner_.Init(shot_num_);
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if(Timer > shot_cnt_ * delay_ + delay_)
                {
                    ++shot_cnt_;
                    // ホーミング弾
                    Vector3 next = Parent.player_.position;
                    Vector3 now = Parent.transform.position;
                    // 目的となる角度を取得する
                    Vector3 d = next - now;
                    float angle = Mathf.Atan2(d.y, d.x);
                    Parent.bullet_spawner_.Shot(new Vector2(Parent.transform.position.x + shot_offset_.x * Parent.dir_, Parent.transform.position.y
                         + shot_offset_.y), new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)), "Player", Parent.player_, 1.0f, 10.0f);

                    if(shot_cnt_ >= shot_num_)
                    {
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

        // 行動4状態
        [System.Serializable]
        private class StatePlasmaBig : StateMachine<KabtBossController>.StateBase
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