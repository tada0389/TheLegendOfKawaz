using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Enemy;

/// <summary>
/// 電気属性のボス
/// </summary>

namespace Actor.Enemy
{
    public class ThunderBossController : BaseBossController
    {
        //[SerializeField]
        //private StateIdle idle_state_;


        protected override void Start()
        {
            base.Start();

            // ステートを登録する
            //state_machine_.AddState((int)eState.Idle, idle_state_);
            //state_machine_.AddState((int)eState.Walk, walk_state_);
            //state_machine_.AddState((int)eState.Jump, jump_state_);
            //state_machine_.AddState((int)eState.Damage, damage_state_);
            //state_machine_.AddState((int)eState.Action1, hoge_state_);
            //state_machine_.AddState((int)eState.Action2, piyo_state_);
            //state_machine_.AddState((int)eState.Action3, piyo_state_);
            //state_machine_.AddState((int)eState.Action4, piyo_state_);
            //state_machine_.AddState((int)eState.Action5, piyo_state_);

            // 始めのステートを設定
            state_machine_.SetInitialState((int)eState.Idle);
        }

        protected override void Proc()
        {
            base.Proc();
        }
    }
}