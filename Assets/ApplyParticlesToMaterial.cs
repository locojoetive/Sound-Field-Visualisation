using UnityEngine;

public class ApplyParticlesToMaterial : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    private static readonly int SegmentCount = Shader.PropertyToID("_SegmentCount");
    private static readonly int Segments = Shader.PropertyToID("_Segments");

    void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        float [] array = new float[] { 1f, 0.5f };
        Material material = _meshRenderer.sharedMaterial;
        material.SetFloatArray(Segments, array);
        material.SetInt(SegmentCount, 2);
        _meshRenderer.sharedMaterial = material;
    }

    void Update()
    {
    }
}
