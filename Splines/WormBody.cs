using System.Collections.Generic;
using UnityEngine;

namespace BLBits
{
    public class WormBody : MonoBehaviour
    {
        public int resolution;
        public float radius;

        private Mesh mesh;

        public bool fullUvs;

        public int splineDivisions;

        private List<Vector3> splinePoints;

        public Transform startPoint;
        public Transform endPoint;

        public float startRotation;
        public float endRotation;

        public float startAngleOffset;
        public float endAngleOffset;

        [Range(0, 1)]
        public float foodPosition;

        public bool randomScale;
        public float minScaleMultiplier;
        public float maxScaleMultiplier;

        Vector3[] vertices;
        Vector2[] uv;
        int[] triangles;

        private Material bodyMat;

        private bool initialised = false;

        public bool flat = false;

        public void InitialiseSplinePoints()
        {
            if (!initialised)
            {
                initialised = true;
                splinePoints = new List<Vector3>();
                mesh = new Mesh();
            }
        }

        // Use this for initialization
        void Start()
        {
            mesh = new Mesh();

            splinePoints = new List<Vector3>();

            bodyMat = GetComponent<Renderer>().material;
        }

        void RegenerateSplineMesh()
        {
            if (resolution < 3 || splineDivisions < 1)
            {
                return;
            }

            Vector3 lastForward = Vector3.forward;
            Vector3 lastUp = Vector3.up;
            Vector3 lastRight = Vector3.right;

            for (int point = 0; point < splinePoints.Count; point++)
            {
                Vector3 pos = splinePoints[point];
                Vector3 forward;

                if (point >= splinePoints.Count - 1)
                {
                    forward = pos - splinePoints[point - 1];
                }
                else
                {
                    forward = splinePoints[point + 1] - pos;
                }

                Quaternion axisRotation = Quaternion.FromToRotation(lastForward, forward);
                Vector3 right = axisRotation * lastRight;
                Vector3 up = axisRotation * lastUp;

                lastForward = forward;
                lastRight = right;
                lastUp = up;

                for (int vert = 0; vert < resolution; vert++)
                {
                    float angle = (float)vert / (float)resolution * 2 * Mathf.PI;

                    if (startPoint && point == 0)
                    {
                        right = startPoint.right;
                        up = startPoint.up;
                        forward = startPoint.forward;
                        angle += startRotation * Mathf.Deg2Rad;//Quaternion.Angle(Quaternion.Euler(startRotation), Quaternion.LookRotation(up));
                    }
                    else if (endPoint && point == (splinePoints.Count - 1))
                    {
                        right = endPoint.right;
                        up = endPoint.up;
                        forward = startPoint.forward;
                        angle += endRotation * Mathf.Deg2Rad;//Quaternion.Angle(Quaternion.Euler(endRotation), Quaternion.LookRotation(up));
                    }
                    else
                    {
                        angle += Mathf.Lerp(startAngleOffset, endAngleOffset, (float)point / splinePoints.Count);
                    }

                    float radiusScale = radius;// * (1.0f + (Mathf.Sin(Mathf.PI * foodPosition) * (Mathf.Sin((Mathf.Clamp(((float)point / splinePoints.Count) - foodPosition, -0.05f, 0.05f) * 10.0f + 0.5f) * Mathf.PI))));

                    vertices[point * resolution + vert] = transform.InverseTransformPoint(pos + up * radiusScale * Mathf.Sin(angle) - right * radiusScale * Mathf.Cos(angle));

                }
            }

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            GetComponent<MeshFilter>().mesh = mesh;
            GetComponent<MeshFilter>().mesh.RecalculateBounds();
        }

