using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class MainManager : MonoBehaviour
{
    public static MainManager m_Instance;
    World world;
    public int selectInt;
    Color[] selectColor;
    public Vector2Int unit;
    public Texture2D brush;
    public Material oMateral;

    //public Texture2D temperatrueTex;
    //public Texture2D brightnessTex;
    //public GameObject brightnessShower;
    //public GameObject temperatureShower;

    public int[,] tempSave;
    Vector2Int size = new Vector2Int(10, 24); //Vector2Int.one * 256;
    List<Room> roomList;

    public float refushCD = 1;
    public float refushTime;
    private void Awake()
    {
        m_Instance = this;
        unit = new Vector2Int(32, 32);
    }

    // Use this for initialization
    void Start()
    {
        selectColor = brush.GetPixels(selectInt * 32, 0, unit.x, unit.y);
        world = new World();
        world.grouds = new Map(Vector2Int.zero, size);
        world.grouds.World = world;
        world.grouds.Controller = new MapController(Vector2Int.zero, size);
        world.grouds.Shower = new MapTextureShower(world.grouds, new Vector2Int(unit.x, unit.y), (v) =>
        {
            selectInt = v;
            selectColor = brush.GetPixels(selectInt * 32, 0, unit.x, unit.y);
            return MainManager.m_Instance.selectColor;
        }, oMateral);
        world.grouds.InitAllNode(1);
        world.grouds.onUpdate += GroundUpdate;
        //world.wallObjects = new Map(Vector2Int.zero, size);
        //world.wallObjects.Shower = new MapNodeShower(world.wallObjects, WallShower);
        //world.wallObjects.onAddMapNode += (IMapable e) =>
        //{
        //    RoomCheck(e.Index);
        //};

        //world.wallObjects.onRemoveMapNode += (IMapable e) =>
        //{
        //    if (e != null)
        //    {
        //        RemoveRoomCheck(e.Index + Vector2Int.up);
        //        RemoveRoomCheck(e.Index + Vector2Int.down);
        //        RemoveRoomCheck(e.Index + Vector2Int.right);
        //        RemoveRoomCheck(e.Index + Vector2Int.left);
        //        List<Vector2Int> r = new List<Vector2Int>();
        //        RoomCheck(e.Index, r);
        //        if (r.Count <= 100 && r.Count > 4) { Room.CreateRoom(r); }
        //    }
        //};
        //roomList = new List<Room>();
        //Room.onCreateRoom += (Room r) =>
        //{
        //    roomList.Add(r);
        //    foreach (var item in r.roomNodes)
        //    {
        //        world.plants.SetNode(item, new Plant(1));
        //    }
        //};
        //Room.onDeleteRoom += (Room r) =>
        //{
        //    foreach (var item in r.roomNodes)
        //    {
        //        world.plants.RemoveNode(item);
        //    }
        //    roomList.Remove(r);
        //};
    }
    public void AddSelectID()
    {
        selectInt++;
        selectInt = Mathf.Clamp(selectInt, 0, 6);
        selectColor = brush.GetPixels(selectInt * 32, 0, unit.x, unit.y);
    }

    public void SubSelectID()
    {
        selectInt--;
        selectInt = Mathf.Clamp(selectInt, 0, 6);
        selectColor = brush.GetPixels(selectInt * 32, 0, unit.x, unit.y);
    }
    void RemoveRoomCheck(Vector2Int pos)
    {
        if (pos.x < 0) return;
        if (pos.y < 0) return;
        if (pos.x >= size.x) return;
        if (pos.y >= size.y) return;
        if (world.wallObjects.GetNode(pos) != Map.MAP_NULL) return;

        Room tr = null;
        foreach (var item in roomList)
        {
            if (item.roomNodes.Contains(pos))
            {
                tr = item;
                break;
            }
        }
        if (tr != null)
        {
            Room.DeleteRoom(tr);
        }
    }

    void RoomCheck(Vector2Int pos)
    {
        Vector2Int up = pos + Vector2Int.up;
        Vector2Int down = pos + Vector2Int.down;
        Vector2Int right = pos + Vector2Int.right;
        Vector2Int left = pos + Vector2Int.left;
        List<Vector2Int> u = new List<Vector2Int>();
        List<Vector2Int> d = new List<Vector2Int>();
        List<Vector2Int> r = new List<Vector2Int>();
        List<Vector2Int> l = new List<Vector2Int>();
        RoomCheck(up, u);
        RoomCheck(down, d);
        RoomCheck(right, r);
        RoomCheck(left, l);
        if (u.Count <= 100 && u.Count > 4) { Room.CreateRoom(u); }
        if (d.Count <= 100 && d.Count > 4) { Room.CreateRoom(d); }
        if (r.Count <= 100 && r.Count > 4) { Room.CreateRoom(r); }
        if (l.Count <= 100 && l.Count > 4) { Room.CreateRoom(l); }
    }
    void RoomCheck(Vector2Int pos, List<Vector2Int> r)
    {
        if (r.Count > 100)
        {
            r.Add(Vector2Int.zero); return;
        }
        if (r.Contains(pos)) return;
        if (pos.x < 0) return;
        if (pos.y < 0) return;
        if (pos.x >= size.x) return;
        if (pos.y >= size.y) return;

        if (world.wallObjects.GetNode(pos) != Map.MAP_NULL) return;
        r.Add(pos);
        Vector2Int up = pos + Vector2Int.up;
        Vector2Int down = pos + Vector2Int.down;
        Vector2Int right = pos + Vector2Int.right;
        Vector2Int left = pos + Vector2Int.left;
        RoomCheck(up, r);
        RoomCheck(down, r);
        RoomCheck(right, r);
        RoomCheck(left, r);
    }

    GameObject PlantShower(IMapable n)
    {
        GameObject g;
        switch (n.ID)
        {
            case 0:
            case 1:
            // return GameObject.Instantiate<GameObject>(fileManager.plants[n.ID]);
            default: return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000, 1 << 16))
            {
                world.grouds.SetNode(new Vector2Int((int)hit.point.x, (int)hit.point.z), selectInt);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000, 1 << 16))
            {
                world.grouds.RemoveNode(new Vector2Int((int)hit.point.x, (int)hit.point.z));
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            tempSave = world.grouds.Save();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (tempSave != null)
            {
                world.grouds.Load(tempSave);
            }
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            AddSelectID();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            SubSelectID();
        }

        refushTime += Time.deltaTime;
        if (refushTime > refushCD)
        {
            refushTime -= refushCD;
            world.grouds.Update();
            world.grouds.Shower.Refresh();
        }
    }



    //void TemperatureUpdateAction()
    //{
    //    foreach (var item in world.temperature.Nodes)
    //    {
    //        if (item != null)
    //        {
    //            item.Update();
    //            float t = (float)item.ID / 100;
    //            //temperatrueTex.SetPixel(item.Index.x, item.Index.y, new Color(t, (1 - t) * 0.75f, 1 - t, 0.5f));
    //        }
    //    }
    //    //temperatrueTex.Apply(false);
    //}
    void AddLight()
    {

    }

    void RemoveLigth()
    {

    }


    void GroundUpdate(Map map)
    {
        for (int x = 0; x < map.nodes.GetLength(0); x++)
        {
            for (int y = 0; y < map.nodes.GetLength(1); y++)
            {
                GroundUpdate(x, y, map);
            }
        }
    }

    void GroundUpdate(int x, int y, Map map)
    {
        switch (map.nodes[x, y])
        {
            case 1:
                GoundUpdate_Sad(x, y, map);
                break;

            default:
                break;
        }
    }

    void GoundUpdate_Sad(int x, int y, Map map)
    {
        Vector2Int index = Vector2Int.zero;
        index.x = x;
        index.y = y;
        Vector2Int target = index;
        if (UnityEngine.Random.value <1f)
        {
            target = index + Vector2Int.up;
            if ((map.GetNode(target) == 0)) map.SetNode(target, 1);
            target = index + Vector2Int.down;
            if ((map.GetNode(target) == 0)) map.SetNode(target, 1);
            target = index + Vector2Int.right;
            if ((map.GetNode(target) == 0)) map.SetNode(target, 1);
            target = index + Vector2Int.left;
            if ((map.GetNode(target) == 0)) map.SetNode(target, 1);
        }
    }
}

