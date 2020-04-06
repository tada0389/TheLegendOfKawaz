using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TadaLib;
using KoitanLib;
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
            Anchor1, // ビートアンカー 準備
            Anchor2, // ビートアンカー 移動
            PlasmaMini, // ビートプラズマ (小)
            PlasmaBig1, // ビートプラズマ (大) 準備
            PlasmaBig2, // ビートプラズマ (大) 発射
            Jump, // ジャンプでプレイヤーの方向に飛んでいく
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
        private StateAnchor1 state_anchor1_;
        [SerializeField]
        private StateAnchor2 state_anchor2_;
        [SerializeField]
        private StatePlasmaMini state_plasma_mini_;
        [SerializeField]
        private StatePlasmaBig1 state_plasma_big1_;
        [SerializeField]
        private StatePlasmaBig2 state_plasma_big2_;
        [SerializeField]
        private StateJump state_jump_;
        #endregion

        private BulletSpawner bullet_spawner_;

        // 物理演算 trb_.Velocityをいじって移動する
        private TadaRigidbody trb_;

        // 現在見ている方向
        private float dir_ = 1f;

        // プレイヤーの座標
        [SerializeField]
        private Transform player_;

        [SerializeField]
        private float muteki_time_ = 1.0f;
        private Timer muteki_timer_;
        [SerializeField]
        private GameObject mesh_;

        // タックルする方向
        private float tackle_angle_;

        [SerializeField]
        private Transform not_reverse_;

        private void Start()
        {
            muteki_timer_ = new Timer(muteki_time_);

            HP = 32;

            trb_ = GetComponent<TadaRigidbody>();
            bullet_spawner_ = GetComponent<BulletSpawner>();

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
            state_machine_.AddState((int)eState.Anchor1, state_anchor1_);
            state_machine_.AddState((int)eState.Anchor2, state_anchor2_);
            state_machine_.AddState((int)eState.PlasmaMini, state_plasma_mini_);
            state_machine_.AddState((int)eState.PlasmaBig1, state_plasma_big1_);
            state_machine_.AddState((int)eState.PlasmaBig2, state_plasma_big2_);
            state_machine_.AddState((int)eState.Jump, state_jump_);

            // 初期ステートを設定
            state_machine_.SetInitialState((int)eState.Fall);

            // デバッグ表示
            DebugBoxManager.Display(this).SetSize(new Vector2(500, 400)).SetOffset(new Vector2(0, 0));
        }

        private void Update()
        {
            // 即死コマンド
            if (UnityEngine.InputSystem.Keyboard.current[UnityEngine.InputSystem.Key.K].wasPressedThisFrame) state_machine_.ChangeState((int)eState.Dead);

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
                if (not_reverse_) not_reverse_.localEulerAngles = new Vector3(not_reverse_.localEulerAngles.x, 180f, not_reverse_.localEulerAngles.z);
            }
            // else if(data_.velocity.x > 0f)
            else if (dir == eDir.Right && transform.localEulerAngles.y != 0f)
            {
                dir_ = 1f;
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0f, transform.localEulerAngles.z);
                if (not_reverse_) not_reverse_.localEulerAngles = new Vector3(not_reverse_.localEulerAngles.x, 0f, not_reverse_.localEulerAngles.z);
            }
        }

        // ダメージを受ける
        public override void Damage(int damage)
        {
            if (!muteki_timer_.IsTimeout()) return;
            HP = Mathf.Max(0, HP - damage);
            muteki_timer_.TimeReset();
            StartCoroutine(Tenmetu());
            if (HP == 0)
            {
                state_machine_.ChangeState((int)eState.Dead);
                Debug.Log("Defeated");
            }
        }

        // このボスにぶつかるとダメージを受ける
        private void OnTriggerStay2D(Collider2D collider)
        {
            if (collider.tag == "Player")
            {
                collider.GetComponent<BaseActorController>().Damage(4);
            }
        }

        //点滅
        private IEnumerator Tenmetu()
        {
            if (muteki_timer_.GetTime() < muteki_time_ / 2f)
            {
                mesh_.SetActive(false);
                yield return new WaitForEndOfFrame();
                mesh_.SetActive(true);
                yield return new WaitForEndOfFrame();
            }
            else
            {
                mesh_.SetActive(false);
                yield return new WaitForSeconds(0.05f);
                mesh_.SetActive(true);
                yield return new WaitForSeconds(0.05f);
            }
            if (!muteki_timer_.IsTimeout())
            {
                StartCoroutine(Tenmetu());
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

            // 驚くぐらい強引
            [SerializeField]
            private float stage_left_x = 0f;
            [SerializeField]
            private float stage_right_x = 7f;
            [SerializeField]
            private float anchor_min_length = 4f;

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
                // 敵との距離
                float distance = (Parent.player_.position - Parent.transform.position).magnitude;

                float value = Random.value;

                // 敵と遠いかったらホーミング弾
                // 敵と近かったらアンカーショット

                if (value < 0.23f)
                {
                    ChangeState((int)eState.Tackle1);
                }
                else if (value < 0.46f)
                {
                    ChangeState((int)eState.Jump);
                }
                else if (value < 0.69f)
                {
                    ChangeState((int)eState.PlasmaBig1);
                }
                else
                {
                    if (CanAnchorShot()) ChangeState((int)eState.Anchor1);
                    else ChangeState((int)eState.PlasmaMini);
                }
            }

            private bool CanAnchorShot()
            {
                // プレイヤーの方向
                float dir = Mathf.Sign(Parent.player_.position.x - Parent.transform.position.x);
                if(dir < 0f)
                {
                    float dist_to_wall = Parent.transform.position.x - stage_left_x;
                    return (dist_to_wall < anchor_min_length);
                }
                else
                {
                    float dist_to_wall = stage_right_x - Parent.transform.position.x;
                    return (dist_to_wall < anchor_min_length);
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
            // 死亡時の爆発のエフェクト
            [SerializeField]
            private ParticleSystem explosion_effect_;

            private bool a = false;

            // 開始時に呼ばれる
            public override void OnStart()
            {
                Parent.trb_.Velocity = Vector2.zero;
                explosion_effect_.gameObject.SetActive(true);
                if(!a) Time.timeScale = 0.3f;
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if(Timer > 2.0f && !a)
                {
                    Time.timeScale = 1.0f;
                    a = true;
                    Actor.Player.SkillManager.Instance.GainSkillPoint(1000, Parent.transform.position);
                }
                if(Timer > 6.0f)
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene("ZakkyScene");
                }
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }
        }

        // 少し宙に浮いて突っ込む準備をする
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
        // 突っ込む
        [System.Serializable]
        private class StateTackle2 : StateMachine<KabtBossController>.StateBase
        {
            [SerializeField]
            private float wait_time_ = 0.5f;

            [SerializeField]
            private BaseParticle rush_eff_;

            [SerializeField]
            private BaseParticle wall_hit_eff_;

            private bool rush_inited_;

            // 開始時に呼ばれる
            public override void OnStart()
            {
                // 速度をリセット
                Parent.trb_.Velocity = Vector2.zero;

                // 敵の方向をむく
                float dir = Mathf.Sign(Parent.player_.position.x - Parent.transform.position.x);
                Parent.SetDirection((dir < 0f) ? eDir.Left : eDir.Right);

                rush_inited_ = false;
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if (Timer < wait_time_) return;

                if (!rush_inited_)
                {
                    rush_inited_ = true;
                    EffectPlayer.Play(rush_eff_, Parent.transform.position + new Vector3(Parent.dir_ * 2.0f, 0f, 0f), new Vector3(0f, 0f, Parent.dir_), Parent.not_reverse_);
                }

                // 壁にぶつかったら次のステートへ
                if(Parent.trb_.LeftCollide || Parent.trb_.RightCollide)
                {
                    EffectPlayer.Play(wall_hit_eff_, Parent.transform.position + new Vector3(Parent.dir_, 0f, 0f), new Vector3(Parent.dir_, 0f, 0f));
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
        // 突っ込んだ反動で後退する
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
                if(Timer > rigidy_time_ && Parent.trb_.ButtomCollide)
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


        // 敵の方向にアンカーを飛ばす
        [System.Serializable]
        private class StateAnchor1 : StateMachine<KabtBossController>.StateBase
        {
            [SerializeField]
            private float delay_ = 1.5f;

            // 発火するまでの時間
            [SerializeField]
            private float flush_time_ = 0.5f;
            private int flush_cnt_;

            // 飛ばした後の待ち時間
            [SerializeField]
            private float rigity_time_ = 0.5f;

            private int shot_cnt_;

            [SerializeField]
            private List<AnchorController> anchors_;

            private Vector2 target_dir_;

            // 開始時に呼ばれる
            public override void OnStart()
            {
                shot_cnt_ = 0;
                flush_cnt_ = 0;
                target_dir_ = Vector2.zero;

                // その場でとどまる
                Parent.trb_.Velocity = Vector2.zero;
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                // アンカーを飛ばす
                if (Timer > shot_cnt_ * delay_ + delay_ && shot_cnt_ != 2)
                {
                    ShotAnchor();
                    ++shot_cnt_;
                }

                // 飛ばしたアンカーの軌道を発光させる
                if (flush_cnt_ < shot_cnt_ && Timer > flush_cnt_ * delay_ + delay_ + flush_time_)
                {
                    anchors_[flush_cnt_].Flush();
                    anchors_[flush_cnt_].Stop();
                    ++flush_cnt_;
                }

                // 終了
                if (Timer > 2f * delay_ + flush_time_ + rigity_time_)
                {
                    // 飛ばす方向をあらかじめ与えておく しかたない
                    Parent.trb_.Velocity = target_dir_.normalized;
                    ChangeState((int)eState.Anchor2);
                }
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {
                anchors_[0].Finish();
                anchors_[1].Finish();
            }

            // アンカーを飛ばす
            private void ShotAnchor()
            {
                // 敵の方向をむく 1回目だけ
                if (target_dir_.magnitude < 0.01f)
                {
                    float dir = Mathf.Sign(Parent.player_.position.x - Parent.transform.position.x);
                    Parent.SetDirection((dir < 0f) ? eDir.Left : eDir.Right);
                }

                Vector3 next = Parent.player_.position;
                Vector3 now = Parent.transform.position;

                // 目的となる角度を取得する
                Vector3 d = next - now;
                float angle = Mathf.Atan2(d.y, d.x);
                Vector2 dir_vec = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                // 前回の方向と同じ方向にする
                if(target_dir_.magnitude > 0.01f && Mathf.Sign(dir_vec.x) != Mathf.Sign(target_dir_.x))
                {
                    dir_vec = Vector2.up;
                }
                target_dir_ += dir_vec / 2f;
                anchors_[shot_cnt_].Init(Parent.transform.position,
                    dir_vec, 3, "Player", Parent.player_);
            }
        }

        // 飛ばした2つのアンカーの真ん中へ突っ込む
        [System.Serializable]
        private class StateAnchor2 : StateMachine<KabtBossController>.StateBase
        {
            [SerializeField]
            private float init_speed_ = 0.4f;

            [SerializeField]
            private float accel_ = 0.02f;

            private Vector2 add_speed_;

            // 開始時に呼ばれる
            public override void OnStart()
            {
                Parent.trb_.Velocity *= init_speed_;

                add_speed_ = Parent.trb_.Velocity * accel_;

                // 飛ぶ方向を向く
                Parent.SetDirection((Parent.trb_.Velocity.x < 0f) ? eDir.Left : eDir.Right);
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if(Parent.trb_.LeftCollide || Parent.trb_.RightCollide || Parent.trb_.TopCollide)
                {
                    // 使いまわし
                    ChangeState((int)eState.Tackle3);
                    return;
                }

                ActorUtils.ProcSpeed(ref Parent.trb_.Velocity, add_speed_, MaxAbsSpeed);
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }
        }

        // ホーミング弾を放つ
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

            [SerializeField]
            private BaseBulletController bullet_;

            // 開始時に呼ばれる
            public override void OnStart()
            {
                shot_cnt_ = 0;

                // その場でとどまる
                Parent.trb_.Velocity = Vector2.zero;

                // 敵の方向をむく
                float dir = Mathf.Sign(Parent.player_.position.x - Parent.transform.position.x);
                Parent.SetDirection((dir < 0f) ? eDir.Left : eDir.Right);
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if(Timer > shot_cnt_ * delay_ + delay_)
                {
                    ++shot_cnt_;
                    Vector3 next = Parent.player_.position;
                    Vector3 now = Parent.transform.position;
                    // 目的となる角度を取得する
                    Vector3 d = next - now;
                    float angle = Mathf.Atan2(d.y, d.x);
                    Parent.bullet_spawner_.Shot(bullet_, new Vector2(Parent.transform.position.x + shot_offset_.x * Parent.dir_, Parent.transform.position.y
                         + shot_offset_.y), new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)), "Player", Parent.transform, 1.0f, 10.0f);

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

        // 画面中央にジャンプして次の攻撃の準備をする
        [System.Serializable]
        private class StatePlasmaBig1 : StateMachine<KabtBossController>.StateBase
        {
            // 移動先
            [SerializeField]
            private Vector3 target_pos_ = Vector2.zero;

            [SerializeField]
            private float move_time_ = 2.0f;


            // 開始時に呼ばれる
            public override void OnStart()
            {
                Vector2 d = target_pos_ - Parent.transform.position;
                float t = move_time_ * 60f;
                Parent.trb_.Velocity.x = d.x / t;
                Parent.trb_.Velocity.y = d.y / t - Accel.y * t / 2f;
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                ActorUtils.ProcSpeed(ref Parent.trb_.Velocity, new Vector2(0f, Accel.y), Vector2.one * 10f);

                if(Timer > move_time_)
                {
                    ChangeState((int)eState.PlasmaBig2);
                    return;
                }
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {
                Parent.trb_.Velocity = Vector2.zero;
            }
        }

        // 4方向にアンカーを飛ばしてホーミング弾を放つ
        [System.Serializable]
        private class StatePlasmaBig2 : StateMachine<KabtBossController>.StateBase
        {
            [SerializeField]
            private List<Vector2> shot_offset_list_;

            [SerializeField]
            private float delay_ = 0.3f;
            [SerializeField]
            private float rigity_time_ = 0.3f;

            [SerializeField]
            private float charge_time_ = 0.3f;

            private int shot_cnt_;
            private bool charged_;

            [SerializeField]
            private List<AnchorController> anchors_;

            [SerializeField]
            private BaseBulletController bullet_;
            private bool bullet_inited_ = false;

            // 開始時に呼ばれる
            public override void OnStart()
            {
                shot_cnt_ = 0;
                charged_ = false;

                // 4方向にアンカーを伸ばす
                for(int i = 0; i < shot_offset_list_.Count; ++i)
                {
                    ShotAnchor(i);
                }
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                // 飛ばしたアンカーの軌道を発光させる
                if(Timer > charge_time_ && !charged_)
                {
                    charged_ = true;
                    for (int i = 0; i < shot_offset_list_.Count; ++i)
                    {
                        // 電撃を発生させる
                        anchors_[i].Flush();
                        anchors_[i].Stop();
                    }
                }

                // 弾を撃つ
                if (Timer > shot_cnt_ * delay_ + delay_ + charge_time_ && shot_cnt_ != shot_offset_list_.Count)
                {
                    Shot();

                    ++shot_cnt_;
                }

                // 終了
                if (Timer > shot_offset_list_.Count * delay_ + charge_time_ + rigity_time_)
                {
                    ChangeState((int)eState.Fall);
                    return;
                }
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {
                for (int i = 0; i < shot_offset_list_.Count; ++i)
                {
                    anchors_[i].Finish();
                }
            }

            // アンカーを飛ばす
            private void ShotAnchor(int index)
            {
                // 目的となる角度を取得する
                Vector3 d = (Vector3)shot_offset_list_[index];
                float angle = Mathf.Atan2(d.y, d.x);

                float init_speed = d.magnitude;
                if (charge_time_ > 0.1f) init_speed *= (0.3f / charge_time_);

                anchors_[index].Init(Parent.transform.position,
                    new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)), 3, "Player", Parent.player_, init_speed * 0.2f);
            }

            // 弾を打つ
            private void Shot()
            {
                Vector3 next = Parent.player_.position;
                Vector3 now = Parent.transform.position;
                // 目的となる角度を取得する
                Vector3 d = next - now;
                float angle = Mathf.Atan2(d.y, d.x);
                Parent.bullet_spawner_.Shot(bullet_, Parent.transform.position + (Vector3)shot_offset_list_[shot_cnt_],
                    new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)), "Player", Parent.transform, 1.0f, 10.0f);
            }
        }

        // ジャンプで飛んでいくの状態
        [System.Serializable]
        private class StateJump : StateMachine<KabtBossController>.StateBase
        {
            [SerializeField]
            private float jump_power_ = 0.3f;

            // どれくらいプレイヤーの先へ進むか
            [SerializeField]
            private float over_dx_ = 0.5f;


            // 開始時に呼ばれる
            public override void OnStart()
            {
                // 敵の方向をむく
                float dir = Mathf.Sign(Parent.player_.position.x - Parent.transform.position.x);
                Parent.SetDirection((dir < 0f) ? eDir.Left : eDir.Right);

                // 上向きに速度を加える
                Parent.trb_.Velocity.y = jump_power_;

                // x軸方向の速度はプレイヤーの位置によって変える
                // 着陸予定時間時間
                float t = 2 * (jump_power_ / Mathf.Abs(Accel.y));
                float dx = Parent.player_.position.x - Parent.transform.position.x + over_dx_ * Parent.dir_;
                Parent.trb_.Velocity.x = dx / t;
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
    }
} // namespace Actor.Enemy