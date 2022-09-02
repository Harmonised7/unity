using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    public SimSettings settings;
    public ComputeShader computeShader;
    public RenderTexture renderTexture;
    
    // public Material mat;
    public Vector2 pos, smoothPos;
    public float scale, smoothScale, angle, smoothAngle;
    
    // Start is called before the first frame update
    void Start()
    {
        renderTexture = new RenderTexture(settings.width, settings.height, 24);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();

        computeShader.SetTexture(0, "Result", renderTexture);
        computeShader.SetFloat("width", settings.width);
        computeShader.SetFloat("height", settings.height);
        computeShader.Dispatch(0, renderTexture.width / 8, renderTexture.height / 8, 1);
       
        GetComponent<RawImage>().texture = renderTexture;
    }
    
    private void UpdateShader()
    {
        smoothScale = Mathf.Lerp(smoothScale, scale, 0.1f);
        smoothAngle = Mathf.Lerp(smoothAngle, angle, 0.1f);
        smoothPos = Vector2.Lerp(smoothPos, pos, 0.1f);
        
        float aspect = (float)Screen.width / (float)Screen.height;

        float scaleX = smoothScale;
        float scaleY = smoothScale;

        if (aspect > 1f)
            scaleY /= aspect;
        else
            scaleX *= aspect;
        
        // mat.SetVector("_Area", new Vector4(smoothPos.x, smoothPos.y, scaleX, scaleY));
        // mat.SetFloat("_Angle", smoothAngle);
    }

    private void HandleInputs()
    {
        if(Input.GetKey(KeyCode.Equals))
            scale *= 0.98f;
        if(Input.GetKey(KeyCode.Minus))
            scale *= 1.02f;

        Vector2 dir = new Vector2(0.005f * scale, 0);
        float s = Mathf.Sin(angle);
        float c = Mathf.Cos(angle);
        dir = new Vector2(dir.x*c, dir.x*s);

        if(Input.GetKey(KeyCode.A))
            pos -= dir;
        else if(Input.GetKey(KeyCode.D))
            pos += dir;

        dir = new Vector2(-dir.y, dir.x);
        if(Input.GetKey(KeyCode.S))
            pos -= dir;
        else if(Input.GetKey(KeyCode.W))
            pos += dir;
        
        if(Input.GetKey(KeyCode.E))
            angle -= 0.01f;
        else if(Input.GetKey(KeyCode.Q))
            angle += 0.01f;

    }

    void FixedUpdate()
    {
        HandleInputs();
        UpdateShader();
    }
}
