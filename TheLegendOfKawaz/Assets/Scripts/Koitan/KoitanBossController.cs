﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TadaLib;
using Bullet;
using KoitanLib;
using DG.Tweening;

/// <summary>
/// 全てのボスの基礎 これをコピペしてください
/// ただし，BaseBossController => 〇〇〇BossController と書き換えること
/// ファイル一つで済ませる
/// 詳しくは，テストで作ったThunderBossControllerを見てください
/// </summary>

namespace Actor.Enemy
{
    [RequireComponent(typeof(TadaRigidbody))]
    public class KoitanBossController : BaseActorController
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
            Bite,
            DashEnd,
            ShotSwamp,
            Start,
            Fly,
            FlyAttack,
            Dead,
            WinTalk,
        }

        // 向いている方向
        private enum eDir
        {
            Left,
            Right,
        }

        // ステートマシン
        private StateMachine<KoitanBossController> state_machine_;

        // ステートのインスタンス
        #region state class
        [SerializeField]
        private StateThink state_think_;
        [SerializeField]
        private WinTalkState state_win_talk_;
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
        [SerializeField]
        private StateAction6 state_action6_;
        [SerializeField]
        private StateStart state_start_;
        [SerializeField]
        private StateFly state_fly_;
        [SerializeField]
        private StateFlyAttack state_fly_attack_;
        [SerializeField]
        private StateDead state_dead_;
        #endregion

        // 物理演算 trb_.Velocityをいじって移動する
        private TadaRigidbody trb_;

        // 現在見ている方向
        private float dir_ = 1f;

        // プレイヤーの座標
        [SerializeField]
        private Transform player_;

        private BulletSpawner bullet_spawner_;

        //ダメージを受けているかどうか        
        private float mutekiTime = 0;

        //メッシュ
        private GameObject mesh;

        //アニメーター
        private Animator animator;

        private static readonly int hashIdle = Animator.StringToHash("Idle");
        private static readonly int hashIsMove = Animator.StringToHash("isMove");
        private static readonly int hashAttack1 = Animator.StringToHash("Attack1");
        private static readonly int hashAttack2 = Animator.StringToHash("Attack2");
        private static readonly int hashAttack3 = Animator.StringToHash("Attack3");
        private static readonly int hashStart = Animator.StringToHash("Start");
        private static readonly int hashFly = Animator.StringToHash("Fly");
        private static readonly int hashFlying = Animator.StringToHash("Flying");
        private static readonly int hashFlyAttack = Animator.StringToHash("FlyAttack");
        private static readonly int hashDead = Animator.StringToHash("Dead");

        //オブジェクトプール
        [SerializeField]
        BaseBulletController venomBullet;
        [SerializeField]
        BaseBulletController swampBullet;

        //シークエンス
        Sequence seq;


        private void Start()
        {
            trb_ = GetComponent<TadaRigidbody>();
            bullet_spawner_ = GetComponent<BulletSpawner>();
            mesh = transform.GetChild(0).gameObject;//危険!
            animator = GetComponent<Animator>();

            //animator.Play("Idle");


            // ステートマシンのメモリ確保 自分自身を渡す
            state_machine_ = new StateMachine<KoitanBossController>(this);

            // ステートを登録
            state_machine_.AddState((int)eState.Think, state_think_);
            state_machine_.AddState((int)eState.Idle, state_idle_);
            state_machine_.AddState((int)eState.Damage, state_damage_);
            state_machine_.AddState((int)eState.Shot, state_action1_);
            state_machine_.AddState((int)eState.PreDash, state_action2_);
            state_machine_.AddState((int)eState.Dash, state_action3_);
            state_machine_.AddState((int)eState.Bite, state_action4_);
            state_machine_.AddState((int)eState.DashEnd, state_action5_);
            state_machine_.AddState((int)eState.ShotSwamp, state_action6_);
            state_machine_.AddState((int)eState.Start, state_start_);
            state_machine_.AddState((int)eState.Fly, state_fly_);
            state_machine_.AddState((int)eState.FlyAttack, state_fly_attack_);
            state_machine_.AddState((int)eState.Dead, state_dead_);
            state_machine_.AddState((int)eState.WinTalk, state_win_talk_);

            // 初期ステートを設定
            state_machine_.SetInitialState((int)eState.Start);

            //オブジェクトプール
            /*
            ObjectPoolManager.Init(venomBullet, this, 6);
            //ObjectPoolManager.Init(swampBullet, this, 2);
            ObjectPoolManager.Init(shotEff, this, 6);
            ObjectPoolManager.Init(hitEff, this, 6);
            */

            // デバッグ表示
            DebugBoxManager.Display(this).SetSize(new Vector2(500, 400)).SetOffset(new Vector2(0, -100));
        }

        private void FixedUpdate()
        {
            state_machine_.Proc();
            if (mutekiTime > 0)
            {
                mutekiTime -= Time.fixedDeltaTime;
            }

            // 敵を倒していたら勝利のセリフを吐く
            CheckWin();
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
            if (mutekiTime <= 0)
            {
                HP = Mathf.Max(0, HP - damage);
                if (HP == 0) Debug.Log("Defeated");

                DamageDisplayer.eDamageType type = DamageDisplayer.eDamageType.Mini;
                if (damage >= 3) type = DamageDisplayer.eDamageType.Big;
                else if (damage >= 2) type = DamageDisplayer.eDamageType.Normal;
                DamageDisplayer.Instance.ShowDamage(damage, transform.position, type);

                mutekiTime = 1f;
                StartCoroutine(Tenmetu());
                if (HP == 0) Dead();
            }
        }

        // 死亡したときに呼ばれる関数 基底クラスから呼ばれる わかりにくい
        private void Dead()
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
                "\nState : " + state_machine_.ToString();
        }

        //点滅
        private IEnumerator Tenmetu()
        {
            if (mutekiTime < 1f)
            {
                mesh.SetActive(false);
                yield return new WaitForEndOfFrame();
                mesh.SetActive(true);
                yield return new WaitForEndOfFrame();
            }
            else
            {
                mesh.SetActive(false);
                yield return new WaitForSeconds(0.05f);
                mesh.SetActive(true);
                yield return new WaitForSeconds(0.05f);
            }
            if (mutekiTime > 0)
            {
                StartCoroutine(Tenmetu());
            }
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
        private class StateThink : StateMachine<KoitanBossController>.StateBase
        {
            [SerializeField]
            private float think_time_ = 1.0f;

            // 開始時に呼ばれる
            public override void OnStart()
            {
                Parent.trb_.Velocity = Vector2.zero;
                //Parent.animator.Play(hashIdle);
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if (Timer > think_time_)
                {                    
                    float r = Random.Range(0f, 100f);
                    //Debug.Log("random:" + r);
                    if (r < 30f) ChangeState((int)eState.Shot);
                    else if (r < 60f) ChangeState((int)eState.Bite);
                    else if (r < 80f) ChangeState((int)eState.PreDash);
                    else ChangeState((int)eState.Fly);
                    return;

                    //ChangeState((int)eState.Shot);
                }
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }
        }

        // アイドリング状態
        [System.Serializable]
        private class StateIdle : StateMachine<KoitanBossController>.StateBase
        {
            // 開始時に呼ばれる
            public override void OnStart()
            {
                Parent.animator.Play(hashIdle);
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
        private class StateDamage : StateMachine<KoitanBossController>.StateBase
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
        private class StateAction1 : StateMachine<KoitanBossController>.StateBase
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

            //玉の速さ
            [SerializeField]
            private float speed = 3;

            private int shot_cnt_ = 0;
            private int trial_cnt_ = 0;

            bool end_ = false;


            [SerializeField]
            private Vector2 shot_offset_ = new Vector2(1.0f, 0.0f);

            private float dir;

            // 開始時に呼ばれる
            public override void OnStart()
            {
                //Parent.bullet_spawner_.Init(shot_num_ * 3);
                Parent.animator.Play(hashAttack2);
                dir = Mathf.Sign(Parent.player_.position.x - Parent.transform.position.x);
                Parent.SetDirection((dir < 0f) ? eDir.Left : eDir.Right);
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if (!end_ && Timer > (shot_cnt_) * shot_interval_ + delay_time_)
                {
                    ++shot_cnt_;

                    Parent.bullet_spawner_.Shot(Parent.venomBullet, new Vector3(Parent.transform.position.x + shot_offset_.x * dir, Parent.transform.position.y
                        + shot_offset_.y), Quaternion.AngleAxis(dir * 0f, Vector3.back) * new Vector3(dir, 0f) * speed, "Player");
                    Parent.bullet_spawner_.Shot(Parent.venomBullet, new Vector3(Parent.transform.position.x + shot_offset_.x * dir, Parent.transform.position.y
                        + shot_offset_.y), Quaternion.AngleAxis(dir * -30f, Vector3.back) * new Vector3(dir, 0f) * speed, "Player");
                    Parent.bullet_spawner_.Shot(Parent.venomBullet, new Vector3(Parent.transform.position.x + shot_offset_.x * dir, Parent.transform.position.y
                        + shot_offset_.y), Quaternion.AngleAxis(dir * -60f, Vector3.back) * new Vector3(dir, 0f) * speed, "Player");

                    if (shot_cnt_ >= shot_num_)
                    {
                        shot_cnt_ = 0;
                        if (trial_cnt_ >= trial_num_)
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

                if (Timer > shot_num_ * shot_interval_ + delay_time_ * 2)
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
        private class StateAction2 : StateMachine<KoitanBossController>.StateBase
        {
            [SerializeField]
            private float charge_time_ = 1.0f;

            // 開始時に呼ばれる
            public override void OnStart()
            {
                // 敵の方向を見る
                float dir = Mathf.Sign(Parent.player_.position.x - Parent.transform.position.x);
                Parent.SetDirection((dir < 0f) ? eDir.Left : eDir.Right);
                //動き
                Parent.animator.SetBool(hashIsMove, true);
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
        private class StateAction3 : StateMachine<KoitanBossController>.StateBase
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
                Parent.animator.SetBool(hashIsMove, false);
            }
        }

        // かみつきこうげき
        [System.Serializable]
        private class StateAction4 : StateMachine<KoitanBossController>.StateBase
        {
            [SerializeField]
            private float startup_time = 1;
            [SerializeField]
            private float active_time = 1;
            [SerializeField]
            private float recovery_time = 1;

            // 開始時に呼ばれる
            public override void OnStart()
            {
                Parent.animator.Play(hashAttack1);
                float dir = Mathf.Sign(Parent.player_.position.x - Parent.transform.position.x);
                Parent.SetDirection((dir < 0f) ? eDir.Left : eDir.Right);
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if (Timer < startup_time)
                {

                }
                else if (Timer < startup_time + active_time)
                {
                    ActorUtils.ProcSpeed(ref Parent.trb_.Velocity, new Vector2(Parent.dir_, 1f) * Accel, MaxAbsSpeed);
                }
                else if (Timer > startup_time + active_time)
                {
                    ChangeState((int)eState.DashEnd);
                }
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }
        }

        // 止まる
        [System.Serializable]
        private class StateAction5 : StateMachine<KoitanBossController>.StateBase
        {
            [SerializeField]
            private float break_time_ = 1.0f;

            // 開始時に呼ばれる
            public override void OnStart()
            {
                //動き
                Parent.animator.SetBool(hashIsMove, false);
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if (Timer > break_time_) ChangeState((int)eState.Think);

                ActorUtils.ProcSpeed(ref Parent.trb_.Velocity, new Vector2(-Parent.dir_, 1f) * Accel, MaxAbsSpeed);
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }
        }

        // 沼を作る
        [System.Serializable]
        private class StateAction6 : StateMachine<KoitanBossController>.StateBase
        {
            [SerializeField]
            private float startup_time = 1;
            [SerializeField]
            private float active_time = 1;
            [SerializeField]
            private float recovery_time = 1;

            [SerializeField]
            private Vector2 shot_offset_ = new Vector2(1.0f, 0.0f);

            private float dir;

            //玉の速さ
            [SerializeField]
            private float speed = 3;

            private bool isShot;

            // 開始時に呼ばれる
            public override void OnStart()
            {
                Parent.animator.Play(hashAttack3);
                float dir = Mathf.Sign(Parent.player_.position.x - Parent.transform.position.x);
                Parent.SetDirection((dir < 0f) ? eDir.Left : eDir.Right);
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if (Timer < startup_time)
                {

                }
                else if (Timer < startup_time + active_time && !isShot)
                {
                    isShot = true;
                    Parent.bullet_spawner_.Shot(Parent.venomBullet, new Vector3(Parent.transform.position.x + shot_offset_.x * dir, Parent.transform.position.y
                        + shot_offset_.y), Quaternion.AngleAxis(dir * 0f, Vector3.back) * new Vector3(dir, 0f) * speed, "Player");
                    Parent.bullet_spawner_.Shot(Parent.venomBullet, new Vector3(Parent.transform.position.x + shot_offset_.x * dir, Parent.transform.position.y
                        + shot_offset_.y), Quaternion.AngleAxis(dir * -30f, Vector3.back) * new Vector3(dir, 0f) * speed, "Player");
                    Parent.bullet_spawner_.Shot(Parent.venomBullet, new Vector3(Parent.transform.position.x + shot_offset_.x * dir, Parent.transform.position.y
                        + shot_offset_.y), Quaternion.AngleAxis(dir * -60f, Vector3.back) * new Vector3(dir, 0f) * speed, "Player");
                }
                else if (Timer > startup_time + active_time)
                {
                    ChangeState((int)eState.Think);
                }
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }
        }

        // 最初
        [System.Serializable]
        private class StateStart : StateMachine<KoitanBossController>.StateBase
        {
            [SerializeField]
            Sprite im;
            [SerializeField, Multiline(3)]
            private string[] message;
            private int index = 0;
            private bool isEnd;

            [SerializeField]
            private BaseParticle par;
            [SerializeField]
            private Transform parPos;

            [SerializeField]
            private Animator name_display_ui_;

            // 開始時に呼ばれる
            public override void OnStart()
            {
                float dir = Mathf.Sign(Parent.player_.position.x - Parent.transform.position.x);
                Parent.SetDirection((dir < 0f) ? eDir.Left : eDir.Right);
                MessageManager.OpenMessageWindow(message[0], im);
                MessageManager.StartMessageAnimation();
                ObjectPoolManager.Init(par, Parent, 1);
                Global.GlobalPlayerInfo.ActionEnabled = false;
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if (!isEnd)
                {
                    if (ActionInput.GetButtonDown(ActionCode.Decide))
                    {
                        if (MessageManager.isSending())
                        {
                            MessageManager.FinishMessage();
                        }
                        else if (index < message.Length - 1)
                        {
                            index++;
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
                Parent.seq.Kill();
                // ボス名を表示させる
                if (name_display_ui_ != null)
                {
                    name_display_ui_.gameObject.SetActive(true);
                    name_display_ui_.Play("BossText");
                }
                //Parent.animator.Play(hashStart);
            }

            private void EndSeq()
            {
                MessageManager.FinishMessage();
                MessageManager.StopMessageAnimation();
                MessageManager.CloseMessageWindow();
                // プレイヤー動けなくさせる
                Global.GlobalPlayerInfo.ActionEnabled = true;
                Parent.player_.GetComponent<Player.Player>().DoRoarState();
                Parent.seq = DOTween.Sequence()
                    .OnStart(() =>
                    {
                        isEnd = true;
                        Parent.animator.Play(hashStart);
                        EffectPlayer.Play(par, parPos.position, Vector3.zero, parPos);
                    })
                    .AppendInterval(3.0f)
                    .AppendCallback(() =>
                    {
                        ChangeState((int)eState.Think);
                    });
            }
        }

        [System.Serializable]
        private class StateFly : StateMachine<KoitanBossController>.StateBase
        {
            // 開始時に呼ばれる
            public override void OnStart()
            {

                float dir = Mathf.Sign(Parent.player_.position.x - Parent.transform.position.x);
                Parent.SetDirection((dir < 0f) ? eDir.Left : eDir.Right);
                Parent.seq = DOTween.Sequence()
                    .OnStart(() =>
                    {
                        Parent.animator.Play(hashFly);
                    })
                    .AppendInterval(0.6f)
                    .AppendCallback(() =>
                    {
                        //tadaRigidbodyがうまく動かなかった
                        //Parent.trb_.Velocity = new Vector2(0, 5);
                        Parent.transform.DOMoveY(6.5f, 1f).SetRelative();
                        //ChangeState((int)eState.Think);
                    })
                    .AppendInterval(1f)
                    .AppendCallback(() =>
                    {
                        ChangeState((int)eState.FlyAttack);
                    });

            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {
                Parent.seq.Kill();
            }
        }

        [System.Serializable]
        private class StateFlyAttack : StateMachine<KoitanBossController>.StateBase
        {
            //玉の速さ
            [SerializeField]
            private float speed = 3;

            [SerializeField]
            private Transform shotPos;

            float dir;

            // 開始時に呼ばれる
            public override void OnStart()
            {
                dir = Mathf.Sign(Parent.player_.position.x - Parent.transform.position.x);
                Parent.SetDirection((dir < 0f) ? eDir.Left : eDir.Right);
                Parent.seq = DOTween.Sequence()
                    .AppendInterval(1f)
                    .AppendCallback(() =>
                    {
                        Parent.animator.Play(hashFlyAttack);
                    })
                    .AppendInterval(1f)
                    .AppendCallback(() =>
                    {
                        //玉をうつ
                        Parent.bullet_spawner_.Shot(Parent.venomBullet, shotPos.position, new Vector3(dir, 0f) * speed, "Player");
                        Parent.bullet_spawner_.Shot(Parent.venomBullet, shotPos.position, Quaternion.AngleAxis(dir * 30f, Vector3.back) * new Vector3(dir, 0f) * speed, "Player");
                        Parent.bullet_spawner_.Shot(Parent.venomBullet, shotPos.position, Quaternion.AngleAxis(dir * 60f, Vector3.back) * new Vector3(dir, 0f) * speed, "Player");
                        Parent.bullet_spawner_.Shot(Parent.venomBullet, shotPos.position, Quaternion.AngleAxis(dir * 90f, Vector3.back) * new Vector3(dir, 0f) * speed, "Player");
                    })
                    .AppendInterval(0.3f)
                    .AppendCallback(() =>
                    {
                        Parent.bullet_spawner_.Shot(Parent.venomBullet, shotPos.position, Quaternion.AngleAxis(dir * 15f, Vector3.back) * new Vector3(dir, 0f) * speed, "Player");
                        Parent.bullet_spawner_.Shot(Parent.venomBullet, shotPos.position, Quaternion.AngleAxis(dir * 45f, Vector3.back) * new Vector3(dir, 0f) * speed, "Player");
                        Parent.bullet_spawner_.Shot(Parent.venomBullet, shotPos.position, Quaternion.AngleAxis(dir * 75f, Vector3.back) * new Vector3(dir, 0f) * speed, "Player");
                    })
                    .AppendInterval(0.3f)
                    .AppendCallback(() =>
                    {
                        Parent.bullet_spawner_.Shot(Parent.venomBullet, shotPos.position, new Vector3(dir, 0f) * speed, "Player");
                        Parent.bullet_spawner_.Shot(Parent.venomBullet, shotPos.position, Quaternion.AngleAxis(dir * 30f, Vector3.back) * new Vector3(dir, 0f) * speed, "Player");
                        Parent.bullet_spawner_.Shot(Parent.venomBullet, shotPos.position, Quaternion.AngleAxis(dir * 60f, Vector3.back) * new Vector3(dir, 0f) * speed, "Player");
                        Parent.bullet_spawner_.Shot(Parent.venomBullet, shotPos.position, Quaternion.AngleAxis(dir * 90f, Vector3.back) * new Vector3(dir, 0f) * speed, "Player");
                    })
                    .AppendInterval(0.3f)
                    .AppendCallback(() =>
                    {
                        Parent.bullet_spawner_.Shot(Parent.venomBullet, shotPos.position, Quaternion.AngleAxis(dir * 15f, Vector3.back) * new Vector3(dir, 0f) * speed, "Player");
                        Parent.bullet_spawner_.Shot(Parent.venomBullet, shotPos.position, Quaternion.AngleAxis(dir * 45f, Vector3.back) * new Vector3(dir, 0f) * speed, "Player");
                        Parent.bullet_spawner_.Shot(Parent.venomBullet, shotPos.position, Quaternion.AngleAxis(dir * 75f, Vector3.back) * new Vector3(dir, 0f) * speed, "Player");
                    })
                    .AppendInterval(0.3f)
                    .AppendCallback(() =>
                    {
                        Parent.bullet_spawner_.Shot(Parent.venomBullet, shotPos.position, new Vector3(dir, 0f) * speed, "Player");
                        Parent.bullet_spawner_.Shot(Parent.venomBullet, shotPos.position, Quaternion.AngleAxis(dir * 30f, Vector3.back) * new Vector3(dir, 0f) * speed, "Player");
                        Parent.bullet_spawner_.Shot(Parent.venomBullet, shotPos.position, Quaternion.AngleAxis(dir * 60f, Vector3.back) * new Vector3(dir, 0f) * speed, "Player");
                        Parent.bullet_spawner_.Shot(Parent.venomBullet, shotPos.position, Quaternion.AngleAxis(dir * 90f, Vector3.back) * new Vector3(dir, 0f) * speed, "Player");
                    })
                    .AppendInterval(0.3f)
                    .AppendCallback(() =>
                    {
                        Parent.bullet_spawner_.Shot(Parent.venomBullet, shotPos.position, Quaternion.AngleAxis(dir * 15f, Vector3.back) * new Vector3(dir, 0f) * speed, "Player");
                        Parent.bullet_spawner_.Shot(Parent.venomBullet, shotPos.position, Quaternion.AngleAxis(dir * 45f, Vector3.back) * new Vector3(dir, 0f) * speed, "Player");
                        Parent.bullet_spawner_.Shot(Parent.venomBullet, shotPos.position, Quaternion.AngleAxis(dir * 75f, Vector3.back) * new Vector3(dir, 0f) * speed, "Player");
                    })
                    .AppendCallback(() =>
                    {
                        Parent.transform.DOMoveY(-6.5f, 1f).SetRelative();
                    })
                    .AppendInterval(1f)
                    .AppendCallback(() =>
                    {
                        Parent.animator.Play(hashIdle);
                        ChangeState((int)eState.Think);
                    });
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                dir = Mathf.Sign(Parent.player_.position.x - Parent.transform.position.x);
                Parent.SetDirection((dir < 0f) ? eDir.Left : eDir.Right);
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {
                Parent.seq.Kill();
            }
        }

        [System.Serializable]
        private class StateDead : StateMachine<KoitanBossController>.StateBase
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
                // 時間をゆっくりにする
                TadaLib.TimeScaler.Instance.RequestChange(new_time_scale_, time_change_duration_);
                Global.GlobalPlayerInfo.IsMuteki = true;
                Global.GlobalPlayerInfo.BossDefeated = true;
                // 動きをとめる
                Parent.trb_.Velocity = Vector2.zero;
                Parent.animator.CrossFade(hashDead, 0.5f);

                // ボスが死んだ回数を加算する
                Global.GlobalDataManager.AddBossDefeatCnt(Global.eBossType.VernmDrake);
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                ActorUtils.ProcSpeed(ref Parent.trb_.Velocity, new Vector2(0f, 1f) * Accel, MaxAbsSpeed);

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

                talk_end_timer_ += Time.fixedDeltaTime;

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

                Actor.Player.SkillManager.Instance.GainSkillPoint(750, Parent.transform.position, 0.7f);
                //実績解除
                AchievementManager.FireAchievement("VenomDrake");
                if (Parent.player_.GetComponent<Player.Player>().IsNoDamage())
                {
                    AchievementManager.FireAchievement("VenomDrake_nodamage");
                }
            }
        }

        // プレイヤー撃墜後のセリフ
        [System.Serializable]
        private class WinTalkState : StateMachine<KoitanBossController>.StateBase
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
                Global.GlobalDataManager.AddDeathCnt(Global.eBossType.VernmDrake);
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

        // VenomDrake
    }
} // namespace Actor.Enemy