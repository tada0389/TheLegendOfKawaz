using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TadaLib;
using Bullet;
using TadaInput;

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
        Left = 0,
        Right = 1,
    }

    public enum eShotType
    {
        Normal,
        DashNormal,
        Charge,
        DashCharge,
        None,
    }

    public enum eAnimType
    {
        Play, 
        SetBoolTrue,
        SetBoolFalse,
        Restart,
        None,
    }

    // ステート間で共有するデータ
    public class Data
    {
        // プレイヤーの物理演算
        public TadaRigidbody trb { private set; get; }

        public Transform transform;

        // 体力
        public int HP { private set; get; }
        public int MaxHP { private set; get; }
        // 攻撃力
        public float Power { private set; get; }
        // 基礎移動速度
        public float InitSpeed { set { trb.InitSpeed = value; } get { return trb.InitSpeed; } }
        // 壁蹴りできるか
        public bool CanWallKick { private set; get; }
        // 自動回復できるか
        public float AutoHealInterval { private set; get; }
        // 弾の数
        public int MaxShotNum { private set; get; }
        // チャージ完了時間
        public float ChargeEndTime { private set; get; }

        // 空中ジャンプの最大回数
        public int AirJumpNumMax { private set; get; }
        // 空中ジャンプ回数
        private int air_jump_num_;

        // ダッシュできるかどうか
        public bool CanAirDashMove { private set; get; }

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

        // 地面の摩擦係数
        public float GroundFriction => trb.GroundFriction;

        // 水中にいるかどうか
        public bool IsUnderWater => trb.IsUnderWater;

        // アニメーター
        public Animator animator;
        // オーディオソース
        public AudioSource audio;
        public BulletSpawner bullet_spawner_;

        // スキルの一時的な強化数
        private List<int> temporary_skills_;

        private List<Skill> skills_;

        // コンストラクタ
        public Data(Player body, List<Skill> skills)
        {
            Dir = eDir.Right;
            transform = body.transform;

            animator = body.GetComponent<Animator>();
            audio = body.GetComponent<AudioSource>();
            trb = body.GetComponent<TadaRigidbody>();
            bullet_spawner_ = body.GetComponent<BulletSpawner>();

            skills_ = skills;

            MaxHP = PlayerUtils.GetSkillValue(skills_, eSkill.HP);
            HP = MaxHP;
            Power = PlayerUtils.GetSkillValue(skills_, eSkill.Attack);
            InitSpeed = PlayerUtils.GetSkillValue(skills_, eSkill.Speed);
            CanWallKick = PlayerUtils.GetSkillValue(skills_, eSkill.WallKick);
            AutoHealInterval = PlayerUtils.GetSkillValue(skills_, eSkill.AutoHeal);
            MaxShotNum = PlayerUtils.GetSkillValue(skills_, eSkill.ShotNum);
            ChargeEndTime = PlayerUtils.GetSkillValue(skills_, eSkill.ChargeShot);
            AirJumpNumMax = PlayerUtils.GetSkillValue(skills_, eSkill.AirJumpNum);
            air_jump_num_ = AirJumpNumMax;
            CanAirDashMove = PlayerUtils.GetSkillValue(skills_, eSkill.AirDushNum);
            prev_dash_time_ = 0f;

            int sz = System.Enum.GetNames(typeof(eSkill)).Length;
            temporary_skills_ = new List<int>(sz);
            for (int i = 0; i < sz; ++i) temporary_skills_.Add(0);

        }

        // ダッシュできるか
        public bool CanGroundDash()
        {
            if (!IsGround) return false;
            if (is_dashed_ || (IsGround && Time.time - prev_dash_time_ < 0.5f)) return false;
            return true;
        }

        public void DashCalled()
        {
            if (!IsGround) is_dashed_ = true;
            prev_dash_time_ = Time.time;
        }

        // 空中ダッシュできるか
        public bool CanAirDash()
        {
            if (IsGround) return false;
            if (!CanAirDashMove || is_dashed_) return false;
            return true;
        }

        public void AirDashCalled()
        {
            is_dashed_ = true;
            prev_dash_time_ = Time.time;
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
            return air_jump_num_ >= 1;
        }

        public void ArialJumpCalled()
        {
            --air_jump_num_;
        }

        public void ArialJumpDismissed()
        {
            air_jump_num_ = Mathf.Min(air_jump_num_ + 1, AirJumpNumMax);
        }

        public void ReflectVelocity(bool is_in)
        {
            if (is_in) trb.Velocity = velocity;
            else velocity = trb.Velocity;
        }

        // 向いている方向を反転する
        public void ReverseFaceDirection() => Dir = (Dir == eDir.Left) ? eDir.Right : eDir.Left;
        public void ChangeDirection(eDir dir) => Dir = dir;

        public void SetHP(int new_hp) => HP = Mathf.Clamp(new_hp, 0, MaxHP);

        public bool AquireTmpSkill(eSkill skill, bool is_minigame_mode)
        {
            int v = ++temporary_skills_[(int)skill];

            return ChangeTmpSkill(skills_, skill, v);
        }

        public bool ReleaseTmpSkill(eSkill skill, bool is_minigame_mode)
        {
            int v = --temporary_skills_[(int)skill];

            return ChangeTmpSkill(skills_, skill, v);
        }

        // スキルの値を一時的に変更する
        private bool ChangeTmpSkill(List<Skill> Skills, eSkill skill, int v)
        {
            // くそこーど
            switch (skill)
            {
                case eSkill.HP:
                    MaxHP = PlayerUtils.GetSkillValue(Skills, skill, v);
                    HP = Mathf.Min(HP, MaxHP);
                    break;
                case eSkill.Speed:
                    InitSpeed = PlayerUtils.GetSkillValue(Skills, skill, v);
                    break;
                case eSkill.Attack:
                    Power = PlayerUtils.GetSkillValue(Skills, skill, v);
                    break;
                case eSkill.AirJumpNum:
                    AirJumpNumMax = PlayerUtils.GetSkillValue(Skills, skill, v);
                    air_jump_num_ = AirJumpNumMax;
                    break;
                case eSkill.AirDushNum:
                    CanAirDashMove = PlayerUtils.GetSkillValue(Skills, skill, v);
                    break;
                case eSkill.AutoHeal:
                    AutoHealInterval = PlayerUtils.GetSkillValue(Skills, skill, v);
                    break;
                case eSkill.WallKick:
                    CanWallKick = PlayerUtils.GetSkillValue(Skills, skill, v);
                    break;
                case eSkill.ChargeShot:
                    ChargeEndTime = PlayerUtils.GetSkillValue(Skills, skill, v);
                    break;
                case eSkill.ShotNum:
                    MaxShotNum = PlayerUtils.GetSkillValue(Skills, skill, v);
                    break;
                default:
                    return false;
            }
            return true;
        }
    }

    // プレイヤークラス partialによりファイル間で分割してクラスを実装
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(BulletSpawner))]
//    [RequireComponent(typeof(BasePlayerInput))]
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
            DashJump, // ダッシュジャンプ
            Damage, // ダメージを受けたときのステート
            Dead, // 死亡ステート
            WaterIdle, // 水中のアイドリング
            WaterWalk, // 水中の移動
            WaterJump, // 水中ジャンプ
            CloseEar, // 咆哮とうで耳をふさぐステート
        }

        public eDir Dir => data_.Dir;
        public Vector2 Velocity => data_.velocity;

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
        private StateDashJump dashjump_state_;
        [SerializeField]
        private StateDamage damage_state_;
        [SerializeField]
        private StateDead dead_state_;
        [SerializeField]
        private StateWaterIdle water_idle_state_;
        [SerializeField]
        private StateWaterWalk water_walk_state_;
        [SerializeField]
        private StateWaterJump water_jump_state_;
        [SerializeField]
        private StateCloseEar close_ear_state_;
