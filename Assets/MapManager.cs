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
        IMapShower shower;
        IMapContrler contrler;
        Vector2 perlinNoise;
        
        private void Awake()
        {
            
            world = new Dictionary<Vector2Int, MapNode>();
            unitSize = new Vector2Int(100, 100);
            perlinNoise = Vector2.one * 5;
        }
        // Start is called before the first frame update
        void Start()
        {
            shower = new ColorShower();
            CreateNewMap(Vector2Int.zero);
            CreateNewMap(new Vector2Int(0, 1));
            CreateNewMap(new Vector2Int(0, 2));

            foreach (var item in world)
            {
                shower.Show(item.Value);
            }

        }

        // Update is called once per frame
        void Update()
        {

        }

        //public Vector3 MapIndex2StartPos(Vector2Int index) { 

        //}

        public void CreateNewMap(Vector2Int index)
        {
            MapData data = new MapData();

            SetMapData4PN(data, index);
            MapNode mn = new MapNode(index, data);
            mn.index = index;
            world[index] = mn;
            //mn.shower = colorShower;
        }

        public void CreateMap(MapNode node)
        {

        }

        void SetMapData4PN(MapData data, Vector2Int index)
        {
            int offsetX = data.data.GetLength(0) * index.x;
            int offsetY = data.data.GetLength(1) * index.y;

            for (int i = 0; i < data.data.GetLength(0); i++)
            {
                for (int j = 0; j < data.data.GetLength(1); j++)
                {
                    data.data[i, j] = (byte)(Mathf.PerlinNoise((i + offsetX) / (float)data.data.GetLength(0)* perlinNoise.x, (j + offsetY) / (float)data.data.GetLength(1) * perlinNoise.y) * 256f);
                }
            }
        }

    }

    public class MapNode
    {
        MapManager manager;
        public Vector2Int size;
        public Collider contrller;
        public Vector2Int index;
        public MapData data;

        public MapNode(Vector2Int index, MapData data)
        {
            this.index = index;
            this.data = data;
        }

        public MapManager Manager
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
        public MapData()
        {
            InitData();
        }

        public byte[,] data;
        public void InitData()
        {
            data = new byte[WFMapTools.MapManager.Instance.unitSize.x, WFMapTools.MapManager.Instance.unitSize.y];
        }

        public string SaveData2Json()
        {
            return "";
        }

        public void LoadData4Json(string jsonData)
        {

        }

        public void LoadData4Byte(byte[] byteData)
        {

        }

        public byte[] SaveData2Byte()
        {
            return new byte[] { };
        }

    }

    public class ColorShower : IMapShower
    {
        public void Show(MapNode showNode)
        {
            for (int i = 0; i < showNode.data.data.GetLength(0); i++)
            {
                for (int j = 0; j < showNode.data.data.GetLength(1); j++)
                {
                    int x = i + showNode.index.x * MapManager.Instance.unitSize.x;
                    int y = j + showNode.index.y * MapManager.Instance.unitSize.y;
                    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    go.transform.position = new Vector3(x, 0, y);
                    //Debug.LogError(showNode.data.data[i, j]);
                    go.GetComponent<MeshRenderer>().material.color = Color.HSVToRGB(showNode.data.data[i, j] / 256f, 0.5f, 0.5f);  //new Color(showNode.data.data[i, j] / 256f, showNode.data.data[i, j] / 256f, showNode.data.data[i, j] / 256f);
                }
            }
        }
    }

    public interface IMapShower
    {
        void Show(MapNode showNode);
    }

    public interface IMapContrler {
        void Update();
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