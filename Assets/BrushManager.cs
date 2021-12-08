using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushManager : SingleMonoManager<BrushManager>
{
    public Sprite[] planets;
    public Dictionary<string, Sprite[]> brush;
    public Sprite[] tests;
    private void Awake()
    {
        brush = new Dictionary<string, Sprite[]>();
        brush["NumBrush"] = Resources.LoadAll<Sprite>(@"Brush\NumBrush"); //Resources.Load<Sprite>(@"\NumBrush");
        brush["ConerBrush"] = Resources.LoadAll<Sprite>(@"Brush\ConerBrush"); //Resources.Load<Sprite>(@"\NumBrush");
        tests = brush["ConerBrush"];
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadBrush() { 
    
    }
}
