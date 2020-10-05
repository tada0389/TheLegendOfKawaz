using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Player;
using TadaLib;
using UnityEngine.InputSystem.Utilities;

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

    public class SkillData : TadaLib.Save.BaseSaver<SkillData>
    {
        //// 現在保持しているスキルレベル
        //[SerializeField]
        //private List<Skill> skills_;
        //public List<Skill> Skill => skills_;
        public List<Skill> Skills;

        //// 現在の保持スキルコイン
        //[SerializeField]
        //private int skill_point_;
        //public int SkillPoint => skill_point_;
        public int SkillPoint;

        private const string kFileName = "Skill";

        public bool Load()
        {
            SkillData data = Load(kFileName);
            if (data == null)
            {
                Debug.Log("データ読み込み失敗");
                return false;
            }
            else
            {
                // スキルレベルだけレベルアップ
                int cnt = 0;
                foreach(var skill in data.Skills)
                {
                    for(int i = 0; i < skill.Level; ++i)
                    {
                        Skills[cnt].LevelUp();
                    }
                    ++cnt;
                }
                SkillPoint = data.SkillPoint;
                return true;
            }
        }

        // セーブリクエストを送る
        public void Save()
        {
            if (save_completed_)
            {
                save_completed_ = false;
                TadaLib.Save.SaveManager.Instance.RequestSave(() => { Save(kFileName); save_completed_ = true; });
            }
        }

        public void DeleteSaveData()
        {
            TadaLib.Save.SaveManager.Instance.DeleteData(kFileName);
        }
    }

    public class SkillManager : SingletonMonoBehaviour<SkillManager>
    {
        [SerializeField]
        [HideInInspector]
        private SkillData data_;

        // パラメータ情報
        public List<Skill> Skills => data_.Skills;
        public int SkillPoint => data_.SkillPoint;


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

            data_ = new SkillData();
            PlayerSkills reader = new PlayerSkills(file_name_);
            data_.Skills = new List<Skill>(reader.Skills);
            // セーブデータがあるならそれを呼び出す
            if (!data_.Load())
            {
                // ポイントをゼロに
                data_.SkillPoint = initial_skill_point_;
            }

            PlayerSkills reader_2 = new PlayerSkills(file_name_); // Skillが参照渡しになってるからもう一度...
            LevelOneSkills = new List<Skill>(reader_2.Skills);
        }

        // 指定したパラメータを取得する
        public Skill GetSkill(int id) => Skills[id];

        // 指定したパラメータのレベルを上げる
        public bool LevelUp(int id) {
            bool ret = Skills[id].LevelUp(); 
            data_.Save();
            return ret;
        }

        // スキルポイントを獲得する
        public void GainSkillPoint(int point, Vector3 point_spawner_pos, float time_scale = 1.0f)
        {
            if (time_scale < 1e-6) return;
            skill_point_ctrl_.GainSkillPoint(point, point_spawner_pos, time_scale);
            data_.SkillPoint += point;
            data_.Save();
        }

        // スキルポイントを消費する できないならfalse
        public void SpendSkillPoint(int point, float time_scale = 1.0f)
        {
            if (time_scale < 1e-6) return;
            skill_point_ctrl_.SpendSkillPoint(point, time_scale);
            data_.SkillPoint = Mathf.Max(0, SkillPoint - point);
            data_.Save();
        }

        // スキルポイントのUIを常に表示する
        public void ShowUIEternal()
        {
            skill_point_ctrl_.ShowUIEternal();
        }

        // スキルポイントのUIの常時表示をキャンセル
        public void CancelUIEternal()
        {
            skill_point_ctrl_.CancelUIEternal();
        }

        // スコアのセーブデータを削除する
        public void DeleteSaveData()
        {
            data_.DeleteSaveData();

            // スキル状態をリセット
            foreach(var skill in data_.Skills)
            {
                skill.LevelReset();
            }
            data_.SkillPoint = initial_skill_point_;
        }

        // 指定したスキルのレベルを返す
        public int GetSkillLevel(eSkill skill)
        {
            var skills = data_.Skills[(int)skill];

            return skills.Level;
        }

        // スキルレベルがすべて1なのか返す
        public bool IsNoSkill()
        {
            bool res = true;
            foreach(var skill in data_.Skills)
            {
                if(skill.Level != 0)
                {
                    res = false;
                    break;
                }
            }
            return res;
        }
    }
} // namespace Actor.Player