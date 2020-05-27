using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>

namespace Stage
{
    public class ActiveChageTrigger : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> GoActiveTrueObjs;
        [SerializeField]
        private List<GameObject> GoActiveFalseObjs;

        [SerializeField]
        private string trigger_tag_ = "Player";

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.tag == trigger_tag_)
            {
                foreach (var obj in GoActiveTrueObjs) obj.SetActive(true);
                foreach (var obj in GoActiveFalseObjs) obj.SetActive(false);
            }
        }
    }
}