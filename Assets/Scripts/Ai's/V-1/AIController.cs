using System.Collections.Generic;
using Ai_s.V_1;
using Attributes;
using Tank;
using UnityEngine;

namespace Ai_s
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] public AITankData data;
        [ReadOnlyInspector] public AIState _state;
        private TankExploding _tankExploding;
        [HideInInspector] public List<Transform> visibleTargets = new();


        private void Start()
        {
            _state = AIState.Patrol;
            _tankExploding = GetComponent<TankExploding>();
        }

        private void Update()
        {
            _state = visibleTargets.Count > 0 ? AIState.Attack : AIState.Patrol;
        }

        public void HitTank(Vector3 bulletPos)
        {
            _tankExploding.Explode(bulletPos);
        }
    }
}