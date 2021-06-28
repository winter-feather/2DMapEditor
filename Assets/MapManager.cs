using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFMapTools
{
    public class MapManager : SingleManager<MapManager>
    {
        Dictionary<Vector2Int, MapNode> world;
        public Vector2Int unitSize;
        public long nodeSeed;

        private void Awake()
        {
            unitSize = new Vector2Int(100, 100);
        }
        // Start is called before the first frame update
        void Start()
        {
            MapNode n = new MapNode(Vector2Int.zero, new MapData());
        }

        // Update is called once per frame
        void Update()
        {

        }

        //public Vector3 MapIndex2StartPos(Vector2Int index) { 

        //}

        public void CreateMap(MapNode node)
        {


        }

    }

    public class MapNode
    {
        public Vector2Int index;
        public MapData data;
        public MapNode(Vector2Int index, MapData data)
        {
            this.index = index;
            this.data = data;
        }
        MainManager manager;
        public Vector2Int size;
        public Collider contrller;
        public MainManager Manager
        {
            get
            {
                if (manager == null) manager = MapManager.Instance;
                return manager;
            }
        }

        public void CreateMap()
        {

        }
    }


    public class MapData
    {

        byte[,] data;
        public void InitData()
        {
            data = new byte[1, 1];
                 WFMapTools.MapManager.Instance.brush = null;
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



    void OnDestroy()
    {
        isDestroy = true;
    }
}