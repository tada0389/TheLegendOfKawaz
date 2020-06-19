using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// C++のstd::pair型とほぼ同じ機能を持つ これは嘘
/// </summary>

namespace TadaLib
{
    [System.Serializable]
    public class Pair<T, U>
    {
        public T first;
        public U second;

        public Pair(T v1, U v2)
        {
            first = v1;
            second = v2;
        }
    }
}