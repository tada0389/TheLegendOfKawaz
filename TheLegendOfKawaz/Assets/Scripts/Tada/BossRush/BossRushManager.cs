using Actor.Player;
using KoitanLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ボスラッシュを管理するクラス

namespace BossRush
{
    [System.Serializable]
    public class BossRushData
    {
        // ボスシーン名
        [field: SerializeField]
        public string SceneName { private set; get; }

        // 受け取れるコイン
        [field: SerializeField]
        public int ObtainSkillPoint { private set; get; }

        // ボスの画像
        [field: SerializeField]
        public Sprite BossImage { private set; get; }
    }

    public class BossRushManager : TadaLib.SingletonMonoBehaviour<BossRushManager>
    {
        [SerializeField]
        private List<BossRushData> boss_data_;

        [SerializeField]
        private Vector3 skill_point_spawner_pos_ = Vector3.zero;

        // 現在の階層
        private int cur_index_;

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        // 次のボスステージへと進む
        public void TransitionBossScene()
        {
            FadeManager.FadeIn(0.5f, boss_data_[cur_index_ - 1].SceneName);
        }

        // 待合室に戻る
        public void TransitionWaitScene(bool is_clear)
        {
            if (is_clear)
            {
                // 報酬獲得
                SkillManager.Instance.GainSkillPoint(boss_data_[cur_index_].ObtainSkillPoint, skill_point_spawner_pos_);
            }
            else
            {

            }
        }

        // ボスラッシュのデータを初期化する
        private void Initialize()
        {
            cur_index_ = 0;
        }

        // このクラスを削除する
        public void Delete()
        {
            Destroy(gameObject);
        }
    }
}