using System;
using System.Collections;
using System.Collections.Generic;
using Attributes;
using Unity.VisualScripting;
using UnityEngine;

namespace Ai_s.V_1
{
    public class AIAttack : MonoBehaviour
    {
        [ReadOnlyInspector] private int bulletsShot;
        [SerializeField] private Transform _bulletStartPos;

        // Turret moving
        [SerializeField] private List<GameObject> _turretRotatingObjects;
        [SerializeField] private Transform _barrel;

        private AIController _aiController;
        private AITankData _aiTankData;
        private Quaternion shootRotation;
        private bool mayShoot;
        private readonly object _lock = new();
        private float missedAngle;
        private bool friendlyFireCheck;
        private float fireRateTimestamp;

        private void Start()
        {
            _aiController = gameObject.GetComponent<AIController>();
            _aiTankData = _aiController.data;
            missedAngle = AngleMissed();
        }


        private void Update()
        {
            if (_aiController._state != AIState.Attack) return;
            if (MayShoot() && !FriendlyFireCheck()) Shoot();
            //TODO: Can shoot twice in a row
        }

        private bool MayShoot()
        {
            return _turretRotatingObjects[0].transform.rotation == shootRotation;
        }

        private void FixedUpdate()
        {
            if (_aiController._state != AIState.Attack) return;
            RotateTurret();
        }

        private void RotateTurret()
        {
            foreach (var turretPart in _turretRotatingObjects)
            {
                lock (_lock)
                {
                    if (_aiController.visibleTargets.Count == 0) return;

                    // Determine which direction to rotate towards
                    var direction = (_aiController.visibleTargets[0].position - turretPart.transform.position)
                        .normalized;

                    var targetRotation = Quaternion.LookRotation(direction);
                    Debug.Log($"targetRotation: {shootRotation.eulerAngles}");
                    Debug.Log($"missedAngle: {missedAngle}");

                    shootRotation.eulerAngles = new Vector3(0, targetRotation.eulerAngles.y + missedAngle, 0);

                    Debug.Log($"shootRotation: {shootRotation.eulerAngles}");

                    // The step size is equal to speed times frame time.
                    var step = _aiController.data.turretTurningSpeedInDegree * Time.deltaTime;

                    // Rotate our transform a step closer to the target's.
                    turretPart.transform.rotation =
                        Quaternion.RotateTowards(turretPart.transform.rotation, shootRotation, step);
                }
            }
        }

        private float AngleMissed()
        {
            return UnityEngine.Random.Range(0, _aiTankData.MaxAngleMissedShot - _aiTankData.MaxAngleMissedShot * (_aiTankData.Accuracy / 100)) *
                   (UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1);
        }

        private void Shoot()
        {
            if (Time.time < fireRateTimestamp) return;
            fireRateTimestamp = Time.time + _aiController.data.FireRate;
            Debug.Log(gameObject.name + " Fired!!");
            var bulletStartPos = _bulletStartPos.transform;
            var bullet = Instantiate(_aiTankData.Projectile, bulletStartPos.position, bulletStartPos.rotation);
            bullet.name = "Bullet " + ++bulletsShot;
            bullet.GetComponent<Bullet>().MaxWallBounces = _aiController.data.BulletWallBounces;
            bullet.GetComponent<Bullet>().Speed = _aiController.data.BulletSpeed;
            missedAngle = AngleMissed();
        }

        private bool FriendlyFireCheck()
        {
            return Physics.SphereCast(_barrel.position, 0.8f, _bulletStartPos.forward, out var hit,
                Mathf.Infinity) && hit.collider.gameObject.CompareTag("AITank");
        }

        private void OnDrawGizmos()
        {
            if (_aiController == null) return;
            var position = _barrel.transform.position;

            foreach (var target in _aiController.visibleTargets)
            {
                Gizmos.color = Color.red;
                var targetPosition = target.transform.position;
                Gizmos.DrawLine(position, new Vector3(targetPosition.x, position.y, targetPosition.z));
            }

            if (_aiController._state != AIState.Attack) return;
            Gizmos.color = Color.yellow;
            if (Physics.Raycast(position, _bulletStartPos.forward, out var hit, int.MaxValue))
            {
                Gizmos.DrawRay(position, _bulletStartPos.forward * hit.distance);
            }
        }
    }
}