using System.Collections;
using System.Collections.Generic;
using TadaLib;
using UnityEngine;

namespace Test
{
    public class AccelationTest : MonoBehaviour
    {
        [SerializeField]
        private Vector2 accelPower = new Vector2(0.5f, 0.0f);

        [SerializeField]
        private eForceMode mode = eForceMode.VelocityChange;

        private void OnTriggerStay2D(Collider2D collision)
        {
            Debug.Log("called");
            var trb = collision.GetComponent<TadaLib.TadaRigidbody>();

            if (trb != null)
            {
                //trb.AddForce(accelPower, mode);
                Debug.Log("called");
            }
        }
    }
}