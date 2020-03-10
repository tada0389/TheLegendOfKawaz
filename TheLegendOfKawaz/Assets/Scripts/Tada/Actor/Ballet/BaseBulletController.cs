using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 弾のすべての基底クラス
/// </summary>

namespace Bullet
{
    public abstract class BaseBulletController : MonoBehaviour
    {
        // 実際に移動する対称
        [SerializeField]
        protected GameObject move_body_;
        // 初期化する 発射位置と方向を得る
        public abstract void Init(Vector2 pos, Vector2 dir, int damage, string opponent_tag, float init_speed = 1.0f, float life_time = -1f);
        // 移動する
        protected abstract void Move();
    }
}