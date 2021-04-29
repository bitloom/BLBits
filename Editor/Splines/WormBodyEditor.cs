using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

namespace BLBits
{
    [CustomEditor(typeof(WormBody)), CanEditMultipleObjects]
    public class WormBodyEditor : Editor
    {
        private static GUIStyle toggleButtonStyleNormal = null;
        private static GUIStyle toggleButtonStyleToggled = null;

        private bool update = false;
        private int updateFrequency = 5;
        private int updateCounter = 5;

        void Awake()
        {
            (target as WormBody).InitialiseSplinePoints();
            updateCounter = updateFrequency;
            update = false;
        }

        public override void OnInspectorGUI()
        {
            if (toggleButtonStyleNormal == null || toggleButtonStyleToggled == null)
            {
                toggleButtonStyleNormal = "Button";
                toggleButtonStyleToggled = new GUIStyle(toggleButtonStyleNormal);
                toggleButtonStyleToggled.normal.background = toggleButtonStyleToggled.active.background;
            }

            base.OnInspectorGUI();

            if (GUILayout.Button("Update Mesh", update ? toggleButtonStyleToggled : toggleButtonStyleNormal))
            {
                update = !update;
                //(target as WormBody).CreateSplineMesh();
            }

            if (GUILayout.Button("Update Collider"))
            {
                GameObject gameObj = (target as WormBody).gameObject;
                MeshCollider collider = gameObj.GetComponent<MeshCollider>();
                if (collider == null)
                {
                    collider = gameObj.AddComponent<MeshCollider>();
                }

                collider.sharedMesh = gameObj.GetComponent<MeshFilter>().sharedMesh;
            }

        }

        void OnSceneGUI()
        {
            if (Selection.Contains((target as WormBody).gameObject))
            {
                if (update)
                {
                    updateCounter--;
                    if (updateCounter <= 0)
                    {
                        (target as WormBody).CreateSplineMesh();
                        updateCounter = updateFrequency;
                    }
                }
            }
        }
    }
}
#endif