using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// csvファイルから内容を読み込むクラス
/// 全て文字列で返す 行ごとにリストで分ける
/// </summary>

namespace TadaLib
{
    public class CSVReader
    {
        // ファイル名に応じたcsvファイルを読み込む
        public static List<string[]> LoadCSVFile(string file_name)
        {
            List<string[]> csv_datas = new List<string[]>();
            TextAsset csv_file = Resources.Load(file_name) as TextAsset;
            StringReader reader = new StringReader(csv_file.text);

            while (reader.Peek() != -1)
            {
                string line = reader.ReadLine();
                csv_datas.Add(line.Split(','));
            }

            return csv_datas;
        }
    }
} // namespace TadaLib