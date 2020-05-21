using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// set
/// http://yambe2002.hatenablog.com/entry/2017/02/07/122421 より参照
/// 挿入，削除，検索を O(logN) で行える
/// 値は種類ごとに持つ
/// </summary>

namespace TadaLib
{
    /// <summary>
    /// C-like set
    /// </summary>
    public class Set<T> where T : IComparable
    {
        protected SB_BinarySearchTree<T>.Node _root;

        public T this[int idx] { get { return ElementAt(idx); } }

        public int Count()
        {
            return SB_BinarySearchTree<T>.Count(_root);
        }

        public virtual void Insert(T v)
        {
            if (_root == null) _root = new SB_BinarySearchTree<T>.Node(v);
            else
            {
                if (SB_BinarySearchTree<T>.Find(_root, v) != null) return;
                _root = SB_BinarySearchTree<T>.Insert(_root, v);
            }
        }

        public void Clear()
        {
            _root = null;
        }

        public void Remove(T v)
        {
            _root = SB_BinarySearchTree<T>.Remove(_root, v);
        }

        public bool Contains(T v)
        {
            return SB_BinarySearchTree<T>.Contains(_root, v);
        }

        public T ElementAt(int k)
        {
            var node = SB_BinarySearchTree<T>.FindByIndex(_root, k);
            if (node == null) throw new IndexOutOfRangeException();
            return node.Value;
        }

        public int Count(T v)
        {
            return SB_BinarySearchTree<T>.UpperBound(_root, v) - SB_BinarySearchTree<T>.LowerBound(_root, v);
        }

        public int LowerBound(T v)
        {
            return SB_BinarySearchTree<T>.LowerBound(_root, v);
        }

        public int UpperBound(T v)
        {
            return SB_BinarySearchTree<T>.UpperBound(_root, v);
        }

        public Tuple<int, int> EqualRange(T v)
        {
            if (!Contains(v)) return new Tuple<int, int>(-1, -1);
            return new Tuple<int, int>(SB_BinarySearchTree<T>.LowerBound(_root, v), SB_BinarySearchTree<T>.UpperBound(_root, v) - 1);
        }

        public List<T> ToList()
        {
            return new List<T>(SB_BinarySearchTree<T>.Enumerate(_root));
        }
    }
}