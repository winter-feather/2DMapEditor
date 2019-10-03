using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using WFUnityTools;
using System;
using System.Text;
using System.IO;
using WinterFeather;

namespace WFUnityTools.MapEditor
{
    public class MapEditorWindows : EditorWindow
    {
        public static MapEditorWindows windows;
        private static MapManager mapManager;
        public int x, y;
        public int interactiveLayer;
        string xStr, yStr, layerStr;

        int selectID;
        string selectBrushStr;

        public Map currentMap;

        public static MapManager MapManager
        {
            get
            {
                //MapEditorWindows.windows.FindMapInScene();
                if (mapManager == null)
                {
                    GameObject go = new GameObject("map");
                    mapManager = go.AddComponent<MapManager>();
                    mapManager.Init();
                }
                return mapManager;
            }

            set
            {
                mapManager = value;
            }
        }

        [MenuItem("Window/OpenTestWindow")]//在unity菜单Window下有MyWindow选项
        static void OpenWindow()
        {
            if (windows == null)
            {
                windows = (MapEditorWindows)EditorWindow.GetWindow(typeof(MapEditorWindows), false, "MapEditor", true);//创建窗口
            }
            windows.Show();//展示
            MapEditorWindows.windows.FindMapInScene();
            SceneView.onSceneGUIDelegate += OnSceneGUI;
        }

