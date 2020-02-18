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
    public enum eParam
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

    public class ParamManager : SingletonMonoBehaviour<ParamManager>
    {
        // パラメータ情報
        public List<Param> Params { private set; get; }
         
        protected override void Awake()
        {
            base.Awake();

            // パラメータを取得
            Parametors reader = new Parametors("PlayerParams");
            Params = new List<Param>(reader.Params);
        }

        // 指定したパラメータを取得する
        public Param GetParam(eParam id) => Params[(int)id];

        // 指定したパラメータのレベルを上げる
        public bool LevelUp(eParam id) => Params[(int)id].LevelUp();
    }
} // namespace Actor.Player