﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// グローバルに持っておきたい変数たち
/// </summary>

namespace Global
{
    public class GlobalPlayerInfo
    {
        // 無敵かどうか
        public static bool IsMuteki = false;
        // 行動できるかどうか
        public static bool ActionEnabled = true;
        // ボスを倒したかどうか
        public static bool BossDefeated = false;
    }
}