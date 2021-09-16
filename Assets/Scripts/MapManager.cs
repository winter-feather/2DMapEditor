using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFMapTools
{
    public class MapManager : SingleMonoManager<MapManager>
    {
        Dictionary<Vector2Int, MapNode> world;
        public Vector2Int mapUnit;
        public long nodeSeed;
        IMapShower shower;
        IMapContrler contrler;
        Vector2 perlinNoise;

        private void Awake()
        {
            world = new Dictionary<Vector2Int, MapNode>();
            perlinNoise = Vector2.one * 5;
        }
        // Start is called before the first frame update
        void Start()
        {
            //shower = new ColorShower();
            MapNode.perlinNoiseAmplitude = 0.05f;
            MapNode.perlinNoiseOffsetSeed = new Vector2Int(1000, 1000);
            shower = new TextureShower();
            CreateNewMap(Vector2Int.zero);
            CreateNewMap(new Vector2Int(0, 1));
            CreateNewMap(new Vector2Int(0, 2));

            foreach (var item in world)
            {
                item.Value.InitByPerlinNoise();
                shower.InitMapNode(item.Value);
            }

        }


        public void CreateNewMap(Vector2Int index)
        {
            MapData data = new MapData();

            MapNode mn = new MapNode(index, data);
            mn.index = index;
            world[index] = mn;
        }

        public void CreateMap(MapNode node)
        {

        }



    }

    public class MapNode
    {
        MapManager manager;
        public static float perlinNoiseAmplitude = 0.1f;
        public static Vector2Int perlinNoiseOffsetSeed = Vector2Int.zero;
        public Vector2Int size;
        public Collider contrller;
        public Vector2Int index;
        public MapData data;

        public MapNode(Vector2Int index, MapData data)
        {
            this.index = index;
            this.data = data;
        }

        public void InitByPerlinNoise()
        {
            int offsetX = data.data.GetLength(0) * index.x;
            int offsetY = data.data.GetLength(1) * index.y;

            for (int i = 0; i < data.data.GetLength(0); i++)
            {
                for (int j = 0; j < data.data.GetLength(1); j++)
                {
                    data.data[i, j] = (byte)(Mathf.PerlinNoise((i + offsetX + perlinNoiseOffsetSeed.x) * perlinNoiseAmplitude, (j + offsetY + perlinNoiseOffsetSeed.y) * perlinNoiseAmplitude) * 256f);
                }
            }
        }

        public void InitByRadomValue()
        {
            int offsetX = data.data.GetLength(0) * index.x;
            int offsetY = data.data.GetLength(1) * index.y;

            for (int i = 0; i < data.data.GetLength(0); i++)
            {
                for (int j = 0; j < data.data.GetLength(1); j++)
                {
                    data.data[i, j] = (byte)(Random.value * 256);
                }
            }
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
            data = new byte[WFMapTools.MapManager.Instance.mapUnit.x, WFMapTools.MapManager.Instance.mapUnit.y];

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
        Dictionary<Vector2Int, MapNode> maps;//unit:Index
        Dictionary<Vector2Int, GameObject> nodes;//unit:Pos
        public ColorShower()
        {
            nodes = new Dictionary<Vector2Int, GameObject>();
        }

        public void InitMapNode(MapNode showNode)
        {
            Vector2Int index = Vector2Int.one;
            for (int i = 0; i < showNode.data.data.GetLength(0); i++)
            {
                for (int j = 0; j < showNode.data.data.GetLength(1); j++)
                {
                    index.x = i; index.y = j;
                    Show(showNode, index);
                }
            }
        }
        Vector2Int TransToWorldIndex(Vector2Int nodeIndex, Vector2Int index)
        {
            int x = nodeIndex.x * MapManager.Instance.mapUnit.x + index.x;
            int y = nodeIndex.y * MapManager.Instance.mapUnit.y + index.y;
            return new Vector2Int(x, y);
        }

        public void Show(MapNode node, Vector2Int index)
        {
            Vector2Int pos = TransToWorldIndex(node.index, index);
            GameObject go;
            if (!nodes.ContainsKey(pos)) go = nodes[pos];
            else
            {
                go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                nodes.Add(pos, go);
            }
            go.transform.position = new Vector3(pos.x, node.data.data[index.x, index.y] / 32, pos.y);
            go.GetComponent<MeshRenderer>().material.color = Color.HSVToRGB(node.data.data[index.x, index.y] / 256f, 0.5f, 0.5f);
        }

        public void UnShow(MapNode node)
        {
            throw new System.NotImplementedException();
        }
    }

    public class TextureShower : IMapShower
    {
        Dictionary<Vector2Int, Texture2D> textures;
        Dictionary<Vector2Int, MeshRenderer> renderers;


        public TextureShower()
        {
            textures = new Dictionary<Vector2Int, Texture2D>();
            renderers = new Dictionary<Vector2Int, MeshRenderer>();
        }
        public void InitMapNode(MapNode showNode)
        {
            Vector2Int index = Vector2Int.one;
            for (int i = 0; i < showNode.data.data.GetLength(0); i++)
            {
                for (int j = 0; j < showNode.data.data.GetLength(1); j++)
                {
                    index.x = i; index.y = j;
                    Show(showNode, index);
                }
            }
            textures[showNode.index].Apply();
        }
        public void Show(MapNode node, Vector2Int index)
        {
            Vector2Int currentIndex = node.index;
            Vector2Int unit = MapManager.Instance.mapUnit;
            MeshRenderer mr;
            if (renderers.ContainsKey(currentIndex) && renderers[currentIndex] != null)
            {
                mr = renderers[node.index];
            }
            else
            {

                GameObject panel = GameObject.CreatePrimitive(PrimitiveType.Quad);
                panel.transform.position = new Vector3(node.index.x * MapManager.Instance.mapUnit.x, 0, node.index.y * MapManager.Instance.mapUnit.y);
                panel.transform.rotation = Quaternion.AngleAxis(90, Vector3.right);
                panel.transform.localScale = new Vector3(node.data.data.GetLength(0), node.data.data.GetLength(1));

                mr = panel.GetComponent<MeshRenderer>();
                textures[currentIndex] = new Texture2D(1024, 1024);
                Debug.LogError("生成出来的材质是黑色无受光的，可能需要换成预置体");
                //mr.material.mainTexture = textures[currentIndex];
                renderers[currentIndex] = mr;
            }

            for (int i = index.x * unit.x; i < index.x * unit.x + unit.x; i++)
            {
                for (int j = index.y * unit.y; j < index.y * unit.y + unit.y; j++)
                {
                    textures[currentIndex].SetPixel(i, j, Color.red);
                }
            }
        }

        
        public void UnShow(MapNode node)
        {
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
                    int x = i + showNode.index.x * MapManager.Instance.mapUnit.x;
                    int y = j + showNode.index.y * MapManager.Instance.mapUnit.y;
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

        public void UnShow(MapNode node)
        {
            throw new System.NotImplementedException();
        }
    }
    public interface IMapShower
    {

        void InitMapNode(MapNode showNode);
        void Show(MapNode node, Vector2Int index);
        void UnShow(MapNode node);
    }

    public interface IMapContrler
    {
        void Update();
    }

    public class ShowerManager : SingleMonoManager<ShowerManager>
    {

    }

    public class BrushManager : SingleMonoManager<BrushManager>
    {

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