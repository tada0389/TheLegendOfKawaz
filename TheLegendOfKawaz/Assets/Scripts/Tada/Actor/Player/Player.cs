﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TadaLib;
using Bullet;

/// <summary>
/// キャラクターを動かす元となるクラス
/// 
/// 操作方法
/// 十字キー右，左で移動
/// 高速で二回移動キーを押すとダッシュ
/// スペースでジャンプ
/// 
/// このクラスは触らなくても大丈夫
/// 勝手にやってくれる (いじりたければどうぞ)
/// Data(実質グローバル変数)には，変数付け加えてもいいかも
/// 
/// DataのVelocityを各ステートクラスでいじると，
/// その速度に応じて座標移動してくれる
/// 
/// 今回のUnityのInspectorで変数をいじれない仕様になってるのは許して
/// 
/// それと，各ステートでPlayerクラスのprivate変数をいじれるようになってる 内部クラスは外部クラスの変数をすべて見れる
/// 
/// </summary>

namespace Actor.Player
{
    // 方向
    public enum eDir
    {
        Left,
        Right,
    }

    // ステート間で共有するデータ
    public class Data
    {
        // プレイヤーの物理演算
        private TadaRigidbody trb;

        public Transform transform;

        // 体力
        public int HP { private set; get; }
        public int MaxHP { private set; get; }
        // 攻撃力
        public int Power { private set; get; }
        // 基礎移動速度
        public float InitSpeed { set { trb.InitSpeed = value; } get { return trb.InitSpeed; } }
        // 壁蹴りできるか
        public bool CanWallKick { private set; get; }
        // 自動回復できるか
        public bool CanAutoHeal { private set; get; }
        // 弾の数
        public int MaxShotNum { private set; get; }
        // チャージ完了時間
        public float ChargeEndTime { private set; get; }

        // 空中ジャンプの最大回数
        public int AirJumpNumMax { private set; get; }
        // 空中ジャンプ回数
        private int air_jump_num_;

        // ダッシュできるかどうか
        public bool CanDashMove { private set; get; }

        // すり抜ける床をすり抜けるか
        public bool IsThrough { set { trb.IsThrough = value; } get { return trb.IsThrough; } }

        private bool is_dashed_;

        private float prev_dash_time_;

        // プレイヤーの速度
        public Vector2 velocity;
        //public Vector2 velocity => rigidbody_.Velocity;
        // 向いている方向 1.0で右, -1.0で左
        public eDir Dir { private set; get; }

        // 接地しているかどうか
        public bool IsGround => trb.ButtomCollide;
        // 天井に頭がぶつかっているかどうか
        public bool IsHead => trb.TopCollide; // 変数名が思いつかない
        // 左方向にぶつかっている
        public bool IsLeft => trb.LeftCollide;
        // 右方向にぶつかっている
        public bool IsRight => trb.RightCollide;

        // アニメーター
        public Animator animator;
        // バレット 0が通常，1がチャージ
        public BulletSpawner[] bullet_spawners_;
        // オーディオソース
        public AudioSource audio;

        // それぞれのステートのデータ

        // コンストラクタ
        public Data(Player body)
        {
            Dir = eDir.Right;
            transform = body.transform;

            animator = body.GetComponent<Animator>();
            bullet_spawners_ = body.GetComponents<BulletSpawner>();
            audio = body.GetComponent<AudioSource>();
            trb = body.GetComponent<TadaRigidbody>();

            var Skills = SkillManager.Instance.Skills;

            MaxHP = Skills[(int)eSkill.HP].Value;
            HP = MaxHP;
            Power = Skills[(int)eSkill.Attack].Value;
            InitSpeed = Skills[(int)eSkill.Speed].Value / (float)100f;
            CanWallKick = Skills[(int)eSkill.WallKick].Value != 0;
            CanAutoHeal = Skills[(int)eSkill.AutoHeal].Value != 0;
            MaxShotNum = Skills[(int)eSkill.ShotNum].Value;
            ChargeEndTime = Skills[(int)eSkill.ChargeShot].Value / 10f;
            AirJumpNumMax = Skills[(int)eSkill.AirJumpNum].Value;
            air_jump_num_ = AirJumpNumMax;
            CanDashMove = Skills[(int)eSkill.AirDushNum].Value != 0;
            prev_dash_time_ = 0f;

            bullet_spawners_[0].Init(MaxShotNum);
            bullet_spawners_[1].Init(2);
        }

