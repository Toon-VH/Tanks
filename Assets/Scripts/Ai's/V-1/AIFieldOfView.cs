using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Ai_s.V_1
{
    public class AIFieldOfView : MonoBehaviour
    {
        //Turret moving
        [SerializeField] private List<GameObject> _turretRotatingObjects;

        //Masks
        [SerializeField] private LayerMask targetMask;
        [SerializeField] private LayerMask obstacleMask;

        private AIController _aiController;
        private Quaternion _randomAngle;
        private Coroutine _setRandomLookingPos;
        private bool _lookingAround;
        private Transform _turretMiddleLocation;
        private AITankData _aiTankData;


        private void Start()
        {
            _turretMiddleLocation = _turretRotatingObjects[0].transform;
            _aiController = GetComponent<AIController>();
            _aiTankData = _aiController.data;
            StartCoroutine(FindTargets(0.2f));
            _setRandomLookingPos = StartCoroutine(SetRandomLookingAngle());
            _lookingAround = true;
        }

        private void Update()
        {
            if (_aiController._state == AIState.Patrol)
            {
                if (_lookingAround) return;
                _lookingAround = true;
                _setRandomLookingPos = StartCoroutine(SetRandomLookingAngle());
            }

            else
            {
                _lookingAround = false;
                StopCoroutine(_setRandomLookingPos);
            }
        }

        private void FixedUpdate()
        {
            if (_aiController._state != AIState.Patrol) return;
            RotateTurret();
        }

        private IEnumerator FindTargets(float delay)
        {
            for (;;)
            {
                _aiController.visibleTargets.Clear();
                var targetsInViewRadius =
                    Physics.OverlapSphere(_turretMiddleLocation.transform.position, _aiTankData.ViewRadius, targetMask);
                var targetsInAgroRadius =
                    Physics.OverlapSphere(transform.position, _aiTankData.AgroRadius, targetMask);

                foreach (var t in targetsInViewRadius)
                {
                    var target = t.transform;
                    var dirToTarget = (target.position - _turretMiddleLocation.transform.position).normalized;
                    if (!(Vector3.Angle(_turretMiddleLocation.transform.forward, dirToTarget) <
                          _aiTankData.ViewAngle / 2))
                        continue;
                    var dstToTarget = Vector3.Distance(_turretMiddleLocation.transform.position, target.position);
                    if (!Physics.Raycast(_turretMiddleLocation.transform.position, dirToTarget, dstToTarget,
                            obstacleMask))
                    {
                        _aiController.visibleTargets.Add(target);
                    }
                }

                foreach (var t in targetsInAgroRadius)
                {
                    var target = t.transform;
                    var position = _turretMiddleLocation.transform.position;
                    var targetPosition = target.position;
                    var dirToTarget = (targetPosition - position).normalized;
                    var dstToTarget = Vector3.Distance(position, targetPosition);
                    if (!Physics.Raycast(_turretMiddleLocation.transform.position, dirToTarget, dstToTarget,
                            obstacleMask))
                    {
                        _aiController.visibleTargets.Add(target);
                    }
                }

                yield return new WaitForSeconds(delay);
            }
        }

        private void RotateTurret()
        {
            foreach (var turretPart in _turretRotatingObjects)
            {
                // The step size is equal to speed times frame time.
                var step = _aiController.data.turretTurningSpeedInDegree * Time.deltaTime;

                // Rotate our transform a step closer to the target's.
                turretPart.transform.localRotation =
                    Quaternion.RotateTowards(turretPart.transform.localRotation, _randomAngle, step);
            }
        }

        private IEnumerator SetRandomLookingAngle()
        {
            for (;;)
            {
                _randomAngle = Quaternion.Euler(0,
                    Random.Range(-_aiTankData.TurretRotatingDegree / 2, _aiTankData.TurretRotatingDegree / 2), 0);
                yield return new WaitForSeconds(Random.Range(_aiTankData.MinSecondsBetweenLookingPos,
                    _aiTankData.MaxSecondsBetweenLookingPos));
            }
        }

        private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += _turretRotatingObjects[0].transform.eulerAngles.y;
            }

            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            //FOV
            var position = _turretRotatingObjects[0].transform.position;
            Handles.color = Color.cyan;
            var viewAngleA = DirFromAngle(-_aiTankData.ViewAngle / 2, false);
            var viewAngleB = DirFromAngle(_aiTankData.ViewAngle / 2, false);
            Handles.DrawLine(position, position + viewAngleA * _aiTankData.ViewRadius);
            Handles.DrawLine(position, position + viewAngleB * _aiTankData.ViewRadius);
            var turretForward = _turretRotatingObjects[0].transform.forward;
            Handles.DrawWireArc(position, Vector3.up, turretForward, _aiTankData.ViewAngle / 2, _aiTankData.ViewRadius);
            Handles.DrawWireArc(position, Vector3.up, turretForward, -_aiTankData.ViewAngle / 2, _aiTankData.ViewRadius);
            Handles.color = Color.blue;
            Handles.DrawWireArc(position, Vector3.up, Vector3.forward, 360, _aiTankData.AgroRadius);
            //Angle radius
            Handles.color = Color.red;
            var forward = transform.forward;
            Handles.DrawWireArc(position, Vector3.up, forward, _aiTankData.TurretRotatingDegree / 2, 1);
            Handles.DrawWireArc(position, Vector3.up, forward, -_aiTankData.TurretRotatingDegree / 2, 1);

        }
    }
}