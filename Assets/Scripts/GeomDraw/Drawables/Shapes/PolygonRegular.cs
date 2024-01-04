using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace GeomDraw
{
    public class PolygonRegular : Polygon
    {
        public int nVertices;
        public Vector2 center;
        public Vector2 scale;
        public float rotation;

        public PolygonRegular(int nVertices, Vector2 center, Vector2 scale, float rotation, Color color, LineStyle lineStyle = new LineStyle()) :
            base(new Vector2[nVertices], color, lineStyle)
        {
            this.nVertices = Mathf.Max(3, nVertices);
            this.center = center;
            this.scale = scale;
            this.rotation = rotation;
            CreateVertices();
        }

        public PolygonRegular(int nVertices, Vector2 center, Vector2 scale, Color color, LineStyle lineStyle = new LineStyle()) :
        base(new Vector2[nVertices], color, lineStyle)
        {
            this.nVertices = Mathf.Max(3, nVertices);
            this.center = center;
            this.scale = scale;
            this.rotation = 0;
            CreateVertices();
        }
        

        private void CreateVertices()
        {
            vertices = new Vector2[nVertices];
            float angle = 2 * Mathf.PI / nVertices;
            float phase = rotation;
            if (nVertices % 2 == 0) phase += angle * 0.5f;

            for (int i = 0; i < nVertices; i++)
            {
                float x = center.x + scale.x * 0.5f * Mathf.Sin(i * angle + phase);
                float y = center.y + scale.y * 0.5f * Mathf.Cos(i * angle + phase);
                vertices[i] = new Vector2(x, y);
            }
        }
    }
}
