using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor;
using TadaLib;
using DG.Tweening;
using System;

namespace Actor.Enemy.Thousand
{
    public class ArmController : BaseBossController
    {
        private enum eState
        {
            Idle,
            Dead1,
            Dead2,
            Revive1,
            Revive2,
            Stretch,
            Throw1,
            Throw2,
            Throw3,
            Burst1,
            Burst2,
            InitialBurst,
        }

        private StateMachine<ArmController> state_machine_;

        private Transform boss_;

        private int index_ = 0;
        private int arm_sum_ = 16;

        [SerializeField]
        private float radius_ = 2f;

        [SerializeField]
        private SpriteRenderer body_;

        private bool dead_ = false;

        private bool action_ = false;

        #region state
        [SerializeField]
        private IdleState idle_state_;
        [SerializeField]
        private DeadState1 dead_state1_;
        [SerializeField]
        private DeadState2 dead_state2_;
        [SerializeField]
        private ReviveState1 revive_state1_;
        [SerializeField]
        private ReviveState2 revive_state2_;
        [SerializeField]
        private StretchState stretch_state_;
        [SerializeField]
        private ThrowState1 throw_state1_;
        [SerializeField]
        private ThrowState2 throw_state2_;
        [SerializeField]
        private ThrowState3 throw_state3_;
        [SerializeField]
        private BurstState1 burst_state1_;
        [SerializeField]
        private BurstState2 burst_state2_;
        [SerializeField]
        private InitialBurstState initial_burst_state_;
        #endregion

        private float degree_ = 0f;

        private bool move_stop_ = false;

        private bool is_burst_ = false;

        private BoxCollider2D hit_box_;

        private Vector3 default_scale_;

        private eState initial_state_;

        public void Init(int arm_index, int arm_sum, Transform boss, Transform player, bool burst = false)
        {
            boss_ = boss;
            player_ = player;
            index_ = arm_index;
            degree_ = (360f / arm_sum) * index_;

            default_scale_ = transform.localScale;

            //transform.position = boss_.position + radius_ * new Vector3(Mathf.Cos(degree_ * Mathf.Deg2Rad), Mathf.Sin(degree_ * Mathf.Deg2Rad), 0f);
            //transform.localEulerAngles = new Vector3(0f, 0f, degree_ - 90f);

            float degree = degree_;
            transform.position = boss_.position + radius_ * new Vector3(Mathf.Cos(degree * Mathf.Deg2Rad), Mathf.Sin(degree * Mathf.Deg2Rad), 0f);
            transform.localEulerAngles = new Vector3(0f, 0f, degree);

            // 最初に破壊されるかどうか
            if (burst) initial_state_ = eState.InitialBurst;
            else initial_state_ = eState.Idle;
        }

        private void Start()
        {
            hit_box_ = GetComponent<BoxCollider2D>();

            state_machine_ = new StateMachine<ArmController>(this);

            state_machine_.AddState((int)eState.Idle, idle_state_);
            state_machine_.AddState((int)eState.Dead1, dead_state1_);
            state_machine_.AddState((int)eState.Dead2, dead_state2_);
            state_machine_.AddState((int)eState.Revive1, revive_state1_);
            state_machine_.AddState((int)eState.Revive2, revive_state2_);
            state_machine_.AddState((int)eState.Stretch, stretch_state_);
            state_machine_.AddState((int)eState.Throw1, throw_state1_);
            state_machine_.AddState((int)eState.Throw2, throw_state2_);
            state_machine_.AddState((int)eState.Throw3, throw_state3_);
            state_machine_.AddState((int)eState.Burst1, burst_state1_);
            state_machine_.AddState((int)eState.Burst2, burst_state2_);
            state_machine_.AddState((int)eState.InitialBurst, initial_burst_state_);

            state_machine_.SetInitialState((int)initial_state_);

            // デバッグ表示
            //DebugBoxManager.Display(this).SetSize(new Vector2(500, 400)).SetOffset(new Vector2(0, 0));
        }

        private void FixedUpdate()
        {
            state_machine_.Proc();
        }

        // 蘇生
        private void Revive()
        {
            state_machine_.ChangeState((int)eState.Revive1);
        }

