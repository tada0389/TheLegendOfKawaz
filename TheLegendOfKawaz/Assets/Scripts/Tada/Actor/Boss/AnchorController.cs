using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TadaLib;
using Bullet;
using Actor.Enemy;

/// <summary>
/// KabtBossControllerが放つアンカー
/// </summary>

namespace Actor.Enemy
{
    public class AnchorController : MonoBehaviour
    {
        // 実際に移動する対称
        [SerializeField]
        private GameObject move_body_;

        [SerializeField]
        private AnchorEffect effect_prefab_;

        // どの間隔でエフェクトを発生させるか
        [SerializeField]
        private float effect_interval_length_ = 1f;

        [SerializeField]
        private float init_speed_ = 0.3f;
        [SerializeField]
        private float init_back_speed_ = 1.5f;

        private Vector2 dir_;
        private bool stoped_;

        private string opponent_tag_;
        private int damage_;
        private float speed_;

        private Vector2 init_pos_;

        private bool back_;

        // 現在の移動距離
        private float move_sum_;

        // 現在置いたエフェクトのリスト
        private List<AnchorEffect> setted_effects_;

        private void Awake()
        {
            setted_effects_ = new List<AnchorEffect>();
        }

        private void Update()
        {
            if(!stoped_) Move();
            if (back_) MoveReverse();
        }

        public void Init(Vector2 pos, Vector2 dir, int damage, string opponent_tag = "Player", Transform target = null, float init_speed = 1.0f)
        {
            transform.position = (Vector3)pos;
            move_body_.transform.position = (Vector3)pos;
            init_pos_ = pos;
            dir_ = dir;
            damage_ = damage;
            opponent_tag_ = opponent_tag;
            speed_ = init_speed_ * init_speed;
            stoped_ = false;
            gameObject.SetActive(true);
            move_sum_ = 0f;
            back_ = false;
        }

        // エフェクトを発火させる
        public void Flush()
        {
            foreach(var eff in setted_effects_)
            {
                eff.Create();
            }
        }

        // 移動を終了させる
        public void Stop()
        {
            stoped_ = true;
        }

        // 終了させる
        public void Finish()
        {
            back_ = true;
        }

        private void Move()
        {
            Vector3 d = (Vector3)dir_ * speed_ * 60f * Time.deltaTime;
            move_body_.transform.position += d;
            move_sum_ += d.magnitude;
            if (move_sum_ > effect_interval_length_ * (1 + setted_effects_.Count)) SetEffect();
        }

        // 今までの移動方向と逆側に進む
        private void MoveReverse()
        {
            Vector3 d = -(Vector3)dir_ * speed_ * 60f * Time.deltaTime * init_back_speed_;
            move_body_.transform.position += d;
            move_sum_ -= d.magnitude;
            if (move_sum_ < effect_interval_length_ * (1 + setted_effects_.Count)) RemoveEffect();
            if (move_sum_ < 0f) Dead();
        }

        // エフェクトを設定する
        private void SetEffect()
        {
            Vector3 pos = init_pos_ + dir_ * (setted_effects_.Count + 1);
            AnchorEffect effect = Instantiate(effect_prefab_, pos, Quaternion.identity);
            setted_effects_.Add(effect);
        }

        private void RemoveEffect()
        {
            if (setted_effects_.Count <= 0) return;
            AnchorEffect effect = setted_effects_[setted_effects_.Count - 1];
            setted_effects_.RemoveAt(setted_effects_.Count - 1);
            Destroy(effect.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.tag == "Stage" || collider.tag == opponent_tag_)
            {
                if (collider.tag == opponent_tag_) collider.GetComponent<Actor.BaseActorController>().Damage(damage_);
            }
        }

        private void Dead()
        {
            foreach(var eff in setted_effects_)
            {
                Destroy(eff.gameObject);
            }
            setted_effects_.Clear();
            gameObject.SetActive(false);
        }

        // エフェクトを生成する オブジェクトプール使いたいので仮
        private void CreateEffect(ParticleSystem effect, Vector3 pos)
        {
            var eff = Instantiate(effect, pos, Quaternion.identity);
            eff.transform.localEulerAngles = new Vector3(0f, Mathf.Sign(dir_.x) * 90f - 90f, 0f);
            eff.gameObject.SetActive(true);
            eff.Play();
            Destroy(eff.gameObject, 2.0f);
        }
    }
}