public class Map : IMap
{
    public static int MAP_NULL = -1;

    public Vector2Int id;
    private Vector2Int size;
    public Vector2Int Size { get { return size; } }
    public int[,] nodes;
    public World World { get; set; }
    public MapController Controller { get; set; }
    public IMapShower Shower { get; set; }

    public Action<Vector2Int> onAddMapNode, onRemoveMapNode, onSetMapNode;
    public Action<Map> onUpdate;
    public Map(Vector2Int id, Vector2Int size)
    {
        this.id = id;
        this.size = size;
        nodes = new int[size.x, size.y];
    }
    public void InitAllNode(int initID)
    {
        for (int x = 0; x < nodes.GetLength(0); x++)
        {
            for (int y = 0; y < nodes.GetLength(1); y++)
            {
                SetNode(new Vector2Int(x, y), initID);
            }
        }
    }

    public void SetNode(Vector2Int index, int value)
    {
        if (onSetMapNode != null)
        {
            onAddMapNode(index);
        }
        //RemoveNode(index);
        AddNode(index, value);
    }

    public void AddNode(Vector2Int index, int node)
    {
        nodes[index.x, index.y] = node;
        if (onAddMapNode != null)
        {
            onAddMapNode(index);
        }
    }

    public void RemoveNode(Vector2Int index)
    {
        if (onRemoveMapNode != null)
        {
            onRemoveMapNode(index);
        }
        nodes[index.x, index.y] = MAP_NULL;
    }
    public int GetNode(Vector2Int index)
    {
        if (index.x < 0) return MAP_NULL;
        if (index.y < 0) return MAP_NULL;
        if (index.x >= Size.x) return MAP_NULL;
        if (index.y >= Size.y) return MAP_NULL;
        return nodes[index.x, index.y];
    }

