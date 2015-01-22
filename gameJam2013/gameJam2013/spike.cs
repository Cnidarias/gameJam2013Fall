using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace gameJam2013
{
    class spike
    {
        int[,] _tiles;
        List<BoundingBox> spikeBB = new List<BoundingBox>();

        public void load(int[,] tiles, int xTile, int yTile, int tileWidth, int tileHeight)
        {
            _tiles = tiles;

            for (int i = 0; i < xTile; i++)
            {
                for (int j = 0; j < yTile; j++)
                {
                    if (_tiles[i, j] == 2 || _tiles[i, j] == 3 || _tiles[i, j] == 4 || _tiles[i, j] == 5 || _tiles[i, j] == 6)
                    {
                        BoundingBox bb = new BoundingBox(new Vector3(i * tileWidth, j * tileHeight, 0), new Vector3((i + 1) * tileWidth, (j + 1) * tileHeight, 0));

                        spikeBB.Add(bb);
                    }
                }
            }
        }

        public bool Update(BoundingBox charaBB)
        {
            foreach (BoundingBox b in spikeBB)
            {
                if (b.Intersects(charaBB))
                    return true;
            }
            return false;
        }
    }
}
