using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor;
using TadaLib;

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
            Attack,
        }

        private StateMachine<ArmController> state_machine_;

        [SerializeField]
        private Transform boss_;

        [SerializeField]
        private int index_ = 0;
        private int arm_sum_ = 6;

        #region state
        [SerializeField]
        private IdleState idle_state_;
        [SerializeField]
        private DeadState1 dead_state1_;
        [SerializeField]
        private DeadState2 dead_state2_;
        [SerializeField]
        private ReviveState revive_state_;
        #endregion

        private float degree_ = 0f;

        private void Start()
        {
            state_machine_ = new StateMachine<ArmController>(this);

            state_machine_.AddState((int)eState.Idle, idle_state_);
            state_machine_.AddState((int)eState.Dead1, dead_state1_);
            state_machine_.AddState((int)eState.Dead2, dead_state2_);
            state_machine_.AddState((int)eState.Revive, revive_state_);

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
            }

            public override void Proc()
            {
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
        private class AttackState : StateMachine<ArmController>.StateBase
        {

        }


        // ===================================================================
    }
} // namespace Actor.Enemy.Thousand