#endregion
        
        private Timer shot_anim_timer_;
        private float charge_timer_;

        // もうしょうがない！
        [SerializeField]
        private GameObject charge_shot_pre_;
        [SerializeField]
        private GameObject charge_shot_end_;

        [SerializeField]
        private NormalBullet normal_bullet_;
        [SerializeField]
        private NormalBullet charge_bullet_;

        // 自動回復のオブジェクト
        [SerializeField]
        private AutoHealController heal_ctrl_;
        private bool heal_inited_ = false;

        // プレイヤーが反転しても右側を向き続けるオブジェクト
        [SerializeField]
        private Transform not_reverse_;

        [SerializeField]
        private float muteki_time_ = 2.0f;
        private bool is_nodamage_ = true;
        [SerializeField]
        private GameObject mesh_;
        private Timer muteki_timer_;

        [SerializeField]
        private string enemy_tag_ = "Enemy";

        [SerializeField]
        private bool is_minigame_mode_ = false;

        // ダッシュ時間がどれくらい残っているか ダッシュジャンプに使う
        private float dash_remain_time_;

        private bool prev_is_ground_;

        private bool inited_;


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

        // プレイヤーの入力クラス
        private TadaInput.BasePlayerInput input_;

        private PlayerDataLogger logger_;

        private void Awake()
        {
            input_ = GetComponent<BasePlayerInput>();
            logger_ = GetComponent<PlayerDataLogger>();

            inited_ = false;
        }

        // プレイヤーを初期化する どのスキル群を使うか指定する
        public void Init(List<Skill> skills)
        {
            inited_ = true;

            prev_is_ground_ = true;

            shot_anim_timer_ = new Timer(0.3f);
            muteki_timer_ = new Timer(muteki_time_);

            data_ = new Data(this, skills);
            HP = data_.HP;
            if (data_.AutoHealInterval > 0.01f)
            {
                heal_ctrl_.Init(data_.AutoHealInterval);
                heal_inited_ = true;
            }

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
            state_machine_.AddState((int)eState.DashJump, dashjump_state_);
            state_machine_.AddState((int)eState.Damage, damage_state_);
            state_machine_.AddState((int)eState.Dead, dead_state_);
            state_machine_.AddState((int)eState.WaterIdle, water_idle_state_);
            state_machine_.AddState((int)eState.WaterWalk, water_walk_state_);
            state_machine_.AddState((int)eState.WaterJump, water_jump_state_);
            state_machine_.AddState((int)eState.CloseEar, close_ear_state_);

            // 始めのステートを設定
            state_machine_.SetInitialState((int)eState.Wait);

            // デバッグ表示
            DebugBoxManager.Display(this).SetSize(new Vector2(500, 400)).SetOffset(new Vector2(0, -100));

            GetSkillSet();

            // ショットのプーリング
            KoitanLib.ObjectPoolManager.Init(normal_bullet_, this, data_.MaxShotNum);

            Vector3 new_pos = TadaScene.TadaSceneManager.GetPrevPosition();
            if (new_pos != Vector3.zero) transform.position = new_pos;
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            if (!inited_) return;

            if (!Global.GlobalPlayerInfo.ActionEnabled)
            {
                input_.ActionEnabled = false;
                // 速度をデフォルトに (重力で少し下に)
                data_.velocity = new Vector2(0f, Mathf.Min(-0.1f, data_.velocity.y));
                data_.ReflectVelocity(true);
                return;
            }
            else input_.ActionEnabled = true;

            if (!Global.GlobalPlayerInfo.BossDefeated && !IsDead() && 
                Time.timeScale > 0.5f && !KoitanLib.FadeManager.is_fading && input_.GetButtonDown(ActionCode.Pause))
            {
                SettingManager.RequestOpenWindow();
            }

            // 接地しているかどうかなどで，状態を変更する
            RefectRigidbody();

            // 変更された速度を取得する
            data_.ReflectVelocity(false);


            // 状態を更新する
            state_machine_.Proc();

            // 向いている方向を正しくする
            CheckFaceDirChange();

            // 速度に応じて移動する
            data_.ReflectVelocity(true);

            if (state_machine_.CurrentStateId == (int)eState.Dead) return;

            // ごみ 自動回復のエフェクトを出すか出さないか
            if (data_.AutoHealInterval > 0.01f && !heal_inited_)
            {
                heal_ctrl_.Init(data_.AutoHealInterval);
                heal_inited_ = true;
            }
            else if(data_.AutoHealInterval < 0.01f && heal_inited_)
            {
                heal_ctrl_.Finish();
                heal_inited_ = false;
            }
            else if (data_.AutoHealInterval > 0.01f && heal_inited_ && heal_ctrl_.CanHeal())
            {
                data_.SetHP(data_.HP + 1);
                HP = data_.HP;
                heal_ctrl_.PlayHealEffect();
            }

            // 弾を撃つかどうか決める
            CheckShot();

            // 入力情報をリセット (UpdateとFixedUpdateを同期させるため)
            input_.Reset();
        }

        // ショットするかをチェックする
        private void CheckShot()
        {
            if (input_.GetButtonDown(ActionCode.Shot))
            {
                charge_timer_ = 0.0f;
                Shot(false);
            }
            else if (input_.GetButton(ActionCode.Shot))
            {
                charge_timer_ += Time.fixedDeltaTime;
                if(charge_timer_ >= data_.ChargeEndTime / 4f && !charge_shot_pre_.activeSelf)
                {
                    charge_shot_pre_.SetActive(true);
                }
                if(charge_timer_ >= data_.ChargeEndTime && !charge_shot_end_.activeSelf)
                {
                    charge_shot_end_.SetActive(true);
                }
            }

            if (input_.GetButtonUp(ActionCode.Shot))
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
            bool dashed = (state_machine_.CurrentStateId == (int)eState.Dush);
            float speed = (dashed) ? 1.5f : 1.0f;
            NormalBullet bullet = (is_charged) ? charge_bullet_ : normal_bullet_;
            bool can_shot = data_.bullet_spawner_.Shot(bullet, transform.position + new Vector3(dir * 1.5f, 0f, 0f),
                new Vector2(dir, 0f), enemy_tag_, not_reverse_, speed, -1, speed * data_.Power);
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

            // 以下ゴースト作るのに使う
            if (logger_ != null)
            {
                eShotType type = eShotType.None;
                if (is_charged)
                {
                    if (dashed) type = eShotType.DashCharge;
                    else type = eShotType.Charge;
                }
                else
                {
                    if (dashed) type = eShotType.DashNormal;
                    else type = eShotType.Normal;
                }

                logger_.AddLog(type);
            }
        }

        public void PlayAnim(string anim, eAnimType type = eAnimType.Play)
        {
            switch (type)
            {
                case eAnimType.Play:
                    data_.animator.Play(anim);
                    break;
                case eAnimType.SetBoolTrue:
                    data_.animator.SetBool(anim, true);
                    break;
                case eAnimType.SetBoolFalse:
                    data_.animator.SetBool(anim, false);
                    break;
                case eAnimType.Restart:
                    data_.animator.Play(anim, 0, 0.0f);
                    break;
                default:
                    break;
            }

            if (logger_ != null) logger_.AddLog(anim, type);
        }

        // コライド情報などで状態を更新する
        private void RefectRigidbody()
        {

            data_.IsThrough = (input_.GetAxis(AxisCode.Vertical) < -0.5f);

            //data_.animator.SetBool("isGround", data_.IsGround);

            if (data_.IsGround)
            {
                // 空中ジャンプ回数をリセットする
                data_.ResetArialJump();
                data_.ResetDash();
                if(!prev_is_ground_) PlayAnim("isGround", eAnimType.SetBoolTrue);
            }
            else if(prev_is_ground_) PlayAnim("isGround", eAnimType.SetBoolFalse);

            prev_is_ground_ = data_.IsGround;
        }

        // 方向転換するか確かめる
        private void CheckFaceDirChange()
        {
            // 浮動小数点型で==はあんまよくないけど・・・
            if (data_.Dir == eDir.Left && transform.localEulerAngles.y != 180f)
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 180f, transform.localEulerAngles.z);
                if(not_reverse_) not_reverse_.localEulerAngles = new Vector3(not_reverse_.localEulerAngles.x, 180f, not_reverse_.localEulerAngles.z);
            }
            // else if(data_.velocity.x > 0f)
            else if (data_.Dir == eDir.Right && transform.localEulerAngles.y != 0f)
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0f, transform.localEulerAngles.z);
                if (not_reverse_) not_reverse_.localEulerAngles = new Vector3(not_reverse_.localEulerAngles.x, 0f, not_reverse_.localEulerAngles.z);
            }
        }

        // 咆哮を受けるステートに変更する
        public void DoRoarState()
        {
            state_machine_.ChangeState((int)eState.CloseEar);
        }

        // ダメージを受ける
        public override void Damage(int damage)
        {
            if (Global.GlobalPlayerInfo.IsMuteki) return;
            if (!is_nodamage_ && !muteki_timer_.IsTimeout()) return;
            if (state_machine_.CurrentStateId == (int)eState.Damage) return;
            if (state_machine_.CurrentStateId == (int)eState.Dead) return;

            is_nodamage_ = false;
            DamageDisplayer.eDamageType type = DamageDisplayer.eDamageType.Mini;
            if (damage >= 3) type = DamageDisplayer.eDamageType.Big;
            else if (damage >= 2) type = DamageDisplayer.eDamageType.Normal;
            DamageDisplayer.Instance.ShowDamage(damage, transform.position, type);
            state_machine_.ChangeState((int)eState.Damage);
            data_.SetHP(HP - damage);
            HP = data_.HP;
            if (HP == 0)
            {
                state_machine_.ChangeState((int)eState.Dead);
            }
            muteki_timer_.TimeReset();
            StartCoroutine(Tenmetu());
        }

        //点滅
        private IEnumerator Tenmetu()
        {
            mesh_.SetActive(false);
            yield return new WaitForSeconds(0.05f);
            mesh_.SetActive(true);
            yield return new WaitForSeconds(0.05f);
            if (!muteki_timer_.IsTimeout())
            {
                StartCoroutine(Tenmetu());
            }
        }

        // デバッグで最初から指定したスキルを持っている
        private void GetSkillSet()
        {
            foreach(InitialSkill skill in demo_skills_)
            {
                for(int i = 0; i < skill.level_; ++i)
                {
                    data_.AquireTmpSkill(skill.type_, is_minigame_mode_);
                }
            }
        }

        // スキルを一時的に取得する
        public bool AquireTemporarySkill(eSkill skill)
        {
            if(skill == eSkill.ShotNum) // オブジェクトプーリングを再設定 強引
            {
                KoitanLib.ObjectPoolManager.Release(normal_bullet_);
                bool ok = data_.AquireTmpSkill(skill,is_minigame_mode_);
                KoitanLib.ObjectPoolManager.Init(normal_bullet_, this, data_.MaxShotNum);
                return ok;
            }
            return data_.AquireTmpSkill(skill, is_minigame_mode_);
        }

        // 一時的に取得したスキルを開放する
        public bool ReleaseTemporarySkill(eSkill skill)
        {
            if (skill == eSkill.ShotNum) // オブジェクトプーリングを再設定 強引
            {
                KoitanLib.ObjectPoolManager.Release(normal_bullet_);
                bool ok = data_.ReleaseTmpSkill(skill, is_minigame_mode_);
                KoitanLib.ObjectPoolManager.Init(normal_bullet_, this, data_.MaxShotNum);
                return ok;
            }
            return data_.ReleaseTmpSkill(skill, is_minigame_mode_);
        }

        // HPが最大かどうか
        public bool IsNoDamage()
        {
            return data_.HP == data_.MaxHP;
        }

        // 死亡しているかどうか
        public bool IsDead()
        {
            return state_machine_.CurrentStateId == (int)eState.Dead;
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