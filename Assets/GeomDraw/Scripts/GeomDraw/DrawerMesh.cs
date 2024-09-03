using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeomDraw
{
    /// <summary>
    /// Class containing all the functions for drawing on a meshRenderer.
    /// </summary>
    public class DrawerMesh
    {
        MeshRenderer meshRenderer;
        MyRendererMesh myRenderer;
        [SerializeField, HideInInspector] public ComputeShader fillTexCS;
        [SerializeField, HideInInspector] public RenderTexture texture;

        // OPTIONS
        public bool UpdateMipMaps { get; set; }
        public bool Antialiase { get; set; }


        /// <summary> 
        /// Build the drawer over the Mesh Renderer
        /// </summary>
        public DrawerMesh(MeshRenderer meshRenderer, bool updateMipMaps = false, bool antialiase = false)
        {
            this.meshRenderer = meshRenderer;
            UpdateMipMaps = updateMipMaps;
            Antialiase = antialiase;
        }


        // PUBLIC FUNCTIONS


        /// <summary>
        /// It substitutes the meshRendere texture with an empty one. Width and height in world units 
        /// </summary>
        /// <param name="width">New sprite width in world units</param>
        /// <param name="height">New sprite width in world units</param>
        /// <param name="pixelsPerUnity">Number of pixels per one world unit</param>
        /// <param name="backgroundColor">Background color of the empty sprite</param>
        public void NewEmptyMesh(
            float width,
            float height,
            float pixelsPerUnity,
            Color backgroundColor
        )
        {
            //AddUndoer();

            int wInt = Mathf.RoundToInt(width * pixelsPerUnity);
            int hInt = Mathf.RoundToInt(height * pixelsPerUnity);
            texture = ComputeUtils.CreateRenderTexture(wInt, hInt, UpdateMipMaps);
            meshRenderer.sharedMaterial.mainTexture = texture;
            meshRenderer.transform.localScale = new Vector2 (width, height);

            fillTexCS = Resources.Load<ComputeShader>("ComputeShaders/FillTex");
            fillTexCS.SetInt("width", wInt);
            fillTexCS.SetInt("height", hInt);
            fillTexCS.SetFloats("color", new float[4] { backgroundColor.r, backgroundColor.g, backgroundColor.b, backgroundColor.a });
            fillTexCS.SetTexture(0, "tex", texture);
            ComputeUtils.Dispatch(fillTexCS, wInt, hInt, 1, 0);
        }


        /// <summary>
        /// Draw a generic Drawable element of the MeshRenderer
        /// </summary>
        /// <param name="drawable">Element to draw</param>
        /// <param name="updateUndoer">Whether the DrawnSprite component of the new drawing has to be updated</param>
        public void Draw(IDrawable drawable, bool updateUndoer = false)
        {
            if (meshRenderer.sharedMaterial == null)
            {
                Debug.LogError("No material assigned to the Mesh");
                return;
            }
            if (meshRenderer.sharedMaterial.mainTexture == null)
            {
                Debug.LogError("No texture assigned to the material");
                return;
            }
            myRenderer = new MyRendererMesh(meshRenderer, this);

            if (!drawable.CheckDrawability(myRenderer.pxUnit))
            {
                Debug.LogError("Drawability check not passed");
                return;
            }

            
            //myMerger = new TextureMerger(spriteRenderer);

            //if (updateUndoer)
            //{
            //    //AddUndoer();
            //    //spriteRenderer.GetComponent<Undoer>().NewDraw(drawable);
            //}

            if (drawable is IDrawableLine)
                return;
            //myRenderer.DrawLine((IDrawableLine)drawable);
            else if (drawable is IDrawableShape)
                myRenderer.DrawShape((IDrawableShape)drawable);
            else if (drawable is DrawableTexture)
                return;
                //myMerger.DrawTexture((DrawableTexture)drawable);
            else
                Debug.LogError("Invalid IDrawable");

            //spriteRenderer.sprite.texture.Apply(UpdateMipMaps);
            
        }
    }
}

