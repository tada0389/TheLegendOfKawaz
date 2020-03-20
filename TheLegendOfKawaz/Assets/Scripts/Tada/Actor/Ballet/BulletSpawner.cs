using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bullet;
using KoitanLib;

/// <summary>
/// 弾を生成するクラス
/// 発射するキャラクターと一緒にアタッチする
/// </summary>

namespace Bullet
{
    [System.Serializable]
    public class BulletPrefab
    {
        // 弾のプレハブ
        [SerializeField]
        private BaseBulletController bullet_;
        public BaseBulletController Bullet => bullet_;

        // いくつプーリングするか
        [SerializeField]
        private int max_num_;
        public int MaxNum => max_num_;
    }

    [System.Serializable]
    public class EffectPrefab
    {
        // エフェクトのプレハブ
        [SerializeField]
        private BaseParticle effect_;
        public BaseParticle Effect => effect_;

        // いくつプーリングするか
        [SerializeField]
        private int max_num_;
        public int MaxNum => max_num_;
    }

    public class BulletSpawner : MonoBehaviour
    {
        // あらかじめプーリングしておく弾たち
        [SerializeField]
        private List<BulletPrefab> pre_pooled_bullets_;
        [SerializeField]
        private List<EffectPrefab> pre_pooled_effects_;

        // オブジェクトプールで登録済みの弾
        private List<BaseBulletController> bullets_;

        private void Awake()
        {
            bullets_ = new List<BaseBulletController>();
            foreach(var bullet in pre_pooled_bullets_)
            {
                CreateBulletPool(bullet.Bullet, bullet.MaxNum);
            }
            foreach(var effect in pre_pooled_effects_)
            {
                ObjectPoolManager.Init(effect.Effect, this, effect.MaxNum);
            }
        }

        private void OnDestroy()
        {
            foreach(var bullet in pre_pooled_bullets_)
            {
                ObjectPoolManager.Release(bullet.Bullet);
            }
            foreach(var effect in pre_pooled_effects_)
            {
                ObjectPoolManager.Release(effect.Effect);
            }
        }

        // 弾のオブジェクトプールを生成する
        public void CreateBulletPool(BaseBulletController bullet, int max_num)
        {
            ObjectPoolManager.Init(bullet, this, max_num);
        }

        // 発射する 弾を出せなかったらfalse
        public bool Shot (BaseBulletController bullet, Vector2 pos, Vector2 dir, string opponent_tag, Transform owner = null,
            float speed_rate = 1.0f, float life_time = -1f, float damage_rate = 1f)
        {
            // オブジェクトプールから新しい弾を生成する
            BaseBulletController new_bullet = ObjectPoolManager.GetInstance<BaseBulletController>(bullet);
            if(new_bullet != null)
            {
                new_bullet.Init(pos, dir, opponent_tag, owner, speed_rate, life_time, damage_rate);
                return true;
            }
            return false;
        }

        // 指定した弾を開放する
        public void Release(BaseBulletController bullet)
        {
            ObjectPoolManager.Release(bullet);
            bullets_.Remove(bullet);
        }

        // 全ての弾を開放する
        public void Release()
        {
            foreach(var bullet in bullets_)
            {
                Release(bullet);
            }
        }
    }
}