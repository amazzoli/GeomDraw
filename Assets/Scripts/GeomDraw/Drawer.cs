using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;


namespace GeomDraw
{
    public class Drawer
    {
        private SpriteRenderer spriteRenderer;
        private MyRenderer myRenderer;
        private TextureMerger myMerger;


        /// <summary> 
        /// Build the drawer over the sprite renderer
        /// </summary>
        public Drawer(SpriteRenderer spriteRenderer)
        {
            this.spriteRenderer = spriteRenderer;
        }


        // PUBLIC FUNCTIONS


        /// <summary>
        /// It substitutes the renderer sprite with an empty one. Width and height in world units 
        /// </summary>
        /// <param name="width">New sprite width in world units</param>
        /// <param name="height">New sprite width in world units</param>
        /// <param name="pixelsPerUnity">Number of pixels per one world unit</param>
        /// <param name="backgroundColor">Background color of the empty sprite</param>
        public void NewEmptySprite(
            float width,
            float height,
            float pixelsPerUnity,
            Color backgroundColor
        )
        {
            AddDrawnSprite();

            int wInt = Mathf.RoundToInt(width * pixelsPerUnity);
            int hInt = Mathf.RoundToInt(height * pixelsPerUnity);
            Texture2D tex = new Texture2D(wInt, hInt);
            int L = tex.width * tex.height;
            Color[] colors = new Color[L];
            for (int i = 0; i < L; i++)
                colors[i] = backgroundColor;

            tex.SetPixels(colors);
            tex.Apply(false);

            Rect rect = new Rect(0.0f, 0.0f, tex.width, tex.height);
            spriteRenderer.sprite = Sprite.Create(tex, rect, new Vector2(0.5f, 0.5f), pixelsPerUnity);
        }

        /// <summary>
        /// Draw a generic Drawable element of the SpriteRenderer
        /// </summary>
        /// <param name="drawable">Element to draw</param>
        /// <param name="updateDrawnSprite">Whether the DrawnSprite component of the new drawing has to be updated</param>
        public void Draw(IDrawable drawable, bool updateDrawnSprite = false)
        {
            Profiler.BeginSample("Drawability we");
            myRenderer = new MyRenderer(spriteRenderer, this);
            myMerger = new TextureMerger(spriteRenderer);

            if (spriteRenderer.sprite == null)
            {
                Debug.LogError("SpriteRenderer without a sprite");
                return;
            }

            if (!drawable.CheckDrawability(spriteRenderer.sprite.pixelsPerUnit))
            {
                Debug.LogError("Drawability check not passed");
                return;
            }
            Profiler.EndSample();
            Profiler.BeginSample("DrawnSprite we");
            if (updateDrawnSprite)
            {
                AddDrawnSprite();
                spriteRenderer.GetComponent<DrawnSprite>().NewDraw(drawable);
            }
            Profiler.EndSample();
            Profiler.BeginSample("Draw we");
            if (drawable is IDrawableLine)
                myRenderer.DrawLine((IDrawableLine)drawable);
            else if (drawable is IDrawableShape)
                myRenderer.DrawShape((IDrawableShape)drawable);
            else if (drawable is DrawableTexture)
                myMerger.DrawTexture((DrawableTexture)drawable);
            else
                Debug.LogError("Invalid IDrawable");
            Profiler.EndSample();
        }

        /// <summary>
        /// Use the Bucket tool that colors all the neighbour pixels of "point" until a
        /// "big" difference in color is found
        /// </summary>
        /// <param name="point">Point from which a new color spreads</param>
        /// <param name="color">New color to spread</param>
        /// <param name="sensitivity">Minimum difference between the old color at point and the 
        /// color in a neighbour pixel that stop the spread</param>
        /// <param name="updateDrawnSprite">Whether the DrawnSprite component of the new drawing has to be updated</param>
        public void Bucket(Vector2 point, Color color, float sensitivity, bool updateDrawnSprite = false)
        {
            Bucket bucket = new Bucket(spriteRenderer, point, color, sensitivity);
            if (updateDrawnSprite)
            {
                AddDrawnSprite();
                spriteRenderer.GetComponent<DrawnSprite>().NewDraw();
            }
            bucket.Run();
        }

        public void AddDrawnSprite(){
            if (spriteRenderer.GetComponent<DrawnSprite>() == null){
                DrawnSprite draw = spriteRenderer.gameObject.AddComponent<DrawnSprite>();
                draw.Init();
            }
        }
    }
}
