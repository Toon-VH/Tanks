using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ai_s.V_1
{
    public class AIAttack : MonoBehaviour
    {
        public int bulletsShot = 0;
        public GameObject projectile;

        [SerializeField] private float Accuracy = 0.5f;
        [SerializeField] private float MaxAngleMissedShot = 30;
        [SerializeField] private List<GameObject> _turretRotatingObjects;
        [SerializeField] private Transform _bulletStartPos;
        [SerializeField] private AudioClip _shotAudio;

        private AIController _aiController;
        private AudioSource _engineAudioSource;
        private float nextShot = 0.0f;
        private bool shooting;
        private object _lock = new object();

        private void Start()
        {
            _aiController = gameObject.GetComponent<AIController>();
            _engineAudioSource = gameObject.GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (_aiController._state == AIState.Attack)
            {
                if (shooting) return;
                shooting = true;
                StartCoroutine(Shoot());
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

        private void AimDirection()
        {
            var targetDirection =
                _aiController.visibleTargets[0].position - _turretRotatingObjects[0].transform.position;
            
            
        }


        IEnumerator Shoot()
        {
            for (;;)
            {
                Debug.Log("AI Shoot!!");
                _engineAudioSource.PlayOneShot(_shotAudio);
                var bulletStartPos = _bulletStartPos.transform;
                var bullet = Instantiate(projectile, bulletStartPos.position, bulletStartPos.rotation);
                bullet.name = "Bullet " + ++bulletsShot;
                bullet.GetComponent<Bullet>().MaxWallBounces = _aiController.data.BulletWallBounces;
                bullet.GetComponent<Bullet>().Speed = _aiController.data.BulletSpeed;
                yield return new WaitForSeconds(_aiController.data.FireRate);
            }
        }
    }
}