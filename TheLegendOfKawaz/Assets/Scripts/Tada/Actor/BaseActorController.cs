using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アクターに共通する要素を持つ
/// </summary>

namespace Actor
{
    public abstract class BaseActorController : MonoBehaviour
    {
        [SerializeField]
        private int hp_ = 20;
        public int HP { protected set { hp_ = value; } get { return hp_; } }

        // ダメージを受ける
        public abstract void Damage(int damage);
    }
}