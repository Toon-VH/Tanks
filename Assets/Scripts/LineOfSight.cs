using System;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    private void Start()
    {
        var mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        var fov = 90f;
        var origin = Vector3.zero;
        var rayCount = 2;
        float angle = 0;
        var angleIncrease = fov / rayCount;
        var viewDistance = 50f;

        var vertices = new Vector3[rayCount + 1 + 1];
        var uvs = new Vector2[vertices.Length];
        var triangles = new int[rayCount * 3];

        vertices[0] = origin;
        for (var i = 0; i <= rayCount; i++)
        {
            var angleRad = Mathf.Deg2Rad * angle;
            var dir = new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad));
            if (Physics.Raycast(origin, dir, out var hit, viewDistance))
            {
                vertices[i + 1] = hit.point;
            }
            else
            {
                vertices[i + 1] = origin + dir * viewDistance;
            }

            angle -= angleIncrease;
        }

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
    }
}