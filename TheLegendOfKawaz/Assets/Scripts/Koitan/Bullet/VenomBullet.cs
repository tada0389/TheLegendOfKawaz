using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KoitanLib;

public class VenomBullet : MonoBehaviour
{
    [SerializeField]
    int damage = 1;

    [SerializeField]
    float lifeTime = 3;

    [SerializeField]
    Vector3 gravity = Vector3.down;

    private float time = 0;

    private Vector3 velocity;
    private string targetTag;

    [SerializeField]
    private TimeLimitObject shotEff;
    [SerializeField]
    private TimeLimitObject hitEff;

    public void Init(Vector3 pos, Vector3 v, string tag = "Player")
    {
        time = 0;
        transform.position = pos;
        velocity = v;
        targetTag = tag;
        TimeLimitObject eff = ObjectPoolManager.GetInstance<TimeLimitObject>(shotEff);
        if (eff != null)
        {
            eff.Init(pos);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        if (time >= lifeTime)
        {
            Dead();
        }
        time += Time.deltaTime;
    }

    private void Move()
    {
        velocity += gravity * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;
    }

    private void Dead()
    {
        TimeLimitObject eff = ObjectPoolManager.GetInstance<TimeLimitObject>(hitEff);
        if (eff != null)
        {
            eff.Init(transform.position);
        }
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Stage" || collider.tag == targetTag)
        {
            if (collider.tag == targetTag) collider.GetComponent<Actor.BaseActorController>().Damage(damage);
            Dead();
        }
    }
}
