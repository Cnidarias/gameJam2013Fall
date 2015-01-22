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
    struct batStruct
    {
        public BoundingBox batBB;
        public Vector2 position;
        public Vector2 endPoints;
        public int speed;
        public int direction;
    }
    class bats
    {
        int bpl;
        batStruct[] batStructArray;
        List<BoundingBox> bbList = new List<BoundingBox>();
        Texture2D tex;

        float aniTimerBat = 0;
        float aniIntervalBat = 100f;
        int aniCurFrameBat = 0;
        int aniSpriteWidthBat;
        int aniSpriteHeightBat;
        Rectangle aniSpriteRectBat;

        public bats(string level, ContentManager Content)
        {
            tex = Content.Load<Texture2D>("Graphics\\enemy\\bat");
            aniSpriteWidthBat = tex.Width / 8;
            aniSpriteHeightBat = tex.Height;
            aniSpriteRectBat = new Rectangle(aniCurFrameBat * aniSpriteWidthBat, 0, 20, 20);
            try
            {
                StreamReader sr = new StreamReader("Content\\maps\\bats\\bats" + level + ".txt");

                bpl = Convert.ToInt32(sr.ReadLine());
                batStructArray = new batStruct[bpl];
                for (int i = 0; i < bpl; i++)
                {
                    batStructArray[i].position.X = (float)Convert.ToDouble(sr.ReadLine());
                    batStructArray[i].position.Y = (float)Convert.ToDouble(sr.ReadLine());

                    batStructArray[i].endPoints.X = (float)Convert.ToDouble(sr.ReadLine());
                    batStructArray[i].endPoints.Y = (float)Convert.ToDouble(sr.ReadLine());

                    batStructArray[i].speed = Convert.ToInt32(sr.ReadLine());

                    batStructArray[i].direction = Convert.ToInt32(sr.ReadLine());
                }
                sr.Close();
            }
            catch { }
        }

        public bool batCollision(BoundingBox playerBB, GameTime gt)
        {
            bbList = new List<BoundingBox>();

            for (int i = 0; i < bpl; i++)
            {
                if (batStructArray[i].position.X >= batStructArray[i].endPoints.Y)
                    batStructArray[i].direction = -1;
                if (batStructArray[i].position.X <= batStructArray[i].endPoints.X)
                    batStructArray[i].direction = 1;

                batStructArray[i].position.X += (batStructArray[i].speed * batStructArray[i].direction);
                BoundingBox b = new BoundingBox(new Vector3(batStructArray[i].position.X, batStructArray[i].position.Y, 0), new Vector3(batStructArray[i].position.X + 20, batStructArray[i].position.Y + 20, 0));
                bbList.Add(b);
            }
            foreach (BoundingBox b in bbList)
            {
                if (b.Intersects(playerBB))
                    return true;
            }
            aniTimerBat += (float)gt.ElapsedGameTime.TotalMilliseconds;
            if (aniTimerBat >= aniIntervalBat)
            {
                aniTimerBat = 0;
                if (aniCurFrameBat >= 7)
                    aniCurFrameBat = 0;
                else
                    aniCurFrameBat++;
            }

            aniSpriteRectBat.X = aniCurFrameBat * aniSpriteWidthBat;
            return false;
        }
     
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < bpl; i++)
            {
               spriteBatch.Draw(tex, new Rectangle((int)batStructArray[i].position.X, (int)batStructArray[i].position.Y, aniSpriteWidthBat, aniSpriteHeightBat), aniSpriteRectBat, Color.White);
            }

        }
    }
}
 