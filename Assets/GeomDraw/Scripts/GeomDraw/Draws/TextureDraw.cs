using System.Collections.Generic;
using GeomDraw;
using UnityEngine;

namespace GeomDraw
{
    public class TextureDraw : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            Drawer drawer = new Drawer(spriteRenderer);
            float wCanvas = 12, hCanvas = 8;
            drawer.NewEmptySprite(wCanvas, hCanvas, 100, ColorUtils.ColorHEX("#3f9b0b"));

            // Importing the trees from the Resources folder and converting into Drawables
            Sprite tree1 = Resources.Load<Sprite>("tree1");
            Sprite tree2 = Resources.Load<Sprite>("tree2");
            DrawableTexture dTree1 = new DrawableTexture(tree1, Vector2.zero);
            dTree1.Deform(Axis.x, 0.15f, 0, false);
            dTree1.Deform(Axis.y, 0.15f, 0, false);
            DrawableTexture dTree2 = new DrawableTexture(tree2, Vector2.zero);
            dTree2.Deform(Axis.x, 0.15f, 0, false);
            dTree2.Deform(Axis.y, 0.15f, 0, false);

            // Set the seed for reproducibility
            Random.InitState(1);

            int nTrees = 50;

            // Simple random spawining algorithm (almost) without overlap
            // A grid is built and each cell is stored in the availableSpots
            int nXSpots = Mathf.CeilToInt(wCanvas / dTree1.Size.x);
            int nYSpots = Mathf.CeilToInt(hCanvas / dTree1.Size.y);
            List<int> availableSpots = new List<int>();
            for (int i = 0; i < nXSpots * nYSpots; i++) availableSpots.Add(i);
            // Random choosing the cells in the grid where the tree will be drawn
            List<int> spawnedSpots = new List<int>();
            for (int i = 0; i < nTrees; i++)
            {
                // Exctrating the cell and its coordinates
                int iSpot = availableSpots[Random.Range(0, availableSpots.Count - 1)];
                spawnedSpots.Add(iSpot);
                // Removing the cell from the availabe spots
                availableSpots.Remove(iSpot);
            }

            // Sorting the cells in a way that the ones in the background 
            // are not spawned in front
            spawnedSpots.Sort();

            float spotRandFract = 0.5f;
            float xRand = spotRandFract * dTree1.Size.x, yRand = spotRandFract * dTree1.Size.y;
            for (int i = nTrees - 1; i >= 0; i--)
            {

                // Random choice between the two trees
                float p = Random.Range(0.0f, 1.0f);
                DrawableTexture tree;
                if (p > 0.5f) tree = dTree1;
                else tree = dTree2;

                int iSpot = spawnedSpots[i];
                float xSpot = iSpot % nXSpots * dTree1.Size.x;
                float ySpot = (iSpot - xSpot) / (float)nXSpots * dTree1.Size.y;
                // Add a small random shift to the cell poistion
                float x = xSpot + Random.Range(-xRand, xRand) * 0.5f;
                float y = ySpot + Random.Range(-yRand, yRand) * 0.5f;
                tree.Translate(new Vector2(x, y) - tree.Origin);
                drawer.Draw(tree);
            }

            drawer.SavePng("Forest");
        }
    }
}
