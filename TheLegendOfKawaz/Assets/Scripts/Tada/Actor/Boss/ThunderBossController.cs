using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TadaLib;

/// <summary>
/// 電気属性のボス
/// ボスクラスの書き方はBaseBossControlerを見てね
/// </summary>

namespace Actor.Enemy
{
    [RequireComponent(typeof(TadaRigidbody))]
    public class ThunderBossController : MonoBehaviour
    {
        // Bossのステート一覧
        private enum eState
        {
            Think, // 次の行動を考えるステート
            Idle, // 待機中のステート アイドリング
            Damage, // ダメージを受けたときのステート

            // 以下，任意の行動 それぞれのボスに合わせて実装する
            Action1,
            Action2,
            Action3,
            Action4,
            Action5,
        }

        // ステートマシン
        private StateMachine<ThunderBossController> state_machine_;

        // ステートのインスタンス
        #region state class
        [SerializeField]
        private StateThink state_think_;
        [SerializeField]
        private StateIdle state_idle_;
        [SerializeField]
        private StateDamage state_damage_;
        [SerializeField]
        private StateAction1 state_action1_;
        [SerializeField]
        private StateAction2 state_action2_;
        [SerializeField]
        private StateAction3 state_action3_;
        [SerializeField]
        private StateAction4 state_action4_;
        [SerializeField]
        private StateAction5 state_action5_;
        #endregion

        // 物理演算 trb_.Velocityをいじって移動する
        private TadaRigidbody trb_;

        private void Start()
        {
            trb_ = GetComponent<TadaRigidbody>();

            // ステートマシンのメモリ確保 自分自身を渡す
            state_machine_ = new StateMachine<ThunderBossController>(this);

            // ステートを登録
            state_machine_.AddState((int)eState.Think, state_think_);
            state_machine_.AddState((int)eState.Idle, state_idle_);
            state_machine_.AddState((int)eState.Damage, state_damage_);
            state_machine_.AddState((int)eState.Action1, state_action1_);
            state_machine_.AddState((int)eState.Action2, state_action2_);
            state_machine_.AddState((int)eState.Action3, state_action3_);
            state_machine_.AddState((int)eState.Action4, state_action4_);
            state_machine_.AddState((int)eState.Action5, state_action5_);

            // 初期ステートを設定
            state_machine_.SetInitialState((int)eState.Think);

            // デバッグ表示
            DebugBoxManager.Display(this).SetSize(new Vector2(500, 400));
        }

        private void Update()
        {
            state_machine_.Proc();
        }

        public override string ToString()
        {
            return "(" + trb_.Velocity.x.ToString("F2") + ", " + trb_.Velocity.y.ToString("F2") + ")" +
                "\nState : " + state_machine_.ToString();
        }


        // ===== 以下，ステートクラス =============================================================================

        // 説明
        // OnStart() ： ステートが切り替わったタイミングで呼ばれる
        // OnProc() ： 毎フレーム呼ばれる
        // OnEnd() ： ステートが終了するタイミングで呼ばれる

        // ChangeState(int key) ： ステートを変更するときに使う
        // Timer ： ステートが開始してからの経過時間を取得できる
        // Parent ： ボスの本体クラスにアクセスできる
        //           例えば，Parent.trb_.Velocityをいじることで，移動ができる

        // Inspector について
        // [SerializeField] をつけて変数宣言すると，Inspectorに表示される
        // デフォルトで，加速度と移動速度の最大値などをInspectorでいじれる

        // ========================================================================================================

        // 次の行動を思考するステート
        [System.Serializable]
        private class StateThink : StateMachine<ThunderBossController>.StateBase
        {
            // 開始時に呼ばれる
            public override void OnStart()
            {
                if (Random.value > 0.5f) ChangeState((int)eState.Idle);
                else ChangeState((int)eState.Action1);
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {

            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }
        }

        // アイドリング状態
        [System.Serializable]
        private class StateIdle : StateMachine<ThunderBossController>.StateBase
        {
            // 開始時に呼ばれる
            public override void OnStart()
            {

            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if (Timer > 3.0f) ChangeState((int)eState.Think);
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }
        }

        // ダメージを受けたときの状態
        [System.Serializable]
        private class StateDamage : StateMachine<ThunderBossController>.StateBase
        {
            // 開始時に呼ばれる
            public override void OnStart()
            {

            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if (Timer > 3.0f) ChangeState((int)eState.Think);
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }
        }

        // 行動1状態
        [System.Serializable]
        private class StateAction1 : StateMachine<ThunderBossController>.StateBase
        {
            // 開始時に呼ばれる
            public override void OnStart()
            {

            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if (Timer > 3.0f) ChangeState((int)eState.Action2);
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }
        }

        // 行動2状態
        [System.Serializable]
        private class StateAction2 : StateMachine<ThunderBossController>.StateBase
        {
            // 開始時に呼ばれる
            public override void OnStart()
            {

            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if (Timer > 3.0f) ChangeState((int)eState.Think);
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }
        }

        // 行動3状態
        [System.Serializable]
        private class StateAction3 : StateMachine<ThunderBossController>.StateBase
        {
            // 開始時に呼ばれる
            public override void OnStart()
            {

            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {

            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }
        }

        // 行動4状態
        [System.Serializable]
        private class StateAction4 : StateMachine<ThunderBossController>.StateBase
        {
            // 開始時に呼ばれる
            public override void OnStart()
            {

            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {

            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }
        }

        // 行動5状態
        [System.Serializable]
        private class StateAction5 : StateMachine<ThunderBossController>.StateBase
        {
            // 開始時に呼ばれる
            public override void OnStart()
            {

            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {

            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }
        }
    }
} // namespace Actor.Enemy