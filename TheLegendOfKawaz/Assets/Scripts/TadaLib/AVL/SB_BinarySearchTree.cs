using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 平行二分探索木
/// http://yambe2002.hatenablog.com/entry/2017/02/07/122421 より参照
/// 挿入，削除，検索を O(logN) で行える
/// </summary>

namespace TadaLib
{
    /// <summary>
    /// Self-Balancing Binary Search Tree
    /// (using Randamized BST)
    /// </summary>
    public class SB_BinarySearchTree<T> where T : IComparable
    {
        public class Node
        {
            public T Value;
            public Node LChild;
            public Node RChild;
            public int Count;

            public Node(T v)
            {
                Value = v;
                Count = 1;
            }
        }

        static Random _rnd = new Random();
        public static int Count(Node t)
        {
            return t == null ? 0 : t.Count;
        }

        static Node Update(Node t)
        {
            t.Count = Count(t.LChild) + Count(t.RChild) + 1;
            return t;
        }

        public static Node Merge(Node l, Node r)
        {
            if (l == null || r == null) return l == null ? r : l;

            if ((double)Count(l) / (double)(Count(l) + Count(r)) > _rnd.NextDouble())
            {
                l.RChild = Merge(l.RChild, r);
                return Update(l);
            }
            else
            {
                r.LChild = Merge(l, r.LChild);
                return Update(r);
            }
        }

        /// <summary>
        /// split as [0, k), [k, n)
        /// </summary>
        public static Tuple<Node, Node> Split(Node t, int k)
        {
            if (t == null) return new Tuple<Node, Node>(null, null);
            if (k <= Count(t.LChild))
            {
                var s = Split(t.LChild, k);
                t.LChild = s.Item2;
                return new Tuple<Node, Node>(s.Item1, Update(t));
            }
            else
            {
                var s = Split(t.RChild, k - Count(t.LChild) - 1);
                t.RChild = s.Item1;
                return new Tuple<Node, Node>(Update(t), s.Item2);
            }
        }

        public static Node Remove(Node t, T v)
        {
            if (Find(t, v) == null) return t;
            return RemoveAt(t, LowerBound(t, v));
        }

        public static Node RemoveAt(Node t, int k)
        {
            var s = Split(t, k);
            var s2 = Split(s.Item2, 1);
            return Merge(s.Item1, s2.Item2);
        }

        public static bool Contains(Node t, T v)
        {
            return Find(t, v) != null;
        }

        public static Node Find(Node t, T v)
        {
            while (t != null)
            {
                var cmp = t.Value.CompareTo(v);
                if (cmp > 0) t = t.LChild;
                else if (cmp < 0) t = t.RChild;
                else break;
            }
            return t;
        }

        public static Node FindByIndex(Node t, int idx)
        {
            if (t == null) return null;

            var currentIdx = Count(t) - Count(t.RChild) - 1;
            while (t != null)
            {
                if (currentIdx == idx) return t;
                if (currentIdx > idx)
                {
                    t = t.LChild;
                    currentIdx -= (Count(t == null ? null : t.RChild) + 1);
                }
                else
                {
                    t = t.RChild;
                    currentIdx += (Count(t == null ? null : t.LChild) + 1);
                }
            }

            return null;
        }

        public static int UpperBound(Node t, T v)
        {
            var torg = t;
            if (t == null) return -1;

            var ret = Int32.MaxValue;
            var idx = Count(t) - Count(t.RChild) - 1;
            while (t != null)
            {
                var cmp = t.Value.CompareTo(v);

                if (cmp > 0)
                {
                    ret = Math.Min(ret, idx);
                    t = t.LChild;
                    idx -= (Count(t == null ? null : t.RChild) + 1);
                }
                else if (cmp <= 0)
                {
                    t = t.RChild;
                    idx += (Count(t == null ? null : t.LChild) + 1);
                }
            }
            return ret == Int32.MaxValue ? Count(torg) : ret;
        }

        public static int LowerBound(Node t, T v)
        {
            var torg = t;
            if (t == null) return -1;

            var idx = Count(t) - Count(t.RChild) - 1;
            var ret = Int32.MaxValue;
            while (t != null)
            {
                var cmp = t.Value.CompareTo(v);
                if (cmp >= 0)
                {
                    if (cmp == 0) ret = Math.Min(ret, idx);
                    t = t.LChild;
                    if (t == null) ret = Math.Min(ret, idx);
                    idx -= t == null ? 0 : (Count(t.RChild) + 1);
                }
                else if (cmp < 0)
                {
                    t = t.RChild;
                    idx += (Count(t == null ? null : t.LChild) + 1);
                    if (t == null) return idx;
                }
            }
            return ret == Int32.MaxValue ? Count(torg) : ret;
        }

        public static Node Insert(Node t, T v)
        {
            var ub = LowerBound(t, v);
            return InsertByIdx(t, ub, v);
        }

        static Node InsertByIdx(Node t, int k, T v)
        {
            var s = Split(t, k);
            return Merge(Merge(s.Item1, new Node(v)), s.Item2);
        }

        public static IEnumerable<T> Enumerate(Node t)
        {
            var ret = new List<T>();
            Enumerate(t, ret);
            return ret;
        }

        static void Enumerate(Node t, List<T> ret)
        {
            if (t == null) return;
            Enumerate(t.LChild, ret);
            ret.Add(t.Value);
            Enumerate(t.RChild, ret);
        }
    }
}