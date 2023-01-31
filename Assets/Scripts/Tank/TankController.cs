using System;
using Unity.Netcode;
using UnityEngine;

namespace Tank
{
    public class TankController : MonoBehaviour
    {
        [SerializeField] public TankData _data;
        public float _velocity;
        public float _rotationalVelocity;

        

        public float Velocity
        {
            get => _velocity;
            set => _velocity = value * _data.speed;
        }

        public float RotationalVelocity
        {
            get => _rotationalVelocity;
            set => _rotationalVelocity = value * _data.turningSpeed;
        }

        private TankExploding _tankExploding;


        private void Start()
        {
            _tankExploding = GetComponent<TankExploding>();
        }

        public void HitTank(Vector3 bulletPos)
        {
            _tankExploding.Explode(bulletPos);
        }
    }
}