using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Player;
using UnityEngine.UI;

/// <summary>
/// スキル購入シーンでの一つのスキルアイテム
/// </summary>

namespace SkillItem
{
    public class SkillItemController : MonoBehaviour
    {
        // スキル本体
        private Skill body_;

        // 画像
        [SerializeField]
        private Image sprite_;
        // アイコンのフレーム
        [SerializeField]
        private Image frame_;
        // スキルレベルのアイコン
        [SerializeField]
        private Image level_icon_prefab_;
        // スキルレベルのアイコンの初期座標
        [SerializeField]
        private Vector3 level_icon_default_pos_ = new Vector3(-50f, -50f, 0f);
        // スキルレベルのアイコンの間隔
        [SerializeField]
        private float level_icon_dist_ = 20.0f;

        // スキルアイコン
        private List<Image> skill_icons_;

        // 有効になった時にスキル情報を取得する
        public void Init(int id, Sprite image, Canvas canvas)
        {
            body_ = SkillManager.Instance.GetSkill(id);
            skill_icons_ = new List<Image>();
            // 現在のスキルレベル
            int current_level = body_.Level;
            for (int i = 0; i < body_.LevelLimit; ++i)
            {
                Image icon = Instantiate(level_icon_prefab_, canvas.transform);
                icon.transform.parent = transform;
                icon.transform.localPosition = level_icon_default_pos_ + new Vector3(level_icon_dist_ * i, 0f, 0f);
                //icon.transform.localScale = Vector3.one;
                skill_icons_.Add(icon);
                if (current_level <= i) skill_icons_[i].color = new Color(0.5f, 0.5f, 0.5f);
            }

            SetImage(image);
            ChangeFrameColor(Color.black);
        }

        // スキルの説明を取得する
        public string Explonation => body_.Explonation;

        // 必要なポイントを取得する
        public int NeedPoint => body_.NeedPoint();

        // アイコンの画像を取得する
        public Sprite Image => sprite_.sprite;

        public string Name => body_.Name;

        // スキルのレベルを上げる
        public void LevelUp()
        {
            body_.LevelUp();
            skill_icons_[body_.Level - 1].color = Color.white;
        }

        // 画像を設定する
        public void SetImage(Sprite sprite)
        {
            sprite_.sprite = sprite;
        }

        // アイコンのフレームの色を変える
        public void ChangeFrameColor(Color color)
        {
            frame_.color = color;
        }
    }
}