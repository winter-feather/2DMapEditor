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
        public bool isAutoCreate;


        IMapShower groundShower;
        SpriteShower plantShower;

        Vector2Int selectNodeIndex, selectIndex;

        bool isSelected;
        public bool IsSelected => isSelected;
        public Vector2Int SelectNodeIndex => selectNodeIndex;
        public Vector2Int SelectIndex => selectIndex;


        int EdgeLeft = 0, EdgeRight = 2, EdgeUp = 1, EdgeDown = 3;
        int ConerLU = 0, ConerRU = 1, ConerRD = 2, ConerLD = 3;
        Vector2Int LU, RU, RD, LD;

        private void Awake()
        {
            ground = new Dictionary<Vector2Int, MapNode>();
            plant = new Dictionary<Vector2Int, MapNode>();
            LU = new Vector2Int(-1, 1);
            RU = new Vector2Int(1, 1);
            RD = new Vector2Int(1, -1);
            LD = new Vector2Int(-1, -1);
        }
        // Start is called before the first frame update
        void Start()
        {
            //shower = new ColorShower();
            MapNode.perlinNoiseAmplitude = 0.4f;
            MapNode.perlinNoiseOffsetSeed = new Vector2Int(10, 10);
            groundShower = new BasicTextureShower(this); //new FogTextureShower(this);//
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
                    //ground[new Vector2Int(i, j)].InitByPerlinNoise();
                    //groundShower.InitMapNode(ground[new Vector2Int(i, j)]);
                    yield return null;
                }
            }

            foreach (var item in ground)
            {
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
        public byte groundID = 0;
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) groundID = 1;
            if (Input.GetKeyDown(KeyCode.Alpha2)) groundID = 2;
            if (Input.GetKeyDown(KeyCode.Alpha3)) groundID = 3;
            if (Input.GetKeyDown(KeyCode.Alpha4)) groundID = 4;
            if (Input.GetKeyDown(KeyCode.Alpha5)) groundID = 5;
            if (Input.GetKeyDown(KeyCode.Alpha6)) groundID = 6;
            if (Input.GetKeyDown(KeyCode.Alpha7)) groundID = 7;
            if (Input.GetKeyDown(KeyCode.Alpha0)) groundID = 0;


            UpdateSelectInfo();
            if (Input.GetMouseButtonDown(0))
            {
                if (isSelected)
                {
                    SetMapNodeData(selectNodeIndex, selectIndex, groundID);
                }
            }
        }

        public void FixedUpdate()
        {
            //Test
            (groundShower as BasicTextureShower).Update();
            //(groundShower as FogTextureShower).Update();
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
        Vector2Int dataLU, dataU, dataRU, dataR, dataRD, dataD, dataLD, dataL,dataC;
        void SetMapNodeData(Vector2Int mapIndex, Vector2Int index, byte colorID)
        {
            dataC = index;
            dataLD = index + LD;
            dataL = index + Vector2Int.left;
            dataLU = index + LU;
            dataU = index + Vector2Int.up;
            dataRU = index + RU;
            dataR = index + Vector2Int.right;
            dataRD = index + RD;
            dataD = index + Vector2Int.down;

            SetData(mapIndex,dataC,colorID);

            SetConer(mapIndex, dataLD, colorID, ConerRU);
            SetConer(mapIndex, dataLU, colorID, ConerRD);
            SetConer(mapIndex, dataRD, colorID, ConerLU);
            SetConer(mapIndex, dataRU, colorID, ConerLD);

            SetEdge(mapIndex, dataL, colorID, EdgeRight);
            SetEdge(mapIndex, dataR, colorID, EdgeLeft);
            SetEdge(mapIndex, dataD, colorID, EdgeUp);
            SetEdge(mapIndex, dataU, colorID, EdgeDown);
          
            

        }

        void SetData(Vector2Int mapIndex, Vector2Int index, byte colorID) {
            PosCheck(ref mapIndex, ref index);
            if (mapIndex.x < 0 || mapIndex.y < 0) return;
          
            if (!ground.ContainsKey(mapIndex))
            {
                if (isAutoCreate)
                {
                    CreateNewMap(mapIndex);
                    groundShower.InitMapNode(ground[mapIndex]);
                    var node = ground[mapIndex];
                    node.SetData(index.x, index.y, colorID);
                    groundShower.Show(node, index);
                }
            }
            else
            {
                var node = ground[mapIndex];
                node.SetData(index.x, index.y, colorID);
                groundShower.Show(node, index);
            }
        }

        void SetConer(Vector2Int mapIndex, Vector2Int index, byte colorID, int conerIndex) {
            PosCheck(ref mapIndex, ref index);
            if (mapIndex.x < 0 || mapIndex.y < 0) return;
          
            if (!ground.ContainsKey(mapIndex))
            {
                if (isAutoCreate)
                {
                    CreateNewMap(mapIndex);
                    groundShower.InitMapNode(ground[mapIndex]);
                    var node = ground[mapIndex];
                    node.SetConer(index.x, index.y, conerIndex, colorID);
                    groundShower.Show(node, index);
                }
            }
            else
            {
                var node = ground[mapIndex];
                node.SetConer(index.x, index.y, conerIndex, colorID);
                groundShower.Show(node, index);
            }

        }

        void SetEdge(Vector2Int mapIndex, Vector2Int index, byte colorID, int edgeIndex) {
            PosCheck(ref mapIndex, ref index);
            if (mapIndex.x < 0 || mapIndex.y < 0) return;

            if (!ground.ContainsKey(mapIndex))
            {
                if (isAutoCreate)
                {
                    CreateNewMap(mapIndex);
                    groundShower.InitMapNode(ground[mapIndex]);
                    var node = ground[mapIndex];
                    node.SetEdge(index.x, index.y, edgeIndex, colorID);
                    groundShower.Show(node, index);
                }
            }
            else
            {
                var node = ground[mapIndex];
                node.SetEdge(index.x, index.y, edgeIndex, colorID);
                groundShower.Show(node, index);
            }
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
        MapData data;
        public MapData Data => data;
        public MapNode(Vector2Int index, MapData data)
        {
            this.index = index;
            this.data = data;
            this.size = Manager.mapUnit;
        }



        public void SetData(int x, int y, byte v)
        {
            data.SetData(x, y, v);
        }

        public void SetConer(int x, int y, int i, byte v) {
            data.SetConerData(x, y, i, v);
        }

        public void SetEdge(int x, int y, int i, byte v) {
            data.SetEdgesData(x, y, i, v);
        }

        public void InitByPerlinNoise()
        {
            int offsetX = data.data.GetLength(0) * index.x;
            int offsetY = data.data.GetLength(1) * index.y;

            for (int i = 0; i < data.data.GetLength(0); i++)
            {
                for (int j = 0; j < data.data.GetLength(1); j++)
                {
                    data.SetData(i, j, (byte)(Mathf.PerlinNoise((i + offsetX + perlinNoiseOffsetSeed.x) * perlinNoiseAmplitude, (j + offsetY + perlinNoiseOffsetSeed.y) * perlinNoiseAmplitude) * 256f));



                    //data.data[i, j] = (byte)(Mathf.PerlinNoise((i + offsetX + perlinNoiseOffsetSeed.x) * perlinNoiseAmplitude, (j + offsetY + perlinNoiseOffsetSeed.y) * perlinNoiseAmplitude) * 256f);
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
        public byte[,,] coners;
        public byte[,,] edges;
        public void InitData()
        {
            data = new byte[WFMapTools.MapManager.Instance.mapUnit.x, WFMapTools.MapManager.Instance.mapUnit.y];
            coners = new byte[WFMapTools.MapManager.Instance.mapUnit.x, WFMapTools.MapManager.Instance.mapUnit.y, 4];
            edges = new byte[WFMapTools.MapManager.Instance.mapUnit.x, WFMapTools.MapManager.Instance.mapUnit.y, 4];
        }

        public void SetData(int x, int y, byte v)
        {
            data[x, y] = v;
        }
        //public void SetData(Vector2Int index, byte v)
        //{
        //    SetData(index.x, index.y, v);
        //}

        public void SetConerData(int x, int y, int conerIndex, byte v)
        {
            coners[x, y, conerIndex] = v;
        }

        public void SetEdgesData(int x, int y, int edgeindex, byte v)
        {
            edges[x, y, edgeindex] = v;
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
            for (int i = 0; i < showNode.size.x; i++)
            {
                for (int j = 0; j < showNode.size.y; j++)
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
            if (nodes.ContainsKey(pos)) go = nodes[pos];
            else
            {
                go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                nodes.Add(pos, go);
            }
            go.transform.position = new Vector3(pos.x + 0.5f, 0, pos.y + 0.5f); //node.data.data[index.x, index.y] / 32,
            go.GetComponent<MeshRenderer>().material.color = node.Data.data[index.x, index.y] == 1 ? Color.red : Color.blue; // new Color(node.data.data[index.x, index.y]/7, node.data.data[index.x, index.y] / 7, node.data.data[index.x, index.y] / 7); //Color.HSVToRGB(node.data.data[index.x, index.y] / 256f, 0.5f, 0.5f);
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
        Dictionary<byte, byte>.Enumerator enumraterConerDic;
        Vector2Int unit;
        MapManager mapManager;
        Dictionary<byte, byte> cid;
        Dictionary<byte, byte> eid;
        public BasicTextureShower(MapManager mapManager)
        {
            textures = new Dictionary<Vector2Int, Texture2D>();
            renderers = new Dictionary<Vector2Int, MeshRenderer>();
            updateTexture = new Dictionary<Vector2Int, bool>();
            cid = new Dictionary<byte, byte>();
            eid = new Dictionary<byte, byte>();
            this.mapManager = mapManager;
            unit = mapManager.mapUnit;
        }

        public void InitMapNode(MapNode showNode)
        {
            Vector2Int index = Vector2Int.zero;
            for (int i = 0; i < showNode.size.x; i++)
            {
                for (int j = 0; j < showNode.size.y; j++)
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


        struct MarkIndex {
            public Vector2Int nodeIndex, index;


            public MarkIndex(Vector2Int nodeIndex, Vector2Int index) {
                this.nodeIndex = nodeIndex;
                this.index = index;
            }

            public override bool Equals(object obj)
            {
                MarkIndex m = (MarkIndex)obj;
                return nodeIndex.Equals(m.nodeIndex) && index.Equals(m.index);
            }
        }

        Dictionary<MarkIndex, bool> markDic;

        public void ShowMark(MapNode node, Vector2Int index) { 
        
        
        }

        public void Show(MapNode node, Vector2Int index)
        {
            Vector2Int currentIndex = node.index;
            Vector2Int unit = MapManager.Instance.mapUnit;
            //MeshRenderer mr;
            if (!renderers.ContainsKey(currentIndex) || renderers[currentIndex] == null)
            {
                CreateNewTexture(node);
            }
            Color[] brush = BrushManager.Instance.gourndTextures[node.Data.data[index.x, index.y]].GetPixels(32*0, 32, 32, 32);
            textures[currentIndex].SetPixels(index.x * unit.x, index.y * unit.y, unit.x, unit.y,brush);

            ShowConer(node, index);//node
            ShowEdge(node, index);





            updateTexture[node.index] = true;
        }



        void ShowConer(MapNode node, Vector2Int index) {
            cid.Clear();
            for (byte i = 0; i < node.Data.coners.GetLength(2); i++)
            {
                if (node.Data.coners[index.x, index.y, i] == 0) continue;
                if (cid.ContainsKey(node.Data.coners[index.x, index.y, i]))
                {
                    cid[node.Data.coners[index.x, index.y, i]] += (byte)(1 << i);
                }
                else
                {
                    cid[node.Data.coners[index.x, index.y, i]] = (byte)(1 << i);
                }
            }
            enumraterConerDic = cid.GetEnumerator();
            Color targetColor = Color.white;
            while (enumraterConerDic.MoveNext())
            {
                for (int i = 0; i < unit.x; i++)
                {
                    for (int j = 0; j < unit.y; j++)
                    {
                        targetColor = BrushManager.Instance.gourndTextures[enumraterConerDic.Current.Key].GetPixel((enumraterConerDic.Current.Value) * unit.x + i, j);
                        if (targetColor.a != 0)
                        {
                            textures[node.index].SetPixel(index.x * unit.x + i, index.y * unit.y + j, targetColor);
                        }
                    }
                }
            }
        }

        void ShowEdge(MapNode node, Vector2Int index)
        {
            eid.Clear();
            for (byte i = 0; i < node.Data.edges.GetLength(2); i++)
            {
                if (node.Data.edges[index.x, index.y, i] == 0) continue;
                if (eid.ContainsKey(node.Data.edges[index.x, index.y, i]))
                {
                    eid[node.Data.edges[index.x, index.y, i]] += (byte)(1 << i);
                }
                else
                {
                    eid[node.Data.edges[index.x, index.y, i]] = (byte)(1 << i);
                }
            }
            enumraterConerDic = eid.GetEnumerator();
            Color targetColor = Color.white;
            while (enumraterConerDic.MoveNext())
            {
                for (int i = 0; i < unit.x; i++)
                {
                    for (int j = 0; j < unit.y; j++)
                    {
                        targetColor = BrushManager.Instance.gourndTextures[enumraterConerDic.Current.Key].GetPixel((enumraterConerDic.Current.Value) * unit.x + i, j+32);
                        if (targetColor.a != 0)
                        {
                            textures[node.index].SetPixel(index.x * unit.x + i, index.y * unit.y + j,targetColor);
                        }
                    }
                }
            }


        }

            public void UnShow(MapNode node)
        {
        }

        void CreateNewTexture(MapNode node) {
            Vector2Int currentIndex = node.index;
            GameObject panel = GameObject.CreatePrimitive(PrimitiveType.Quad);
            panel.name = currentIndex.ToString();
            panel.transform.position = new Vector3(currentIndex.x * MapManager.Instance.mapUnit.x + 16, 0, currentIndex.y * MapManager.Instance.mapUnit.y + 16);
            panel.transform.rotation = Quaternion.AngleAxis(90, Vector3.right);
            panel.transform.localScale = new Vector3(node.size.x, node.size.y, 1);
            MeshRenderer mr = panel.GetComponent<MeshRenderer>();
            textures[currentIndex] = new Texture2D(1024, 1024);//,TextureFormat.ARGB32,false);
            mr.material = mapManager.groundMaterial;
            mr.material.mainTexture = textures[currentIndex];
            renderers[currentIndex] = mr;
        }
    }

    public class FogTextureShower : IMapShower
    {
        Dictionary<Vector2Int, Texture2D> textures;
        Dictionary<Vector2Int, MeshRenderer> renderers;
        Dictionary<Vector2Int, bool> updateTexture;
        Dictionary<Vector2Int, bool>.Enumerator enumerater;
        Dictionary<byte, byte>.Enumerator enumraterConerDic;
        Vector2Int unit;
        MapManager mapManager;
        //Queue
        public FogTextureShower(MapManager mapManager)
        {
            textures = new Dictionary<Vector2Int, Texture2D>();
            renderers = new Dictionary<Vector2Int, MeshRenderer>();
            updateTexture = new Dictionary<Vector2Int, bool>();
            this.mapManager = mapManager;
            unit = mapManager.mapUnit;
        }
        public void InitMapNode(MapNode showNode)
        {
            Vector2Int index = Vector2Int.zero;
            for (int i = 0; i < showNode.size.x; i++)
            {
                for (int j = 0; j < showNode.size.y; j++)
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
                panel.transform.localScale = new Vector3(node.size.x, node.size.y, 1);
                mr = panel.GetComponent<MeshRenderer>();
                textures[currentIndex] = new Texture2D(1024, 1024);//,TextureFormat.ARGB32,false);
                mr.material = mapManager.groundMaterial;
                mr.material.mainTexture = textures[currentIndex];
                renderers[currentIndex] = mr;
            }
            Dictionary<byte, byte> cid = new Dictionary<byte, byte>();
            for (byte i = 0; i < node.Data.coners.GetLength(2); i++)
            {
                if (node.Data.coners[index.x, index.y, i] == 0) continue;
                if (cid.ContainsKey(node.Data.coners[index.x, index.y, i]))
                {
                    cid[node.Data.coners[index.x, index.y, i]] += (byte)(i + 1);
                }
                else
                {
                    cid[node.Data.coners[index.x, index.y, i]] = i;
                }
            }

            enumraterConerDic = cid.GetEnumerator();
            Color targetColor = Color.white;

            textures[currentIndex].SetPixels(index.x * unit.x, index.y * unit.y, unit.x, unit.y, BrushManager.Instance.gourndTextures[enumraterConerDic.Current.Key].GetPixels(0, 0, 32, 32));
            while (enumraterConerDic.MoveNext())
            {
                for (int i = 0; i < unit.x; i++)
                {
                    for (int j = 0; j < unit.y; j++)
                    {
                        targetColor = BrushManager.Instance.gourndTextures[enumraterConerDic.Current.Key].GetPixel((enumraterConerDic.Current.Value + 1) * unit.x + i, j);
                        if (targetColor.a != 0)
                        {
                            textures[currentIndex].SetPixel(index.x * unit.x + i, index.y * unit.y + j, BrushManager.Instance.gourndTextures[enumraterConerDic.Current.Key].GetPixel((enumraterConerDic.Current.Value + 1) * unit.x + i, j));
                        }
                    }
                }
                //textures[currentIndex].SetPixels(index.x * unit.x, index.y * unit.y, unit.x, unit.y, BrushManager.Instance.gourndTextures[enumraterConerDic.Current.Key].GetPixels((enumraterConerDic.Current.Value + 1) * unit.x, 0, unit.x, unit.y));
            }



            updateTexture[node.index] = true;
        }

        public void UnShow(MapNode node)
        {
        }
    }

    public class RPGMakerTextureShower
    {

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
            for (int i = 0; i < showNode.Data.data.GetLength(0); i++)
            {
                for (int j = 0; j < showNode.Data.data.GetLength(1); j++)
                {
                    index.x = i; index.y = j;
                    Show(showNode, index);
                }
            }
        }

        public void Show(MapNode node, Vector2Int index)
        {
            if (node.Data.data[index.x, index.y] > 200) return;
            Vector2Int pos = TransToWorldIndex(node.index, index);
            SpriteRenderer sr;
            if (spriteRnderers.ContainsKey(pos)) sr = spriteRnderers[pos];
            else
            {
                sr = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("SpriteRenderer")).GetComponent<SpriteRenderer>();

                spriteRnderers.Add(pos, sr);
            }

            sr.transform.position = new Vector3(pos.x + 0.5f, 0.5f, pos.y + 0.5f);
            sr.transform.rotation = Quaternion.Euler(0, 0, 0);

            if (node.Data.data[index.x, index.y] > 200)
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