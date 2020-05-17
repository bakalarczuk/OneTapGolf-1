using System.Collections.Generic;
using UnityEngine;

namespace MustHave.Utilities
{
    public static class TransformExtensionMethods
    {
        public static void DestroyAllChildren(this Transform transform)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject.Destroy(transform.GetChild(i).gameObject);
            }
            transform.DetachChildren();
        }

        public static void DestroyAllChildrenImmediate(this Transform transform)
        {
            List<Transform> children = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                children.Add(transform.GetChild(i).transform);
            }
            foreach (Transform child in children)
            {
                GameObject.DestroyImmediate(child.gameObject);
            }
            transform.DetachChildren();
        }

        public static T GetComponentInParents<T>(this Transform transform) where T : Component
        {
            Transform parent = transform.parent;
            T component = null;
            while (parent && component == null)
            {
                component = parent.GetComponent<T>();
                parent = parent.parent;
            }
            return component;
        }

        public static bool IsInCameraView(this Transform transform, Camera camera, float worldHeight, float viewportOffsetX = 0f, float viewportOffsetY = 0f)
        {
            Vector3 p1 = transform.position;
            Vector3 p2 = p1 + transform.up * worldHeight;

            Vector3 screenP1 = camera.WorldToViewportPoint(p1);
            Vector3 screenP2 = camera.WorldToViewportPoint(p2);

            float viewportXmin = -viewportOffsetX;
            float viewportXmax = 1f + viewportOffsetX;
            float viewportYmin = -viewportOffsetY;
            float viewportYmax = 1 + viewportOffsetY;

            Rect viewportRect = Rect.MinMaxRect(viewportXmin, viewportYmin, viewportXmax, viewportYmax);

            bool isInCameraView =
                (viewportRect.Contains(screenP1) && screenP1.z > camera.nearClipPlane && screenP1.z < camera.farClipPlane) ||
                (viewportRect.Contains(screenP2) && screenP2.z > camera.nearClipPlane && screenP2.z < camera.farClipPlane);

            return isInCameraView;
        }
    }
}