        // 死亡
        protected override void Dead()
        {
            if (is_burst_) return;
            // ダメージを受けない
            if (state_machine_.CurrentStateId == (int)eState.Stretch)
            {
                HP = 1;
                return;
            }
            if (dead_) return;
            state_machine_.ChangeState((int)eState.Dead1);
        }

        // 外部から呼び出す

        // 腕の動きを止める
        public void Stop(float stop_duration_from_order = 0.5f)
        {
            if (is_burst_) return;
            if (dead_) return;
            if (action_) return;
            move_stop_ = true;
        }

        // 腕を赤くする
        public void ChargeStart()
        {
            if (is_burst_) return;
            if (dead_) return;
            body_.DOColor(Color.red, 0.5f);
            transform.DOScale(default_scale_.x * 2.0f, 0.5f);
        }

        // 腕を通常通りの色にする
        private void ChargeEnd()
        {
            body_.DOColor(Color.white, 0.5f);
            transform.DOScale(default_scale_.x, 0.5f);
        }

        // 腕を伸ばす
        public void Stretch()
        {
            if (is_burst_) return;
            if (dead_) return;
            if (action_) return;
            state_machine_.ChangeState((int)eState.Stretch);
        }

        public void Move()
        {
            if (is_burst_) return;
            if (dead_) return;
            if (action_) return;
            state_machine_.ChangeState((int)eState.Idle);
        }

        public void Throw()
        {
            if (is_burst_) return;
            if (dead_) return;
            if (action_) return;
            state_machine_.ChangeState((int)eState.Throw1);
        }

        // 千手観音が飛んでぶっ飛ばす
        public void Burst()
        {
            state_machine_.ChangeState((int)eState.Burst1);
        }

        private bool NotActive()
        {
            return is_burst_ || dead_;
        }

        public override string ToString()
        {
            return 
                "\nState : " + state_machine_.ToString();
        }

        // === 以下，ステート ================================================

        [System.Serializable]
        private class IdleState : StateMachine<ArmController>.StateBase
        {
            // ほかの腕との間隔 degree
            private float arm_interval_;

            [SerializeField]
            private float speed_ = 1f;


            public override void OnStart()
            {
                arm_interval_ = 360f / Parent.arm_sum_;
                Parent.move_stop_ = false;
                Parent.action_ = false;
            }

            public override void Proc()
            {
                if (Parent.move_stop_) return;

                Parent.degree_ += Time.fixedDeltaTime * speed_;
                float degree = Parent.degree_;
                Parent.transform.position = Parent.boss_.position + Parent.radius_ * new Vector3(Mathf.Cos(degree * Mathf.Deg2Rad), Mathf.Sin(degree * Mathf.Deg2Rad), 0f);
                Parent.transform.localEulerAngles = new Vector3(0f, 0f, degree);
            }

            public override void OnEnd()
            {
                Parent.action_ = true;
            }
        }

        // 死亡したときのステート
        // 反転して落ちる
        [System.Serializable]
        private class DeadState1 : StateMachine<ArmController>.StateBase
        {
            private Vector2 velocity_ = new Vector2(0f, 0f);

            [SerializeField]
            private Vector2 power_ = new Vector2(0f, 1f);

            [SerializeField]
            private float gravity_ = -0.98f;

            // 反転するまでの時間 上に上がってもとに位置に戻るまでの時間
            private float flip_time_;

            private float face_down_degree_per_s_;

            [SerializeField]
            private float ground_boader_ = -5f;

            [SerializeField]
            private BaseParticle dead_eff_;

            public override void OnStart()
            {
                Parent.dead_ = true;

                velocity_ = power_;

                flip_time_ = 2f * velocity_.y / -gravity_;

                while (Parent.degree_ > 360f) Parent.degree_ -= 360f;

                // 真下を向くまでの角度 真下は180° + 90
                face_down_degree_per_s_ = (180f + 90f - Parent.degree_) / flip_time_;

                EffectPlayer.Play(dead_eff_, Parent.transform.position, Vector3.zero, Parent.transform);

                //Parent.body_.DOFade(0.25f, 1.5f);
            }

            public override void Proc()
            {
                if(Timer < flip_time_)
                {
                    Parent.degree_ += face_down_degree_per_s_ * Time.fixedDeltaTime;
                    Parent.transform.localEulerAngles = new Vector3(0f, 0f, Parent.degree_ - 90f);
                }

                if(Parent.transform.position.y < ground_boader_)
                {
                    ChangeState((int)eState.Dead2);
                    return;
                }

                velocity_.y += gravity_ * Time.fixedDeltaTime;

                Parent.transform.position += (Vector3)velocity_ * Time.fixedDeltaTime;
            }
        }

