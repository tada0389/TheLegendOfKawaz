using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TadaLib;
using KoitanLib;
using Bullet;
using CameraSpace;

/// <summary>
/// ロックマンゼロ 減らくリウスのパクリ
/// </summary>

namespace Actor.Enemy
{
    [RequireComponent(typeof(TadaRigidbody))]
    public class KabtBossController : BaseBossController
    {
        // Bossのステート一覧
        private enum eState
        {
            Talk, // 戦闘前の会話
            WinTalk,
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

        // ステートマシン
        private StateMachine<KabtBossController> state_machine_;

        // ステートのインスタンス
        #region state class
        [SerializeField]
        private TalkState state_talk_;
        [SerializeField]
        private WinTalkState state_win_talk_;
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

        private void Start()
        {
            HP = 32;

            trb_ = GetComponent<TadaRigidbody>();
            bullet_spawner_ = GetComponent<BulletSpawner>();

            // ステートマシンのメモリ確保 自分自身を渡す
            state_machine_ = new StateMachine<KabtBossController>(this);

            // ステートを登録
            state_machine_.AddState((int)eState.Talk, state_talk_);
            state_machine_.AddState((int)eState.WinTalk, state_win_talk_);
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
            state_machine_.SetInitialState((int)eState.Talk);

            // デバッグ表示
            DebugBoxManager.Display(this).SetSize(new Vector2(500, 400)).SetOffset(new Vector2(0, 0));
        }

        private void Update()
        {
            // 即死コマンド
            //if (UnityEngine.InputSystem.Keyboard.current[UnityEngine.InputSystem.Key.K].wasPressedThisFrame) state_machine_.ChangeState((int)eState.Dead);

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

        // 最初の会話
        [System.Serializable]
        private class TalkState : StateMachine<KabtBossController>.StateBase
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
                Global.GlobalPlayerInfo.ActionEnabled = true;
            }

            private void EndSeq()
            {
                ChangeState((int)eState.Think);
            }

        }

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

            [SerializeField]
            private float new_time_scale_ = 0.45f;
            [SerializeField]
            private float time_change_duration_ = 1.5f;
            [SerializeField]
            private float talk_wait_time_ = 2.0f;

            // 死亡時のセリフ
            [SerializeField]
            Sprite im;
            [SerializeField, Multiline(3)]
            private string[] message;
            private int index = 0;
            private bool talk_end_;

            private bool open_window_ = false;

            private float talk_end_timer_ = 0.0f;
            [SerializeField]
            private float scene_transition_time_ = 4.0f;

            // ステートの初期化
            public override void OnInit()
            {

            }

            // 開始時に呼ばれる
            public override void OnStart()
            {
                Parent.trb_.Velocity = Vector2.zero;
                // 時間をゆっくりにする
                TadaLib.TimeScaler.Instance.RequestChange(new_time_scale_, time_change_duration_);
                explosion_effect_.gameObject.SetActive(true);
                Global.GlobalPlayerInfo.IsMuteki = true;
                Global.GlobalPlayerInfo.BossDefeated = true;

                // ボスが死んだ回数を加算する
                Global.GlobalDataManager.AddBossDefeatCnt();
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                // セリフを出してすぐにスキップされるのを防ぐ (ジャンプとかダッシュキーを押すと進んでしまうため)
                if (Timer > talk_wait_time_ * 2f / 3f)
                {
                    if (!open_window_)
                    {
                        MessageManager.OpenMessageWindow(message[0], im);
                        open_window_ = true;
                    }
                }

                // 死亡アニメーションが終わるまでまつ
                if (Timer < talk_wait_time_) return;

                if (!talk_end_)
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

                    return;
                }

                talk_end_timer_ += Time.deltaTime;

                if (talk_end_timer_ > scene_transition_time_)
                {
                    KoitanLib.FadeManager.FadeIn(0.5f, "ZakkyScene");
                    talk_end_timer_ = -10000f;
                }
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }

            private void EndSeq()
            {
                talk_end_ = true;
                MessageManager.CloseMessageWindow();

                Actor.Player.SkillManager.Instance.GainSkillPoint(1000, Parent.transform.position, 0.7f);
                //実績解除
                AchievementManager.FireAchievement("Kabt");
                if (Parent.player_.GetComponent<Actor.Player.Player>().IsNoDamage())
                    AchievementManager.FireAchievement("Kabt_nodamage");
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
                if((Parent.trb_.LeftCollide && Parent.dir_ < 0f) || (Parent.trb_.RightCollide && Parent.dir_ > 0f))
                {
                    // カメラを揺らす
                    CameraShaker.Shake(0.6f, 0.3f);
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

            [SerializeField]
            private BaseParticle rush_eff_;

            [SerializeField]
            private BaseParticle wall_hit_eff_;

            private bool rush_inited_;

            // 開始時に呼ばれる
            public override void OnStart()
            {
                Parent.trb_.Velocity *= init_speed_;

                add_speed_ = Parent.trb_.Velocity * accel_;

                // 飛ぶ方向を向く
                Parent.SetDirection((Parent.trb_.Velocity.x < 0f) ? eDir.Left : eDir.Right);

                rush_inited_ = false;
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if (!rush_inited_)
                {
                    rush_inited_ = true;
                    EffectPlayer.Play(rush_eff_, Parent.transform.position + new Vector3(Parent.dir_ * 2.0f, 0f, 0f), new Vector3(0f, 0f, Parent.dir_), Parent.not_reverse_);
                }

                if (Parent.trb_.LeftCollide || Parent.trb_.RightCollide || Parent.trb_.TopCollide)
                {
                    // カメラを揺らす
                    CameraShaker.Shake(0.6f, 0.3f);
                    EffectPlayer.Play(wall_hit_eff_, Parent.transform.position + new Vector3(Parent.dir_, 0f, 0f), new Vector3(Parent.dir_, 0f, 0f));
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

        // プレイヤー撃墜後のセリフ
        [System.Serializable]
        private class WinTalkState : StateMachine<KabtBossController>.StateBase
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
                Parent.trb_.Velocity = Vector2.zero;
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
} // namespace Actor.Enemy