        // ダッシュできるか
        public bool CanDash()
        {
            if (!CanDashMove || is_dashed_ || (IsGround && Time.time - prev_dash_time_ < 0.5f)) return false;
            if(!IsGround) is_dashed_ = true;
            prev_dash_time_ = Time.time;
            return true;
        }

        public void ResetDash() => is_dashed_ = false;

        // 空中ジャンプ回数をリセットする
        public void ResetArialJump()
        {
            air_jump_num_ = AirJumpNumMax;
        }

        // 空中ジャンプができるか？
        public bool RequestArialJump()
        {
            --air_jump_num_;
            return air_jump_num_ >= 0;
        }

        public void ReflectVelocity()
        {
            trb.Velocity = velocity;
        }

        // 向いている方向を反転する
        public void ReverseFaceDirection() => Dir = (Dir == eDir.Left) ? eDir.Right : eDir.Left;
        public void ChangeDirection(eDir dir) => Dir = dir;

        public void SetHP(int hp) => HP = hp;
    }

    // プレイヤークラス partialによりファイル間で分割してクラスを実装
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(BulletSpawner))]
    public partial class Player : BaseActorController
    {
        // プレイヤーのステート一覧
        private enum eState
        {
            Wait, // 待機中のステート アイドリング
            Walk, // 歩いているステート
            //Run, // 走っているステート
            Jump, // ジャンプ中のステート
            Fall, // 落下中のステート(ジャンプでの落下はこれじゃない)
            Wall, // 壁に密着しているステート
            Dush, // ダッシュしているステート
            Damage, // ダメージを受けたときのステート
        }

        // ステートマシン
        private StateMachine<Player> state_machine_;

        // ステート間で共有するデータ
        private Data data_;

        #region state class
        // 各ステートのインスタンス これで各ステートのフィールドをInspectorでいじれる 神
        [SerializeField]
        private StateIdle idle_state_;
        [SerializeField]
        private StateWalk walk_state_;
        //[SerializeField]
        //private StateRun run_state_;
        [SerializeField]
        private StateJump jump_state_;
        [SerializeField]
        private StateFall fall_state_;
        [SerializeField]
        private StateWall wall_state_;
        [SerializeField]
        private StateDush dush_state_;
        [SerializeField]
        private StateDamage damage_state_;
#endregion

        private Timer shot_anim_timer_;
        private float charge_timer_;

        // もうしょうがない！
        [SerializeField]
        private GameObject charge_shot_pre_;
        [SerializeField]
        private GameObject charge_shot_end_;

        // 初期スキル
        #region debug
        [System.Serializable]
        private class InitialSkill
        {
            public eSkill type_ = eSkill.WallKick;
            public int level_ = 0;
        }
        [SerializeField]
        private InitialSkill[] demo_skills_;
        #endregion

        // Start is called before the first frame update
        private void Start()
        {
            // デバッグ
            GetSkillSet();

            shot_anim_timer_ = new Timer(0.3f);

            data_ = new Data(this);
            HP = data_.HP;

            // ステートマシンのメモリ確保 自分自身を渡す
            state_machine_ = new StateMachine<Player>(this);
            // ステートを登録
            state_machine_.AddState((int)eState.Wait, idle_state_);
            state_machine_.AddState((int)eState.Walk, walk_state_);
            //state_machine_.AddState((int)eState.Run, run_state_);
            state_machine_.AddState((int)eState.Jump, jump_state_);
            state_machine_.AddState((int)eState.Fall, fall_state_);
            state_machine_.AddState((int)eState.Wall, wall_state_);
            state_machine_.AddState((int)eState.Dush, dush_state_);
            state_machine_.AddState((int)eState.Damage, damage_state_);

            // 始めのステートを設定
            state_machine_.SetInitialState((int)eState.Fall);

            // デバッグ表示
            DebugBoxManager.Display(this).SetSize(new Vector2(500, 400)).SetOffset(new Vector2(0, -300));
        }

        // Update is called once per frame
        private void Update()
        {
            // 接地しているかどうかなどで，状態を変更する
            RefectCollide();

            if (UnityEngine.InputSystem.Keyboard.current[UnityEngine.InputSystem.Key.N].wasPressedThisFrame)
            {
                state_machine_.ChangeState((int)eState.Damage);
            }

            // 状態を更新する
            state_machine_.Proc();

            // 向いている方向を正しくする
            CheckFaceDirChange();

            // 速度に応じて移動する
            data_.ReflectVelocity();

            // デバッグ
            if (UnityEngine.InputSystem.Keyboard.current[UnityEngine.InputSystem.Key.B].wasPressedThisFrame){
                UnityEngine.SceneManagement.SceneManager.LoadScene("SkillGetScene");
            }


            // 弾を撃つかどうか決める
            CheckShot();
        }

        // ショットするかをチェックする
        private void CheckShot()
        {
            if (ActionInput.GetButtonDown(ActionCode.Shot))
            {
                charge_timer_ = 0.0f;
                Shot(false);
            }
            else if (ActionInput.GetButton(ActionCode.Shot))
            {
                charge_timer_ += Time.deltaTime;
                if(charge_timer_ >= data_.ChargeEndTime / 4f && !charge_shot_pre_.activeSelf)
                {
                    charge_shot_pre_.SetActive(true);
                }
                if(charge_timer_ >= data_.ChargeEndTime && !charge_shot_end_.activeSelf)
                {
                    charge_shot_end_.SetActive(true);
                }
            }

            if (ActionInput.GetButtonUp(ActionCode.Shot))
            {
                if(charge_timer_ >= data_.ChargeEndTime / 4f)
                    Shot(charge_timer_ >= data_.ChargeEndTime);
                charge_shot_pre_.SetActive(false);
                charge_shot_end_.SetActive(false);
            }

            // ショット後のアニメーション変更
            if (data_.animator.GetLayerWeight(1) == 1 && shot_anim_timer_.IsTimeout()) data_.animator.SetLayerWeight(1, 0);
        }

        // 弾を撃つ
        private void Shot(bool is_charged)
        {
            float dir = (data_.Dir == eDir.Left) ? -1f : 1f;
            bool can_shot = data_.bullet_spawners_[(is_charged) ? 1 : 0].Shot(transform.position + new Vector3(dir * 1.5f, 0f, 0f), new Vector2(dir, 0f), "Enemy");
            if (!can_shot) return;
            if (data_.animator.GetLayerWeight(1) == 0)
            {
                shot_anim_timer_.TimeReset();
                data_.animator.SetLayerWeight(1, 1);
            }
            else
            {
                data_.animator.SetLayerWeight(1, 0);
            }
            if (is_charged) data_.animator.Play("ChargeShot", 1, 0);
            else data_.animator.Play("Shot", 1, 0);
        }

        // コライド情報などで状態を更新する
        private void RefectCollide()
        {
            data_.IsThrough = (ActionInput.GetAxis(AxisCode.Vertical) < -0.5f);

            data_.animator.SetBool("isGround", data_.IsGround);
            if (data_.IsGround)
            {
                // 空中ジャンプ回数をリセットする
                data_.ResetArialJump();
                data_.ResetDash();
            }
        }

        // 方向転換するか確かめる
        private void CheckFaceDirChange()
        {
            // 浮動小数点型で==はあんまよくないけど・・・
            if (data_.Dir == eDir.Left && transform.localEulerAngles.y != 180f)
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 180f, transform.localEulerAngles.z);
            }
            // else if(data_.velocity.x > 0f)
            else if (data_.Dir == eDir.Right && transform.localEulerAngles.y != 0f)
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0f, transform.localEulerAngles.z);
            }
        }

        // ダメージを受ける
        public override void Damage(int damage)
        {
            if (state_machine_.CurrentStateId == (int)eState.Damage) return;
            state_machine_.ChangeState((int)eState.Damage);
            HP = Mathf.Max(0, HP - damage);
            data_.SetHP(HP);
            if (HP == 0) Debug.Log("Defeated");
        }

        // デバッグで最初から指定したスキルを持っている
        private void GetSkillSet()
        {
            SkillManager instance = SkillManager.Instance;
            foreach(InitialSkill skill in demo_skills_)
            {
                for(int i = 0; i < skill.level_; ++i)
                {
                    if(instance.GetSkill((int)skill.type_).Level < skill.level_)
                        instance.LevelUp((int)skill.type_);
                }
            }
        }

        public override string ToString()
        {
            return "(" + data_.velocity.x.ToString("F2") + ", " + data_.velocity.y.ToString("F2") + ")" +
                "\nHP : " + data_.HP.ToString() + "/" + data_.MaxHP.ToString() +
                "\nSpeed : " + data_.InitSpeed.ToString() + 
                "\nPower : " + data_.Power.ToString() + 
                "\nState : " + state_machine_.ToString() + 
                "\nIsGround : " + data_.IsGround.ToString() + 
                "\nIsHead : " + data_.IsHead.ToString();
        }
    }
}