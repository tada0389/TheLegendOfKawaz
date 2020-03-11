using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Enemy;

namespace Actor.Enemy
{
    public class AnchorEffect : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem body_;

        // エフェクトを発生させる
        public void Create()
        {
            body_.gameObject.SetActive(true);
        }

        // エフェクトを消す
        public void Delete()
        {
            body_.gameObject.SetActive(false);
        }

        // このボスにぶつかるとダメージを受ける
        private void OnTriggerStay2D(Collider2D collider)
        {
            if (collider.tag == "Player")
            {
                collider.GetComponent<BaseActorController>().Damage(2);
            }
        }
    }
}