    public void Update()
    {
        if (onUpdate != null)
        {
            onUpdate(this);
        }
    }

    public int[,] Save()
    {
        int[,] s = new int[size.x, size.y];
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                s[x, y] = nodes[x, y];
            }
        }
        return s;
    }
    public string SaveToString()
    {
        return "";
    }
    public void Load(int[,] data)
    {
        for (int x = 0; x < nodes.GetLength(0); x++)
        {
            for (int y = 0; y < nodes.GetLength(1); y++)
            {
                SetNode(new Vector2Int(x, y), data[x, y]);
            }
        }
    }
    public void Load(string data)
    {

    }
}
public class MapController
{
    public MapController(Vector2Int startPos, Vector2Int size)
    {
        GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
        c.transform.position = new Vector3(size.x / 2, 0, size.y / 2);
        c.transform.localScale = new Vector3(size.x, 1, size.y);
        c.GetComponent<MeshRenderer>().enabled = false;
        c.layer = 16;
    }
}
public class MapTextureShower : IMapShower
{
    public Map map;

    public Action<int> onShow;
    public GameObject showPanel;

    public Texture2D[,] textures;
    public GameObject[,] quads;
    public bool[,] isModify;
    public Vector2Int unit;

    Func<int, Color[]> showerFun;
    public Material quadsMaterial;
    //public Texture2D showTexture;

