using UnityEngine;

namespace Tank
{
    [CreateAssetMenu]
    public class TankData : ScriptableObject
    {
        public float speed;
        public float turningSpeed;
        public float turretTurningSpeedInDegree;
        public float BulletSpeed;
        public int BulletWallBounces;
        public float FireRate;
    }
}