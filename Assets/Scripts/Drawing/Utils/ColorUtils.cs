using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Drawing
{
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

        /// <summary>
        /// Normalized euclidean distance betwee the 3 color components
        /// </summary>
        public static float ColorDist(Color color1, Color color2)
        {
            return Mathf.Sqrt((color1.r - color2.r) * (color1.r - color2.r) +
                (color1.g - color2.g) * (color1.g - color2.g) +
                (color1.b - color2.b) * (color1.b - color2.b)) / Mathf.Sqrt(3);
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
    }
}