using System;
using System.Collections.Generic;
using UnityEngine;

namespace WinterFeather
{
    public static class UnityTools
    {
        public static Transform FindTransformInChild(Transform transform, string name)
        {
            Transform[] transforms = transform.GetComponentsInChildren<Transform>();
            for (int i = 0; i < transforms.Length; i++)
            {
                if (transforms[i].name == name)
                {
                    return transforms[i];
                }
            }
            return null;
        }
        public static bool OutOfRangeCheck<T>(Vector2Int index, T[,] array)
        {
            return index.x < 0 || index.y < 0 || index.x > array.GetLength(0) - 1 || index.y > array.GetLength(1) - 1;
        }
        public static Vector3 GetLocalPos(Transform parent, Vector3 worldpos)
        {
            return Quaternion.Inverse(parent.rotation) * (worldpos - parent.transform.position);
        }
        public static T FindComponentInRange<T>(Vector3 pos, float rad, int layerMask) where T : MonoBehaviour
        {
            Collider[] colliders = Physics.OverlapSphere(pos, rad, layerMask);
            T activeObject = null;

            for (int i = 0; i < colliders.Length; i++)
            {
                activeObject = colliders[i].GetComponent<T>();
                if (activeObject != null)
                    return activeObject;
            }
            return activeObject;
        }
        public static List<T> FindComponentsInRange<T>(Vector3 pos, float rad, int layerMask) where T : MonoBehaviour
        {
            Collider[] colliders = Physics.OverlapSphere(pos, rad);
            List<T> activeObjects = new List<T>();

            for (int i = 0; i < colliders.Length; i++)
            {
                T activeObject = colliders[i].GetComponent<T>();
                if (activeObject != null)
                {
                    activeObjects.Add(activeObject);
                }
            }
            return activeObjects;
        }
        public static Transform CopyTransform(Transform transform, bool sameParent = true)
        {
            Transform t = new GameObject().transform;
            t.position = transform.position;
            t.rotation = transform.rotation;
            if (sameParent) t.SetParent(transform.parent);
            return t;
        }
        public static GameObject InstantiateToChildWhithClear(GameObject go, Transform point, Vector3 localPostion, Quaternion localRotation)
        {
            ClearChild(point);
            return InstantiateToChild(go, point);
        }
        public static GameObject InstantiateToChildWhithClear(GameObject go, Transform point)
        {
            return InstantiateToChildWhithClear(go, point, Vector3.zero, Quaternion.identity);
        }
        public static GameObject InstantiateToChild(GameObject go, Transform point, Vector3 localPostion, Quaternion localRotation)
        {
            if (go != null)
            {
                GameObject g = GameObject.Instantiate<GameObject>(go);
                g.transform.SetParent(point);
                g.transform.localPosition = localPostion;
                g.transform.localRotation = localRotation;
                return g;
            }
            return null;
        }
        public static GameObject InstantiateToChild(GameObject go, Transform point)
        {
            return InstantiateToChild(go, point, Vector3.one, Quaternion.identity);
        }
        public static void SetChildsLayer(Transform origin, Transform target)
        {
            SetChildsLayer(origin, target.gameObject.layer);
        }
        public static void SetChildsLayer(Transform origin, int layer)
        {
            Transform[] childTransform = origin.GetComponentsInChildren<Transform>();
            for (int i = 0; i < childTransform.Length; i++)
            {
                childTransform[i].gameObject.layer = layer;
            }
        }
        public static T GetComponent<T>(GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (component == null) component = gameObject.AddComponent<T>();
            return component;
        }
        public static void ClearChild(Transform transform)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject.Destroy(transform.GetChild(i).gameObject);
            }
        }

    }

    public class SingleManager<T> : MonoBehaviour where T : Component
    {
        static T instance;
        static bool isDestroy = false;
        public static T Instance
        {
            get
            {
                if (isDestroy) return null;
                if (instance == null)
                {
                    instance = FindObjectOfType(typeof(T)) as T;
                    if (instance == null)
                    {
                        instance = new GameObject(typeof(T).ToString()).AddComponent<T>();
                    }
                }
                return instance;
            }
        }

        protected void Awake() {
            Init();
        }

        void Init() {
            this.gameObject.hideFlags = HideFlags.HideInHierarchy;
            this.hideFlags = HideFlags.HideInInspector;
        }

        void OnDestroy() {
            isDestroy = true;
        }
    }
}