using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TextMeshAnimationController : MonoBehaviour
{
    public AnimationCurve scaleCurve;

    public AnimationCurve translateCurve;

    private Vector3[] cachedVertices;

    private Vector3[] vertices;

    private Mesh mesh;

    public int width = 30;

    public int height = 10;

    [Range(-1f, 2f)]
    public float t = 0;

    public float leftScale = 1f;

    public float rightScale = 1f;

    public float delay = 0f;

    [Header("Render")]
    public string sortingLayerName = "Default";

    public int sortingOrder = 1;

    public Camera scaleCamera;

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
                cachedVertices[idx] = vertices[idx] = new Vector3(x, y - height * 0.5f, 0f);
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

        Animate(t);
    }

    void Update()
    {
        Animate(t);
    }

    public void Animate(float t)
    {
        for (int i = 0; i < cachedVertices.Length; i++)
        {
            Vector2 vertex = cachedVertices[i];

            float x = vertex.x / width;
            float m = x - t;

            if (m < 0) m *= leftScale;
            if (m > 0) m *= rightScale;

            float y = scaleCurve.Evaluate(Mathf.Clamp01(m + 0.5f));

            vertices[i].y = vertex.y * y;

            float v = vertex.x + width * (0.5f - x * 0.5f);
            vertices[i].x = v + (vertex.x - v) * translateCurve.Evaluate(Mathf.Clamp01(t - x + delay));
        }

        mesh.vertices = vertices;
    }
}
