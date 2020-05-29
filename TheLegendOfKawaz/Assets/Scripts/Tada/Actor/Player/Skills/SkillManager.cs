using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Player;
using TadaLib;

/// <summary>
/// プレイヤーのパラメータを管理するクラス
/// </summary>

namespace Actor.Player
{
    public enum eSkill
    {
        HP = 0, // 体力
        Speed = 1, // 基礎スピード
        Attack = 2, // 攻撃力
        AirJumpNum = 3, // 空中ジャンプ回数 
        AirDushNum = 4, // 空中ダッシュ回数
        AutoHeal = 5, // 自動回復
        WallKick = 6, // 壁キック
        ChargeShot = 7, // チャージショット
        ShotNum = 8, // ショット数
    }

    public class SkillManager : SingletonMonoBehaviour<SkillManager>
    {
        // パラメータ情報
        public List<Skill> Skills { private set; get; }
        public int SkillPoint { private set; get; }

        [SerializeField]
        private int initial_skill_point_ = 500;

        // スキルポイントを管理するクラス
        [SerializeField]
        private SkillUI.GetSkillPointUI skill_point_ctrl_;

        // 使用するcsvファイル名
        [SerializeField]
        private string file_name_ = "PlayerSkills";

        // 何も強化されていないスキル
        public List<Skill> LevelOneSkills { private set; get; }
         
        protected override void Awake()
        {
            base.Awake();
            // ポイントをゼロに
            SkillPoint = initial_skill_point_;

            // パラメータを取得
            PlayerSkills reader = new PlayerSkills(file_name_);
            Skills = new List<Skill>(reader.Skills);

            PlayerSkills reader_2 = new PlayerSkills(file_name_); // Skillが値渡しになってるからもう一度...
            LevelOneSkills = new List<Skill>(reader_2.Skills);
        }

        // 指定したパラメータを取得する
        public Skill GetSkill(int id) => Skills[id];

        // 指定したパラメータのレベルを上げる
        public bool LevelUp(int id) => Skills[id].LevelUp();

        // スキルポイントを獲得する
        public void GainSkillPoint(int point, Vector3 point_spawner_pos, float time_scale = 1.0f)
        {
            if (time_scale < 1e-6) return;
            skill_point_ctrl_.GainSkillPoint(point, point_spawner_pos, time_scale);
            SkillPoint += point;
        }

        // スキルポイントを消費する できないならfalse
        public void SpendSkillPoint(int point, float time_scale = 1.0f)
        {
            if (time_scale < 1e-6) return;
            skill_point_ctrl_.SpendSkillPoint(point, time_scale);
            SkillPoint = Mathf.Max(0, SkillPoint - point);
        }
    }
} // namespace Actor.Player