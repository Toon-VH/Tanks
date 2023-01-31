using System;
using Ai_s;
using Tank;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed;
    public int MaxWallBounces;
    private int _bounceCount;
    private Vector3 _nextHitPoint;
    private Vector3 _nextBounceDirection;
    private GameObject _prevCollider;

    private Color _color;
    private bool _isColliding;
    private bool _bounceFlag;
    private Vector3 _boxExtends;

    private void Start()
    {
        _bounceFlag = true;
        _boxExtends = GetComponent<BoxCollider>().size / 2;
        _color = UnityEngine.Random.ColorHSV(0, 1, 0, 1, 1, 1, 1, 1);
    }

    private void Update()
    {
        Transform transform1;
        (transform1 = transform).Translate(Vector3.forward * (Time.deltaTime * Speed));
        ShootRay(transform1.position, transform1.forward, 0);
    }

    private void ShootRay(Vector3 start, Vector3 direction, int counter)
    {
        while (true)
        {
            if (Physics.BoxCast(start, _boxExtends, direction, out var hit, Quaternion.identity, int.MaxValue))
                // if (Physics.Raycast(start, direction, out var hit, int.MaxValue))
            {
                Debug.DrawRay(start, direction * hit.distance, _color);

                if (hit.transform.CompareTag("Tank")) return;
                //Draw the normal - can only be seen at the Scene tab, for debugging purposes  
                Debug.DrawRay(hit.point, hit.normal * 3, Color.blue);

                if (counter >= MaxWallBounces - _bounceCount) return;

                var reflectDirection = Vector3.Reflect(direction, hit.normal);

                if (counter == 0)
                {
                    // Debug.Log($"HitP: {hit.point} - Reflection: {reflectDirection} - Bounces{_bounceCount}");
                    _nextHitPoint = hit.point;
                    _nextBounceDirection = reflectDirection;
                }

                start = hit.point;
                direction = reflectDirection;
                counter = ++counter;
                continue;
            }

            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);

            break;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (_isColliding) return;
        if (other.gameObject == _prevCollider) return;
        _isColliding = true;
        switch (other.tag)
        {
            case "Wall":
                if (_bounceCount >= MaxWallBounces)
                {
                    Debug.Log($"Wall - {gameObject.name} collided with {other.transform.name} and gets Destroyed");
                    Destroy(gameObject);
                }
                transform.forward = _nextBounceDirection;
                _bounceCount++;
                break;
            case "Bullet":
                Destroy(gameObject);
                Destroy(other.gameObject);
                Debug.Log($"Bullet - {gameObject.name} collided with {other.transform.name} Destroyed both");
                break;
            case "Tank":
                Debug.Log($"Tank - {gameObject.name} collided with {other.transform.name} Explode Tank");
                Destroy(gameObject);
                other.gameObject.GetComponent<TankController>().HitTank(transform.position);
                break;
            case "AITank":
                Debug.Log($"AITank - {gameObject.name} collided with {other.transform.name} Explode Tank");
                other.gameObject.GetComponent<AIController>().HitTank(transform.position);
                Destroy(gameObject);
                break;
        }

        _prevCollider = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        _isColliding = false;
        _bounceFlag = true;
    }
}