        public void CreateSplineMesh()
        {
            if (resolution < 3 || splineDivisions < 1)
            {
                return;
            }
            BezierSpline spline = GetComponent<BezierSpline>();

            splinePoints.Clear();

            if (startPoint)
            {
                splinePoints.Add(startPoint.position);
            }

            for (int splinePoint = 0; splinePoint < splineDivisions + (endPoint == null ? 1 : 0); splinePoint++)
            {
                splinePoints.Add(spline.GetPointAtDistance(((float)splinePoint / (float)splineDivisions) * (float)spline.GetLength()));
            }

            if (endPoint)
            {
                splinePoints.Add(endPoint.position);
            }

            vertices = new Vector3[(resolution) * splinePoints.Count];
            uv = new Vector2[(resolution) * splinePoints.Count];
            triangles = new int[(resolution * splinePoints.Count) * 6 + 6];

            Vector3 lastForward = Vector3.forward;
            Vector3 lastUp = Vector3.up;
            Vector3 lastRight = Vector3.right;

            for (int point = 0; point < splinePoints.Count; point++)
            {
                Vector3 pos = splinePoints[point];
                Vector3 forward;
                if (point >= splinePoints.Count - 1)
                {
                    forward = pos - splinePoints[point - 1];
                }
                else
                {
                    forward = splinePoints[point + 1] - pos;
                }

                Quaternion axisRotation = Quaternion.FromToRotation(lastForward, forward);
                Vector3 right = axisRotation * lastRight;
                Vector3 up = axisRotation * lastUp;

                lastForward = forward;
                lastUp = up;
                lastRight = right;

                for (int vert = 0; vert < resolution; vert++)
                {
                    float angle = (float)vert / (float)resolution * 2 * Mathf.PI;

                    if (startPoint && point == 0)
                    {
                        forward = startPoint.forward;
                        right = -startPoint.right;
                        up = startPoint.up;
                        angle += startRotation * Mathf.Deg2Rad;
                    }
                    else if (endPoint && point == (splinePoints.Count - 1))
                    {
                        forward = endPoint.forward;
                        right = -endPoint.right;
                        up = -endPoint.up;
                        angle += endRotation * Mathf.Deg2Rad;
                    }
                    else
                    {
                        angle += Mathf.Lerp(startAngleOffset, endAngleOffset, (float)point / splinePoints.Count) * Mathf.Deg2Rad;
                    }

                    float radiusScale = radius * (1.0f + (Mathf.Sin(Mathf.PI * foodPosition) * (Mathf.Sin((Mathf.Clamp(((float)point / splinePoints.Count) - foodPosition, -0.05f, 0.05f) * 10.0f + 0.5f) * Mathf.PI))));

                    if (randomScale)
                    {
                        radiusScale = radius * Random.Range(minScaleMultiplier, maxScaleMultiplier);
                    }

                    if (flat)
                    {
                        vertices[point * resolution + vert] = transform.InverseTransformPoint(pos - right * radiusScale * Mathf.Cos(angle));
                    }
                    else
                    {
                        vertices[point * resolution + vert] = transform.InverseTransformPoint(pos + up * radiusScale * Mathf.Sin(angle) - right * radiusScale * Mathf.Cos(angle));
                    }

                    if (fullUvs)
                    {
                        uv[point * resolution + vert] = new Vector2((float)point / splinePoints.Count, (float)vert / resolution);
                    }
                    else
                    {
                        uv[point * resolution + vert] = new Vector2(point % 2, Mathf.Sin(angle));
                    }
                }
            }

            int index = 0;
            for (int vert = 0; vert < vertices.Length - resolution - 1; vert++)
            {
                triangles[index] = vert;
                triangles[index + 1] = vert + resolution;
                triangles[index + 2] = vert + 1;
                triangles[index + 3] = vert + 1;
                triangles[index + 4] = vert + resolution;
                triangles[index + 5] = vert + resolution + 1;

                index += 6;
            }

            triangles[index] = 0;
            triangles[index + 1] = resolution - 1;
            triangles[index + 2] = resolution;
            triangles[index + 3] = vertices.Length - 1;
            triangles[index + 4] = (splinePoints.Count - 1) * resolution;
            triangles[index + 5] = (splinePoints.Count - 1) * resolution - 1;

            if (mesh == null)
            {
                mesh = new Mesh();
            }
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            GetComponent<MeshFilter>().mesh = mesh;
            GetComponent<MeshFilter>().sharedMesh.RecalculateBounds();
        }

        public void SetFoodPosition(float t)
        {
            foodPosition = t;
            bodyMat.SetFloat("_FoodPosition", t);
        }
    }
}