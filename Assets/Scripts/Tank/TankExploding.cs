using Unity.Netcode;
using UnityEngine;

namespace Tank
{
    public class TankExploding : MonoBehaviour
    {
        [SerializeField] public GameObject originalObject;
        [SerializeField] public GameObject FracturedObject;
        [SerializeField] public GameObject explosionVFX;

        public float explosionMinForce = 5;
        public float explosionMaxForce = 100;
        public float explosionForceRadius = 10;
        public float fragScaleFactor = 0.5f;

        private GameObject fractureObj;


        public void Explode(Vector3 bulletImpact)
        {
            originalObject.SetActive(false);
            fractureObj = Instantiate(FracturedObject, originalObject.transform.position,
                originalObject.transform.rotation);

            fractureObj.GetComponent<FracturedTank.FracturedTank>().Explode(bulletImpact, explosionMinForce,
                explosionMaxForce,
                explosionForceRadius, fragScaleFactor);


            Destroy(fractureObj, 20);

            if (explosionVFX == null) return;
            var exploVFX = Instantiate(explosionVFX, bulletImpact, Quaternion.identity);
            Destroy(exploVFX, 7);
            Destroy(gameObject, 7);
        }
    }
}