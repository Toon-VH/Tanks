using System.Collections.Generic;
using Tank;
using UnityEngine;

namespace Ai_s
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] public AITankData data;
        public AIState _state;
        private TankExploding _tankExploding;
        public List<Transform> visibleTargets = new();
        private GameObject _tankTop;


        private void Start()
        {
            _tankTop = transform.Find("Top").gameObject;
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

        private void OnDrawGizmos()
        {
            if (visibleTargets == null) return;

            foreach (var target in visibleTargets)
            {
                Gizmos.color = Color.red;
                var position = _tankTop.transform.position;
                var targetPosition = target.transform.position;
                Gizmos.DrawLine(position, new Vector3(targetPosition.x, position.y, targetPosition.z));
            }
        }
    }
}