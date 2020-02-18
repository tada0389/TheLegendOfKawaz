﻿using System.Collections;
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
    public class Param
    {
        // パラメータの名前
        public string Name { private set; get; }
        // 現在の能力値
        public int Value { private set; get; }
        // レベル
        public int Level { private set; get; }
        // レベルごとの能力値と必要なポイント Item1に能力値，Item2に必要なポイント
        public IReadOnlyList<System.Tuple<int, int>> Sheet { private set; get; }

        public Param(string name, List<System.Tuple<int, int>> list)
        {
            Name = name;
            Sheet = list.AsReadOnly();
            Value = Sheet[0].Item1;
            Level = 0;
        }

        // レベルを1つ上げる できなければfalseを返す
        public bool LevelUp()
        {
            // もうレベルアップできない
            if (Level + 1 >= Sheet.Count) return false;

            Value = Sheet[Level + 1].Item1;
            ++Level;

            return true;
        }

        // 次に必要なポイント もうレベルを上げれないなら -1
        public int NeedPoint()
        {
            if (Level + 1 >= Sheet.Count) return -1;

            return Sheet[Level + 1].Item2;
        }

        public override string ToString() => Name  +
            " Level : " + Level.ToString() +
            " Value : " + Value.ToString();
    }

    public class Parametors : MonoBehaviour
    {
        // 各種パラメータ
        public List<Param> Params { private set; get; }

        public Parametors(string file_name = "PlayerParams")
        {
            Params = new List<Param>();
            LoadParametors(Params, file_name);
        }

        // パラメータを読み込む
        private void LoadParametors(List<Param> out_param, string file_name)
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
                // パラメータ登録
                out_param.Add(new Param(name, list));
            }
        }
    }
} // namespace Actor.Player