using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor;
using TadaLib;

namespace Actor.Enemy.Thousand
{
    public class ArmController : BaseActorController
    {
        [SerializeField]
        private float arive_time_ = 5.0f;

        private Timer arive_timer_;

        private void Awake()
        {
            arive_timer_ = new Timer(arive_time_);
        }

        public override void Damage(int damage)
        {
            HP = Mathf.Max(0, HP - damage);
            if (HP == 0) Dead();
        }

        // 蘇生
        private void Alive()
        {

        }

        // 死亡
        private void Dead()
        {

        }
    }
}