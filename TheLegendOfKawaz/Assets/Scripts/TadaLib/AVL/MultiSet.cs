using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// set
/// http://yambe2002.hatenablog.com/entry/2017/02/07/122421 より参照
/// 挿入，削除，検索を O(logN) で行える
/// 値は重複してもよい
/// </summary>

namespace TadaLib
{
    /// <summary>
    /// C-like multiset
    /// </summary>
    public class MultiSet<T> : Set<T> where T : IComparable
    {
        public override void Insert(T v)
        {
            if (_root == null) _root = new SB_BinarySearchTree<T>.Node(v);
            else _root = SB_BinarySearchTree<T>.Insert(_root, v);
        }
    }
}