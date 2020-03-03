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
        public int HP { protected set; get; }

        // ダメージを受ける
        public abstract void Damage(int damage);
    }
}