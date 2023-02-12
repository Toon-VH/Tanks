using System;
using Unity.Netcode;
using UnityEngine;

namespace Tank
{
    public class TankShooting : MonoBehaviour
    {
        public int bulletsShot = 0;
        public GameObject projectile;

        [SerializeField] private Transform _bulletStartPos;
        [SerializeField] private GameObject _tankTop;
        [SerializeField] private GameObject _tankBarrel;

        private TankController _tankController;
        private Vector3 _point;
        private float nextFire = 0.0F;


        private void Start()
        {
            _tankController = gameObject.GetComponent<TankController>();
        }

        // Update is called once per frame
        private void Update()
        {
            #region Calculate Mouse Pos

            var ray = Camera.main!.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                var position = _bulletStartPos.position;
                _point = new Vector3(hit.point.x, position.y, hit.point.z);
            }

            #endregion

            #region Shoot

            if (Input.GetKeyDown(KeyCode.Space) && Time.time > nextFire)
            {
                Fire();
            }

            #endregion
        }

        private void Fire()
        {
            nextFire = Time.time + _tankController._data.FireRate;
            var transform1 = _bulletStartPos.transform;
            var bullet = Instantiate(projectile, transform1.position,
                transform1.rotation);
            bullet.name = $"Bullet - {++bulletsShot} | {name}";
            bullet.GetComponent<Bullet>().MaxWallBounces = _tankController._data.BulletWallBounces;
            bullet.GetComponent<Bullet>().Speed = _tankController._data.BulletSpeed;
        }

        // rotate the object to the mouse position
        private void FixedUpdate()
        {
            RotateTurret();
        }

        private void RotateTurret()
        {
            // Determine which direction to rotate towards
            var targetDirection = _point - _tankTop.transform.position;

            // The step size is equal to speed times frame time.
            var singleStep = _tankController._data.turretTurningSpeedInDegree * Time.deltaTime;

            //singleStep is in degree make it to radian
            singleStep *= Mathf.Deg2Rad;

            // Rotate the forward vector towards the target direction by one step
            var newDirection = Vector3.RotateTowards(_tankTop.transform.forward, targetDirection, singleStep, 0.0f);

            // Calculate a rotation a step closer to the target and applies rotation to this object
            var rotation = Quaternion.LookRotation(newDirection);
            rotation = Quaternion.Euler(new Vector3(0, rotation.eulerAngles.y, 0)); // Clamp the x and z rotation

            _tankBarrel.transform.rotation = rotation;
            _tankTop.transform.rotation = rotation;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_point, 0.5f);
            if (Physics.Raycast(_bulletStartPos.position, _bulletStartPos.forward, out var hit, int.MaxValue))
            {
                Gizmos.DrawRay(_bulletStartPos.position, _bulletStartPos.forward * hit.distance);
            }
        }
    }
}