        // 復活待機中のステート
        [System.Serializable]
        private class DeadState2 : StateMachine<ArmController>.StateBase
        {
            [SerializeField]
            private float revive_time_ = 5.0f;

            public override void Proc()
            {
                if (Timer > revive_time_)
                {
                    ChangeState((int)eState.Revive1);
                }
            }
        }

        [System.Serializable]
        private class ReviveState1 : StateMachine<ArmController>.StateBase
        {
            [SerializeField]
            private int revived_hp_ = 5;

            [SerializeField]
            private BaseParticle revive_eff_;

            [SerializeField]
            private float delay_ = 3.0f;

            public override void OnStart()
            { 
                Parent.HP = revived_hp_;

                EffectPlayer.Play(revive_eff_, Parent.transform.position, Vector3.zero);
            }

            public override void Proc()
            {
                if(Timer > delay_)
                {
                    ChangeState((int)eState.Revive2);
                    return;
                }
            }

            public override void OnEnd()
            {
                //Parent.body_.DOFade(1.0f, 0.5f);
            }
        }

        // めちゃくちゃ回転しながら戻る
        [System.Serializable]
        private class ReviveState2 : StateMachine<ArmController>.StateBase
        {
            [SerializeField]
            private float back_duration_ = 1.0f;
            [SerializeField]
            private float speed_ = 360.0f;

            private float target_degree_;

            private Vector3 from_;

            public override void OnStart()
            {
                target_degree_ = (360f / Parent.arm_sum_) * Parent.index_;
                from_ = Parent.transform.position;

                Parent.hit_box_.enabled = false;
            }

            public override void Proc()
            {
                if (Timer >= back_duration_)
                {
                    ChangeState((int)eState.Idle);
                    return;
                }
                Parent.degree_ += speed_ * Time.fixedDeltaTime;
                Parent.transform.localEulerAngles = new Vector3(0f, 0f, Parent.degree_);

                Vector3 to = Parent.boss_.position + 3.0f * new Vector3(Mathf.Cos(target_degree_ * Mathf.Deg2Rad), Mathf.Sin(target_degree_ * Mathf.Deg2Rad), 0f);
                Parent.transform.position = to * (Timer / back_duration_) + from_ * (1f - Timer / back_duration_);
            }

            public override void OnEnd()
            {
                Parent.degree_ = target_degree_;
                Parent.transform.localEulerAngles = new Vector3(0f, 0f, Parent.degree_);

                Parent.dead_ = false;

                Parent.hit_box_.enabled = true;
            }
        }

        [System.Serializable]
        private class StretchState : StateMachine<ArmController>.StateBase
        {
            [SerializeField]
            private float stretch_duration_ = 0.5f;

            [SerializeField]
            private Ease ease_ = Ease.OutQuart;

            [SerializeField]
            private float distance_ = 1.5f;

            public override void OnStart()
            {
                Parent.StartCoroutine(Flow());
            }

            public override void Proc()
            {
                
            }

            public override void OnEnd()
            {
                Parent.StopCoroutine(Flow());
            }

            private IEnumerator Flow()
            {
                Parent.ChargeStart();

                yield return new WaitForSeconds(1.0f);

                Vector3 to = Parent.transform.position + new Vector3(Mathf.Cos(Parent.degree_ * Mathf.Deg2Rad), Mathf.Sin(Parent.degree_ * Mathf.Deg2Rad), 0f) * distance_;
                Vector3 from = Parent.transform.position;

                Parent.transform.DOMove(to, stretch_duration_).SetEase(ease_);

                yield return new WaitForSeconds(stretch_duration_ + 0.1f);

                if (!Parent.NotActive())
                {
                    Parent.transform.DOMove(from, stretch_duration_).SetEase(ease_);
                }
                Parent.ChargeEnd();

                yield return new WaitForSeconds(stretch_duration_ + 0.1f);

                if(!Parent.dead_ && !Parent.is_burst_)
                    ChangeState((int)eState.Idle);
            }
        }

        // 敵の方向を向く
        [System.Serializable]
        private class ThrowState1 : StateMachine<ArmController>.StateBase
        {
            private float face_target_degree_per_s_;

