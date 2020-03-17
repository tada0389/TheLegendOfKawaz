using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TadaLib;
using KoitanLib;
using Bullet;

public class KoitanBaseBullet : BaseBulletController
{
    private Vector3 velocity;
    [SerializeField]
    private int damage;
    [SerializeField]
    private Vector3 gravity;
    [SerializeField]
    private float initialSpeed;
    [SerializeField]
    private float finalSpeed;
    [SerializeField]
    private float lifeTime;
    [SerializeField]
    private float rate;
    [SerializeField]
    private int bulletNum;
    [SerializeField]
    private MoveType moveType;
    [SerializeField]
    private bool isPenetratingStage;
    [SerializeField]
    private bool isPenetratingPlayer;
    [SerializeField]
    private KoitanBaseBullet subEmitter;
    private Timer timer;


    public override void Init(Vector2 pos, Vector2 dir, string opponent_tag, Transform target = null, float speed_rate = 1, float life_time = -1, float damage_rate = 1)
    {
        timer = new Timer(lifeTime);
        timer.TimeReset();
    }

    protected override void Move()
    {
        velocity += gravity * Time.deltaTime;
        move_body_.transform.position += velocity * Time.deltaTime;
        if (timer.IsTimeout()) Dead();
    }

    void Dead()
    {
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public enum MoveType
    {
        Straight,
        Homing,
        Curve,
        Wave,
        Bound,
        Transmission
    }
}