    public MapTextureShower(Map map, Vector2Int unit, Func<int, Color[]> showerFun = null, Material quadsMaterial = null)
    {
        this.unit = unit;
        this.map = map;
        this.showerFun = showerFun;
        this.quadsMaterial = quadsMaterial;

        map.onAddMapNode += OnMapAddNode;
        map.onRemoveMapNode += OnMapRemoveNode;
        int x = map.Size.x / (1024 / unit.x);
        int y = map.Size.y / (1024 / unit.y);
        int mx = map.Size.x % (1024 / unit.x);
        int my = map.Size.y % (1024 / unit.y);
        if (mx != 0)
        {
            x += 1;
        }
        if (my != 0)
        {
            y += 1;
        }
        textures = new Texture2D[x, y];
        quads = new GameObject[x, y];
        isModify = new bool[x, y];
        for (int i = 0; i < textures.GetLength(0); i++)
        {
            for (int j = 0; j < textures.GetLength(1); j++)
            {
                int xsize = map.Size.x * unit.x > 1024 ? 1024 : map.Size.x * unit.x;
                int ysize = map.Size.y * unit.y > 1024 ? 1024 : map.Size.y * unit.y;

                if (i == textures.GetLength(0) - 1)
                {

                    xsize = mx == 0 ? xsize : mx * unit.x;
                }
                if (j == textures.GetLength(1) - 1)
                {
                    ysize = my == 0 ? ysize : my * unit.y;
                }
                textures[i, j] = new Texture2D(xsize, ysize);
                textures[i, j].wrapMode = TextureWrapMode.Clamp;
                quads[i, j] = GameObject.CreatePrimitive(PrimitiveType.Quad);
                if (quadsMaterial != null)
                {
                    quads[i, j].GetComponent<MeshRenderer>().material = quadsMaterial;
                }
                quads[i, j].GetComponent<MeshRenderer>().material.mainTexture = textures[i, j];
                quads[i, j].transform.position = new Vector3(i * unit.x, 0, j * unit.y) + new Vector3(xsize / (unit.x * 2), 0, ysize / (unit.y * 2));
                quads[i, j].transform.localScale = new Vector3(xsize / unit.x, ysize / unit.y, 1);
                quads[i, j].transform.rotation = Quaternion.Euler(90, 0, 0);
            }
        }
    }
    public void OnMapAddNode(Vector2Int mapNode)
    {
        int x = mapNode.x;// mapNode.Index.x;
        int y = mapNode.y;// mapNode.Index.y;
        int xd = x % (1024 / unit.x);
        int yd = y % (1024 / unit.y);
        x = x / (1024 / unit.x);
        y = y / (1024 / unit.y);
        Color[] c = showerFun(map.nodes[mapNode.x, mapNode.y]); // MainManager.m_Instance.selectColor;
        textures[x, y].SetPixels(xd * unit.x, yd * unit.x, unit.x, unit.y, c);
        isModify[x, y] = true;
    }

    public void OnMapRemoveNode(Vector2Int mapNode)
    {
        int x = mapNode.x;
        int y = mapNode.y;
        int xd = x % (1024 / unit.x);
        int yd = y % (1024 / unit.y);
        x = x / (1024 / unit.x);
        y = y / (1024 / unit.y);
        Color[] c = new Color[unit.x * unit.y];
        for (int i = 0; i < c.Length; i++)
        {
            c[i] = new Color(1, 1, 1, 1);
        }
        textures[x, y].SetPixels(xd * unit.x, yd * unit.x, unit.x, unit.y, c);
        isModify[x, y] = true;
    }

    public void Show(Vector2Int mapIndex)
    {
        if (onShow != null) onShow.Invoke(map.nodes[mapIndex.x, mapIndex.y]);
    }

    public void ClearShow(Vector2Int mapIndex)
    {
        if (onShow != null) onShow.Invoke(Map.MAP_NULL);
    }

