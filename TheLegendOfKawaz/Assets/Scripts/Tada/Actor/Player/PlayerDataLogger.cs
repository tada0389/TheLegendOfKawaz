using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの座標情報，方向，アニメーションなどのデータを保持する
/// ターゲットを壊せ等でゴーストを作るのに使う
/// </summary>

namespace Actor.Player
{
    public class PlayerDataLogger : MonoBehaviour
    {
        // プレイヤーの座標ログ
        //public List<Vector3> PositionLog { private set; get; }

        // プレイヤーのアニメーションログ firstにアニメーション名 secondに呼び出しタイプ
        public List<TadaLib.Pair<string, eAnimType>> AnimLog { private set; get; }

        // プレイヤーのショットログ
        public List<eShotType> ShotLog { private set; get; }

        public void Start()
        {
            //PositionLog = new List<Vector3>();
            AnimLog = new List<TadaLib.Pair<string, eAnimType>>();
            ShotLog = new List<eShotType>();
        }

        //// ログを追加する
        //public void AddLog(Vector3 pos)
        //{
        //    PositionLog.Add(pos);
        //}

        // ログを追加する
        public void AddLog(string animName, eAnimType type)
        {
            AnimLog.Add(new TadaLib.Pair<string, eAnimType>(animName, type));
        }

        // ログを追加する
        public void AddLog(eShotType type)
        {
            ShotLog.Add(type);
        }

        // ログをリセットする
        public void Reset()
        {
            //PositionLog.Clear();
            AnimLog.Clear();
            ShotLog.Clear();
        }
    }
} // namespace Actor.Player