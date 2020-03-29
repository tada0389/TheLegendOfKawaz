using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor;
using TadaLib;
using Bullet;

namespace Actor.Enemy.Purin
{
    [RequireComponent(typeof(TadaRigidbody))]
    [RequireComponent(typeof(BulletSpawner))]
    public partial class PurinBossController : BaseBossController
    {
        private enum eState
        {
            Think,
            Walk,
            Dead,
            Punch,
            Shot,
            Drop,
        }

        private StateMachine<PurinBossController> state_machine_;

        private TadaRigidbody rigidbody_;

        private BulletSpawner bullet_spawner_;

        #region state
        [SerializeField]
        private ThinkState think_state_;
        [SerializeField]
        private WalkState walk_state_;
        [SerializeField]
        private DeadState dead_state_;
        [SerializeField]
        private PunchState punch_state_;
        [SerializeField]
        private ShotState shot_state_;
        [SerializeField]
        private DropState drop_state_;
        #endregion

        // Start is called before the first frame update
        private void Start()
        {
            rigidbody_ = GetComponent<TadaRigidbody>();
            bullet_spawner_ = GetComponent<BulletSpawner>();

            state_machine_ = new StateMachine<PurinBossController>(this);

            // ステートの登録
            state_machine_.AddState((int)eState.Think, think_state_);
            state_machine_.AddState((int)eState.Walk, walk_state_);
            state_machine_.AddState((int)eState.Dead, dead_state_);
            state_machine_.AddState((int)eState.Punch, punch_state_);
            state_machine_.AddState((int)eState.Shot, shot_state_);
            state_machine_.AddState((int)eState.Drop, drop_state_);
            // 初期ステートの設定
            state_machine_.SetInitialState((int)eState.Think);
        }

        // Update is called once per frame
        private void Update()
        {
            // ステートの更新
            state_machine_.Proc();
        }
    }
}