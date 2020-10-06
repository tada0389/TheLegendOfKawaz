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

        private void OnTriggerStay2D(Collider2D collision)
        {
            var trb = collision.GetComponent<TadaLib.TadaRigidbody>();

            if (trb != null)
            {
                trb.AddForce(accelPower, mode);
            }
        }

        private void OnDrawGizmos()
        {
            float rate = (mode == TadaLib.eForceMode.Accelation) ? 20f : 10f;
            Debug.DrawRay(transform.position, accelPower * rate);   
        }
    }
}