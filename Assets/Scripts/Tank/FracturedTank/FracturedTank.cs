using System.Collections;
using UnityEngine;

namespace Tank.FracturedTank
{
    public class FracturedTank : MonoBehaviour
    {
        public void Explode(Vector3 bulletImpact, float explosionMinForce, float explosionMaxForce,
            float explosionForceRadius, float fragScaleFactor)
        {
            foreach (Transform t in transform)
            {
                var rb = t.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(Random.Range(explosionMinForce, explosionMaxForce),
                        bulletImpact,
                        explosionForceRadius);
                }

                StartCoroutine(Shrink(t, 0, fragScaleFactor));
            }
        }

        private static IEnumerator Shrink(Transform t, float delay, float fragScaleFactor)
        {
            yield return new WaitForSeconds(delay);

            var newScale = t.localScale;

            fragScaleFactor = 0.999f;
            while (newScale.x >= 0.02)
            {
                fragScaleFactor -= 0.0001f;
                newScale *= fragScaleFactor;
                t.localScale = newScale;
                yield return new WaitForSeconds(0.05f);
            }
        }
    }
}