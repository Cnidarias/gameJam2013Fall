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
    class corpse
    {
        Vector2 position;
        Texture2D tex;
        public corpse(Vector2 pos, ContentManager content)
        {
            position = pos;
            tex = content.Load<Texture2D>("graphics\\character\\Corpse");
        }

        public BoundingBox addToGround()
        {
            BoundingBox bb = new BoundingBox(new Vector3(position.X, position.Y, 0), new Vector3(position.X+40, position.Y+20,0));

            return bb; 
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, new Rectangle((int)position.X, (int)position.Y, 40, 20), Color.White);
        }
    }
}
