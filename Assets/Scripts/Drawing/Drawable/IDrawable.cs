using UnityEditor;
using UnityEngine;

namespace Drawing
{
    public interface IDrawable
    {
        /// <summary>
        /// Sanity checks and fix of potential problems of rendering at given pixelsPerUnit and 
        /// preventing invalid parameter ranges
        /// </summary>
        public bool CheckDrawability(float pixelsPerUnit);
    }
}