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
    class spring
    {
        List<BoundingBox> springBB = new List<BoundingBox>();
        int numberOfSprings = 0;
        Texture2D tex;

        public spring(string level, ContentManager Content)
        {
            tex = Content.Load<Texture2D>("Graphics\\Tiles\\spring");
            try
            {
                StreamReader sr = new StreamReader("Content\\maps\\springs\\springs" + level + ".txt");
                numberOfSprings = Convert.ToInt32(sr.ReadLine());

                for (int i = 0; i < numberOfSprings; i++)
                {
                    Vector2 v = new Vector2(Convert.ToInt32(sr.ReadLine()), Convert.ToInt32(sr.ReadLine()));
                    BoundingBox b = new BoundingBox(new Vector3(v.X, v.Y, 0), new Vector3(v.X + 20, v.Y + 20, 0));
                    springBB.Add(b);
                }
                sr.Close();
            }
            catch { }
        }

        public void Update(character c)
        {
            foreach (BoundingBox b in springBB)
            {
                if (b.Intersects(c.generalBB))
                    c.springJump();
            }//60down
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (BoundingBox b in springBB)
            {
                spriteBatch.Draw(tex, new Rectangle((int)b.Min.X, (int)b.Min.Y, 20,20), Color.White);
            }
        }

    }
}
