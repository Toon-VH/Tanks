﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ai_s.V_1
{
    public class AIAttack : MonoBehaviour
    {
        public int bulletsShot = 0;

        // Attack
        [SerializeField] private GameObject projectile;
        [SerializeField] private Transform _bulletStartPos;

        // Turret moving
        [SerializeField] private List<GameObject> _turretRotatingObjects;

        // Accuracy
        [Range(0, 100)] [SerializeField] private float Accuracy = 50;
        [SerializeField] private float MaxAngleMissedShot = 30;

        //---
        [SerializeField] private Transform _barrel;

        private AIController _aiController;
        private AudioSource _engineAudioSource;
        private bool shooting;
        private bool mayShoot;
        private readonly object _lock = new();
        private Coroutine _shootCoroutine;
        private Coroutine _checkIfMayShootCoroutine;

        private void Start()
        {
            _aiController = gameObject.GetComponent<AIController>();
            _engineAudioSource = gameObject.GetComponent<AudioSource>();
        }


        private void Update()
        {
            if (_aiController._state == AIState.Attack)
            {
                _checkIfMayShootCoroutine = StartCoroutine(CheckIfMayShoot());
                if (!shooting && mayShoot)
                {
                    shooting = true;
                    _shootCoroutine = StartCoroutine(Shoot());
                    //TODO: Can shoot twice in a row
                }
                else if (!mayShoot)
                {
                    shooting = false;
                    //check if coroutine is running
                    if (_shootCoroutine == null) return;
                    StopCoroutine(_shootCoroutine);
                }
            }
            else
            {
                shooting = false;
                StopAllCoroutines();
            }
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
                    var targetDirection = _aiController.visibleTargets[0].position - turretPart.transform.position;
                    

                    // The step size is equal to speed times frame time.
                    var singleStep = _aiController.data.turretTurningSpeedInDegree * Time.deltaTime;

                    //singleStep is in degree make it to radian
                    singleStep *= Mathf.Deg2Rad;

                    // Rotate the forward vector towards the target direction by one step
                    var newDirection =
                        Vector3.RotateTowards(turretPart.transform.forward, targetDirection, singleStep, 0.0f);


                    var rotation = Quaternion.LookRotation(newDirection);
                    rotation = Quaternion.Euler(new Vector3(0, rotation.eulerAngles.y,
                        0)); // Clamp the x and z rotation

                    turretPart.transform.rotation = rotation;
                }
            }
        }

        private float AngleMissed()
        {
            return MaxAngleMissedShot * (Accuracy / 100) * (UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1);
        }


        private IEnumerator Shoot()
        {
            for (;;)
            {
                Debug.Log("AI Shoot!!");
                var bulletStartPos = _bulletStartPos.transform;
                var bullet = Instantiate(projectile, bulletStartPos.position, bulletStartPos.rotation);
                bullet.name = "Bullet " + ++bulletsShot;
                bullet.GetComponent<Bullet>().MaxWallBounces = _aiController.data.BulletWallBounces;
                bullet.GetComponent<Bullet>().Speed = _aiController.data.BulletSpeed;
                yield return new WaitForSeconds(_aiController.data.FireRate);
            }
        }

        private IEnumerator CheckIfMayShoot()
        {
            for (;;)
            {
                RaycastHit hit;
                // Does the ray intersect any objects excluding the player layer
                if (Physics.SphereCast(_barrel.position, 0.8f, _bulletStartPos.forward, out hit,
                        Mathf.Infinity))
                {
                    mayShoot = !hit.collider.gameObject.CompareTag("AITank");
                }

                yield return new WaitForSeconds(0.2f);
            }
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