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
    class tokens
    {
        List<BoundingBox> tokenBB = new List<BoundingBox>();
        public int numberOfTokens = 0;
        public int tokensCollected = 0;
        Texture2D tex, texDisappear;
        SoundEffect sCoin;

        float aniTimertoken = 0;
        float aniIntervaltoken = 260f;
        int aniCurFrametoken = 0;
        int aniSpriteWidthtoken;
        int aniSpriteHeighttoken;
        Rectangle aniSpriteRecttoken;

        float aniTimerTokenGet = 0;
        float aniIntervalTokenGet = 60f;
        int aniCurFrameTokenGet = 0;
        int aniSpriteWidthTokenGet;
        int aniSpriteHeightTokenGet;
        Rectangle aniSpriteRectTokenGet;

        bool deadToken = false;
        Vector2 v;


        public tokens(string level, ContentManager Content)
        {
            tex = Content.Load<Texture2D>("Graphics\\Tiles\\token");
            texDisappear = Content.Load<Texture2D>("Graphics\\Tiles\\tokenget");
            sCoin = Content.Load<SoundEffect>("sound\\coin");
            StreamReader sr = new StreamReader("Content\\maps\\tokens\\tokens" + level + ".txt");

            aniSpriteWidthtoken = tex.Width / 6;
            aniSpriteHeighttoken = tex.Height;
            aniSpriteRecttoken = new Rectangle(aniCurFrametoken * aniSpriteWidthtoken, 0, 20, 20);

            aniSpriteWidthTokenGet = tex.Width / 6;
            aniSpriteHeightTokenGet = tex.Height;
            aniSpriteRectTokenGet = new Rectangle(aniCurFrameTokenGet * aniSpriteWidthTokenGet, 0, 20, 20);

           

            numberOfTokens = Convert.ToInt32(sr.ReadLine());

            for (int i = 0; i < numberOfTokens; i++)
            {
                Vector2 v = new Vector2(Convert.ToInt32(sr.ReadLine()), Convert.ToInt32(sr.ReadLine()));
                BoundingBox bb = new BoundingBox(new Vector3(v.X, v.Y, 0), new Vector3(v.X + 20, v.Y + 20, 0));
                tokenBB.Add(bb);
            }
            sr.Close();
        }

        public void Update(BoundingBox characterBB, GameTime gt)
        {
            foreach (BoundingBox b in tokenBB)
            {
                if (b.Intersects(characterBB))
                {
                    sCoin.Play();
                    v.X = b.Min.X; 
                    v.Y = b.Min.Y;
                    deadToken = true;
                    tokenBB.Remove(b);
                    tokensCollected++;
                    break;
                }
            }
            aniTimertoken += (float)gt.ElapsedGameTime.TotalMilliseconds;
            if (aniTimertoken >= aniIntervaltoken)
            {
                aniTimertoken = 0;
                if (aniCurFrametoken >= 5)
                    aniCurFrametoken = 0;
                else
                    aniCurFrametoken++;
            }
            aniSpriteRecttoken.X = aniCurFrametoken * aniSpriteWidthtoken;
            if (deadToken)
            {
                aniSpriteRectTokenGet.X = aniCurFrameTokenGet * aniSpriteWidthTokenGet;

                aniTimerTokenGet += (float)gt.ElapsedGameTime.TotalMilliseconds;
                if (aniTimerTokenGet >= aniIntervalTokenGet)
                {
                    aniTimerTokenGet = 0;
                    if (aniCurFrameTokenGet >= 5)
                    {
                        aniCurFrameTokenGet = 0;
                        deadToken = false;
                    }
                    else
                        aniCurFrameTokenGet++;
                }

                aniSpriteRectTokenGet.X = aniCurFrameTokenGet * aniSpriteWidthTokenGet;
            }
        }
        //400,320

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (BoundingBox b in tokenBB)
                spriteBatch.Draw(tex, new Rectangle((int)b.Min.X, (int)b.Min.Y, aniSpriteWidthtoken, aniSpriteHeighttoken), aniSpriteRecttoken, Color.White);
            if(deadToken)
                spriteBatch.Draw(texDisappear, new Rectangle((int)v.X, (int)v.Y, aniSpriteWidthTokenGet, aniSpriteHeightTokenGet), aniSpriteRectTokenGet, Color.White);
        }


    }
}
