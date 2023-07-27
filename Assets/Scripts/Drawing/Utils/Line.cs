using UnityEngine;


namespace Drawing
{
    public class Line
    {
        /// <summary> Line passing from two points </summary>
        public Line(Vector2 p1, Vector2 p2)
        {
            if (p2.x - p1.x != 0)
            {
                AngCoef = (p2.y - p1.y) / (p2.x - p1.x);
                Intercept = p1.y - AngCoef * p1.x;
                yParallel = false;
                if (p2.y - p1.y != 0)
                {
                    AngCoefInverse = 1 / AngCoef;
                    InterceptInverse = -Intercept / AngCoef;
                    xParallel = false;
                }
                else
                    xParallel = true;
            }
            else
            {
                yParallel = true;
                AngCoefInverse = 0;
                InterceptInverse = p1.x;
            }
        }

        public float AngCoef { get; private set; }
        public float AngCoefInverse { get; private set; }
        public float Intercept { get; private set; }
        public float InterceptInverse { get; private set; }
        public bool xParallel { get; private set; }
        public bool yParallel { get; private set; }



        /// <summary> Position y along the line at given x </summary>
        public float Y(float x)
        {
            if (!yParallel)
                return AngCoef * x + Intercept;
            else
            {
                Debug.LogError("Asking y to a line parallel to y-axis");
                return 0;
            }
        }

        /// <summary> Position x along the line at given y </summary>
        public float X(float y)
        {
            if (!xParallel)
                return AngCoefInverse * y + InterceptInverse;
            else
            {
                Debug.LogError("Asking x to a line parallel to x-axis");
                return 0;
            }
        }
    }
}