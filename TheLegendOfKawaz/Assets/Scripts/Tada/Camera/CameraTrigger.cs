using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CameraSpace;

/// <summary>
/// カメラのコントローラーのパラメータを変更するクラス
/// </summary>

namespace CameraSpace
{
    public class CameraTrigger : MonoBehaviour
    {
        [SerializeField]
        private MultipleTargetCamera camera_;
        [SerializeField]
        private CameraData new_data_;

        private void OnTriggerEnter2D(Collider2D collider)
        {
            Debug.Log("hit_pre");
            if (collider.tag == "Player")
            {
                Debug.Log("hit");
                camera_.ChangeData(new_data_);
            }
        }
    }
} // namespace CameraSpace