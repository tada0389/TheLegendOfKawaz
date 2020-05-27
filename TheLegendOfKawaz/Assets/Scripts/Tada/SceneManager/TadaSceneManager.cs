using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// どの扉から移動したか，などを保持する
/// また，シーン遷移の際にフェードインを行う
/// </summary>

namespace TadaScene
{
    public class TadaSceneManager : MonoBehaviour
    {
        private static Dictionary<string, Vector3> dict_ = new Dictionary<string, Vector3>();

        // シーン遷移をする シーン遷移した座標を記録する
        public static void LoadScene(string next_scene, float feed_duration, Vector3 pos, bool spawn_default_pos = false)
        {
            string cur_scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (!dict_.ContainsKey(cur_scene)) dict_.Add(cur_scene, pos);
            else dict_[cur_scene] = pos;
            if (spawn_default_pos && dict_.ContainsKey(next_scene)) dict_.Remove(next_scene);
            KoitanLib.FadeManager.FadeIn(feed_duration, next_scene);
        }

        // 前回シーン遷移したときの座標を取得する
        public static Vector3 GetPrevPosition()
        {
            string cur_scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (!dict_.ContainsKey(cur_scene))
            {
                //UnityEngine.Assertions.Assert.IsTrue(false, "このシーンでは前回の座標が登録されていません");
                return Vector3.zero;
            }
            else return dict_[cur_scene];
        }
    }
}