using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーを加速させるステージギミック
/// </summary>

namespace Stage
{
    public class AccelerationPoint : MonoBehaviour
    {
        [SerializeField]
        private Vector2 accelPower = new Vector2(0.5f, 0.0f);

        [SerializeField]
        private TadaLib.eForceMode mode = TadaLib.eForceMode.VelocityChange;

        [SerializeField]
        private Vector2 MaxTargetVelocity = new Vector2(0.5f, 0.5f);

        // 一方向のみの加速かどうか
        [SerializeField]
        private bool IsSingleDirection = true;

        private void OnTriggerStay2D(Collider2D collision)
        {
            var trb = collision.GetComponent<TadaLib.TadaRigidbody>();

            if (trb != null)
            {
                if (IsSingleDirection)
                {
                    trb.AddForce(accelPower * Time.deltaTime * 60f, MaxTargetVelocity, mode);
                }
                else
                {
                    float sign = Mathf.Sign(trb.Velocity.x);
                    trb.AddForce(new Vector2(Mathf.Abs(accelPower.x) * sign, accelPower.y) * Time.deltaTime * 60f, MaxTargetVelocity, mode);
                }

            }
        }

        private void OnDrawGizmos()
        {
            float rate = (mode == TadaLib.eForceMode.Accelation) ? 20f : 10f;
            Debug.DrawRay(transform.position, accelPower * rate);
            if (!IsSingleDirection) Debug.DrawRay(transform.position, -accelPower * rate);
        }
    }
}