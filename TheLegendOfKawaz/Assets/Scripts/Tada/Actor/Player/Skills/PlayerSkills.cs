using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Player;
using TadaLib;

/// <summary>
/// Playerのパラメータに関するクラス
/// ここにあるパラメータが強化対象
/// csvファイルからデータを読み込む
/// </summary>

namespace Actor.Player
{ 
    // 一つのステータス
    [System.Serializable]
    public class Skill
    {
        // パラメータの名前
        public string Name { private set; get; }
        // スキルの説明
        public string Explonation { private set; get; }
        // 現在の能力値
        public int Value { private set; get; }
        // レベル
        [SerializeField]
        private int level_;
        public int Level => level_;
        // スキルの最大レベル
        public int LevelLimit => Sheet.Count - 1;
        // スキルレベルが最大化
        public bool ReachLevelLimit => Level == LevelLimit;
        // 次のスキルの値
        public int NextValue { private set; get; }

        // レベルごとの能力値と必要なポイント Item1に能力値，Item2に必要なポイント
        public IReadOnlyList<System.Tuple<int, int>> Sheet { private set; get; }

        public Skill(string name, string explonation, List<System.Tuple<int, int>> list)
        {
            Name = name;
            Explonation = explonation;
            Sheet = list.AsReadOnly();
            Value = Sheet[0].Item1;
            NextValue = (Sheet.Count <= 1) ? -1 : Sheet[1].Item1;
            level_ = 0;
        }

        // レベルを1つ上げる できなければfalseを返す
        public bool LevelUp()
        {
            // もうレベルアップできない
            if (Level + 1 >= Sheet.Count) return false;

            Value = Sheet[Level + 1].Item1;
            ++level_;

            return true;
        }

        // 次に必要なポイント もうレベルを上げれないなら -1
        public int NeedPoint()
        {
            if (Level + 1 >= Sheet.Count) return -1;

            return Sheet[Level + 1].Item2;
        }

        // n次のスキルの値
        public int NextsValue(int add_level)
        {
            int level = Mathf.Clamp(Level + add_level, Level, Sheet.Count - 1);
            return Sheet[level].Item1;
        }

        // レベル情報をリセットする
        public void LevelReset()
        {
            Value = Sheet[0].Item1;
            level_ = 0;
        }

        public override string ToString() => Name  +
            " Level : " + Level.ToString() +
            " Value : " + Value.ToString();
    }

    public class PlayerSkills : MonoBehaviour
    {
        // 各種パラメータ
        public List<Skill> Skills { private set; get; }

        public PlayerSkills(string file_name = "PlayerSkills")
        {
            Skills = new List<Skill>();
            LoadParametors(Skills, file_name);
        }

        // パラメータを読み込む
        private void LoadParametors(List<Skill> out_param, string file_name)
        {
            List<string[]> raw_data = CSVReader.LoadCSVFile(file_name);
            int cnt = -1;
            foreach(string[] data in raw_data)
            {
                ++cnt;
                // 最初の一行は読み込まない
                if (cnt == 0) continue;

                string name = data[0];
                int default_value = int.Parse(data[1]);
                int level_num = int.Parse(data[2]);
                List<System.Tuple<int, int>> list = new List<System.Tuple<int, int>>();
                list.Add(new System.Tuple<int, int>(default_value, 0));
                for(int j = 0; j < level_num; ++j)
                {
                    int value = int.Parse(data[j * 2 + 3]);
                    int need = int.Parse(data[j * 2 + 4]);
                    list.Add(new System.Tuple<int, int>(value, need));
                }
                string explonation = data[level_num * 2 + 3];
                // パラメータ登録
                out_param.Add(new Skill(name, explonation, list));
            }
        }
    }
} // namespace Actor.Player