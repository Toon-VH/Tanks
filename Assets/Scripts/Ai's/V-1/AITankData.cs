using Tank;
using UnityEngine;

namespace Ai_s.V_1
{
    [CreateAssetMenu]
    public class AITankData : TankData
    {
        [Header("Movement")] public float MinDestinationChangeDelay = 2f;
        public float MaxDestinationChangeDelay = 5f;

        [Header("Field of View")] public float AgroRadius = 15f;
        public float ViewRadius = 50f;
        public float ViewAngle = 30f;
        [Range(0, 360)] public float TurretRotatingDegree = 40f;
        public float MinSecondsBetweenLookingPos = 1f;
        public float MaxSecondsBetweenLookingPos = 3f;

        [Header("Accuracy")] [Range(0, 100)] [SerializeField]
        public float Accuracy = 50;
        public float MaxAngleMissedShot = 15;
    }
}