            [SerializeField]
            private float face_time_ = 0.5f;

            public override void OnStart()
            {
                Vector2 dir = Parent.player_.position - Parent.transform.position;
                float target_degree = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                face_target_degree_per_s_ = (target_degree - Parent.degree_) / face_time_;
            }

            public override void Proc()
            {
                if(Timer > face_time_)
                {
                    ChangeState((int)eState.Throw2);
                    return;
                }

                Parent.degree_ += face_target_degree_per_s_ * Time.fixedDeltaTime;
                Parent.transform.localEulerAngles = new Vector3(0f, 0f, Parent.degree_ - 90f);
            }
        }

        [System.Serializable]
        private class ThrowState2 : StateMachine<ArmController>.StateBase
        {
            [SerializeField]
            private float speed_ = 1.0f;
            [SerializeField]
            private float accel_ = 0.1f;

            private Vector2 velocity_;

            [SerializeField]
            private Vector2 stage_boader_x = new Vector2(-5.0f, 15.0f);
           
            [SerializeField]
            private Vector2 stage_boader_y = new Vector2(-5.0f, 5.0f);

            public override void OnStart()
            {
                velocity_ = Vector2.zero;
            }

            public override void Proc()
            {
                Vector2 dir = Parent.player_.position - Parent.transform.position;
                float target_degree = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                velocity_ += accel_ * Time.fixedDeltaTime * new Vector2(Mathf.Cos(target_degree * Mathf.Deg2Rad), Mathf.Sin(target_degree * Mathf.Deg2Rad));

                Parent.transform.position += (Vector3)velocity_ * Time.fixedDeltaTime * 60f;
                target_degree = Mathf.Atan2(velocity_.y, velocity_.x) * Mathf.Rad2Deg;
                Parent.degree_ = target_degree;
                Parent.transform.localEulerAngles = new Vector3(0f, 0f, Parent.degree_ - 90f);

                if (Parent.transform.position.y < stage_boader_y.x || Parent.transform.position.y > stage_boader_y.y || 
                    Parent.transform.position.x < stage_boader_x.x || Parent.transform.position.x > stage_boader_x.y)
                {
                    ChangeState((int)eState.Throw3);
                    return;
                }
            }
        }
        // めちゃくちゃ回転しながら戻る
        [System.Serializable]
        private class ThrowState3 : StateMachine<ArmController>.StateBase
        {
            [SerializeField]
            private float back_duration_ = 1.0f;
            [SerializeField]
            private float speed_ = 360.0f;

            private float target_degree_;

            private Vector3 from_;

            [SerializeField]
            private float rigidy_time_ = 2.0f;

            public override void OnStart()
            {
                target_degree_ = (360f / Parent.arm_sum_) * Parent.index_;
                from_ = Parent.transform.position;
            }

            public override void Proc()
            {
                if (Timer < rigidy_time_) return;
                //Parent.hit_box_.enabled = false;
                if (Timer >= back_duration_ + rigidy_time_)
                {
                    ChangeState((int)eState.Idle);
                    return;
                }
                Parent.degree_ += speed_ * Time.fixedDeltaTime;
                Parent.transform.localEulerAngles = new Vector3(0f, 0f, Parent.degree_);

                Vector3 to = Parent.boss_.position + 3.0f * new Vector3(Mathf.Cos(target_degree_ * Mathf.Deg2Rad), Mathf.Sin(target_degree_ * Mathf.Deg2Rad), 0f);
                float time = Timer - rigidy_time_;
                Parent.transform.position = to * (time / back_duration_) + from_ * (1f - time / back_duration_);
            }

            public override void OnEnd()
            {
                Parent.degree_ = target_degree_;
                Parent.transform.localEulerAngles = new Vector3(0f, 0f, Parent.degree_);

                //Parent.hit_box_.enabled = true;
            }
        }

        // 千手観音が死んだときに手が弾け飛ぶステート
        [System.Serializable]
        private class BurstState1 : StateMachine<ArmController>.StateBase
        {
            private float face_target_degree_per_s_;

            [SerializeField]
            private float face_time_ = 0.5f;

            [SerializeField]
            private Vector2 stage_boader_x = new Vector2(-5.0f, 15.0f);

            [SerializeField]
            private Vector2 stage_boader_y = new Vector2(-5.0f, 5.0f);

