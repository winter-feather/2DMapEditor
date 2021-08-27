using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFMapTools
{
    public class MapManager : SingleMonoManager<MapManager>
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
            //unitSize = new Vector2Int(10, 10);
            perlinNoise = Vector2.one * 5;
        }
        // Start is called before the first frame update
        void Start()
        {
            shower = new ColorShower();
            //shower = new TextureShower();
            CreateNewMap(Vector2Int.zero);
            CreateNewMap(new Vector2Int(0, 1));
            CreateNewMap(new Vector2Int(0, 2));

            foreach (var item in world)
            {
                shower.InitMapNode(item.Value);
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
            #region PNTest
            SetMapData4PN(data, index);
            #endregion

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

        public MapData( )
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
        public byte[] SaveData2Byte()
        {
            return new byte[] { };
        }

        public void LoadData4Json(string jsonData)
        {

        }

        public void LoadData4Byte(byte[] byteData)
        {

        }



    }

    public class ColorShower : IMapShower
    {
        Dictionary<Vector2Int, MapNode> maps;
        Dictionary<Vector2Int, GameObject> nodes;
        public ColorShower() {
            nodes = new Dictionary<Vector2Int, GameObject>();
        }

        public void InitMapNode(MapNode showNode)
        {
            Vector2Int index = Vector2Int.one;
            for (int i = 0; i < showNode.data.data.GetLength(0); i++)
            {
                for (int j = 0; j < showNode.data.data.GetLength(1); j++)
                {
                    index.x = i;index.y = j;
                    Show(showNode, index);
                }
            }
        }
        Vector2Int TransToWorldIndex(Vector2Int nodeIndex,Vector2Int index) {
            int x = nodeIndex.x * MapManager.Instance.unitSize.x + index.x;
            int y = nodeIndex.y * MapManager.Instance.unitSize.y + index.y;
            return new Vector2Int(x, y);
        }
        public void Show(MapNode node) { 
        
        
        }
        public void Show(MapNode node, Vector2Int index)
        {
            Vector2Int pos = TransToWorldIndex(node.index, index);
            GameObject go;
            if (nodes.ContainsKey(pos)) go = nodes[pos];
            else
            {
                go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                nodes.Add(pos, go);
            }
            go.transform.position = new Vector3(pos.x, node.data.data[index.x, index.y] / 32, pos.y);
            go.GetComponent<MeshRenderer>().material.color = Color.HSVToRGB(node.data.data[index.x, index.y] / 256f, 0.5f, 0.5f);
        }

    }

    public class TextureShower : IMapShower
    {
        
        public TextureShower() { 
        
        }

        public MapNode Map => throw new System.NotImplementedException();

        public void InitMapNode(MapNode showNode)
        {
            GameObject panel = GameObject.CreatePrimitive(PrimitiveType.Quad);
            panel.transform.position = new Vector3(showNode.index.x * MapManager.Instance.unitSize.x, 0, showNode.index.y * MapManager.Instance.unitSize.y);
            panel.transform.rotation = Quaternion.AngleAxis( 90, Vector3.right);
            panel.transform.localScale = new Vector3(showNode.data.data.GetLength(0), showNode.data.data.GetLength(1));


            for (int i = 0; i < showNode.data.data.GetLength(0); i++)
            {
                for (int j = 0; j < showNode.data.data.GetLength(1); j++)
                {
                    //data.data[i, j] = (byte)(Mathf.PerlinNoise((i + offsetX) / (float)data.data.GetLength(0) * perlinNoise.x, (j + offsetY) / (float)data.data.GetLength(1) * perlinNoise.y) * 256f);
                }
            }
        }

        public void Show(MapNode node, Vector2Int index)
        {
            throw new System.NotImplementedException();
        }
    }

    public class SpriteShower : IMapShower
    {
        public MapNode Map => throw new System.NotImplementedException();

        public void InitMapNode(MapNode showNode)
        {
            for (int i = 0; i < showNode.data.data.GetLength(0); i++)
            {
                for (int j = 0; j < showNode.data.data.GetLength(1); j++)
                {
                    int x = i + showNode.index.x * MapManager.Instance.unitSize.x;
                    int y = j + showNode.index.y * MapManager.Instance.unitSize.y;
                    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //GameObject.Instantiate<GameObject>();
                    go.transform.position = new Vector3(x, 0, y);
                    Debug.LogError(showNode.data.data[i, j]);
                    go.GetComponent<MeshRenderer>().material.color = Color.HSVToRGB(showNode.data.data[i, j] / 256f, 0.5f, 0.5f);  //new Color(showNode.data.data[i, j] / 256f, showNode.data.data[i, j] / 256f, showNode.data.data[i, j] / 256f);
                }
            }
        }

        public void Show(MapNode node, Vector2Int index)
        {
            throw new System.NotImplementedException();
        }
    }
    public interface IMapShower
    {

        void InitMapNode(MapNode showNode);
        void Show(MapNode node, Vector2Int index);
    }

    public interface IMapContrler {
        void Update();
    }

    public class ShowerManager : SingleMonoManager<ShowerManager> { 
        
    }

    public class BrushManager : SingleMonoManager<BrushManager> { 
    
    }
}


//public class SingleMa

public class SingleMonoManager<T> : MonoBehaviour where T : Component
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