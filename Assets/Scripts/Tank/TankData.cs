using System;
using UnityEditor;
using UnityEngine;

namespace Tank
{
    [CreateAssetMenu]
    public class TankData : ScriptableObject
    {
        [Header("Basics")] public float speed = 2.5f;
        public float turningSpeed = 5;
        public float turretTurningSpeedInDegree = 65;

        [Header("Shooting")] public GameObject Projectile;

        [Range(0.1f, 20)]public float FireRate = 1.5f;
        public float BulletSpeed = 30f;
        public int BulletWallBounces;
    }
}