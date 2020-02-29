using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// すべての移動する床の基底クラス
/// </summary>

namespace TadaLib
{
    public class Mover : MonoBehaviour
    {
        // 現在の座標
        private Vector2 current_pos_;
        // 1フレーム前の座標
        private Vector2 prev_pos_;

        // 移動量
        public Vector2 Diff => current_pos_ - prev_pos_;

        protected virtual void Start()
        {
            current_pos_ = Vector2.zero;
            prev_pos_ = Vector2.zero;
        }

        protected virtual void Update()
        {
            // 座標の更新
            prev_pos_ = current_pos_;
            current_pos_ = transform.position;
        }
    }
}