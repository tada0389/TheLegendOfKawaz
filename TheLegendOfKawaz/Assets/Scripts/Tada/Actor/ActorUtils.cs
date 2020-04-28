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
        public static void ProcSpeed(ref Vector2 velocity, Vector2 accel, Vector2 max_abs_speed, float friction = 1.0f)
        {
            accel *= Time.deltaTime * 60f;
            // 加速度を加算するか
            //if (accel.x > 0f) velocity.x = Mathf.Min(velocity.x + accel.x, max_abs_speed.x); 
            //if (accel.x < 0f) velocity.x = Mathf.Max(velocity.x + accel.x, -max_abs_speed.x); 
            //if (accel.x > 0f) velocity.y = Mathf.Min(velocity.y + accel.y, max_abs_speed.y); 
            //if (accel.x < 0f) velocity.y = Mathf.Max(velocity.y + accel.y, -max_abs_speed.y);

            velocity += accel;

            float rate = Mathf.Pow(0.93f, Time.deltaTime / 0.016666f * friction);
            if (velocity.x > max_abs_speed.x) velocity.x = Mathf.Max(velocity.x * rate, max_abs_speed.x);
            if (velocity.x < -max_abs_speed.x) velocity.x = Mathf.Min(velocity.x * rate, -max_abs_speed.x);
            if (velocity.y > max_abs_speed.y) velocity.y = Mathf.Max(velocity.y * rate, max_abs_speed.y);
            if (velocity.y < -max_abs_speed.y) velocity.y = Mathf.Min(velocity.y * rate, -max_abs_speed.y);
            //velocity.x = Mathf.Clamp(velocity.x, -max_abs_speed.x, max_abs_speed.x);
            //velocity.y = Mathf.Clamp(velocity.y, -max_abs_speed.y, max_abs_speed.y);
            //if (accel.x > 0f && velocity.x < max_abs_speed.x) velocity.x = Mathf.Min(velocity.x + accel.x, max_abs_speed.x);
            //else if(accel.x < 0f && velocity.x > -max_abs_speed.x) velocity.x = Mathf.Max(velocity.x + accel.x, -max_abs_speed.x);

            //if (accel.y > 0f && velocity.y < max_abs_speed.y) velocity.y = Mathf.Min(velocity.y + accel.y, max_abs_speed.y);
            //else if (accel.y < 0f && velocity.y > -max_abs_speed.y) velocity.y = Mathf.Max(velocity.y + accel.y, -max_abs_speed.y);
        }

        // エフェクトを生成する オブジェクトプール使いたいので仮
        public static void CreateEffect(ParticleSystem effect, Vector2 pos, Vector2 dir, float life_time)
        {
            var eff = Instantiate(effect, pos, Quaternion.identity);
            eff.transform.localEulerAngles = new Vector3(0f, Mathf.Sign(dir.x) * 90f - 90f, 0f);
            //eff.gameObject.SetActive(true);
            eff.Play();
            Destroy(eff.gameObject, life_time);
        }
    }
}