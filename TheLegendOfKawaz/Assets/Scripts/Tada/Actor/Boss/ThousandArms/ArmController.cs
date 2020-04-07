using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor;
using TadaLib;
using DG.Tweening;

namespace Actor.Enemy.Thousand
{
    public class ArmController : BaseBossController
    {
        private enum eState
        {
            Idle,
            Dead1,
            Dead2,
            Revive,
            Stretch,
            Throw,
        }

        private StateMachine<ArmController> state_machine_;

        [SerializeField]
        private Transform boss_;

        [SerializeField]
        private int index_ = 0;
        private int arm_sum_ = 6;

        [SerializeField]
        private SpriteRenderer body_;

        #region state
        [SerializeField]
        private IdleState idle_state_;
        [SerializeField]
        private DeadState1 dead_state1_;
        [SerializeField]
        private DeadState2 dead_state2_;
        [SerializeField]
        private ReviveState revive_state_;
        [SerializeField]
        private StretchState stretch_state_;
        [SerializeField]
        private ThrowState throw_state_;
        #endregion

        private float degree_ = 0f;

        private bool move_stop_ = false;

        private void Start()
        {
            state_machine_ = new StateMachine<ArmController>(this);

            state_machine_.AddState((int)eState.Idle, idle_state_);
            state_machine_.AddState((int)eState.Dead1, dead_state1_);
            state_machine_.AddState((int)eState.Dead2, dead_state2_);
            state_machine_.AddState((int)eState.Revive, revive_state_);
            state_machine_.AddState((int)eState.Stretch, stretch_state_);
            state_machine_.AddState((int)eState.Throw, throw_state_);

            state_machine_.SetInitialState((int)eState.Idle);
        }

        private void Update()
        {
            state_machine_.Proc();
        }

        // 蘇生
        private void Revive()
        {
            state_machine_.ChangeState((int)eState.Revive);
        }

        // 死亡
        protected override void Dead()
        {
            state_machine_.ChangeState((int)eState.Dead1);
        }

        // 外部から呼び出す

        // 腕の動きを止める
        public void Stop(float stop_duration_from_order = 0.5f)
        {
            move_stop_ = true;
        }

        // 腕を赤くする
        public void ChargeStart()
        {
            body_.DOColor(Color.red, 0.5f);
            transform.DOScale(1.5f, 0.5f);
        }

        // 腕を通常通りの色にする
        private void ChargeEnd()
        {
            body_.DOColor(Color.white, 0.5f);
            transform.DOScale(1.0f, 0.5f);
        }

        // 腕を伸ばす
        public void Stretch()
        {
            state_machine_.ChangeState((int)eState.Stretch);
        }

        public void Move()
        {
            state_machine_.ChangeState((int)eState.Idle);
        }

        // === 以下，ステート ================================================

        [System.Serializable]
        private class IdleState : StateMachine<ArmController>.StateBase
        {
            // 半径
            [SerializeField]
            private float radius_ = 5f;

            // ほかの腕との間隔 degree
            private float arm_interval_;

            [SerializeField]
            private float speed_ = 1f;


            public override void OnStart()
            {
                arm_interval_ = 360f / Parent.arm_sum_;
                Parent.move_stop_ = false;
            }

            public override void Proc()
            {
                if (Parent.move_stop_) return;

                Parent.degree_ = arm_interval_ * Parent.index_ + Timer * speed_;
                float degree = Parent.degree_;
                Parent.transform.position = Parent.boss_.position + radius_ * new Vector3(Mathf.Cos(degree * Mathf.Deg2Rad), Mathf.Sin(degree * Mathf.Deg2Rad), 0f);
                Parent.transform.localEulerAngles = new Vector3(0f, 0f, degree - 90f);
            }

            public override void OnEnd()
            {

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

            public override void OnStart()
            {
                velocity_ = power_;

                flip_time_ = velocity_.y / -gravity_;
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
                    ChangeState((int)eState.Revive);
                }
            }
        }

        [System.Serializable]
        private class ReviveState : StateMachine<ArmController>.StateBase
        {

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
                yield return new WaitForSeconds(0.3f);

                Vector3 to = Parent.transform.position + new Vector3(Mathf.Cos(Parent.degree_ * Mathf.Deg2Rad), Mathf.Sin(Parent.degree_ * Mathf.Deg2Rad), 0f) * distance_;
                Vector3 from = Parent.transform.position;

                Parent.transform.DOMove(to, stretch_duration_).SetEase(ease_);

                yield return new WaitForSeconds(stretch_duration_ + 0.1f);

                Parent.transform.DOMove(from, stretch_duration_).SetEase(ease_);

                Parent.ChargeEnd();

                yield return new WaitForSeconds(stretch_duration_ + 0.1f);

                ChangeState((int)eState.Idle);
            }
        }

        [System.Serializable]
        private class ThrowState : StateMachine<ArmController>.StateBase
        {

        }


        // ===================================================================
    }
} // namespace Actor.Enemy.Thousand