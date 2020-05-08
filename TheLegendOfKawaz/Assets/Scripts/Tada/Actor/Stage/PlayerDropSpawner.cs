using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーがある下限以下の座標に到達したときに
/// 再リスポーンさせるスクリプト
/// </summary>

namespace Stage
{
    public class PlayerDropSpawner : MonoBehaviour
    {
        [SerializeField]
        private float bottom_boader_ = -10f;

        [SerializeField]
        private Vector3 spawn_pos_ = Vector3.zero;

        [SerializeField]
        private Transform player_;

        private bool is_feed_ = false;

        // Update is called once per frame
        void Update()
        {
            if (!is_feed_ && player_.position.y < bottom_boader_)
            {
                is_feed_ = true;
                TadaScene.TadaSceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, 0.5f, spawn_pos_);
            }
        }
    }
}