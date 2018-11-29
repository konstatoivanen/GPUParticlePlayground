using UnityEngine;
using Utilities.UI;

public class RootRenderer : MonoBehaviour, ISimulationSettings
{
    public BoidSimParams SimulationParameters => parameters;

    public bool AllowInput { get; set; }

    public float brushSize;
    public BoidSimParams parameters;
    [Space(10)]
    public  ComputeShader compute;
    private ComputeBuffer boidBuffer;
    private RenderTexture drawingBuffer;
    private Material      materialMesh;
    private Material      materialBlit;

    private int kernelSimulate;
    private readonly int hashBrushSize      = Shader.PropertyToID("_BrushSize");
    private readonly int hashBrushDraw      = Shader.PropertyToID("_BrushDraw");
    private readonly int hashDrawBuffer     = Shader.PropertyToID("_DrawBuffer");
    private readonly int hashCursorPos      = Shader.PropertyToID("_CursorPos");
    private readonly int hashCollisionMask  = Shader.PropertyToID("CollisionMask");
    private readonly int hashBoidBuffer     = Shader.PropertyToID("BoidBuffer");
    private readonly int hashMSInput        = Shader.PropertyToID("Params4");

    private void Awake()
    {
        materialBlit = new Material(Shader.Find("Hidden/Drawing"))
        {
            hideFlags = HideFlags.HideAndDontSave
        };

        materialMesh = new Material(Shader.Find("Hidden/BoidMeshShader"))
        {
            hideFlags = HideFlags.HideAndDontSave
        };

        kernelSimulate = compute.FindKernel("CSBoidMain");

        parameters.Count = (parameters.Count / 32) * 32;

        boidBuffer = new ComputeBuffer(parameters.Count, sizeof(float) * 8, ComputeBufferType.Append);

        ResetComputeSim();

        Application.targetFrameRate = 120;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
            ResetComputeSim();
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        DrawingBufferDirtyCheck(source.descriptor);

        var mousePosition      = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        var currentBrushWeight = !AllowInput ? 0 : Input.GetKey(KeyCode.Mouse0) ? 1 : 0;
        int massSign           = !AllowInput ? 0 : Input.GetKey(KeyCode.Q) ? 1 : Input.GetKey(KeyCode.E) ? -1 : 0;

        materialBlit.SetInt(hashBrushDraw,    currentBrushWeight);
        materialBlit.SetFloat(hashBrushSize,  brushSize);
        materialBlit.SetVector(hashCursorPos, mousePosition);

        Graphics.Blit(drawingBuffer, source);
        Graphics.Blit(source,        drawingBuffer, materialBlit, 0);
        Graphics.Blit(drawingBuffer, destination,   materialBlit, 1);

        // Do Boid Pass
        parameters.SetComputeParams(compute, mousePosition, massSign);
        compute.Dispatch(kernelSimulate,   parameters.Count / 32, 1, 1);

        materialMesh.SetPass(0);
        Graphics.DrawProcedural(MeshTopology.Triangles, 2 * 3, parameters.Count);
    }

    private void OnDestroy()
    {
        DestroyImmediate(materialBlit);
        DestroyImmediate(materialMesh);
        DestroyImmediate(drawingBuffer);

        boidBuffer.Release();
    }

    private void ResetComputeSim()
    {
        parameters.Count = (parameters.Count / 32) * 32;

        BoidData[] tempArray = new BoidData[parameters.Count];
        for (int i = 0; i < parameters.Count; ++i)
        {
            tempArray[i].position   = new Vector2(Random.value * Screen.width, Random.value * Screen.height);
            tempArray[i].direction  = Random.insideUnitCircle;
            tempArray[i].color      = new Vector4(0.25f, 0.5f, 0.5f, 1.0f);
        }

        boidBuffer.SetData(tempArray);
        compute.SetBuffer(kernelSimulate, hashBoidBuffer, boidBuffer);
        parameters.SetComputeParams(compute, Vector2.zero, 0);
        materialMesh.SetBuffer(hashBoidBuffer, boidBuffer);

        if (drawingBuffer != null) DestroyImmediate(drawingBuffer);
    }

    private void DrawingBufferDirtyCheck(RenderTextureDescriptor desc)
    {
        if (drawingBuffer != null && drawingBuffer.width == desc.width && drawingBuffer.height == desc.height)
            return;

        if (drawingBuffer != null)
            DestroyImmediate(drawingBuffer);

        drawingBuffer = new RenderTexture(desc) { hideFlags = HideFlags.HideAndDontSave };
        drawingBuffer.Create();

        materialBlit.SetTexture(hashDrawBuffer, drawingBuffer);
        compute.SetTexture(kernelSimulate, hashCollisionMask, drawingBuffer);
    }

    public void SetBoidCount(int count)
    {
        parameters.Count = count;
        ResetComputeSim();
    }
}


[System.Serializable]
public class BoidSimParams
{
    public int   Count            = 100;
    [Space(10)]
    [Header("Physics Params")]
    public float Timescale        = 1.0f;
    public float Drag             = 1;
    public float BodyMass         = 100000;
    public float BodyRadius       = 0.5f;
    public float Bounciness       = 1;
    public float UnitScale        = 0.1f;
    public float BoidMass         = 5;

    [Space(10)]
    [Header("Boid Flocking Params")]
    public float BoidMaxForce     = 1;
    [Space(5)]
    public float RepelWeight      = 1;
    public float RepelDistance    = 1;
    [Space(5)]
    public float CohesionWeight   = 1;
    public float CohesionCoef     = 1;
    public float CohesionDistance = 1;
    [Space(5)]
    public float AlignWeight      = 1;
    public float AlignCoef        = 1;
    public float AlignDistance    = 1;

    private readonly int hashCount   = Shader.PropertyToID("NumBoids");

    private readonly int hashParams1 = Shader.PropertyToID("Params1");
    private readonly int hashParams2 = Shader.PropertyToID("Params2");
    private readonly int hashParams3 = Shader.PropertyToID("Params3");
    private readonly int hashParams4 = Shader.PropertyToID("Params4");
    private readonly int hashParams5 = Shader.PropertyToID("Params5");

    public void SetComputeParams(ComputeShader shader, Vector2 mousePosition, float massSign)
    {
        float InvMass = 1.0f / BoidMass;
        shader.SetInt(hashCount, Count);
        shader.SetFloats(hashParams1, new float[4] { RepelWeight,           AlignWeight,                CohesionWeight,         CohesionCoef    });
        shader.SetFloats(hashParams2, new float[4] { RepelDistance,         AlignDistance,              CohesionDistance,       BoidMaxForce    });
        shader.SetFloats(hashParams3, new float[4] { InvMass,               Time.deltaTime * Timescale, Screen.width,           Screen.height   });
        shader.SetFloats(hashParams4, new float[4] { mousePosition.x,       mousePosition.y,            AlignCoef,              BodyRadius      });
        shader.SetFloats(hashParams5, new float[4] { BodyMass * massSign,   Drag,                       UnitScale,              Bounciness      });
    }
}

public struct BoidData
{
    public Vector2 position;
    public Vector2 direction;
    public Vector4 color;
}