        static void OnSceneGUI(SceneView sceneView)
        {
            if (windows == null) return;
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, 1 << MapManager.interactivePanel.interactiveLayerIndex))
                {
                    Vector3 localPos = UnityTools.GetLocalPos(mapManager.interactivePanel.transform, hit.point);
                    Vector2Int index = new Vector2Int((int)localPos.x, (int)localPos.y);
                    mapManager.map.SetID(index, windows.selectID);
                }
            }
        }

        void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Size", GUILayout.Width(100));
            GUILayout.Label("X:", GUILayout.Width(25));
            xStr = GUILayout.TextField(xStr);
            GUILayout.Label("Y:", GUILayout.Width(25));
            yStr = GUILayout.TextField(yStr);
            int.TryParse(xStr, out x);
            int.TryParse(yStr, out y);
            if (GUILayout.Button("ResetMapSize", GUILayout.Width(100)))
            {
                ResetInteractivePanel();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("InterActiveLayer:", GUILayout.Width(100));
            layerStr = GUILayout.TextField(layerStr);
            int layer;
            int.TryParse(layerStr, out layer);
            interactiveLayer = layer;
            if (GUILayout.Button("ResetLayer", GUILayout.Width(100)))
            {
                ResetLayer();
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("CreateMap"))
            {
                ResetCamera();
                CreatMap();
            }

            if (GUILayout.Button("Save"))
            {
                if (mapManager == null) return;
                mapManager.Save();
            }
            if (GUILayout.Button("Load"))
            {
                if (mapManager == null) return;
                CreatMap();
                // FindMapInScene();
                MapManager.Load();
                Debug.Log(mapManager.map.dataIDs == null);
                MapManager.map.Show();
                ReferushOnGUI(mapManager.map.size.x.ToString(),
                mapManager.map.size.y.ToString(),
                mapManager.interactivePanel.interCollider.gameObject.layer.ToString());
            }

            if (GUILayout.Button("Clear"))
            {
                if (MapManager == null) return;
                MapManager.shower.ClearNodeID();
                //UnityTools.ClearChild(MapManager.shower.container);
            }

            if (GUILayout.Button("ReShow"))
            {
                if (MapManager == null) return;
                MapManager.map.Show();
            }

            if (windows != null && MapManager.spriteLibrary != null && MapManager.spriteLibrary.Sprites.Count > 0)
            {
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                GUILayout.Label("BrushCount:", GUILayout.Width(80));
                GUILayout.TextArea(MapManager.spriteLibrary.Sprites.Count.ToString(), GUILayout.Width(20));
                GUILayout.Label("SelectBrushID:", GUILayout.Width(100));
                selectBrushStr = GUILayout.TextField(selectBrushStr);
                int.TryParse(selectBrushStr, out selectID);
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                if (selectID != 0)
                {
                    GUILayout.Label(MapManager.spriteLibrary.Sprites[selectID].sprites[0].texture);
                }
            }
        }

        public void ReferushOnGUI(string x, string y, string layer)
        {
            xStr = x;
            yStr = y;
            layerStr = layer;
        }

        void ResetLayer()
        {
            // MapManager.interactivePanel.SetColliderLayer();
        }

        void ResetInteractivePanel()
        {
            MapManager.CreateInteractivePanel(new Vector2Int(x, y), interactiveLayer);
        }

        void ResetCamera()
        {
            SceneView.lastActiveSceneView.pivot = Vector3.zero;
            SceneView.lastActiveSceneView.rotation = Quaternion.Euler(0, 0, 0);
            GameObject go = new GameObject();
            go.transform.position = Vector3.forward * -10;
            SceneView.lastActiveSceneView.orthographic = true;
            SceneView.lastActiveSceneView.AlignViewToObject(go.transform);
            DestroyImmediate(go);
        }

        void CreatMap()
        {
            MapManager.CreateData(new Vector2Int(x, y));
            MapManager.CreateInteractivePanel(new Vector2Int(x, y), interactiveLayer);
            MapManager.CreateSpriteLibrary();
            MapManager.CreateMapShower(new Vector2Int(x, y));
            ReferushOnGUI(x.ToString(), y.ToString(), interactiveLayer.ToString());
        }

        void FindMapInScene()
        {
            mapManager = GameObject.FindObjectOfType<MapManager>();
            if (mapManager != null)
            {
                ReferushOnGUI(mapManager.map.size.x.ToString(),
                    mapManager.map.size.y.ToString(),
                    mapManager.interactivePanel.interCollider.gameObject.layer.ToString());
                mapManager.CreateSpriteLibrary();
            }
        }

        void Clear()
        {
            mapManager.shower.ClearNodeID();
        }

        private void OnDestroy()
        {
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
        }
        #region 窗体事件调用
        //void OnProjectChange()
        //{
        //    Debug.Log("当场景改变时调用");
        //}

        //void OnHierarchyChange()
        //{
        //    Debug.Log("当选择对象属性改变时调用");
        //}

        //void OnGetFocus()
        //{
        //    Debug.Log("当窗口得到焦点时调用");
        //}

        //private void OnLostFocus()
        //{
        //    Debug.Log("当窗口失去焦点时调用");
        //}

        //private void OnSelectionChange()
        //{
        //    Debug.Log("当改变选择不同对象时调用");
        //}

        //private void OnInspectorUpdate()
        //{
        //    Debug.Log("监视面板调用");
        //}

        //private void OnDestroy()
        //{
        //    Debug.Log("当窗口关闭时调用");
        //    SceneView.onSceneGUIDelegate -= OnSceneGUI;
        //}

        //private void OnFocus()
        //{
        //    Debug.Log("当窗口获取键盘焦点时调用");
        //}
        #endregion
    }


    public class SpriteBrush
    {
        public int id;
        public string path;
        public List<Sprite> sprites;

        public SpriteBrush(string path, Sprite[] sprites)
        {
            this.path = path;
            this.sprites = new List<Sprite>(sprites);
        }
    }

    public class MapManager : MonoBehaviour
    {
        int id;
        public static MapManager instance;
        public Map map;
        public SpriteLibrary spriteLibrary;
        public MapInteractivePanel interactivePanel;
        public MapShower shower;
        public string savePath;
        string tempSaveData;
        public void Init()
        {
            instance = this;
            savePath = Application.streamingAssetsPath + @"\tempSave.sav";
        }
        public void CreateData(Vector2Int size)
        {
            map = new Map(size);
            map.Init(size);
            map.manager = this;
            map.size = size;
        }
        public void CreateInteractivePanel(Vector2Int size, int collitionLayer)
        {
            CreateInteractivePanel();
            interactivePanel.Init(size, collitionLayer);
        }

        public void CreateInteractivePanel()
        {
            if (interactivePanel == null)
            {
                GameObject go = new GameObject("InteractivePanel");
                interactivePanel = go.AddComponent<MapInteractivePanel>();
                interactivePanel.transform.SetParent(transform);
                interactivePanel.transform.localPosition = Vector3.zero;
                interactivePanel.manager = this;
            }
        }

        public void CreateSpriteLibrary()
        {
            spriteLibrary = new SpriteLibrary();// UnityTools.GetComponent<SpriteLibrary>(gameObject);//  gameObject.AddComponent<SpriteLibrary>();
            for (int i = 0; i <= 6; i++)
            {
                spriteLibrary.LoadSprite("MapBrush\\" + i.ToString());
            }
            spriteLibrary.manager = this;
        }
        public void CreateMapShower(Vector2Int index)
        {
            shower = new MapShower(index);
            GameObject containerGO = new GameObject("container");
            containerGO.transform.SetParent(this.transform);
            containerGO.transform.localPosition = Vector3.zero;
            shower.container = containerGO.transform;
        }
        public void Save()
        {
            int[,] data = map.dataIDs;
            string dataStr = "";
            dataStr += data.GetLength(0) + ",";
            dataStr += data.GetLength(1) + "/";
            for (int x = 0; x < data.GetLength(0); x++)
            {
                for (int y = 0; y < data.GetLength(1); y++)
                {
                    dataStr += data[x, y] + ",";
                }
            }
            DataTools.SaveToByte(dataStr, savePath);
            tempSaveData = dataStr;
            //map.dataIDs = new int[data.GetLength(0), data.GetLength(1)];
            //string data = unityto
        }

        public void Save(string path)
        {
            this.savePath = path;
            Save();
        }
        public void Load()
        {
            string dataStr = tempSaveData;
            string loadStr = Encoding.UTF8.GetString(DataTools.LoadFormByte(savePath));
            Debug.Log(loadStr);
            //Debug.Log(loadStr);
            Load(loadStr);

        }

        public void Load(string dataStr)
        {
            string[] testdata = dataStr.Split('/');
            string[] size = testdata[0].Split(',');
            string[] datas = testdata[1].Split(',');
            int index = 0;
            int[,] data = new int[int.Parse(size[0]), int.Parse(size[1])];
            for (int x = 0; x < data.GetLength(0); x++)
            {
                for (int y = 0; y < data.GetLength(1); y++)
                {
                    data[x, y] = int.Parse(datas[index++]);
                }
            }
            map.dataIDs = data;
        }

    }

    public class Map
    {
        public Vector2Int size;
        public int[,] dataIDs;
        public MapManager manager;

        public Map(Vector2Int size)
        {
            Init(size);
        }

        public void Init(Vector2Int size)
        {
            this.size = size;
            dataIDs = new int[size.x, size.y];
        }

        public void SetID(Vector2Int index, int id)
        {
            int x = index.x;
            int y = index.y;
            dataIDs[x, y] = id;
            Show(x, y, id);
        }

        public Vector3 GetCenterPos(Vector2Int index)
        {
            return new Vector3(index.x + 0.5f, index.y + 0.5f, 0);
        }

        public void Show(int x, int y, int id)
        {
            manager.shower.SetNodeID(new Vector2Int(x, y), 1, id);
            manager.shower.SetNodeID(new Vector2Int(x, y + 1), 3, id);
            manager.shower.SetNodeID(new Vector2Int(x + 1, y + 1), 2, id);
            manager.shower.SetNodeID(new Vector2Int(x + 1, y), 0, id);
        }

        public void Show()
        {
            for (int x = 0; x < dataIDs.GetLength(0); x++)
            {
                for (int y = 0; y < dataIDs.GetLength(1); y++)
                {
                    Show(x, y, dataIDs[x, y]);
                }
            }
        }
    }
    [Serializable]
    public class SpriteLibrary
    {
        //public string path;
        public List<SpriteBrush> brushs;
        public MapManager manager;

        public List<SpriteBrush> Sprites
        {
            get
            {
                if (brushs == null)
                {
                    brushs = new List<SpriteBrush>();
                    //sprites.Add(new SpriteBrush("", new Sprite[0]));
                }
                return brushs;
            }

            set
            {
                brushs = value;
            }
        }


        public void LoadSprite(string path)
        {
            if (Contains(path)) return;
            Sprite[] brushs = Resources.LoadAll<Sprite>(path);
            //Debug.Log(brushs.Length);
            SpriteBrush brush = new SpriteBrush(path, brushs);
            Sprites.Add(brush);
        }

        public bool Contains(string path)
        {
            if (Sprites.Count == 0) return false;
            for (int i = 0; i < Sprites.Count; i++)
            {
                if (Sprites[i].path == path) return true;
            }
            return false;
        }

    }

    public class MapShower
    {
        public MapShower(Vector2Int size)
        {
            Init(size);
        }
        public void Init(Vector2Int size)
        {
            nodes = new MapShowerNode[size.x + 1, size.y + 1];
            for (int x = 0; x < nodes.GetLength(0); x++)
            {
                for (int y = 0; y < nodes.GetLength(1); y++)
                {
                    nodes[x, y] = new MapShowerNode(new Vector2Int(x, y), this);
                }
            }
        }
        public MapManager manager;
        public MapShowerNode[,] nodes;
        public Transform container;
        public void SetNodeID(Vector2Int index, int conerID, int id)
        {
            //if (nodes == null) return;
            Debug.Log(nodes == null);
            if (UnityTools.OutOfRangeCheck<MapShowerNode>(index, nodes)) return;
            nodes[index.x, index.y].SetnConerID(conerID, id);
        }

        public void ClearNodeID(Vector2Int index)
        {
            if (UnityTools.OutOfRangeCheck<MapShowerNode>(index, nodes)) return;
            nodes[index.x, index.y].ClearID();
        }

        public void ClearNodeID()
        {
            for (int x = 0; x < nodes.GetLength(0); x++)
            {
                for (int y = 0; y < nodes.GetLength(1); y++)
                {
                    nodes[x, y].ClearID();
                }
            }
        }
    }

    public class MapShowerNode
    {
        public int[] coner;
        List<SpriteRenderer> renderers;
        public Vector2Int index;
        public MapShower parent;

        public Vector3 LocalPosition
        {
            get { return new Vector3(index.x, index.y, 0); }
        }

        public MapShowerNode(Vector2Int index, MapShower parentNode)
        {
            parent = parentNode;
            this.index = index;
            renderers = new List<SpriteRenderer>();
            coner = new int[4];
        }

        public void SetnConerID(int conerID, int id)
        {
            coner[conerID] = id;
            Show();
        }

        public void ClearID()
        {
            for (int i = 0; i < coner.Length; i++)
            {
                coner[i] = 0;
                ClearRenderer();
            }
        }

        public void Show()
        {
            ClearRenderer();
            Dictionary<int, int> value = new Dictionary<int, int>();

            for (int i = 0; i < coner.Length; i++)
            {
                if (coner[i] == 0) continue;
                if (value.ContainsKey(coner[i]))
                    value[coner[i]] |= 1 << i;
                else
                    value[coner[i]] = 1 << i;
            }
            foreach (var item in value)
            {
                AddRenderer(MapManager.instance.spriteLibrary.brushs[item.Key].sprites[item.Value]);
            }
        }

        void AddRenderer(Sprite sprite)
        {
            GameObject go = new GameObject("spriteRender");
            go.transform.position = LocalPosition;
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.transform.SetParent(MapManager.instance.shower.container);
            renderers.Add(sr);
        }



        void ClearRenderer()
        {
            for (int i = 0; i < renderers.Count; i++)
            {
#if UNITY_EDITOR
                GameObject.DestroyImmediate(renderers[i].gameObject);
#elif UNITY_STANDALONE
                GameObject.Destroy(renderers[i]);
#endif
            }
            renderers.Clear();
        }
    }

    public class MapInteractivePanel : MonoBehaviour
    {
        public Vector2Int size;
        public BoxCollider interCollider;
        //public LayerMask mask;
        public MapManager manager;
        public int interactiveLayerIndex;

        public void Init(Vector2Int size, int layer)
        {
            this.size = size;
            if (interCollider == null)
            {
                interCollider = gameObject.AddComponent<BoxCollider>();
                SetColliderLayer(layer);
            }
            SetColliderSize(size);
        }
        public void SetColliderLayer(int layer)
        {
            interCollider.gameObject.layer = MapEditorWindows.windows.interactiveLayer;
            interactiveLayerIndex = MapEditorWindows.windows.interactiveLayer;
            //mask = 1 << interCollider.gameObject.layer;

        }
        public void SetColliderSize(Vector2Int size)
        {
            interCollider.size = new Vector3(size.x, size.y, 0);
            interCollider.center = new Vector3((float)size.x * 0.5f, (float)size.y * 0.5f, 0);
        }
    }
}