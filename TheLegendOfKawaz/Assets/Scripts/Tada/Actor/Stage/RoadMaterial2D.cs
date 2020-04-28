using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地面の摩擦係数などを保持するクラス
/// </summary>

namespace Road
{
    public class RoadMaterial2D : MonoBehaviour
    {
        [SerializeField]
        private float friction_ = 1.0f;
        public float Friction => friction_;
    }
}