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
        HP, // 体力
        Speed, // 基礎スピード
        Attack, // 攻撃力
        AirJumpNum, // 空中ジャンプ回数 
        AirDushNum, // 空中ダッシュ回数
        AutoHeal, // 自動回復
        WallKick, // 壁キック
        ChargeShot, // チャージショット
        ShotNum, // ショット数
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
         
        protected override void Awake()
        {
            base.Awake();
            // ポイントをゼロに
            SkillPoint = initial_skill_point_;

            // パラメータを取得
            PlayerSkills reader = new PlayerSkills(file_name_);
            Skills = new List<Skill>(reader.Skills);
        }

        // 指定したパラメータを取得する
        public Skill GetSkill(int id) => Skills[id];

        // 指定したパラメータのレベルを上げる
        public bool LevelUp(int id) => Skills[id].LevelUp();

        // スキルポイントを獲得する
        public void GainSkillPoint(int point, Vector3 point_spawner_pos)
        {
            skill_point_ctrl_.GainSkillPoint(point, point_spawner_pos);
            SkillPoint += point;
        }

        // スキルポイントを消費する できないならfalse
        public bool SpendSkillPoint(int point)
        {
            if (SkillPoint < point) return false;
            SkillPoint -= point;
            return true;
        }
    }
} // namespace Actor.Player