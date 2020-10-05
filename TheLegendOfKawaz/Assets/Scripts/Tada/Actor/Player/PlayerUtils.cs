using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーに関する便利クラス
/// </summary>

namespace Actor.Player
{
    public class PlayerUtils : MonoBehaviour
    {
        // スキルの値を取得する
        public static SkillType GetSkillValue(List<Skill> skills, eSkill skill, int add_level = 0)
        {
            UnityEngine.Assertions.Assert.IsTrue(add_level >= 0, "add_levelは0以上の値にしてください");
            return new SkillType(skills, skill, add_level);
        }

        // スキルの型をデフォルトのint型から変換させる
        public class SkillType
        {
            private List<Skill> skills_;
            private eSkill skill_;
            private int add_level_;

            public SkillType(List<Skill> skills, eSkill skill, int add_level)
            {
                skills_ = skills;
                skill_ = skill;
                add_level_ = add_level;
            }

            public static implicit operator int(SkillType value)
            {
                int ret = value.skills_[(int)value.skill_].NextsValue(value.add_level_);
                return ret;
            }

            public static implicit operator float(SkillType value)
            {
                int ret = value.skills_[(int)value.skill_].NextsValue(value.add_level_);
                return ret / 100.0f;
            }

            public static implicit operator bool(SkillType value)
            {
                int ret = value.skills_[(int)value.skill_].NextsValue(value.add_level_);
                return ret == 1;
            }
        }
    }
}