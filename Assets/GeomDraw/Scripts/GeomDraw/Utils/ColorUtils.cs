using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GeomDraw
{
    /// <summary>
    /// Static collection of functions related with colors
    /// </summary>
    public static class ColorUtils
    {
        public static Color ColorHEX(string hex)
        {
            Color color;
            if (!ColorUtility.TryParseHtmlString(hex, out color))
                Debug.LogError("HEX color fparsing failed");
            return color;
        }

        public static Color Gradient(float gradientValue, List<Color> colors)
        {
            if (colors.Count == 0)
            {
                Debug.LogError("Empty list of color in Gradient");
                return new Color(0, 0, 0, 0);
            }
            else if (colors.Count == 1)
                return colors[0];

            List<float> colorSpacing = new List<float>() { 0 };
            for (int i = 1; i < colors.Count; i++)
                colorSpacing.Add(i / (float)(colors.Count - 1));
            return Gradient(gradientValue, colors, colorSpacing);
        }

        public static Color Gradient(float gradientValue, List<Color> colors, List<float> colorSpacing)
        {
            if (colors.Count == 0)
            {
                Debug.LogError("Empty list of color in Gradient");
                return new Color(0, 0, 0, 0);
            }
            else if (colors.Count == 1)
                return colors[0];
            else if (colorSpacing.Count != colors.Count)
            {
                Debug.LogError("Invalide size of colorSpacing in Gradient");
                return new Color(0, 0, 0, 0);
            }

            for (int i=0; i<colorSpacing.Count - 1; i++)
            {
                if (gradientValue >= colorSpacing[i] && gradientValue <= colorSpacing[i + 1])
                {
                    float f = (gradientValue - colorSpacing[i]) / (colorSpacing[i + 1] - colorSpacing[i]);
                    return Color.Lerp(colors[i], colors[i + 1], f);
                }
            }

            Debug.LogError("gradientValue is not included in any interval defined by colorSpacing");
            return new Color(0, 0, 0, 0);
        }

        /// <summary>
        /// Normalized euclidean distance betwee the 3 color components
        /// </summary>
        public static float ColorDist(Color color1, Color color2)
        {
            return Mathf.Sqrt((color1.r - color2.r) * (color1.r - color2.r) +
                (color1.g - color2.g) * (color1.g - color2.g) +
                (color1.b - color2.b) * (color1.b - color2.b)) / Mathf.Sqrt(3);
        }
        
        /// <summary> It blends the foreground color on the backgorund one </summary>
        public static Color ColorBlend(Color fg, Color bg)
        {
            if (fg.a < 1e-5)
                return bg;

            float a = 1 - (1 - fg.a) * (1 - bg.a);
            if (a < 1e-5)
                return new Color(0, 0, 0, 0);

            float r = fg.r * fg.a / a + bg.r * bg.a * (1 - fg.a) / a;
            float g = fg.g * fg.a / a + bg.g * bg.a * (1 - fg.a) / a;
            float b = fg.b * fg.a / a + bg.b * bg.a * (1 - fg.a) / a;
            return new Color(r, g, b, a);
        }

        public static Color ColorErase(float fgAlpha, Color bg)
        {
            return new Color(bg.r, bg.g, bg.b, Mathf.Max(0, bg.a - fgAlpha));
        }

        public static Color SampleRandColorAround(Color centerColor, float radius)
        {
            float minRed = Mathf.Max(centerColor.r - radius, 0);
            float redRange = Mathf.Min(centerColor.r + radius, 1) - minRed;
            float newRed = Random.value * redRange + minRed;

            float minGreen = Mathf.Max(centerColor.g - radius, 0);
            float greenRange = Mathf.Min(centerColor.g + radius, 1) - minGreen;
            float newGreen = Random.value * greenRange + minGreen;

            float minBlue = Mathf.Max(centerColor.b - radius, 0);
            float blueRange = Mathf.Min(centerColor.b + radius, 1) - minBlue;
            float newBlue = Random.value * blueRange + minBlue;

            return new Color(newRed, newGreen, newBlue);
        }
    }
}