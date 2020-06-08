using Actor.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// どの扉から移動したか，などを保持する
/// また，シーン遷移の際にフェードインを行う
/// </summary>

namespace TadaScene
{
    // 座標情報 セーブロードできるよう，Vector3ではなく自作クラスで格納
    [System.Serializable]
    public class Position
    {
        public float x, y;
        // この座標情報が有効か
        public bool InValid = false;
        public Position()
        {
            x = 0;
            y = 0;
            InValid = false;
        }
        public Position(float _x, float _y)
        {
            x = _x;
            y = _y;
            InValid = false;
        }
    }

    [System.Serializable]
    public class SceneData : TadaLib.Save.BaseSaver<SceneData>
    {
        public Dictionary<string, Position> dict_;

        public List<string> keys_;
        public List<Position> values_;

        private const string kFileName = "Scene";

        public SceneData()
        {
            dict_ = new Dictionary<string, Position>();
            keys_ = new List<string>();
            values_ = new List<Position>();
        }

        // 辞書に新しい座標情報を登録する
        public void Register(string key, Vector3 value)
        {
            if (dict_.ContainsKey(key))
            {
                // 更新する
                var pos = dict_[key];
                pos.x = value.x;
                pos.y = value.y;
                pos.InValid = false;
            }
            else
            {
                keys_.Add(key);
                Position new_pos = new Position(value.x, value.y);
                values_.Add(new_pos);
                dict_.Add(key, new_pos);
            }
            // セーブする
            Save();
        }

        // 初期化する ロードできないならfalseを返す
        public bool Load()
        {
            SceneData data = Load(kFileName);

            if (data == null || data.keys_ == null || data.values_ == null || data.keys_.Count != data.values_.Count)
            {
                // 正常にロードできなかった
                return false;
            }
            else
            {
                // 辞書型を形成
                //UnityEngine.Assertions.Assert.IsTrue(data.keys_.Count == data.values_.Count);

                values_ = data.values_;
                keys_ = data.keys_;

                for (int i = 0, n = data.keys_.Count; i < n; ++i)
                {
                    dict_.Add(keys_[i], values_[i]);
                }

                return true;
            }
        }

        // セーブする
        public void Save()
        {
            if (save_completed_)
            {
                save_completed_ = false;
                TadaLib.Save.SaveManager.Instance.RequestSave(() => { Save(kFileName); save_completed_ = true; });
            }
        }

        // データを削除する
        public void DeleteSaveData()
        {
            TadaLib.Save.SaveManager.Instance.DeleteData(kFileName);
            dict_.Clear();
            keys_.Clear();
            values_.Clear();
        }
    }

    public class TadaSceneManager : TadaLib.SingletonMonoBehaviour<TadaSceneManager>
    {
        private SceneData data_;

        protected override void Awake()
        {
            base.Awake();

            data_ = new SceneData();
            data_.Load();
        }

        // シーン遷移をする シーン遷移した座標を記録する
        public static void LoadScene(string next_scene, float feed_duration, Vector3 pos, bool spawn_default_pos = false, int mask = 0)
        {
            string cur_scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            Instance.data_.Register(cur_scene, pos);

            // すでに登録された座標情報を無効にし，そのシーンに遷移してもデフォルトの座標にプレイヤーを出現させる
            if (spawn_default_pos && Instance.data_.dict_.ContainsKey(next_scene)) Instance.data_.dict_[next_scene].InValid = true;
            KoitanLib.FadeManager.FadeIn(feed_duration, next_scene, mask);
        }

        // 前回シーン遷移したときの座標を取得する
        public static Vector3 GetPrevPosition()
        {
            string cur_scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (Instance.data_.dict_.ContainsKey(cur_scene) && !Instance.data_.dict_[cur_scene].InValid) { 
                var pos = Instance.data_.dict_[cur_scene];
                return new Vector3(pos.x, pos.y, 0f);

            } 
            else {
                //UnityEngine.Assertions.Assert.IsTrue(false, "このシーンでは前回の座標が登録されていません");
                return Vector3.zero;
            }
        }

        // セーブデータを削除する
        public static void DeleteSaveData()
        {
            Instance.data_.DeleteSaveData();
        }
    }
}