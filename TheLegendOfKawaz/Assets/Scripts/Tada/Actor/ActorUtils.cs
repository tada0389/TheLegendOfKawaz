using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Actorクラスに関するヘルパークラス
/// </summary>

namespace Actor
{
    public class ActorUtils : MonoBehaviour
    {
        // 速度に加速度を加える ただし，一定の速度を超えていたら加算しない
        public static void ProcSpeed(ref Vector2 velocity, Vector2 accel, Vector2 max_abs_speed)
        {
            accel *= Time.deltaTime * 60f;
            if (accel.x > 0f && velocity.x < max_abs_speed.x) velocity.x = Mathf.Min(velocity.x + accel.x, max_abs_speed.x);
            else if(accel.x < 0f && velocity.x > -max_abs_speed.x) velocity.x = Mathf.Max(velocity.x + accel.x, -max_abs_speed.x);

            if (accel.y > 0f && velocity.y < max_abs_speed.y) velocity.y = Mathf.Min(velocity.y + accel.y, max_abs_speed.y);
            else if (accel.y < 0f && velocity.y > -max_abs_speed.y) velocity.y = Mathf.Max(velocity.y + accel.y, -max_abs_speed.y);
        }
    }
}