    public void ShowAll()
    {
        Vector2Int target = Vector2Int.zero;
        for (int x = 0; x < map.nodes.GetLength(0); x++)
        {
            for (int y = 0; y < map.nodes.GetLength(1); y++)
            {
                target.x = x;
                target.y = y;
                Show(target);
            }
        }
    }
    public void ClearShowAll()
    {
        Vector2Int target = Vector2Int.zero;
        for (int x = 0; x < map.nodes.GetLength(0); x++)
        {
            for (int y = 0; y < map.nodes.GetLength(1); y++)
            {
                target.x = x;
                target.y = y;
                ClearShow(target);
            }
        }
    }

    public void Refresh()
    {
        for (int i = 0; i < isModify.GetLength(0); i++)
        {
            for (int j = 0; j < isModify.GetLength(1); j++)
            {
                if (isModify[i, j])
                {
                    isModify[i, j] = false;
                    textures[i, j].Apply();
                }
            }
        }
    }
}

public interface IMapShower
{
    void Refresh();
}

public class MapNodeShower : IMapShower
{
    public Map map;
    public GameObject[,] nodes;
    public Renderer[,] nodesRenderer;
    public Func<int, GameObject> showFunc;

    public MapNodeShower(Map map, Func<int, GameObject> showFunc)
    {
        this.map = map;
        nodes = new GameObject[map.Size.x, map.Size.y];
        nodesRenderer = new Renderer[map.Size.x, map.Size.y];
        if (showFunc == null)
        {
            throw new Exception("showFunc can not be null!!");
        }
        this.showFunc = showFunc;
        map.onAddMapNode += AddShowNode;
        map.onRemoveMapNode += RemoveShowNode;
    }
    public void RemoveShowNode(IMapable node)
    {
        if (node != null)
        {
            RemoveShowNode(node.Index);
        }
    }
    public void AddShowNode(IMapable node)
    {
        AddShowNode(node.Index);
    }
    public void ShowNode(Vector2Int index)
    {
        RemoveShowNode(index);
        AddShowNode(index);
    }
    void RemoveShowNode(Vector2Int index)
    {
        GameObject t = nodes[index.x, index.y];
        if (t != null)
        {
            GameObject.Destroy(t);
            t = null;
        }
    }
    void AddShowNode(Vector2Int index)
    {
        nodes[index.x, index.y] = showFunc(map.GetNode(index));
        nodesRenderer[index.x, index.y] = nodes[index.x, index.y].GetComponentInChildren<Renderer>();
        if (nodes[index.x, index.y] != null)
        {
            nodes[index.x, index.y].transform.position = new Vector3(index.x, -0.5f, index.y) + Vector3.one * 0.5f;
        }
    }
    public void ShowAll()
    {
        Vector2Int s = map.Size;
        for (int x = 0; x < s.x; x++)
        {
            for (int y = 0; y < s.y; y++)
            {
                ShowNode(new Vector2Int(x, y));
            }
        }
    }

    public GameObject GetNode(Vector2Int index)
    {
        return nodes[index.x, index.y];
    }

    public void Refresh()
    {
        //throw new NotImplementedException();
    }
}
public class World
{
    public Map grouds;
    public Map plants;
    public Map placeObjects;
    public Map wallObjects;
    public Map temperature;
    public Map birghtness;
    public Map wather;
    public Map water;
    public Map electr;

    public void CreateGroundMap(Vector2Int size, Func<int, GameObject> funcShower)
    {
        grouds = new Map(Vector2Int.zero, size);
        grouds.Shower = new MapNodeShower(grouds, funcShower);
        grouds.Controller = new MapController(Vector2Int.zero, size);
        grouds.InitAllNode(0);
    }

    public void CreatePlantMap(Vector2Int size, Func<int, GameObject> funcShower)
    {
        plants = new Map(Vector2Int.zero, Vector2Int.one * 64);
        plants.Shower = new MapNodeShower(plants, funcShower);
        plants.InitAllNode(0);
    }

}
public class Ground
{
    public int id;
    public void Update()
    {
        switch (id)
        {
            case 0:
                //MadUpdate();
                break;
            case 1:
                //SadUpdate();
                break;

        }
        //Debug.Log("GroundUpdate");
        //Debug.Log(Index);
    }


