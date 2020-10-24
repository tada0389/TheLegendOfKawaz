using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TadaLib;
using Bullet;

/// <summary>
/// ツートンブラザーズの兄
/// </summary>

namespace Actor.Enemy.TwoTon.Younger
{
    [RequireComponent(typeof(TadaRigidbody))]
    [RequireComponent(typeof(BulletSpawner))]
    public partial class TwoTonYoungerBossController : BaseBossController
    {
        // 親クラス
        [SerializeField]
        private BaseActorController two_ton_manager_;

        private enum eState
        {
            Think,
            Walk,
            Dead,
        }

        // ステートを管理して起動するクラス
        private StateMachine<TwoTonYoungerBossController> state_machine_;

        // 物理演算や速度を変更するやつ
        private TadaRigidbody trb_;

        // 弾を管理して発射させるやつ
        private BulletSpawner bullet_spawner_;

        // アニメータ
        private Animator animator_;

        #region state
        //[SerializeField]
        //private ThinkState think_state_;
        //[SerializeField]
        //private WalkState walk_state_;
        //[SerializeField]
        //private DeadState dead_state_;
        #endregion

        private int prev_hp_;

        // Start is called before the first frame update
        private void Start()
        {
            trb_ = GetComponent<TadaRigidbody>();
            bullet_spawner_ = GetComponent<BulletSpawner>();
            animator_ = GetComponent<Animator>();

            state_machine_ = new StateMachine<TwoTonYoungerBossController>(this);

            //// ステートの登録
            //state_machine_.AddState((int)eState.Think, think_state_);
            //state_machine_.AddState((int)eState.Walk, walk_state_);
            //state_machine_.AddState((int)eState.Dead, dead_state_);
            //// 初期ステートの設定
            //state_machine_.SetInitialState((int)eState.Think);

            // デバッグ表示
            DebugBoxManager.Display(this).SetSize(new Vector2(500, 400)).SetOffset(new Vector2(0, 0));

            prev_hp_ = 20;

            StartCoroutine(Flow());
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            // ステートの更新
            //state_machine_.Proc();

            CheckHPChange();
        }

        // HPの減少をチェックする
        private void CheckHPChange()
        {
            int hp_diff = prev_hp_ - HP;
            if (hp_diff == 0) return;

            // こいつにHPを実質持たせないから回復
            HP = 20;

            // ボス管理クラスにダメージを受けさせる
            two_ton_manager_.Damage(hp_diff);
        }

        // 死亡したときに呼ばれる関数 基底クラスから呼ばれる わかりにくい
        protected override void Dead()
        {
            return;
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

        private IEnumerator Flow()
        {
            while (true)
            {
                mesh_.GetComponent<SpriteRenderer>().color = Color.grey;

                transform.position = new Vector3(player_.transform.position.x, 6f);

                yield return new WaitForSeconds(1.0f);

                mesh_.GetComponent<SpriteRenderer>().color = Color.white;

                float accel = -10.0f;
                float vel = -15.0f;

                while (true)
                {
                    vel += accel * Time.deltaTime;
                    transform.position += new Vector3(0f, vel * Time.deltaTime);
                    yield return null;

                    if (transform.position.y <= -2.5f) break;
                }

                CameraSpace.CameraShaker.Shake(0.3f, 0.3f);

                yield return new WaitForSeconds(1.0f);
            }
        }
    }
}