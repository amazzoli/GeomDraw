using System.Collections.Generic;
using UnityEngine;

namespace GeomDraw
{
    public enum Dir
    {
        north,
        south,
        east,
        west
    }


    public static class DirUtils
    {

        public static Dir DirFromAngle(float degAngle)
        {
            degAngle = (degAngle + 360) % 360;
            if (degAngle >= 45 && degAngle < 135)
                return Dir.west;
            else if (degAngle >= 135 && degAngle < 225)
                return Dir.south;
            else if (degAngle >= 225 && degAngle < 315)
                return Dir.east;
            else
                return Dir.north;
        }

        public static float AngleFromDir(Dir direction)
        {
            switch (direction)
            {
                case Dir.north:
                    return 0f;
                case Dir.south:
                    return 180f;
                case Dir.east:
                    return 270;
                default:
                    return 90;
            }
        }

        public static Dir DirFromPointVector(float[] vector)
        {
            if (vector[0] == 0)
            {
                if (vector[1] > 0)
                    return Dir.north;
                else if (vector[1] < 0)
                    return Dir.south;
                else
                {
                    Debug.LogError("Zero vector");
                    return Dir.north;
                }
            }

            float angleRad = Mathf.Atan(vector[1] / vector[0]);
            if (vector[0] > 0)
                return DirFromAngle(Mathf.Rad2Deg * angleRad + 270);
            else
                return DirFromAngle(Mathf.Rad2Deg * angleRad + 90);
        }

        public static int[] PointVectorFromDir(Dir dir)
        {
            int[] vector = new int[2];

            if (dir == Dir.north)
                vector[1] = 1;
            else if (dir == Dir.south)
                vector[1] = -1;
            else if (dir == Dir.east)
                vector[0] = 1;
            else if (dir == Dir.west)
                vector[0] = -1;

            return vector;
        }

        public static Vector3 VectorFromDir(Dir direction)
        {
            switch (direction)
            {
                case Dir.north:
                    return Vector3.up;
                case Dir.south:
                    return Vector3.down;
                case Dir.east:
                    return Vector3.right;
                default:
                    return Vector3.left;
            }
        }

        public static Dir OppositeDir(Dir direction)
        {
            switch (direction)
            {
                case Dir.north:
                    return Dir.south;
                case Dir.south:
                    return Dir.north;
                case Dir.east:
                    return Dir.west;
                default:
                    return Dir.east;
            }
        }

        public static Dir RightDir(Dir direction)
        {
            switch (direction)
            {
                case Dir.north:
                    return Dir.west;
                case Dir.south:
                    return Dir.east;
                case Dir.east:
                    return Dir.north;
                default:
                    return Dir.south;
            }
        }

        public static Dir LeftDir(Dir direction)
        {
            return OppositeDir(RightDir(direction));
        }

        /// <summary>
        /// Integer coordinates of the adjacent tile at "direction" of the "tilePosition"
        /// </summary>
        public static int[] CoordAtDir(Dir direction, Vector3 tilePosition)
        {
            Vector3 pointingVector = DirUtils.VectorFromDir(direction);
            Vector3 nextStreetCoord = tilePosition + pointingVector;
            return new int[2] { Mathf.RoundToInt(nextStreetCoord.x), Mathf.RoundToInt(nextStreetCoord.y) };
        }
    }
}