    //void SadUpdate()
    //{
    //    if (UnityEngine.Random.value < 1f)
    //    {
    //        Vector2Int up = Index + Vector2Int.up;
    //        Vector2Int down = Index + Vector2Int.down;
    //        Vector2Int right = Index + Vector2Int.right;
    //        Vector2Int left = Index + Vector2Int.left;
    //        if (Map.GetNode(up) != null && (Map.GetNode(up).ID == 0) ) Map.SetNode(up, new Ground(1,Map));//&& Map.World.plants.GetNode(up).ID != 1
    //        if (Map.GetNode(down) != null && (Map.GetNode(down).ID == 0) ) Map.SetNode(down, new Ground(1, Map));
    //        if (Map.GetNode(right) != null && (Map.GetNode(right).ID == 0) ) Map.SetNode(right, new Ground(1, Map));
    //        if (Map.GetNode(left) != null && (Map.GetNode(left).ID == 0)) Map.SetNode(left, new Ground(1, Map));
    //    }
    //}
}
public class Wall : IMapable
{
    public Wall(int id)
    {
        this.ID = id;
    }
    public int ID { get; set; }

    public Vector2Int Index { get; set; }

    public IMap Map { get; set; }

    public void Update()
    {
    }
}
public class Brightness : IMapable
{
    public Brightness(int id)
    {
        ID = id;
    }
    public int ID { get; set; }

    public Vector2Int Index { get; set; }

    public IMap Map { get; set; }

    public void Update()
    {
        float tt = ID;
        tt /= 100;
        //(Map.Shower.nodesRenderer[Index.x, Index.y] as SpriteRenderer).color = new Color(0,0,0,0.75f-tt*0.75f);
    }
}
public class Temperature : IMapable
{
    public Temperature(int id)
    {
        ID = id;
    }
    public int ID { get; set; }

    public Vector2Int Index { get; set; }

    public IMap Map { get; set; }

    public void Update()
    {
        int tt = ID;
        Vector2Int up = Index + Vector2Int.up;
        Vector2Int down = Index + Vector2Int.down;
        Vector2Int right = Index + Vector2Int.right;
        Vector2Int left = Index + Vector2Int.left;
        int upnode = Map.GetNode(up);
        tt += upnode == -1 ? 0 : upnode;
        int downnode = Map.GetNode(down);
        tt += downnode == -1 ? 0 : downnode;
        int rightnode = Map.GetNode(right);
        tt += rightnode == -1 ? 0 : rightnode;
        int leftnode = Map.GetNode(left);
        tt += leftnode == -1 ? 0 : leftnode;
        tt /= 5;
        ID = tt;
        if (upnode != -1) upnode = tt;
        if (downnode != -1) downnode = tt;
        if (rightnode != -1) rightnode = tt;
        if (leftnode != -1) leftnode = tt;

        float ttt = (float)tt / 100;
    }
}
public interface IMap
{
    void SetNode(Vector2Int index, int value);
    MapController Controller { get; set; }
    IMapShower Shower { get; set; }
    int GetNode(Vector2Int index);
    World World { get; set; }
}
public interface IMapable
{
    int ID { get; set; }
    Vector2Int Index { get; set; }
    IMap Map { get; set; }

    void Update();
}


public class Room
{
    Room(List<Vector2Int> node)
    {
        this.roomNodes = node;
    }
    public List<Vector2Int> roomNodes;

    public static Action<Room> onCreateRoom;
    public static Action<Room> onDeleteRoom;
    public static Room CreateRoom(List<Vector2Int> node)
    {
        Room r = new Room(node);
        if (onCreateRoom != null)
        {
            onCreateRoom.Invoke(r);
        }
        return r;
    }

    public static void DeleteRoom(Room r)
    {
        if (onDeleteRoom != null)
        {
            onDeleteRoom.Invoke(r);
        }
    }
}
