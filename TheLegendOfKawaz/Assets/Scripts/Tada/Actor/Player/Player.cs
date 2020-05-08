using System.Collections;
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

        // コンストラクタ
        public Data(Player body)
        {
            Dir = eDir.Right;
            transform = body.transform;

            animator = body.GetComponent<Animator>();
            audio = body.GetComponent<AudioSource>();
            trb = body.GetComponent<TadaRigidbody>();
            bullet_spawner_ = body.GetComponent<BulletSpawner>();

            var Skills = SkillManager.Instance.Skills;

            MaxHP = Skills[(int)eSkill.HP].Value;
            HP = MaxHP;
            Power = Skills[(int)eSkill.Attack].Value / (float)100f;
            InitSpeed = Skills[(int)eSkill.Speed].Value / (float)100f;
            CanWallKick = Skills[(int)eSkill.WallKick].Value != 0;
            AutoHealInterval = Skills[(int)eSkill.AutoHeal].Value;
            MaxShotNum = Skills[(int)eSkill.ShotNum].Value;
            ChargeEndTime = Skills[(int)eSkill.ChargeShot].Value / 10f;
            AirJumpNumMax = Skills[(int)eSkill.AirJumpNum].Value;
            air_jump_num_ = AirJumpNumMax;
            CanAirDashMove = Skills[(int)eSkill.AirDushNum].Value != 0;
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
            if(!IsGround) is_dashed_ = true;
            prev_dash_time_ = Time.time;
            return true;
        }

        // 空中ダッシュできるか
        public bool CanAirDash()
        {
            if (IsGround) return false;
            if (!CanAirDashMove || is_dashed_) return false;
            is_dashed_ = true;
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

        public void ReflectVelocity(bool is_in)
        {
            if (is_in) trb.Velocity = velocity;
            else velocity = trb.Velocity;
        }

        // 向いている方向を反転する
        public void ReverseFaceDirection() => Dir = (Dir == eDir.Left) ? eDir.Right : eDir.Left;
        public void ChangeDirection(eDir dir) => Dir = dir;

        public void SetHP(int new_hp) => HP = Mathf.Clamp(new_hp, 0, MaxHP);

        public bool AquireTmpSkill(eSkill skill)
        {
            var Skills = SkillManager.Instance.Skills;
            int v = ++temporary_skills_[(int)skill];

            // くそこーど
            switch (skill)
            {
                case eSkill.HP:
                    MaxHP = Skills[(int)skill].NextsValue(v);
                    break;
                case eSkill.Speed:
                    InitSpeed = Skills[(int)skill].NextsValue(v) / (float)100f;
                    break;
                case eSkill.Attack:
                    Power = Skills[(int)skill].NextsValue(v) / (float)100f;
                    break;
                case eSkill.AirJumpNum:
                    AirJumpNumMax = Skills[(int)skill].NextsValue(v);
                    air_jump_num_ = AirJumpNumMax;
                    break;
                case eSkill.AirDushNum:
                    CanAirDashMove = Skills[(int)skill].NextsValue(v) != 0;
                    break;
                case eSkill.AutoHeal:
                    AutoHealInterval = Skills[(int)skill].NextsValue(v);
                    break;
                case eSkill.WallKick:
                    CanWallKick = Skills[(int)skill].NextsValue(v) != 0;
                    break;
                case eSkill.ChargeShot:
                    ChargeEndTime = Skills[(int)skill].NextsValue(v) / 10f;
                    break;
                case eSkill.ShotNum:
                    MaxShotNum = Skills[(int)skill].NextsValue(v);
                    break;
                default:
                    return false;
                    break;
            }

            return true;
        }

        public bool ReleaseTmpSkill(eSkill skill)
        {
            int v = --temporary_skills_[(int)skill];

            var Skills = SkillManager.Instance.Skills;

            // くそこーど
            switch (skill)
            {
                case eSkill.HP:
                    MaxHP = Skills[(int)skill].NextsValue(v);
                    HP = Mathf.Min(HP, MaxHP);
                    break;
                case eSkill.Speed:
                    InitSpeed = Skills[(int)skill].NextsValue(v) / (float)100f;
                    break;
                case eSkill.Attack:
                    Power = Skills[(int)skill].NextsValue(v) / (float)100f;
                    break;
                case eSkill.AirJumpNum:
                    AirJumpNumMax = Skills[(int)skill].NextsValue(v);
                    air_jump_num_ = Mathf.Min(air_jump_num_, AirJumpNumMax);
                    break;
                case eSkill.AirDushNum:
                    CanAirDashMove = Skills[(int)skill].NextsValue(v) != 0;
                    break;
                case eSkill.AutoHeal:
                    AutoHealInterval = Skills[(int)skill].NextsValue(v);
                    break;
                case eSkill.WallKick:
                    CanWallKick = Skills[(int)skill].NextsValue(v) != 0;
                    break;
                case eSkill.ChargeShot:
                    ChargeEndTime = Skills[(int)skill].NextsValue(v) / 10f;
                    break;
                case eSkill.ShotNum:
                    MaxShotNum = Skills[(int)skill].NextsValue(v);
                    break;
                default:
                    return false;
                    break;
            }

            return true;
        }
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
            DashJump, // ダッシュジャンプ
            Damage, // ダメージを受けたときのステート
            Dead, // 死亡ステート
            WaterIdle, // 水中のアイドリング
            WaterWalk, // 水中の移動
            WaterJump, // 水中ジャンプ
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
            shot_anim_timer_ = new Timer(0.3f);
            muteki_timer_ = new Timer(muteki_time_);

            data_ = new Data(this);
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

            // 始めのステートを設定
            state_machine_.SetInitialState((int)eState.Fall);

            // デバッグ表示
            DebugBoxManager.Display(this).SetSize(new Vector2(500, 400)).SetOffset(new Vector2(0, -100));

            GetSkillSet();

            // ショットのプーリング
            KoitanLib.ObjectPoolManager.Release(normal_bullet_);
            KoitanLib.ObjectPoolManager.Init(normal_bullet_, this, data_.MaxShotNum);
        }

        private void OnDestroy()
        {
            ReleaseSkillSet();
        }

        // Update is called once per frame
        private void Update()
        {
            if (Time.timeScale < 1e-6) return;

            // 接地しているかどうかなどで，状態を変更する
            RefectRigidbody();

            // 変更された速度を取得する
            data_.ReflectVelocity(false);

            if (UnityEngine.InputSystem.Keyboard.current[UnityEngine.InputSystem.Key.N].wasPressedThisFrame)
            {
                state_machine_.ChangeState((int)eState.Damage);
            }

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
            bool dashed = (state_machine_.CurrentStateId == (int)eState.Dush);
            float speed = (dashed) ? 1.5f : 1.0f;
            NormalBullet bullet = (is_charged) ? charge_bullet_ : normal_bullet_;
            bool can_shot = data_.bullet_spawner_.Shot(bullet, transform.position + new Vector3(dir * 1.5f, 0f, 0f),
                new Vector2(dir, 0f), "Enemy", not_reverse_, speed, -1, speed * data_.Power);
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
        private void RefectRigidbody()
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
                if(not_reverse_) not_reverse_.localEulerAngles = new Vector3(not_reverse_.localEulerAngles.x, 180f, not_reverse_.localEulerAngles.z);
            }
            // else if(data_.velocity.x > 0f)
            else if (data_.Dir == eDir.Right && transform.localEulerAngles.y != 0f)
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0f, transform.localEulerAngles.z);
                if (not_reverse_) not_reverse_.localEulerAngles = new Vector3(not_reverse_.localEulerAngles.x, 0f, not_reverse_.localEulerAngles.z);
            }
        }

        // ダメージを受ける
        public override void Damage(int damage)
        {
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
                Debug.Log("Defeated");
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
                    data_.AquireTmpSkill(skill.type_);
                }
            }
        }

        // デバッグで最初に入手したスキルを開放する
        private void ReleaseSkillSet()
        {
            foreach (InitialSkill skill in demo_skills_)
            {
                for (int i = 0; i < skill.level_; ++i)
                {
                    data_.ReleaseTmpSkill(skill.type_);
                }
            }
        }

        // スキルを一時的に取得する
        public bool AquireTemporarySkill(eSkill skill)
        {
            if(skill == eSkill.ShotNum) // オブジェクトプーリングを再設定 強引
            {
                KoitanLib.ObjectPoolManager.Release(normal_bullet_);
                bool ok = data_.AquireTmpSkill(skill);
                KoitanLib.ObjectPoolManager.Init(normal_bullet_, this, data_.MaxShotNum);
                return ok;
            }
            return data_.AquireTmpSkill(skill);
        }

        // 一時的に取得したスキルを開放する
        public bool ReleaseTemporarySkill(eSkill skill)
        {
            if (skill == eSkill.ShotNum) // オブジェクトプーリングを再設定 強引
            {
                KoitanLib.ObjectPoolManager.Release(normal_bullet_);
                bool ok = data_.ReleaseTmpSkill(skill);
                KoitanLib.ObjectPoolManager.Init(normal_bullet_, this, data_.MaxShotNum);
                return ok;
            }
            return data_.ReleaseTmpSkill(skill);
        }

        // HPが最大かどうか
        public bool IsNoDamage()
        {
            return data_.HP == data_.MaxHP;
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