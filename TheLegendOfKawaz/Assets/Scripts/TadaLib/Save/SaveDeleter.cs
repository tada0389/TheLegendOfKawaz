using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TadaLib.Save;

namespace Save
{
    public class SaveDeleter : MonoBehaviour
    {
        // セーブデータをすべて削除する
        public void DeleteAllData()
        {
            AchievementManager.DeleteSaveData();
            ScoreManager.Instance.DeleteSaveData();
            TadaScene.TadaSceneManager.DeleteSaveData();
            Actor.Player.SkillManager.Instance.DeleteSaveData();
            SettingManager.DeleteSaveData();
            //Global.GlobalDataManager.DeleteSaveData();
        }

        // 一部だけ スキル情報をプレイヤーの座標情報を削除
        public void DeleteTempData()
        {
            Actor.Player.SkillManager.Instance.DeleteSaveData();
            TadaScene.TadaSceneManager.DeleteSaveData();
        }
    }
}