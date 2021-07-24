using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[ExecuteInEditMode]
public class TextEffectDecorComplete : MonoBehaviour
{
    [Header("Blow")]
    public AnimationCurve tweenScaleCurve;

    public AnimationCurve scaleCurve;

    [Range(-1f, 4f)]
    public float blowProgress = 0;

    [Header("Bounce")]
    public AnimationCurve bounceCurve;

    public float bounceX;

    public float bounceY;

    [Range(0, 1f)]
    public float bounceProgress = 0;

    [Header("Grid")]

    public int width = 30;

    public int height = 10;

    [Header("Render")]
    public string sortingLayerName = "Default";

    public int sortingOrder = 1;

    public Camera scaleCamera;

    private Vector3[] cachedVertices;

    private Vector3[] vertices;

    private Mesh mesh;

    void Start()
    {
        int vertexCount = (width + 1) * (height + 1);
        cachedVertices = new Vector3[vertexCount];
        vertices = new Vector3[vertexCount];
        var texCoords = new Vector2[vertexCount];
        var indices = new int[width * height * 6];

        for (int x = 0; x <= width; x++)
        {
            for (int y = 0; y <= height; y++)
            {
                int idx = y * (width + 1) + x;
                cachedVertices[idx] = vertices[idx] = new Vector3(x - width * 0.5f, y - height * 0.5f, 0f);
                texCoords[idx] = new Vector2((float)(x) / width, (float)(y) / height);
            }
        }

        int triangle = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int idx = y * (width + 1) + x;
                indices[triangle + 0] = idx;
                indices[triangle + 1] = idx + width + 1;
                indices[triangle + 2] = idx + width + 2;

                indices[triangle + 3] = idx;
                indices[triangle + 4] = idx + width + 2;
                indices[triangle + 5] = idx + 1;

                triangle += 6;
            }
        }

        mesh = new Mesh();
        mesh.MarkDynamic();
        mesh.vertices = cachedVertices;
        mesh.uv = texCoords;
        mesh.triangles = indices;

        GetComponent<MeshFilter>().mesh = mesh;

        var renderer = GetComponent<MeshRenderer>();
        renderer.sortingLayerName = sortingLayerName;
        renderer.sortingOrder = sortingOrder;
        //transform.parent.localScale = scaleCamera.orthographicSize / 16f * Vector3.one;

        CalculateAnimation();    
    }

    void Update()
    {
        CalculateAnimation();
    }

    public void CalculateAnimation()
    {
        for (int i = 0; i < cachedVertices.Length; i++)
        {
            Vector2 vertex = cachedVertices[i];

            float x = Mathf.Abs(vertex.x) / (width * 0.5f);
            float m = Mathf.Clamp01((x - blowProgress) / 0.65f);

            float y = scaleCurve.Evaluate(1f - m);

            vertex.x += bounceCurve.Evaluate(bounceProgress) * x * Mathf.Sign(vertex.x) * bounceX;
            vertex.y += bounceCurve.Evaluate(bounceProgress) * x * Mathf.Sign(vertex.y) * bounceY;

            vertex.y = vertex.y * y;
            vertex.y -= (1f - tweenScaleCurve.Evaluate(1 - m)) * 2f;

            vertices[i] = vertex;
        }

        mesh.vertices = vertices;
    }
}

