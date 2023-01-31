using System;
using UnityEngine;

namespace Tank.InputControllers
{
    public class KeyboardController : MonoBehaviour
    {
        private TankController _controller;

        private void Start()
        {
            _controller = gameObject.GetComponent<TankController>();
        }

        // Update is called once per frame
        private void Update()
        {
            _controller.RotationalVelocity = Input.GetAxis("Horizontal");
            _controller.Velocity = Input.GetAxis("Vertical");
        }
    }
}