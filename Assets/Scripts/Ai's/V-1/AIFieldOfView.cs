using System;
using System.Collections;
using System.Collections.Generic;
using Tank;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Ai_s
{
    public class AIFieldOfView : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _turretRotatingObjects;
        [SerializeField] private LayerMask targetMask;
        [SerializeField] private LayerMask obstacleMask;
        [SerializeField] public GameObject tankBarrel;
        [SerializeField] public float viewRadius;
        [SerializeField] public float agroRadius;
        [SerializeField] private float _lookDirectionChangeTimer;
        [Range(0, 360)] [SerializeField] public float viewAngle;

        private AIController _aiController;
        private Quaternion _randomRotation;
        private Coroutine setRandomLookingPos;
        private bool lookingAround;


        private void Start()
        {
            _aiController = GetComponent<AIController>();
            StartCoroutine(FindTargetsWithDelay(0.2f));
        }

        private void Update()
        {
            if (_aiController._state == AIState.Patrol)
            {
                if (lookingAround) return;
                lookingAround = true;
                setRandomLookingPos = StartCoroutine(SetRandomLookingPos());
            }

            else
            {
                lookingAround = false;
                StopCoroutine(setRandomLookingPos);
            }
        }

        private void FixedUpdate()
        {
            if (_aiController._state != AIState.Patrol) return;
            RotateTurret();
        }

        private IEnumerator FindTargetsWithDelay(float delay)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);
                FindVisibleTargets();
            }
        }

        private void FindVisibleTargets()
        {
            _aiController.visibleTargets.Clear();
            var targetsInViewRadius =
                Physics.OverlapSphere(tankBarrel.transform.position, viewRadius, targetMask);
            var targetsInAgroRadius =
                Physics.OverlapSphere(transform.position, agroRadius, targetMask);

            foreach (var t in targetsInViewRadius)
            {
                var target = t.transform;
                var dirToTarget = (target.position - tankBarrel.transform.position).normalized;
                if (!(Vector3.Angle(tankBarrel.transform.forward, dirToTarget) < viewAngle / 2)) continue;
                var dstToTarget = Vector3.Distance(tankBarrel.transform.position, target.position);
                if (!Physics.Raycast(tankBarrel.transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    _aiController.visibleTargets.Add(target);
                }
            }

            foreach (var t in targetsInAgroRadius)
            {
                var target = t.transform;
                var position = tankBarrel.transform.position;
                var targetPosition = target.position;
                var dirToTarget = (targetPosition - position).normalized;
                var dstToTarget = Vector3.Distance(position, targetPosition);
                if (!Physics.Raycast(tankBarrel.transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    _aiController.visibleTargets.Add(target);
                }
            }
        }

        public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += tankBarrel.transform.eulerAngles.y;
            }

            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }

        private void RotateTurret()
        {
            foreach (var turretPart in _turretRotatingObjects)
            {
                // // The step size is equal to speed times frame time.
                var step = _aiController.data.turretTurningSpeedInDegree * Time.deltaTime;

                // Rotate our transform a step closer to the target's.
                turretPart.transform.rotation =
                    Quaternion.RotateTowards(turretPart.transform.rotation, _randomRotation, step);
            }
        }

        private IEnumerator SetRandomLookingPos()
        {
            for (;;)
            {
                _randomRotation = Quaternion.Euler(0, Random.Range(-viewAngle / 2, viewAngle / 2) +
                                                      transform.eulerAngles.y, 0);
                yield return new WaitForSeconds(_lookDirectionChangeTimer);
            }
        }

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            var position = tankBarrel.transform.position;
            Handles.color = Color.cyan;
            var viewAngleA = DirFromAngle(-viewAngle / 2, false);
            var viewAngleB = DirFromAngle(viewAngle / 2, false);
            Handles.DrawLine(position, position + viewAngleA * viewRadius);
            Handles.DrawLine(position, position + viewAngleB * viewRadius);
            var forward = tankBarrel.transform.forward;
            Handles.DrawWireArc(position, Vector3.up, forward, viewAngle / 2, viewRadius);
            Handles.DrawWireArc(position, Vector3.up, forward, -viewAngle / 2, viewRadius);
            Handles.color = Color.blue;
            Handles.DrawWireArc(position, Vector3.up, Vector3.forward, 360, agroRadius);
#endif
        }
    }
}