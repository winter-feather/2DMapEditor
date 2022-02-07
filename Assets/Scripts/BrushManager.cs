using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushManager : SingleMonoManager<BrushManager>
{
    public Sprite[] planets;
    public Dictionary<string, Sprite[]> brush;
    public List<Texture2D>  gourndTextures;
    private void Awake()
    {
        //brush = new Dictionary<string, Sprite[]>();
        //brush["NumBrush"] = Resources.LoadAll<Sprite>(@"Brush\NumBrush"); //Resources.Load<Sprite>(@"\NumBrush");
        //brush["ConerBrush"] = Resources.LoadAll<Sprite>(@"Brush\ConerBrush"); //Resources.Load<Sprite>(@"\NumBrush");
        gourndTextures = new List<Texture2D>();
        gourndTextures.Add(Resources.Load<Texture2D>(@"Brush\EdgeConerGround\A"));
        gourndTextures.Add(Resources.Load<Texture2D>(@"Brush\EdgeConerGround\B"));
        gourndTextures.Add(Resources.Load<Texture2D>(@"Brush\EdgeConerGround\C"));
        gourndTextures.Add(Resources.Load<Texture2D>(@"Brush\EdgeConerGround\D"));
        gourndTextures.Add(Resources.Load<Texture2D>(@"Brush\EdgeConerGround\E"));
        gourndTextures.Add(Resources.Load<Texture2D>(@"Brush\EdgeConerGround\F"));
        gourndTextures.Add(Resources.Load<Texture2D>(@"Brush\EdgeConerGround\G"));
        gourndTextures.Add(Resources.Load<Texture2D>(@"Brush\EdgeConerGround\H"));

        //gourndTextures.Add(Resources.Load<Texture2D>(@"Brush\FGGround\1"));
        //gourndTextures.Add(Resources.Load<Texture2D>(@"Brush\FGGround\2"));
        //gourndTextures.Add(Resources.Load<Texture2D>(@"Brush\FGGround\3"));
        //gourndTextures.Add(Resources.Load<Texture2D>(@"Brush\FGGround\4"));
        //gourndTextures.Add(Resources.Load<Texture2D>(@"Brush\FGGround\5"));
        //gourndTextures.Add(Resources.Load<Texture2D>(@"Brush\FGGround\6"));
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
