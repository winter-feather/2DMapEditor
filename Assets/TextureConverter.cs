using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextureConverter : MonoBehaviour
{
    public RawImage show;
    // Start is called before the first frame update
    void Start()
    {
        show.texture = CopyToTargetTexture(16, 16);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Texture2D sourceTexture;
    Texture2D targetTexture;

    public Texture2D CopyToTargetTexture(int sizeX,int sizeY) {
        int xCount = sourceTexture.width / sizeX;
        int yCount = sourceTexture.height / sizeY;
        int targetWidth = xCount * yCount * sizeX, targetHeight = sizeY;
        targetTexture = new Texture2D(targetWidth, targetHeight);
        targetTexture.filterMode = FilterMode.Bilinear;
        int count = 0;
        for (int i = 0; i < xCount; i++)
        {
            for (int j = 0; j < yCount; j++)
            {
                targetTexture.SetPixels(count++ * sizeX, 0, sizeX, sizeY, sourceTexture.GetPixels(i * sizeX, j * sizeY, sizeX, sizeY));
            }
        }
        targetTexture.Apply();
        return targetTexture;
    }
}
