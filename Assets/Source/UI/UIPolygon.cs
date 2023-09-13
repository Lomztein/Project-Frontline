/// Credit CiaccoDavide
/// Sourced from - http://ciaccodavi.de/unity/uipolygon

using System.Collections.Generic;
using UnityEngine.UIElements;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/Primitives/UI Polygon")]
    public class UIPolygon : MaskableGraphic
    {
        [SerializeField]
        Texture m_Texture;
        public Vector2[] Verts = new Vector2[] { Vector2.up, Vector2.right, Vector2.down, Vector2.left };
        public float Size = 0;

        protected override void Awake()
        {
            base.Awake();
            Update();
        }

        public override Texture mainTexture
        {
            get
            {
                return m_Texture == null ? s_WhiteTexture : m_Texture;
            }
        }
        public Texture texture
        {
            get
            {
                return m_Texture;
            }
            set
            {
                if (m_Texture == value) return;
                m_Texture = value;
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }

        public void DrawPolygon(Vector2[] verts)
        {
            Verts = verts;
            SetVerticesDirty();
            SetMaterialDirty();
        }

        void Update()
        {
            Size = rectTransform.rect.width;
            if (rectTransform.rect.width > rectTransform.rect.height)
                Size = rectTransform.rect.height;
            else
                Size = rectTransform.rect.width;
            SetVerticesDirty();
        }

        protected UIVertex SetVbo(Vector2 vert, Vector2 uv)
        {
            var uiVert = UIVertex.simpleVert;
            uiVert.color = color;
            uiVert.position = vert;
            uiVert.uv0 = uv;
            return uiVert;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            Verts[Verts.Length - 1] = Verts[0];
            vh.AddVert(SetVbo(Vector2.zero, Vector2.zero));
            for (int i = 0; i < Verts.Length; i++)
            {
                Vector2 vert = Verts[i] * Size / 2f;
                vh.AddVert(SetVbo(vert, vert));
            }
            for (int i = 0; i < Verts.Length - 1; i++)
            {
                vh.AddTriangle(i + 1, i + 2, 0);
            }
        }
    }
}
