using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KoitanLib;

/// <summary>
/// スキル購入シーンで説明をしてくれるNPC
/// </summary>

namespace Actor.NPC
{
    public class NPCSkillExplonationer : BaseActorController
    {
        [SerializeField, Multiline(5)]
        private string message;

        [SerializeField, Multiline(5)]
        private string dead_message;

        [SerializeField]
        private Transform player_;

        private void Update()
        {
            if (player_)
            {
                float dir = Mathf.Sign(player_.position.x - transform.position.x);
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * dir, transform.localScale.y, transform.localScale.z);
            }
        }

        public override void Damage(int damage)
        {
            TadaLib.DamageDisplayer.eDamageType type = TadaLib.DamageDisplayer.eDamageType.Mini;
            if (damage >= 3) type = TadaLib.DamageDisplayer.eDamageType.Big;
            else if (damage >= 2) type = TadaLib.DamageDisplayer.eDamageType.Normal;
            TadaLib.DamageDisplayer.Instance.ShowDamage(damage, transform.position, type);

            HP -= damage;
            if (HP <= 0) Dead();
        }

        private void Dead()
        {
            message = dead_message;
        }

        void OnTriggerEnter2D(Collider2D col)
        {

            if (col.tag == "Player")
            {
                MessageManager.OpenKanbanWindow(message);
            }
        }

        //ドアから離れたらドアを縮める
        void OnTriggerExit2D(Collider2D col)
        {
            if (col.tag == "Player")
            {
                MessageManager.CloseKanbanWindow();
            }
        }
    }
}