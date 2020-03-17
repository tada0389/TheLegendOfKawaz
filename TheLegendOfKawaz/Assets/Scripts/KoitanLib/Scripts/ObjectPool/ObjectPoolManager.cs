using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TadaLib;

namespace KoitanLib
{
    public class ObjectPoolManager : SingletonMonoBehaviour<ObjectPoolManager>
    {
        private static Dictionary<int, PoolMonoElement> monoPoolList = new Dictionary<int, PoolMonoElement>();

        /// <summary>
        /// 絶対にコンポーネントをつけたGameObjectを登録すること!
        /// </summary>
        /// <param name="o"></param>
        /// <param name="MaxNum"></param>
        public static void Init(MonoBehaviour o, int MaxNum = 1)
        {
            int key = o.gameObject.GetInstanceID();
            //Debug.Log(o.name + o.GetInstanceID());
            if (!monoPoolList.ContainsKey(key))
            {
                PoolMonoElement elem = new PoolMonoElement(o, MaxNum);
                monoPoolList.Add(key, elem);
            }
        }

        public static T GetInstance<T>(MonoBehaviour o) where T : MonoBehaviour
        {
            int key = o.gameObject.GetInstanceID();
            if (monoPoolList.ContainsKey(key))
            {
                return monoPoolList[key].GetInstance<T>();
            }
            return null;
        }

        public static GameObject GetInstance(MonoBehaviour o)
        {
            int key = o.gameObject.GetInstanceID();
            if (monoPoolList.ContainsKey(key))
            {
                return monoPoolList[key].GetInstanceObj();
            }
            return null;
        }

        public static void Release(MonoBehaviour o)
        {
            int key = o.gameObject.GetInstanceID();
            if (monoPoolList.ContainsKey(key))
            {
                monoPoolList[key].Clear();
                monoPoolList.Remove(key);
            }
        }

        public class PoolMonoElement
        {
            public int maxNum;
            public MonoBehaviour original;
            List<MonoBehaviour> poolList;

            public PoolMonoElement(MonoBehaviour o, int max)
            {
                maxNum = max;
                original = o;
                poolList = new List<MonoBehaviour>(max);
                for (int i = 0; i < maxNum; i++)
                {
                    MonoBehaviour m = Instantiate(original);
                    m.gameObject.SetActive(false);
                    poolList.Add(m);
                }
            }

            //足りないときはnullを返す
            public T GetInstance<T>() where T : MonoBehaviour
            {
                foreach (T elem in poolList)
                {
                    if (!elem.gameObject.activeSelf)
                    {
                        elem.gameObject.SetActive(true);
                        return elem;
                    }
                }
                return null;
            }

            //足りないときはnullを返す
            public GameObject GetInstanceObj()
            {
                foreach (MonoBehaviour elem in poolList)
                {
                    if (!elem.gameObject.activeSelf)
                    {
                        elem.gameObject.SetActive(true);
                        return elem.gameObject;
                    }
                }
                return null;
            }

            public void Clear()
            {
                for (int i = maxNum - 1; i >= 0; i--)
                {
                    Destroy(poolList[i].gameObject);
                }
            }
        }
    }
}
