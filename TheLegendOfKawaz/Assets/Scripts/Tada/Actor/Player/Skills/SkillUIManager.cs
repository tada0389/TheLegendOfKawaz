using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Actor.Player;

namespace SkillItem
{
    public class SkillUIManager : MonoBehaviour
    {
        [SerializeField]
        private TextMeshPro explonation_text_;
        [SerializeField]
        private TextMeshPro money_text_;

        private Queue<eSkill> skill_queue_;
        private int delete_requests_;

        // Start is called before the first frame update
        void Start()
        {
            skill_queue_ = new Queue<eSkill>();
            delete_requests_ = 0;

            money_text_.text = "<size=24></size> <sprite index=0> " + SkillManager.Instance.SkillPoint.ToString();
            explonation_text_.text = "";
        }

        private void Update()
        {
            if(skill_queue_.Count != 0)
            {
                while (skill_queue_.Count != 1) skill_queue_.Dequeue();
                eSkill skill_item = skill_queue_.Dequeue();

                Skill skill = SkillManager.Instance.GetSkill((int)skill_item);

                if (skill.ReachLevelLimit)
                    explonation_text_.text = " <color=red><size=24>Lv Max</size>  " + skill.Name + "</size></color>";
                else
                    explonation_text_.text = "<size=24>Lv" + (skill.Level + 1).ToString() + "</size>  <color=red>" + skill.Name + "</color> <size=24>値段 " + skill.NeedPoint().ToString() + "SP</size>";
                delete_requests_ = 0;
                //money_text_.text = "<size=24></size> <sprite index=0> " + SkillManager.Instance.SkillPoint.ToString();
            }
            else if(delete_requests_ >= 1)
            {
                delete_requests_ = 0;
                explonation_text_.text = "";
            }
            money_text_.text = "<size=24></size> <sprite index=0> " + SkillManager.Instance.SkillPoint.ToString();
        }

        public void ChangeExplonation(eSkill skill)
        {
            skill_queue_.Enqueue(skill);
        }

        public void DeleteExplonation()
        {
            ++delete_requests_;
        }
    }
}