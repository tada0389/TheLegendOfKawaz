using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 3つの型を格納できるクラス
/// シリアライズできるのでJsonUtilityでセーブできる
/// </summary>

namespace TadaLib
{
    [System.Serializable]
    public class Triple<T, U, V>
    {
        [SerializeField]
        private T f;
        public T first => f;
        [SerializeField]
        private U s;
        public U second => s;

        [SerializeField]
        private V t;
        public V third => t;

        public Triple(T v1, U v2, V v3)
        {
            f = v1;
            s = v2;
            t = v3;
        }
    }
}