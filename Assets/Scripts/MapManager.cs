using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFMapTools
{
    public class MapManager : SingleMonoManager<MapManager>
    {
        public Dictionary<Vector2Int, MapNode> ground;
        public Dictionary<Vector2Int, MapNode> plant;
        public Vector2Int mapUnit;
        public Material groundMaterial;


        BasicTextureShower groundShower;
        SpriteShower plantShower;

        Vector2Int selectNodeIndex, selectIndex;
        bool isSelected;
        public Vector2Int SelectNodeIndex => selectNodeIndex;
        public Vector2Int SelectIndex => selectIndex;

        private void Awake()
        {
            ground = new Dictionary<Vector2Int, MapNode>();
            plant = new Dictionary<Vector2Int, MapNode>();
        }
        // Start is called before the first frame update
        void Start()
        {
            //shower = new ColorShower();
            MapNode.perlinNoiseAmplitude = 0.4f;
            MapNode.perlinNoiseOffsetSeed = new Vector2Int(10, 10);
            groundShower = new BasicTextureShower(this);
            plantShower = new SpriteShower();
            StartCoroutine(LoadMap());
        }

        IEnumerator LoadMap()
        {
            yield return null;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    CreateNewMap(new Vector2Int(i, j));
                    yield return null;
                }
            }

            foreach (var item in ground)
            {
                item.Value.InitByPerlinNoise();
                groundShower.InitMapNode(item.Value);
                //plantShower.InitMapNode(item.Value);
                yield return null;
            }
        }


        public void CreateNewMap(Vector2Int index)
        {
            MapData data = new MapData();

            MapNode mn = new MapNode(index, data);
            mn.index = index;
            ground[index] = mn;
        }

        public void CreateMap(MapNode node)
        {

        }
        RaycastHit hit;

        public void Update()
        {
            UpdateSelectInfo();
            if (Input.GetMouseButton(0))
            {
                if (isSelected)
                {
                    ChangeColor(selectNodeIndex, selectIndex,255);
                    ChangeColor(selectNodeIndex, selectIndex + Vector2Int.up);
                    ChangeColor(selectNodeIndex, selectIndex - Vector2Int.up);
                    ChangeColor(selectNodeIndex, selectIndex + Vector2Int.right);
                    ChangeColor(selectNodeIndex, selectIndex - Vector2Int.right);
                    //groundShower.TestShow(selectNodeIndex, selectIndex);
                    //groundShower.TestShow(selectNodeIndex, selectIndex + Vector2Int.up);
                    //groundShower.TestShow(selectNodeIndex, selectIndex - Vector2Int.up);
                    //groundShower.TestShow(selectNodeIndex, selectIndex + Vector2Int.right);
                    //groundShower.TestShow(selectNodeIndex, selectIndex - Vector2Int.right);
                }
            }
        }

        public void FixedUpdate()
        {
            //Test
            groundShower.Update();
        }

        #region UpdateFunc
        void UpdateSelectInfo()
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                isSelected = true;
                PosConverMapIndex(hit.point, out selectNodeIndex, out selectIndex);
            }
            else
            {
                isSelected = false;
            }
        }
        #endregion



        #region ActionFunc
        void ChangeColor(Vector2Int mapIndex, Vector2Int index,byte? colorID = null) {
            PosCheck(ref mapIndex, ref index);
            if (mapIndex.x < 0 || mapIndex.y < 0) return;
            if (!ground.ContainsKey(mapIndex))
            {
                CreateNewMap(mapIndex);
            }
            var node = ground[mapIndex];
            if (colorID!=null)
            {
                node.data.data[index.x, index.y] = colorID.Value;
            }
            groundShower.Show(node, index);
        }


        #endregion

        #region ToolsFunc
        void PosConverMapIndex(Vector3 hitPoint, out Vector2Int nodeIndex, out Vector2Int index)
        {
            int x = Mathf.FloorToInt(hitPoint.x);
            int y = Mathf.FloorToInt(hitPoint.z);
            nodeIndex = Vector2Int.zero;
            nodeIndex.x = x / mapUnit.x;
            nodeIndex.y = y / mapUnit.y;
            index = Vector2Int.zero;
            index.x = x % mapUnit.x;
            index.y = y % mapUnit.y;
        }

        public void PosCheck(ref Vector2Int nodeIndex, ref Vector2Int index)
        {
            if (index.x < 0)
            {
                nodeIndex.x -= 1;
                index.x = mapUnit.x - 1;
            }
            else if (index.x > mapUnit.x - 1)
            {
                nodeIndex.x += 1;
                index.x = 0;
            }

            if (index.y < 0)
            {
                nodeIndex.y -= 1;
                index.y = mapUnit.y - 1;
            }
            else if (index.y > mapUnit.y - 1)
            {
                nodeIndex.y += 1;
                index.y = 0;
            }
        }
        #endregion


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

    public class BasicTextureShower : IMapShower
    {
        Dictionary<Vector2Int, Texture2D> textures;
        Dictionary<Vector2Int, MeshRenderer> renderers;
        Dictionary<Vector2Int, bool> updateTexture;
        Dictionary<Vector2Int, bool>.Enumerator enumerater;
        Vector2Int unit;
        MapManager mapManager;
        //Queue
        public BasicTextureShower(MapManager mapManager)
        {
            textures = new Dictionary<Vector2Int, Texture2D>();
            renderers = new Dictionary<Vector2Int, MeshRenderer>();
            updateTexture = new Dictionary<Vector2Int, bool>();
            this.mapManager = mapManager;
            unit = mapManager.mapUnit;
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
        public void Apply(Vector2Int index)
        {
            textures[index].Apply(false);
        }


        public void Update()
        {
            if (updateTexture.Count > 0)
            {
                enumerater = updateTexture.GetEnumerator();
                while (enumerater.MoveNext())
                {
                    Apply(enumerater.Current.Key);
                }
                updateTexture.Clear();
            }
        }

        public void PosCheck(ref Vector2Int nodeIndex, ref Vector2Int index)
        {
            if (index.x < 0)
            {
                nodeIndex.x -= 1;
                index.x = unit.x - 1;
            }
            else if (index.x > unit.x - 1)
            {
                nodeIndex.x += 1;
                index.x = 0;
            }

            if (index.y < 0)
            {
                nodeIndex.y -= 1;
                index.y = unit.y - 1;
            }
            else if (index.y > unit.y - 1)
            {
                nodeIndex.y += 1;
                index.y = 0;
            }
        }

        public void TestShow(Vector2Int mapIndex, Vector2Int index)
        {
            //Debug.LogError(index);
            PosCheck(ref mapIndex, ref index);
            if (mapIndex.x < 0 || mapIndex.y < 0) return;
            if (!mapManager.ground.ContainsKey(mapIndex))
            {
                mapManager.CreateNewMap(mapIndex);
            }
            var node = mapManager.ground[mapIndex];
            node.data.data[index.x,index.y] = 0;
            Show(node, index);
            return;
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
                panel.name = node.index.ToString();
                panel.transform.position = new Vector3(node.index.x * MapManager.Instance.mapUnit.x + 16, 0, node.index.y * MapManager.Instance.mapUnit.y + 16);
                panel.transform.rotation = Quaternion.AngleAxis(90, Vector3.right);
                panel.transform.localScale = new Vector3(node.data.data.GetLength(0), node.data.data.GetLength(1), 1);
                mr = panel.GetComponent<MeshRenderer>();
                textures[currentIndex] = new Texture2D(1024, 1024,TextureFormat.ARGB32,false);
                mr.material = mapManager.groundMaterial;
                mr.material.mainTexture = textures[currentIndex];
                renderers[currentIndex] = mr;
            }
            //textures[currentIndex].SetPixels32(index.x * unit.x, index.y * unit.y,  32,32, BrushManager.Instance.brush["ConerBrush"][1].texture.GetPixels32());
            for (int i = index.x * unit.x; i < index.x * unit.x + unit.x; i++)
            {
                for (int j = index.y * unit.y; j < index.y * unit.y + unit.y; j++)
                {
                    int r = index.x + index.y;

                    //TODO::
                    float v = node.data.data[index.x, index.y];//Mathf.FloorToInt(node.data.data[index.x, index.y] * 0.2f) * 5;


                    textures[currentIndex].SetPixel(i, j, BrushManager.Instance.brush["ConerBrush"][5].texture.GetPixel(i - index.x * unit.x+5*unit.x, j - index.y * unit.y));

                }
            }
            updateTexture[node.index] = true;
        }

        public void UnShow(MapNode node)
        {
        }
    }

    public class SpriteShower : IMapShower
    {
        Dictionary<Vector2Int, SpriteRenderer> spriteRnderers;//unit:pos
        public SpriteShower()
        {
            spriteRnderers = new Dictionary<Vector2Int, SpriteRenderer>();
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

        public void Show(MapNode node, Vector2Int index)
        {
            if (node.data.data[index.x, index.y] > 200) return;
            Vector2Int pos = TransToWorldIndex(node.index, index);
            SpriteRenderer sr;
            if (spriteRnderers.ContainsKey(pos)) sr = spriteRnderers[pos];
            else
            {
                sr = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("SpriteRenderer")).GetComponent<SpriteRenderer>();
                spriteRnderers.Add(pos, sr);
            }

            sr.transform.position = new Vector3(pos.x + 0.5f, 0.05f, pos.y + 0.5f);
            sr.transform.rotation = Quaternion.Euler(90, 0, 0);

            if (node.data.data[index.x, index.y] > 200)
            {
                //Resources.Load<Sprite>("colorBar_0");
            }
            spriteRnderers[pos].sprite = BrushManager.Instance.planets[Random.Range(0, BrushManager.Instance.planets.Length)];
            //go.GetComponent<SpriteRenderer>().material.color = Color.HSVToRGB(node.data.data[index.x, index.y] / 256f, 0.5f, 0.5f);
        }

        Vector2Int TransToWorldIndex(Vector2Int nodeIndex, Vector2Int index)
        {
            int x = nodeIndex.x * MapManager.Instance.mapUnit.x + index.x;
            int y = nodeIndex.y * MapManager.Instance.mapUnit.y + index.y;
            return new Vector2Int(x, y);
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