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
        // 初期化する 発射位置と方向を得る
        public abstract void Init(Vector2 pos, Vector2 dir);
        // 移動する
        protected abstract void Move();
    }
}