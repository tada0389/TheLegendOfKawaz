using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor;
using TadaLib;

namespace Actor.Enemy.Thousand
{
    public class ArmController : BaseActorController
    {
        private enum eState
        {
            Idle,
            Dead,
            Attack,
        }

        [SerializeField]
        private float revive_time_ = 5.0f;

        private Timer revive_timer_;

        private StateMachine<ArmController> state_machine_;

        [SerializeField]
        private Transform boss_;

        [SerializeField]
        private Transform player_;

        [SerializeField]
        private int index_ = 0;
        private int arm_sum_ = 6;

        [SerializeField]
        private IdleState idle_state_;

        private float degree_ = 0f;

        private void Start()
        {
            state_machine_ = new StateMachine<ArmController>(this);
            revive_timer_ = new Timer(revive_time_);

            state_machine_.AddState((int)eState.Idle, idle_state_);

            state_machine_.SetInitialState((int)eState.Idle);
        }

        private void Update()
        {
            state_machine_.Proc();
        }

        public override void Damage(int damage)
        {
            HP = Mathf.Max(0, HP - damage);
            if (HP == 0) Dead();
        }

        // 蘇生
        private void Revive()
        {

        }

        // 死亡
        private void Dead()
        {

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

            private float rate_ = 0.3f;
            private bool up_ = true;

            public override void OnStart()
            {
                rate_ = 0.3f;
                up_ = true;
                arm_interval_ = 360f / Parent.arm_sum_;
            }

            public override void Proc()
            {
                if (up_)
                {
                    rate_ += Time.deltaTime;
                    if (rate_ > 1.5f)
                    {
                        up_ = false;
                    }
                }
                else
                {
                    rate_ -= Time.deltaTime;
                    if (rate_ < 0.5f)
                    {
                        up_ = true;
                    }
                }
                Parent.degree_ = arm_interval_ * Parent.index_ + Timer * speed_;
                float degree = Parent.degree_;
                Parent.transform.position = Parent.boss_.position + radius_ * rate_ * new Vector3(Mathf.Cos(degree * Mathf.Deg2Rad), Mathf.Sin(degree * Mathf.Deg2Rad), 0f);
                Parent.transform.localEulerAngles = new Vector3(0f, 0f, degree - 90f);
            }

            public override void OnEnd()
            {

            }
        }

        [System.Serializable]
        private class DeadState : StateMachine<ArmController>.StateBase
        {

        }

        [System.Serializable]
        private class AttackState : StateMachine<ArmController>.StateBase
        {

        }


        // ===================================================================
    }
} // namespace Actor.Enemy.Thousand