using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swamp : MonoBehaviour
{
    private string opponent_tag_ = "Player";
    private int damage_ = 2;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Stage" || collider.tag == opponent_tag_)
        {
            if (collider.tag == opponent_tag_)
            {
                collider.GetComponent<Actor.BaseActorController>().Damage(damage_);
                Destroy(gameObject, 1f);
            }

        }
    }
}
