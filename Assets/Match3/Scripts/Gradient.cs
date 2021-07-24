namespace Game2019.JungleGemBlast
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    [ExecuteInEditMode]
    public class Gradient : BaseMeshEffect
    {
        [SerializeField]
        private Color32 bottomColor = Color.black;

        [Range(0f, 1f)]
        public float Offset = 0.5f;

        [SerializeField]
        private Color32 topColor = Color.white;

        public override void ModifyMesh(VertexHelper vh)
        {
            if (IsActive())
            {
                List<UIVertex> list = new List<UIVertex>();
                vh.GetUIVertexStream(list);
                ModifyVertices(list);
                vh.Clear();
                vh.AddUIVertexTriangleStream(list);
            }
        }

        public void ModifyVertices(List<UIVertex> vertexList)
        {
            if (!IsActive())
            {
                return;
            }
            for (int i = 0; i < vertexList.Count; i++)
            {
                if (i % 6 == 0 || i % 6 == 1 || i % 6 == 5)
                {
                    UIVertex value = vertexList[i];
                    value.color = Color.Lerp(bottomColor, topColor, Offset - 0.5f + 1f);
                    vertexList[i] = value;
                }
                if (i % 6 == 2 || i % 6 == 3 || i % 6 == 4)
                {
                    UIVertex value2 = vertexList[i];
                    value2.color = Color.Lerp(topColor, bottomColor, 1f - Offset - 0.5f + 1f);
                    vertexList[i] = value2;
                }
            }
        }

        public void ChangeColor(Color32 top, Color32 bottom)
        {
            topColor = top;
            bottomColor = bottom;
        }
    }
}