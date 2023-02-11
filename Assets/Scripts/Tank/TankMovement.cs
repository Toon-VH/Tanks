using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tank
{
    public class TankMovement : MonoBehaviour
    {
        private TankController _tankController;
        private Rigidbody rb;

        private void Start()
        {
            _tankController = gameObject.GetComponent<TankController>();
            rb = gameObject.GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            // if (!IsOwner) return;

            // Move Tank Forward
            var wantedPosition = transform.position + (transform.forward *
                                                       (_tankController._velocity * _tankController._data.speed / 100 *
                                                        Time.deltaTime));
            rb.MovePosition(wantedPosition);

            // Rotate Tank
            var wantedRotation = transform.rotation * Quaternion.Euler(Vector3.up *
                                                                       (_tankController._data.turningSpeed *
                                                                        _tankController._rotationalVelocity *
                                                                        _tankController._data.turningSpeed *
                                                                        Time.deltaTime));
            rb.MoveRotation(wantedRotation);
        }
    }
}