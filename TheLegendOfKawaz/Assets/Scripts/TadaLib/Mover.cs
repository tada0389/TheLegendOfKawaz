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
        // 数フレーム前の座標
        private Vector2 prev_pos_;

        public Vector2 Diff => current_pos_ - prev_pos_;
        // 移動量
        public Vector2 GetDiff()
        {
            var res = (Vector2)transform.position - prev_pos_;
            prev_pos_ = transform.position;
            return res;
        }

        protected virtual void Start()
        {
            current_pos_ = Vector2.zero;
            prev_pos_ = Vector2.zero;
        }

        protected virtual void FixedUpdate()
        {
            prev_pos_ = current_pos_;
            current_pos_ = transform.position;
        }
    }
}