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
    public enum eParam
    {
        HP, // 体力
        Speed, // 基礎スピード
        Attack, // 攻撃力
        ArialJumpNum, // 空中ジャンプ回数 
        ArialDushNum, // 空中ダッシュ回数
        AutoHeal, // 自動回復
        WallKick, // 壁キック
        ChargeShot, // チャージショット
        ShotNum, // ショット数
    }

    public class Parametors : MonoBehaviour
    {
        public IReadOnlyCollection<int> para;

        // 各ステータスの能力上昇後の値
        private IReadOnlyCollection<int> params_;

        // なんか違うな

        // パラメータを読み込む
        private void LoadParametors()
        {
            List<string[]> raw_data = CSVReader.LoadCSVFile("paramator.csv");
            
        }
    }
} // namespace Actor.Player