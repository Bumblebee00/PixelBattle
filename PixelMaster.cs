using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelMaster : MonoBehaviour
{
    public ComputeShader battleshader;
    public Vector2 renderTextureDimensions;
    private RenderTexture renderTexture;

    private ComputeBuffer squaresBuffer;
    private ComputeBuffer newBattlefieldBuffer;
    public int gridSize = 2;
    public Color color0;
    public Color color1;

    private void Start()
    {
        SetupBattlefield();
    }

    struct Square
    {
        public Vector2 position;
        public float color;
    }

    void SetupBattlefield()
    {
        List<Square> battlefield = new List<Square>();
        List<Square> newBattlefield = new List<Square>();
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Square square = new Square();
                square.position = new Vector2(x, y);
                square.color = (y >= (gridSize/2) ? 1 : 0) ;
                battlefield.Add(square);
                newBattlefield.Add(square);
            }
        }
        // Assign to compute buffer
        if (squaresBuffer != null)
            squaresBuffer.Release();
        int positionsize = sizeof(float) * 2;
        int colorsize = sizeof(float) * 1;
        squaresBuffer = new ComputeBuffer(battlefield.Count, positionsize  + colorsize);
        squaresBuffer.SetData(battlefield);
        // Assign to compute buffer
        if (newBattlefieldBuffer != null)
            newBattlefieldBuffer.Release();
        newBattlefieldBuffer = new ComputeBuffer(battlefield.Count, positionsize + colorsize);
        newBattlefieldBuffer.SetData(battlefield);
       
    }

    private void Render(RenderTexture destination)
    {
        // Make sure we have a current render target
        InitRenderTexture();
        // Set the target and dispatch the compute shader
        battleshader.SetTexture(0, "Result", renderTexture);
        int threadGroupsX = Mathf.CeilToInt(renderTextureDimensions.x / 32.0f);
        int threadGroupsY = Mathf.CeilToInt(renderTextureDimensions.y / 32.0f);
        battleshader.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        // Blit the result texture to the screen
        Graphics.Blit(renderTexture, destination);
    }
    private void InitRenderTexture()
    {
        if (renderTexture == null || renderTexture.width != Screen.width || renderTexture.height != Screen.height)
        {
            // Release render texture if we already have one
            if (renderTexture != null)
                renderTexture.Release();
            // Get a render target for Ray Tracing
            renderTexture = new RenderTexture((int) renderTextureDimensions.x, (int) renderTextureDimensions.y, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();
        }
    }

    private void SetShaderParameters()
    {
        battleshader.SetFloat("width", renderTextureDimensions.x);
        battleshader.SetFloat("height", renderTextureDimensions.y);
        battleshader.SetInt("gridSize", gridSize);
        battleshader.SetFloat("_Seed", Random.value);
        battleshader.SetFloat("color0r", color0.r);
        battleshader.SetFloat("color0g", color0.g);
        battleshader.SetFloat("color0b", color0.b);
        battleshader.SetFloat("color1r", color1.r);
        battleshader.SetFloat("color1g", color1.g);
        battleshader.SetFloat("color1b", color1.b);


        if (squaresBuffer != null)
        {
            battleshader.SetBuffer(0, "squares", squaresBuffer);
        }
        if (newBattlefieldBuffer != null)
        {
            battleshader.SetBuffer(0, "newBattlefield", newBattlefieldBuffer);
        }
    }

    void UpdateBuffer()
    {
        Square[] x = new Square[gridSize * gridSize];
        newBattlefieldBuffer.GetData(x);
        // Assign to compute buffer
        if (squaresBuffer != null)
            squaresBuffer.Release();
        int positionsize = sizeof(float) * 2;
        int colorsize = sizeof(float) * 1;
        squaresBuffer = new ComputeBuffer(x.Length, positionsize + colorsize);
        squaresBuffer.SetData(x);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        SetShaderParameters();
        Render(destination);
        UpdateBuffer();
    }
}