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
    class character
    {

        public enum State
        {
            walking,
            jumping,
            falling,
            spawning
        }

        State currentState = State.walking;

        const int MOVE_LEFT = -1;
        const int MOVE_RIGHT = 1;
        const int MOV_SPEED = 120;
        readonly Vector2 gravity = new Vector2(0, 0.8f);

        float aniTimerWalk = 0;
        float aniIntervalWalk = 60f;
        int aniCurFrameWalk = 0;
        int aniSpriteWidthWalk;
        int aniSpriteHeightWalk;

        float aniTimerJump = 0;
        float aniIntervalJump = 60f;
        int aniCurFrameJump = 0;
        int aniSpriteWidthJump;
        int aniSpriteHeightJump;

        float aniTimerFall = 0;
        float aniIntervalFall = 60f;
        int aniCurFrameFall = 0;
        int aniSpriteWidthFall;
        int aniSpriteHeightFall;

        float aniTimerRespawn = 0;
        float aniIntervalRespawn = 100f;
        int aniCurFrameRespawn = 0;
        int aniSpriteWidthRespawn;
        int aniSpriteHeightRespawn;

        int lastDirection= MOVE_RIGHT;

        SoundEffect jump1, jump2;
        

        Rectangle aniSpriteRectWalk, aniSpriteRecJump, aniSpriteRecFall, aniSpriteRecRespawn;

        private Texture2D characterTextureRight, characterTextureJump, characterTextureFall, characterTextureRespawn;
        public Vector2 position, theDirection = Vector2.Zero, theSpeed = Vector2.Zero, velocity;
        BoundingBox _characterBB_TOP, _characterBB_LEFT, _characterBB_RIGHT, _characterBB_BOTTOM;
        public BoundingBox generalBB;
        List<BoundingBox> groundBB = new List<BoundingBox>();

        bool canMoveRight, canMoveLeft;
        SpriteBatch _spriteBatch;
        
        int jumpCounter = 0;
        int[,] _Tiles;
        int _tileWidth, _tileHeight, numberOfXTiles, numberOfYTiles;

        GameTime gt;

        public character(ContentManager Content, SpriteBatch spriteBatch, Vector2 Position)
        {
            characterTextureRight = Content.Load<Texture2D>("Graphics\\character\\walkRight");
            characterTextureJump = Content.Load<Texture2D>("Graphics\\character\\jumpSprite");
            characterTextureFall = Content.Load<Texture2D>("Graphics\\character\\landSprite");
            characterTextureRespawn = Content.Load<Texture2D>("Graphics\\character\\respawn");

            jump1 = Content.Load<SoundEffect>("sound\\jump");
            jump2 = Content.Load<SoundEffect>("sound\\jump2");
            currentState = State.spawning;

            _spriteBatch = spriteBatch;
            position = Position;

            aniSpriteWidthWalk = characterTextureRight.Width / 8;
            aniSpriteHeightWalk = characterTextureRight.Height;
            aniSpriteRectWalk = new Rectangle(aniCurFrameWalk * aniSpriteWidthWalk, 0, 20, 40);

            aniSpriteWidthJump = characterTextureJump.Width / 4;
            aniSpriteHeightJump = characterTextureJump.Height;
            aniSpriteRecJump = new Rectangle(aniCurFrameJump * aniSpriteWidthJump, 0, 20, 40);

            aniSpriteWidthFall = characterTextureFall.Width / 4;
            aniSpriteHeightFall = characterTextureFall.Height;
            aniSpriteRecFall = new Rectangle(aniCurFrameFall * aniSpriteWidthFall, 0, 20, 40);

            aniSpriteWidthRespawn = characterTextureRespawn.Width / 9;
            aniSpriteHeightRespawn = characterTextureRespawn.Height;
            aniSpriteRecRespawn = new Rectangle(aniCurFrameRespawn * aniSpriteWidthRespawn, 0, 20, 40);
        }

        public void loadGroundBB(int xTiles, int yTiles, int tileWidth, int tileHeight, int[,] tiles)
        {
            _Tiles = tiles;
            _tileHeight = tileHeight;
            _tileWidth = tileWidth;
            numberOfXTiles = xTiles;
            numberOfYTiles = yTiles;

            for (int i = 0; i < xTiles; i++)
            {
                for (int j = 0; j < yTiles; j++)
                {
                    if (tiles[i, j] == 1)
                    {
                        BoundingBox b = new BoundingBox(new Vector3(i * tileWidth, j * tileHeight, 0),
                            new Vector3((i + 1) * tileWidth, (j + 1) * tileHeight, 0));

                        groundBB.Add(b);
                    }

                }
            }
        }

        public void addToGroundBB(BoundingBox bb)
        {
            groundBB.Add(bb);
        }
        public void setStateToSpawning()
        {
            currentState = State.spawning;
            aniCurFrameRespawn = 0;
        }

        public void Update(KeyboardState keys, KeyboardState oldKeys, GameTime gameTime)
        {
            canMoveLeft = true;
            canMoveRight = true;
            gt = gameTime;
            #region Create Bounding Boxes at Character Position
            _characterBB_BOTTOM = new BoundingBox(new Vector3(position.X + 10, position.Y + 40 - 5, 0),
                new Vector3(position.X + 20 - 10, position.Y + 40 + 5, 0));

            _characterBB_TOP = new BoundingBox(new Vector3(position.X + 10, position.Y - 5, 0),
                new Vector3(position.X + 20 - 10, position.Y + 5, 0));

            _characterBB_LEFT = new BoundingBox(new Vector3(position.X, position.Y + 10, 0),
                new Vector3(position.X + 5, position.Y + 40 - 10, 0));

            _characterBB_RIGHT = new BoundingBox(new Vector3(position.X + 20 - 5, position.Y + 10, 0),
                new Vector3(position.X + 20, position.Y + 40 - 10, 0));

            generalBB = new BoundingBox(new Vector3(position.X+5, position.Y+5, 0), new Vector3(position.X + 15, position.Y + 40, 0));
            #endregion

            updateMovement(keys);
            
            velocity += gravity;

            if (isThereIntersectionWithGround(_characterBB_BOTTOM))
            {
                jumpCounter = 0;
                velocity = Vector2.Zero;
            }
            else { if (jumpCounter == 0)jumpCounter = 1; }
            if (keys.IsKeyDown(Keys.LeftShift) && oldKeys.IsKeyUp(Keys.LeftShift) && jumpCounter < 2)
            {
                jump();
            }
            if (isThereContainmentWithGroundNONPARTY(_characterBB_TOP))
            {
                position.Y += 1;
                velocity.Y = 0.0f;
            }
            if (currentState != State.spawning)
            {
                if (velocity.Y > 0)
                    currentState = State.falling;
                else if (velocity.Y < 0)
                    currentState = State.jumping;
                else
                    currentState = State.walking;
            }
            if (currentState == State.spawning)
            {
                aniTimerRespawn += (float)gt.ElapsedGameTime.TotalMilliseconds;
                if (aniTimerRespawn >= aniIntervalRespawn)
                {
                    aniTimerRespawn = 0;
                    aniCurFrameRespawn++;
                    if (aniCurFrameRespawn == 8)
                        currentState = State.walking;
                }

                aniSpriteRecRespawn.X = aniCurFrameRespawn * aniSpriteWidthRespawn;
            }

            #region animation counters
            if (keys.IsKeyDown(Keys.Left) || keys.IsKeyDown(Keys.Right))
            {
                if (currentState == State.walking)
                {
                    aniTimerWalk += (float)gt.ElapsedGameTime.TotalMilliseconds;
                    if (aniTimerWalk >= aniIntervalWalk)
                    {
                        aniTimerWalk = 0;
                        if (aniCurFrameWalk >= 7)
                            aniCurFrameWalk = 0;
                        else
                            aniCurFrameWalk++;
                    }

                    aniSpriteRectWalk.X = aniCurFrameWalk * aniSpriteWidthWalk;
                }
                if (currentState == State.jumping)
                {
                    aniTimerJump += (float)gt.ElapsedGameTime.TotalMilliseconds;
                    if (aniTimerJump >= aniIntervalJump)
                    {
                        aniTimerJump = 0;
                        if (aniCurFrameJump >= 3)
                            aniCurFrameJump = 0;
                        else
                            aniCurFrameJump++;
                    }

                    aniSpriteRecJump.X = aniCurFrameJump * aniSpriteWidthJump;
                }
                if (currentState == State.falling)
                {
                    aniTimerFall += (float)gt.ElapsedGameTime.TotalMilliseconds;
                    if (aniTimerFall >= aniIntervalFall)
                    {
                        aniTimerFall = 0;
                        if (aniCurFrameFall >= 3)
                            aniCurFrameFall = 0;
                        else
                            aniCurFrameFall++;
                    }

                    aniSpriteRecFall.X = aniCurFrameFall * aniSpriteWidthFall;
                }
            }
            #endregion
            if (currentState != State.spawning)
            {
                position += velocity;
                position += theDirection * theSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }


        }

        private void updateMovement(KeyboardState cKeys)
        {
            theSpeed = Vector2.Zero;
            theDirection = Vector2.Zero;
            canMoveLeft = true;
            canMoveRight = true;

            if (cKeys.IsKeyDown(Keys.Right))
            {
                foreach (BoundingBox b in groundBB)
                {
                    if (b.Intersects(_characterBB_RIGHT)||(position.X-20)>=800)
                        canMoveRight = false;
                }
                if (canMoveRight)
                {
                    theSpeed.X = MOV_SPEED;
                    theDirection.X = MOVE_RIGHT;
                }
                lastDirection = MOVE_RIGHT;
            }
            else if (cKeys.IsKeyDown(Keys.Left))
            {
                foreach (BoundingBox b in groundBB)
                {
                    if (b.Intersects(_characterBB_LEFT)||position.X<=0)
                        canMoveLeft = false;
                }
                if (canMoveLeft)
                {
                    theSpeed.X = MOV_SPEED;
                    theDirection.X = MOVE_LEFT;
                }
                lastDirection = MOVE_LEFT;
            }
        }

        private void jump()
        {
            if (jumpCounter == 0)
                jump1.Play();
            else
                jump2.Play();
            jumpCounter++;
            velocity.Y = -10;
        }
        public void springJump()
        {
            velocity.Y = -19;
            //jumpCounter++;
        }

        private bool isThereIntersectionWithGround(BoundingBox personBB)
        {
            foreach (BoundingBox b in groundBB)
            {
                if (b.Intersects(personBB))
                {
                    position.Y = b.Min.Y - 2 * _tileHeight;
                    return true;
                }
            }
            return false;
        }

        private bool isThereContainmentWithGroundNONPARTY(BoundingBox personBB)
        {
            foreach (BoundingBox b in groundBB)
            {
                if (b.Contains(personBB) == ContainmentType.Intersects)
                    return true;
            }
            return false;
        }

        public void drawCharacter()
        {
            if (currentState == State.spawning)
            {
                _spriteBatch.Draw(characterTextureRespawn, new Rectangle((int)position.X, (int)position.Y, aniSpriteWidthRespawn, aniSpriteHeightRespawn), aniSpriteRecRespawn, Color.White);
            }
            if (lastDirection == MOVE_RIGHT)
            {
                if(currentState == State.walking)
                    _spriteBatch.Draw(characterTextureRight, new Rectangle((int)position.X, (int)position.Y, aniSpriteWidthWalk, aniSpriteHeightWalk), aniSpriteRectWalk, Color.White);
                else if(currentState == State.jumping)
                    _spriteBatch.Draw(characterTextureJump, new Rectangle((int)position.X, (int)position.Y, aniSpriteWidthJump, aniSpriteHeightJump), aniSpriteRecJump, Color.White);
                else if(currentState == State.falling)
                    _spriteBatch.Draw(characterTextureFall, new Rectangle((int)position.X, (int)position.Y, aniSpriteWidthFall, aniSpriteHeightFall), aniSpriteRecFall, Color.White);
            }
            else if (lastDirection == MOVE_LEFT)
            {
                if(currentState==State.walking)
                    _spriteBatch.Draw(characterTextureRight, new Rectangle((int)position.X, (int)position.Y, aniSpriteWidthWalk, aniSpriteHeightWalk), aniSpriteRectWalk, Color.White, 0.0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0f);
                else if(currentState==State.jumping)
                    _spriteBatch.Draw(characterTextureJump, new Rectangle((int)position.X, (int)position.Y, aniSpriteWidthJump, aniSpriteHeightJump), aniSpriteRecJump, Color.White, 0.0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0f);
                else if(currentState == State.falling)
                    _spriteBatch.Draw(characterTextureFall, new Rectangle((int)position.X, (int)position.Y, aniSpriteWidthFall, aniSpriteHeightFall), aniSpriteRecFall, Color.White, 0.0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0f);
            }     

        }

    }
}
