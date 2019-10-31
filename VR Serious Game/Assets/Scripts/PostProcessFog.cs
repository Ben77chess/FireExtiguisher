using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessFog : MonoBehaviour
{
    private Material mFogMat;
    public Shader mFogShader;

    [Header("Fog")]
    public Color f_Color = Color.grey;
    public float f_bottom = -10.0f;
    public float f_top = 10.0f;
    // Start is called before the first frame update
    void Start()
    {
        if (mFogMat == null)
        {
            mFogMat = new Material(mFogShader);
        }
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Matrix4x4 cameraToWorldMatrix = Camera.main.cameraToWorldMatrix;
        mFogMat.SetMatrix("_CameraToWorldMatrix", cameraToWorldMatrix);
        mFogMat.SetColor("_FogColor", f_Color);
        mFogMat.SetFloat("_FogBottomStart", f_bottom);
        mFogMat.SetFloat("_FogTopEnd", f_top);
        Graphics.Blit(source, destination, mFogMat);
    }
}
