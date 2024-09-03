using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace GeomDraw
{
    public static class ComputeUtils
    {
        /// Convenience method for dispatching a compute shader.
        /// It calculates the number of thread groups based on the number of iterations needed.
        public static void Dispatch(ComputeShader cs, int numIterationsX, int numIterationsY = 1, int numIterationsZ = 1, int kernelIndex = 0)
        {
            Vector3Int threadGroupSizes = GetThreadGroupSizes(cs, kernelIndex);
            int numGroupsX = Mathf.CeilToInt(numIterationsX / (float)threadGroupSizes.x);
            int numGroupsY = Mathf.CeilToInt(numIterationsY / (float)threadGroupSizes.y);
            int numGroupsZ = Mathf.CeilToInt(numIterationsZ / (float)threadGroupSizes.y);
            cs.Dispatch(kernelIndex, numGroupsX, numGroupsY, numGroupsZ);
        }

        public static Vector3Int GetThreadGroupSizes(ComputeShader compute, int kernelIndex = 0)
        {
            uint x, y, z;
            compute.GetKernelThreadGroupSizes(kernelIndex, out x, out y, out z);
            return new Vector3Int((int)x, (int)y, (int)z);
        }

        public static RenderTexture CreateRenderTexture(int wInt, int hInt, bool updateMipMaps = false)
        {
            RenderTexture texture = new RenderTexture(wInt, hInt, 0);
            texture.enableRandomWrite = true;
            texture.autoGenerateMips = updateMipMaps;
            texture.filterMode = FilterMode.Point;
            //texture.graphicsFormat = 
            texture.Create();
            return texture;
        }

        public static ComputeBuffer LoadArrayOnCS<T>(T[] array, ComputeShader cs, string arrayName, int[] kernelIndexes)
        {
            ComputeBuffer buffer = new(array.Length, Marshal.SizeOf(typeof(System.Single)), ComputeBufferType.Default);
            buffer.SetData(array);
            foreach(int i in kernelIndexes)
                cs.SetBuffer(i, arrayName, buffer);
            return buffer;
        }

        public static ComputeBuffer LoadArrayOnCS<T>(T[] array, ComputeShader cs, string arrayName, int kernelIndex = 0)
        {
            return LoadArrayOnCS(array, cs, arrayName, new int[1] { kernelIndex });
        }
    }
}

