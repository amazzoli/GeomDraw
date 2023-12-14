using UnityEditor;
using UnityEngine;

namespace GeomDraw
{
    public enum Axis { x, y }

    public interface IDrawable
    {
        /// <summary>
        /// It returns a deep copy of the object
        /// </summary>
        public IDrawable Copy();


        /// <summary>
        /// Sanity checks and fix of potential problems of rendering at given pixelsPerUnit and 
        /// preventing invalid parameter ranges
        /// </summary>
        public bool CheckDrawability(float pixelsPerUnit);

        /// <summary>
        /// Transformation that moves the drawable of an amount and direction specified by the argument
        /// </summary>
        public void Translate(Vector2 translation);

        /// <summary>
        /// Transformation that rotates the drawable of an angle specified by the argument with respect
        /// a rotational center. If isRelative the rotational center is relative to the drawable rectangle
        /// center, if is not isRelative the rotational center is in sprite world coordinates
        /// </summary>
        public void Rotate(float radAngle, Vector2 rotCenter, bool isRelative);

        /// <summary>
        /// Transformation that reflects the drawable with respect to the x or y axis. (no deformation)
        /// coord specifies the y or x coordinate of the reflection axis relative or not relative to
        /// the rectangular center.
        /// </summary>
        public void Reflect(Axis axis, float coord = 0, bool isRelative = true);

        /// <summary>
        /// Transformation that deformes the drawable along the axis of a given factor.
        /// coord specifies the y or x coordinate of the reflection axis relative or not relative to
        /// the rectangular center.
        /// </summary>
        public bool Deform(Axis axis, float factor, float coord = 0, bool isRelative = true);
    }
}