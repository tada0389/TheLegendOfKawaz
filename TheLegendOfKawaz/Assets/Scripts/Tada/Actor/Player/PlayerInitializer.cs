using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーを初期化するクラス
/// プレイヤーが出てくるシーンならどのシーンにもこのインスタンスを配置する
/// </summary>

namespace Actor.Player
{
    public class PlayerInitializer : MonoBehaviour
    {
        [SerializeField]
        private Player player_;

        [SerializeField]
        private bool is_minigame_mode_ = false;

        private void Start()
        {
            Global.GlobalPlayerInfo.IsMuteki = false;
            Global.GlobalPlayerInfo.ActionEnabled = true;
            Global.GlobalPlayerInfo.BossDefeated = false;

            List<Skill> skills;
            if (is_minigame_mode_) skills = SkillManager.Instance.LevelOneSkills;
            else skills = SkillManager.Instance.Skills;

            player_.Init(skills);

            // ついで
            // 現在のシーンがステージセレクト画面だったらセーブする
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "ZakkyScene") TadaLib.Save.SaveManager.Instance.Save();
        }
    }
}