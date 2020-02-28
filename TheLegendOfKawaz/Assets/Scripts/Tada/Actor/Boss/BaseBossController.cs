using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TadaLib;

/// <summary>
/// すべてのボスに共通するクラス
/// </summary>

namespace Actor.Enemy
{
    public class BaseBossController : MonoBehaviour
    {
        // Bossのステート一覧
        protected enum eState
        {
            Idle, // 待機中のステート アイドリング
            Walk, // 歩いているステート
            Jump, // ジャンプ中のステート
            Damage, // ダメージを受けたときのステート

            // 以下，任意の行動 それぞれのボスに合わせて実装する
            Action1,
            Action2,
            Action3,
            Action4,
            Action5,
        }

        // ステートマシン
        protected StateMachine<BaseBossController> state_machine_;

        protected virtual void Start()
        {
            // ステートマシンのメモリ確保 自分自身を渡す
            state_machine_ = new StateMachine<BaseBossController>(this);
        }

        protected virtual void Proc()
        {
            state_machine_.Proc();
        }
    }
} // namespace Actor.Enemy