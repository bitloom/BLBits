//from Catlike Coding: https://catlikecoding.com/unity/tutorials/curves-and-splines/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

#endif

namespace BLBits
{
    public class SplineDecorator : MonoBehaviour
    {
        public BezierSpline spline;

        public float decorationSpacing = 0.1f;
        public Vector3 decorationOffset = Vector3.zero;

        public bool lookForward;
        public Vector3 lookOffset;

        public bool updateDecorations;

        public bool chooseRandomDecoration = true;
        public List<GameObject> decorationPrefabs;
        private List<GameObject> decorations;

        public bool randomScale;
        public Vector3 minScale;
        public Vector3 maxScale;

        public bool randomRotation;
        public Vector3 rotationAxis;

        public bool randomOffset;
        public Vector3 maxOffset;

        public bool alignToSurface;
        public Vector3 alignOffset;

        private int curDecoration = 0;


#if UNITY_EDITOR
        public void UpdateDecorations()
        {
            if (spline == null)
            {
                spline = gameObject.GetComponent<BezierSpline>();
            }

            if (decorations == null)
            {
                decorations = new List<GameObject>();
            }

            if (decorationPrefabs.Count == 0)
            {
                Debug.Log("no decoration prefab set");

                return;
            }

            ClearDecorations();

            PlaceDecorationsAlongSpline();
        }

        void ClearDecorations()
        {
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }

            decorations = new List<GameObject>();
        }

        void PlaceDecorationsAlongSpline()
        {
            int decorationCount = (int)(spline.GetLength() / decorationSpacing);

            for (int decorationIndex = 0; decorationIndex < decorationCount; decorationIndex++)
            {
                Vector3 decorationPosition = spline.GetPointAtDistance((float)decorationIndex * decorationSpacing);
                Vector3 nextDecorationPosition = spline.GetPointAtDistance((float)(decorationIndex + 1) * decorationSpacing);

                PlaceDecoration(decorationPosition, nextDecorationPosition);
            }
        }

        void PlaceDecoration(Vector3 position, Vector3 nextPosition)
        {
            int randomPrefab = Random.Range(0, decorationPrefabs.Count);
            if (!chooseRandomDecoration)
            {
                randomPrefab = curDecoration;
                curDecoration++;
                if (curDecoration >= decorationPrefabs.Count)
                {
                    curDecoration = 0;
                }
            }

            if (decorationPrefabs[randomPrefab] == null)
            {
                Debug.Log("Decoration prefab entry not set");
            }
            else
            {
                Quaternion decorationRotation = Quaternion.identity;

                if (lookForward)
                {
                    decorationRotation = Quaternion.LookRotation((nextPosition - position).normalized) * Quaternion.Euler(lookOffset);
                }
                else if (randomRotation)
                {
                    decorationRotation = Quaternion.AngleAxis(Random.Range(0, 360.0f), rotationAxis);
                }

                Vector3 decorationPosition = position;

                if (randomOffset)
                {
                    decorationPosition += decorationRotation * (new Vector3(Random.Range(-maxOffset.x, maxOffset.x), Random.Range(-maxOffset.y, maxOffset.y), Random.Range(-maxOffset.z, maxOffset.z)));
                }

                GameObject decoration = PrefabUtility.InstantiatePrefab(decorationPrefabs[randomPrefab]) as GameObject;
                decoration.transform.position = decorationPosition += decorationOffset;
                decoration.transform.rotation = decorationRotation;
                decoration.transform.parent = transform;

                if (randomScale)
                {
                    decoration.transform.localScale = minScale + (maxScale - minScale) * Random.Range(0.0f, 1.0f);
                }

                if (alignToSurface)
                {
                    RaycastHit hitInfo;
                    bool hit = Physics.Raycast(decoration.transform.position, -Vector3.up, out hitInfo, 1000.0f, 1 << LayerMask.NameToLayer("Default"), QueryTriggerInteraction.Ignore);

                    if (hit)
                    {
                        Vector3 normal = hitInfo.normal;

                        if (lookForward)
                        {
                            RaycastHit directionHitInfo;
                            bool directionHit = Physics.Raycast(nextPosition, -Vector3.up, out directionHitInfo, 1000.0f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

                            if (directionHit)
                            {
                                decoration.transform.rotation = Quaternion.LookRotation(directionHitInfo.point - hitInfo.point) * Quaternion.Euler(lookOffset);
                            }
                            else
                            {
                                decoration.transform.rotation = Quaternion.LookRotation((nextPosition - position).normalized, normal) * Quaternion.Euler(lookOffset);
                            }
                        }
                        else if (randomRotation)
                        {
                            decoration.transform.rotation = Quaternion.FromToRotation(decoration.transform.up, normal);
                            decoration.transform.RotateAround(decoration.transform.position, decoration.transform.up, Random.Range(0.0f, 360.0f));
                        }

                        decoration.transform.position = hitInfo.point + alignOffset;

                        if (randomOffset)
                        {
                            decoration.transform.position += decorationRotation * (new Vector3(Random.Range(-maxOffset.x, maxOffset.x), Random.Range(-maxOffset.y, maxOffset.y), Random.Range(-maxOffset.z, maxOffset.z)));
                        }
                    }
                }

                decorations.Add(decoration);
            }
        }
#endif
    }
}