            public override void OnStart()
            {
                float target_degree = Parent.degree_ - 90f;

                face_target_degree_per_s_ = (target_degree - Parent.degree_) / face_time_;

                Parent.is_burst_ = true;

                Parent.transform.DOKill();
            }

            public override void Proc()
            {
                if (Timer > face_time_)
                {
                    ChangeState((int)eState.Burst2);
                    return;
                }

                if (Parent.transform.position.y < stage_boader_y.x || Parent.transform.position.y > stage_boader_y.y ||
                    Parent.transform.position.x < stage_boader_x.x || Parent.transform.position.x > stage_boader_x.y)
                {
                    return;
                }

                Parent.degree_ += face_target_degree_per_s_ * Time.fixedDeltaTime;
                Parent.transform.localEulerAngles = new Vector3(0f, 0f, Parent.degree_);
            }
        }

        // 千手観音が死んだときに手が弾け飛ぶステート
        [System.Serializable]
        private class BurstState2 : StateMachine<ArmController>.StateBase
        {
            [SerializeField]
            private float accel_ = 0.1f;

            [SerializeField]
            private Vector2 stage_boader_x = new Vector2(-5.0f, 15.0f);

            [SerializeField]
            private Vector2 stage_boader_y = new Vector2(-5.0f, 5.0f);

            private Vector2 velocity_ = new Vector2(0f, 0f);

            public override void OnStart()
            {
                // 進む方向

            }

            public override void Proc()
            {
                float target_degree = Parent.degree_ + 90f;
                velocity_ += accel_ * Time.fixedDeltaTime * new Vector2(Mathf.Cos(target_degree * Mathf.Deg2Rad), Mathf.Sin(target_degree * Mathf.Deg2Rad));

                if (OutOfRange())
                {
                    return;
                }

                Parent.transform.position += (Vector3)velocity_ * Time.fixedDeltaTime;
            }

            private bool OutOfRange()
            {
                if (Parent.transform.position.y < stage_boader_y.x && velocity_.y < 0f) return true;
                if (Parent.transform.position.y > stage_boader_y.y && velocity_.y > 0f) return true;
                if (Parent.transform.position.x < stage_boader_x.x && velocity_.x < 0f) return true;
                if (Parent.transform.position.x > stage_boader_x.y && velocity_.x > 0f) return true;
                return false;
            }
        }

        // 千手観音戦の初めに手が吹き飛ぶステート
        [System.Serializable]
        private class InitialBurstState : StateMachine<ArmController>.StateBase
        {
            private Vector2 velocity_ = new Vector2(0f, 0f);

            [SerializeField]
            private Vector2 power_ = new Vector2(0f, 1f);

            [SerializeField]
            private float gravity_ = -0.98f;

            // 反転するまでの時間 上に上がってもとに位置に戻るまでの時間
            private float flip_time_;

            private float face_down_degree_per_s_;

            [SerializeField]
            private float ground_boader_ = -5f;

            [SerializeField]
            private BaseParticle dead_eff_;

            public override void OnStart()
            {
                Parent.dead_ = true;

                velocity_ = power_;

                flip_time_ = 2f * velocity_.y / -gravity_;

                while (Parent.degree_ > 360f) Parent.degree_ -= 360f;

                // 真下を向くまでの角度 真下は180° + 90
                face_down_degree_per_s_ = (180f - Parent.degree_) / flip_time_;

                EffectPlayer.Play(dead_eff_, Parent.transform.position, Vector3.zero, Parent.transform);

                //Parent.body_.DOFade(0.25f, 1.5f);
            }

            public override void Proc()
            {
                if (Timer < flip_time_)
                {
                    Parent.degree_ += face_down_degree_per_s_ * Time.fixedDeltaTime;
                    Parent.transform.localEulerAngles = new Vector3(0f, 0f, Parent.degree_);
                }

                if (Parent.transform.position.y < ground_boader_)
                {
                    //ChangeState((int)eState.Dead2);
                    // 終了
                    Destroy(Parent.gameObject);
                    return;
                }

                velocity_.y += gravity_ * Time.fixedDeltaTime;

                Parent.transform.position += (Vector3)velocity_ * Time.fixedDeltaTime;
            }
        }

        // ===================================================================
    }
} // namespace Actor.Enemy.Thousand