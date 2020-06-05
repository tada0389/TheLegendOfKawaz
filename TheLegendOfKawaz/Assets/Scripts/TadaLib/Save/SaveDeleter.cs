using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TadaLib.Save;

namespace Save
{
    public class SaveDeleter : MonoBehaviour
    {
        private void Start()
        {
            DeleteAllData();
        }

        // セーブデータをすべて削除する
        public void DeleteAllData()
        {
            AchievementManager.DeleteSaveData();
            ScoreManager.Instance.DeleteSaveData();
            Actor.Player.SkillManager.Instance.DeleteSaveData();
        }

        // 一部だけ 今はスキル情報のみ
        public void DeleteData()
        {
            Actor.Player.SkillManager.Instance.DeleteSaveData();
        }
    }
}