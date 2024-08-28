using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeomDraw
{
    public class DrawerMesh
    {
        MeshRenderer meshRenderer;


        // OPTIONS
        public bool UpdateMipMaps { get; set; }


        /// <summary> 
        /// Build the drawer over the Mesh Renderer
        /// </summary>
        public DrawerMesh(MeshRenderer meshRenderer, bool updateMipMaps = false)
        {
            this.meshRenderer = meshRenderer;
            this.UpdateMipMaps = updateMipMaps;
        }


        // PUBLIC FUNCTIONS


        /// <summary>
        /// It substitutes the renderer sprite with an empty one. Width and height in world units 
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
            RenderTexture texture = new RenderTexture(wInt, hInt, 0);
            texture.enableRandomWrite = true;
            texture.autoGenerateMips = UpdateMipMaps;
            texture.Create();

            ComputeShader fillTexCS = (ComputeShader)Resources.Load("ComputeShaders/FillTex");
            fillTexCS.SetInt("width", wInt);
            fillTexCS.SetInt("height", hInt);
            fillTexCS.SetFloats("color", new float[4] { backgroundColor.r, backgroundColor.g, backgroundColor.b, backgroundColor.a });
            fillTexCS.SetTexture(0, "tex", texture);
            ComputeUtils.Dispatch(fillTexCS, wInt, hInt, 1, 0);
            meshRenderer.sharedMaterial.mainTexture = texture;
        }
    }
}

