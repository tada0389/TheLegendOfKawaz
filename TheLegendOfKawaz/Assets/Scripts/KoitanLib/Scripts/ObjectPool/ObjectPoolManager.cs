using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TadaLib;

namespace KoitanLib
{
    public class ObjectPoolManager : SingletonMonoBehaviour<ObjectPoolManager>
    {
        private static Dictionary<int, PoolMonoElement> poolDic = new Dictionary<int, PoolMonoElement>();

        private void Update()
        {
            foreach (var pool in poolDic)
            {
                for (int i = pool.Value.parents.Count - 1; i >= 0; --i)
                {
                    if (pool.Value.parents[i] == null)
                    {
                        pool.Value.parents.RemoveAt(i);
                    }
                }
                if (pool.Value.parents.Count == 0)
                {
                    pool.Value.Clear();
                }
            }
        }

        /// <summary>
        /// 絶対にコンポーネントをつけたGameObjectを登録すること!
        /// </summary>
        /// <param name="o"></param>
        /// <param name="maxNum"></param>
        public static void Init(MonoBehaviour o, MonoBehaviour p, int maxNum)
        {
            int key = o.gameObject.GetInstanceID();
            //Debug.Log(o.name + o.GetInstanceID());
            if (poolDic.ContainsKey(key))
            {
                poolDic[key].Init(p, maxNum);
            }
            else
            {
                PoolMonoElement elem = new PoolMonoElement(o, p, maxNum);
                poolDic.Add(key, elem);
            }
        }

        public static T GetInstance<T>(MonoBehaviour o) where T : MonoBehaviour
        {
            int key = o.gameObject.GetInstanceID();
            if (poolDic.ContainsKey(key))
            {
                return poolDic[key].GetInstance<T>();
            }
            return null;
        }

        public static GameObject GetInstance(MonoBehaviour o)
        {
            int key = o.gameObject.GetInstanceID();
            if (poolDic.ContainsKey(key))
            {
                return poolDic[key].GetInstance();
            }
            return null;
        }

        public static void Release(MonoBehaviour o)
        {
            int key = o.gameObject.GetInstanceID();
            if (poolDic.ContainsKey(key))
            {
                poolDic[key].Clear();
                poolDic.Remove(key);
            }
        }

        public class PoolMonoElement
        {
            public int maxNum = 0;
            public MonoBehaviour original;
            Queue<MonoBehaviour> monoQue;
            public List<MonoBehaviour> parents = new List<MonoBehaviour>();

            public PoolMonoElement(MonoBehaviour o, MonoBehaviour p, int max)
            {
                original = o;
                monoQue = new Queue<MonoBehaviour>(max);
                Init(p, max);
            }

            public void Init(MonoBehaviour p, int max)
            {
                maxNum = Mathf.Max(maxNum, max);
                parents.Add(p);
                for (int i = 0, n = maxNum - monoQue.Count; i < n; i++)
                {
                    MonoBehaviour m = Instantiate(original);
                    m.gameObject.SetActive(false);
                    monoQue.Enqueue(m);
                    //if (monoQue.Count >= maxNum) break;
                }
            }

            //足りないときはnullを返す
            public T GetInstance<T>() where T : MonoBehaviour
            {
                for (int i = 0; i < maxNum; i++)
                {
                    T m = (T)monoQue.Dequeue();
                    monoQue.Enqueue(m);
                    if (m.gameObject.activeSelf == false)
                    {
                        m.gameObject.SetActive(true);
                        return m;
                    }
                }
                return null;
            }

            //足りないときはnullを返す

            public GameObject GetInstance()
            {
                for (int i = 0; i < maxNum; i++)
                {
                    MonoBehaviour m = monoQue.Dequeue();
                    monoQue.Enqueue(m);
                    if (m.gameObject.activeSelf == false)
                    {
                        m.gameObject.SetActive(true);
                        return m.gameObject;
                    }
                }
                return null;
            }

            public void Clear()
            {
                while (monoQue.Count > 0)
                {
                    MonoBehaviour m = monoQue.Dequeue();
                    if (m != null)
                    {
                        Destroy(m.gameObject);
                    }
                }
                maxNum = 0;
            }
        }
    }
}
