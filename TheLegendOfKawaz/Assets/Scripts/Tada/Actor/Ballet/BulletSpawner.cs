using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bullet;

/// <summary>
/// 弾を生成するクラス
/// 発射するキャラクターと一緒にアタッチする
/// </summary>

namespace Bullet
{
    public class BulletSpawner : MonoBehaviour
    {
        // 発射する弾のプレハブ
        [SerializeField]
        private BaseBulletController bullet_;

        // 発射できる最大数
        private int max_shot_num_;

        // 弾のオブジェクトプール
        private List<BaseBulletController> bullets_;

        // 現在見ている弾のインデックス
        private int index_;

        // 初期化する ここで何発まで発射できるか決める
        public void Init(int max_shot_num)
        {
            index_ = 0;
            max_shot_num_ = max_shot_num;

            // オブジェクトプールの作成
            bullets_ = new List<BaseBulletController>();
            for(int i = 0; i < max_shot_num; ++i)
            {
                BaseBulletController bullet = Instantiate(bullet_);
                bullet.gameObject.SetActive(false);
                bullets_.Add(bullet);
            }
        }

        // 発射する
        public void Shot(Vector2 pos, Vector2 dir)
        {
            // もしすべて使っていたら撃たない 無駄な処理があるから直したい
            for(int i = 0; i < max_shot_num_; ++i)
            {
                if (!bullets_[index_].gameObject.activeSelf)
                {
                    bullets_[index_].gameObject.SetActive(true);
                    bullets_[index_].Init(pos, dir);
                    index_ = (index_ + 1) % max_shot_num_;
                    return;
                }
                index_ = (index_ + 1) % max_shot_num_;
            }
        }
    }
}