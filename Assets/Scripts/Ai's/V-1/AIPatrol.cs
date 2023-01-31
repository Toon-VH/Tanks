using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Ai_s.V_1
{
    public class AIPatrol : MonoBehaviour
    {
        [SerializeField] public float _minDestinationChangeDelay = 6f;
        [SerializeField] public float _maxDestinationChangeDelay = 20f;
        [HideInInspector] public AIController aiController;

        private bool _patrolling;
        private NavMeshAgent _agent;


        private void Start()
        {
            _agent = gameObject.GetComponent<NavMeshAgent>();
            aiController = gameObject.GetComponent<AIController>();
        }

        private void Update()
        {
            if (aiController._state == AIState.Patrol)
            {
                _agent.enabled = true;
                if (_patrolling) return;
                _patrolling = true;
                StartCoroutine(SetRandomNavMeshPos(_agent));
            }

            else
            {
                _patrolling = false;
                _agent.enabled = false;
                StopAllCoroutines();
            }
        }

        private IEnumerator SetRandomNavMeshPos(NavMeshAgent agent)
        {
            for (;;)
            {
                agent.SetDestination(GetRandomLocation());
                yield return new WaitForSeconds(Random.Range(_minDestinationChangeDelay, _maxDestinationChangeDelay));
            }
        }
        
        private Vector3 GetRandomLocation()
        {
            var navMeshData = NavMesh.CalculateTriangulation();

            var maxIndices = navMeshData.indices.Length - 3;
            // Pick the first indice of a random triangle in the nav mesh
            var firstVertexSelected = Random.Range(0, maxIndices);
            var secondVertexSelected = Random.Range(0, maxIndices);
            //Spawn on Verticies
            var point = navMeshData.vertices[navMeshData.indices[firstVertexSelected]];

            var firstVertexPosition = navMeshData.vertices[navMeshData.indices[firstVertexSelected]];
            var secondVertexPosition = navMeshData.vertices[navMeshData.indices[secondVertexSelected]];
            //Eliminate points that share a similar X or Z position to stop grid line formations
            if ((int)firstVertexPosition.x == (int)secondVertexPosition.x ||
                (int)firstVertexPosition.z == (int)secondVertexPosition.z
               )
            {
                point = GetRandomLocation(); //Re-Roll a position - I'm not happy with this recursion it could be better
            }
            else
            {
                // Select a random point on it
                point = Vector3.Lerp(
                    firstVertexPosition,
                    secondVertexPosition, //[t + 1]],
                    Random.Range(0.05f, 0.95f) // Not using Random.value as clumps form around Verticies 
                );
            }

            return point;
        }
    }
}