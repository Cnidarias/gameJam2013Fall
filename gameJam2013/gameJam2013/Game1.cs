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
using System.Diagnostics;

namespace gameJam2013
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        enum GameState
        {
            mainMenu,
            running,
            pause,
            credits,
            endGAME
        }
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Stopwatch stpWatch = new Stopwatch();
        List<String> tSL = new List<String>();

        GameState whatStateAreWeInScotty = GameState.mainMenu;

        LoadLevel ldl;
        character chara;
        spike spikeClass = new spike();
        bats batClass;
        SpriteFont font;
        List<corpse> corpseList = new List<corpse>();
        tokens tokenClass;
        spring springClass;

        int level = 1, deathCounter;


        int mainMenuIndex = 0, pauseMenuIndex = 0;
        Texture2D mainMenuTex, pauseMenuTex, pointerTex, creditsTex, pointerTwoTex, bg1, textureVikBig;

        float aniTimervikAni = 0;
        float aniIntervalvikAni = 100f;
        int aniCurFramevikAniX = 0;
        int aniCurFramevikAniY = 0;
        int aniSpriteWidthvikAni;
        int aniSpriteHeightvikAni;

        Rectangle aniSpriteRecvikAni;

        void setUpShitforFinalAni()
        {
            textureVikBig = Content.Load<Texture2D>("Graphics\\character\\Victory300");

            aniSpriteWidthvikAni = textureVikBig.Width / 5;
            aniSpriteHeightvikAni = textureVikBig.Height/6;
            aniSpriteRecvikAni = new Rectangle(aniCurFramevikAniX * aniSpriteWidthvikAni, aniCurFramevikAniY*aniSpriteHeightvikAni, aniSpriteWidthvikAni, aniSpriteHeightvikAni);
        }

        SoundEffect sdeath, sBGM, slevelComplete, smenuSelect, smenuAccept;
        SoundEffectInstance isBGM;

        bool death = false;

        KeyboardState keys, oldKeys;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        private void resetLevel(string level, bool resetbyR)
        {
            if (level == "26")
            {
                String s = Math.Round(stpWatch.Elapsed.TotalSeconds).ToString();
                tSL.Add(s); 
                whatStateAreWeInScotty = GameState.endGAME;
                setUpShitforFinalAni();
            }
            else
            {
                death = false;
                ldl = new LoadLevel(level, Content, spriteBatch);
                chara = new character(Content, spriteBatch, ldl.startPosition);
                chara.loadGroundBB(ldl.xTile, ldl.yTile, ldl.tileWidth, ldl.tileHeight, ldl.tileNumber);
                spikeClass = new spike();
                spikeClass.load(ldl.tileNumber, ldl.xTile, ldl.yTile, ldl.tileWidth, ldl.tileHeight);
                corpseList = new List<corpse>();
                batClass = new bats(level, Content);
                tokenClass = new tokens(level, Content);
                springClass = new spring(level, Content);
                if (level == "1" ||resetbyR)
                    stpWatch.Restart();
                else
                {
                    String s = Math.Round(stpWatch.Elapsed.TotalSeconds).ToString();
                    tSL.Add(s);
                    stpWatch.Restart();
                }
                whatStateAreWeInScotty = GameState.running;
            }
        }
        private void afterDeath()
        {
            death = false;
            chara.position = ldl.startPosition;
            chara.setStateToSpawning();
        }

        protected override void Initialize()
        {

            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            this.IsMouseVisible = false;
            
            base.Initialize();
        }

        protected override void LoadContent()
        {            
            spriteBatch = new SpriteBatch(GraphicsDevice);
            mainMenuTex = Content.Load<Texture2D>("Graphics\\menu\\mainMenu");
            pauseMenuTex = Content.Load<Texture2D>("Graphics\\menu\\pauseMenu");
            pointerTex = Content.Load<Texture2D>("Graphics\\menu\\pointer");
            creditsTex = Content.Load<Texture2D>("Graphics\\menu\\credits");
            pointerTwoTex = Content.Load<Texture2D>("Graphics\\menu\\cursor1");

            sdeath = Content.Load<SoundEffect>("sound\\death");
            slevelComplete = Content.Load<SoundEffect>("sound\\levelcomplete");
            sBGM = Content.Load<SoundEffect>("sound\\BGM");
            smenuAccept = Content.Load<SoundEffect>("sound\\menuaccept");
            smenuSelect = Content.Load<SoundEffect>("sound\\menuselect");
            bg1 = Content.Load<Texture2D>("Graphics\\bg\\bgone");

       

            isBGM = sBGM.CreateInstance();
            isBGM.IsLooped = true;
            ldl = new LoadLevel(level.ToString(), Content, spriteBatch);
            chara = new character(Content, spriteBatch, new Vector2(50, 50));
            chara.loadGroundBB(ldl.xTile, ldl.yTile, ldl.tileWidth, ldl.tileHeight, ldl.tileNumber);
            spikeClass.load(ldl.tileNumber, ldl.xTile, ldl.yTile, ldl.tileWidth, ldl.tileHeight);
            batClass = new bats(level.ToString(), Content);
            font = Content.Load<SpriteFont>("font1");
            tokenClass = new tokens(level.ToString(), Content);
            springClass = new spring(level.ToString(), Content);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            
            keys = Keyboard.GetState();
            if(keys.IsKeyDown(Keys.LeftAlt)&&keys.IsKeyDown(Keys.Enter))
            {
                if (graphics.IsFullScreen == true)
                    graphics.IsFullScreen = false;
                else
                    graphics.IsFullScreen = true;

                graphics.ApplyChanges();
            }
            if (isBGM.State != SoundState.Playing)
                isBGM.Play();
            this.Window.Title = deathCounter.ToString() + " Deaths at Level " + level.ToString();

            #region GameState running
            if (whatStateAreWeInScotty == GameState.running)
            {
                
                if (keys.IsKeyDown(Keys.R) && oldKeys.IsKeyUp(Keys.R))
                    resetLevel(level.ToString(), true);
                
                if (keys.IsKeyDown(Keys.U) && oldKeys.IsKeyUp(Keys.U))
                {
                    level++;
                    resetLevel(level.ToString(), false);
                }

                if (keys.IsKeyDown(Keys.Y) && oldKeys.IsKeyUp(Keys.Y))
                {
                    level--;
                    resetLevel(level.ToString(), false);
                }

                chara.Update(keys, oldKeys, gameTime);
                death = spikeClass.Update(chara.generalBB);
                if(!death)
                    death = batClass.batCollision(chara.generalBB, gameTime);

                if (death)
                {
                    deathCounter++;
                    corpse c = new corpse(chara.position, Content);
                    chara.addToGroundBB(c.addToGround());
                    corpseList.Add(c);
                    afterDeath();
                    sdeath.Play();
                }
                tokenClass.Update(chara.generalBB, gameTime);
                springClass.Update(chara);
                if (tokenClass.tokensCollected == tokenClass.numberOfTokens)
                {
                    level++;
                    slevelComplete.Play();
                    resetLevel(level.ToString(), false);
                }
                if (keys.IsKeyDown(Keys.P) && oldKeys.IsKeyUp(Keys.P)||(keys.IsKeyDown(Keys.Escape)&&oldKeys.IsKeyUp(Keys.Escape)))
                    whatStateAreWeInScotty = GameState.pause;
            }
            #endregion
            #region GameState mainMenu
            else if (whatStateAreWeInScotty == GameState.mainMenu)
            {
                if (keys.IsKeyDown(Keys.Up) && oldKeys.IsKeyUp(Keys.Up))
                {
                    smenuSelect.Play();
                    if (mainMenuIndex > 0)
                        mainMenuIndex--;
                    else
                        mainMenuIndex = 3;
                }
                if (keys.IsKeyDown(Keys.Down) && oldKeys.IsKeyUp(Keys.Down))
                {
                    smenuSelect.Play();
                    if (mainMenuIndex < 3)
                        mainMenuIndex++;
                    else
                        mainMenuIndex = 0;
                }
                if (keys.IsKeyDown(Keys.Enter) && oldKeys.IsKeyUp(Keys.Enter))
                {
                    smenuAccept.Play();
                    if(mainMenuIndex == 0)
                    {
                        level = 1;
                        resetLevel(level.ToString(), false);
                    }
                    else if (mainMenuIndex == 1)
                    {
                        tSL = new List<String>();
                        try
                        {
                            StreamReader sr = new StreamReader("C:\\imuseless\\please\\dont\\delete\\save.txt");
                            level = Convert.ToInt32(sr.ReadLine());
                            deathCounter = Convert.ToInt32(sr.ReadLine());
                            for (int i = 0; i < level - 1; i++)
                            {
                                String s = Convert.ToString(sr.ReadLine());
                                tSL.Add(s);
                            }
                            sr.Close();
                        }
                        catch
                        {
                            level = 1;
                            resetLevel(level.ToString(), false);
                        }

                        resetLevel(level.ToString(), false);
                    }
                    else if (mainMenuIndex == 2)
                        whatStateAreWeInScotty = GameState.credits;
                    else if (mainMenuIndex == 3)
                        this.Exit();
                    mainMenuIndex = 0;
                }
            }
            #endregion
            #region GameState pause
            else if (whatStateAreWeInScotty == GameState.pause)
            {
                if (keys.IsKeyDown(Keys.Escape) && oldKeys.IsKeyUp(Keys.Escape))
                    whatStateAreWeInScotty = GameState.running;
                if (keys.IsKeyDown(Keys.Up) && oldKeys.IsKeyUp(Keys.Up))
                {
                    if (pauseMenuIndex > 0)
                        pauseMenuIndex--;
                    else
                        pauseMenuIndex = 2;
                }
                if (keys.IsKeyDown(Keys.Down) && oldKeys.IsKeyUp(Keys.Down))
                {
                    if (pauseMenuIndex < 2)
                        pauseMenuIndex++;
                    else
                        pauseMenuIndex = 0;
                }
                if (keys.IsKeyDown(Keys.Enter)&&oldKeys.IsKeyUp(Keys.Enter))
                {
                    if (pauseMenuIndex == 0)
                        whatStateAreWeInScotty = GameState.running;
                    else if (pauseMenuIndex == 1)
                    {
                        try
                        {
                            StreamWriter sw = new StreamWriter("C:\\imuseless\\please\\dont\\delete\\save.txt");
                            sw.WriteLine(level);
                            sw.WriteLine(deathCounter);
                            for (int i = 0; i < level - 1; i++)
                                sw.WriteLine(tSL.ElementAt<String>(i));
                            sw.Close();
                        }
                        catch
                        {
                            Directory.CreateDirectory("C:\\imuseless\\please\\dont\\delete\\");
                            StreamWriter sw = new StreamWriter("C:\\imuseless\\please\\dont\\delete\\save.txt");
                            sw.WriteLine(level);
                            sw.WriteLine(deathCounter);
                            for (int i = 0; i < level - 1; i++)
                                sw.WriteLine(tSL.ElementAt<String>(i));
                            sw.Close();
                        }
                        whatStateAreWeInScotty = GameState.mainMenu;
                    }
                    else if (pauseMenuIndex == 2)
                    {
                        whatStateAreWeInScotty = GameState.mainMenu;
                    }
                    pauseMenuIndex = 0;
                }
                
            }
            #endregion
            #region GameState credits
            else if (whatStateAreWeInScotty == GameState.credits)
            {
                if (keys.IsKeyDown(Keys.Enter)&&oldKeys.IsKeyUp(Keys.Enter))
                    whatStateAreWeInScotty = GameState.mainMenu;
            }
            #endregion
            #region END GAME
            else if (whatStateAreWeInScotty == GameState.endGAME)
            {
                if (keys.IsKeyDown(Keys.G) && oldKeys.IsKeyUp(Keys.G))
                {
                    tSL = new List<string>();
                    whatStateAreWeInScotty = GameState.mainMenu;
                }


                aniTimervikAni += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (aniTimervikAni >= aniIntervalvikAni)
                {
                    aniTimervikAni = 0;
                    if (aniCurFramevikAniX > 3)
                    {
                        aniCurFramevikAniX = 0;
                        aniCurFramevikAniY++;
                        if (aniCurFramevikAniY > 5)
                            aniCurFramevikAniY = 0;
                    }
                    else
                    {
                        aniCurFramevikAniX++;
                    }
                }

                aniSpriteRecvikAni.X = aniCurFramevikAniX * aniSpriteWidthvikAni;
                aniSpriteRecvikAni.Y = aniCurFramevikAniY * aniSpriteHeightvikAni;



            }
            #endregion

            oldKeys = keys;                
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            MouseState m = Mouse.GetState();
            spriteBatch.Begin();
            if (whatStateAreWeInScotty == GameState.running || whatStateAreWeInScotty == GameState.pause)
            {
                spriteBatch.Draw(bg1, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), null,Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0f);
                ldl.drawTiles(spriteBatch);
                chara.drawCharacter();

                foreach (corpse c in corpseList)
                {
                    c.Draw(spriteBatch);
                }
                batClass.Draw(spriteBatch);
                tokenClass.Draw(spriteBatch);
                springClass.Draw(spriteBatch);
                spriteBatch.DrawString(font, tokenClass.tokensCollected.ToString() + "/" + tokenClass.numberOfTokens.ToString(), new Vector2(30, 30), Color.Red);
                spriteBatch.DrawString(font, Math.Round(stpWatch.Elapsed.TotalSeconds).ToString(), new Vector2(400 - font.MeasureString(stpWatch.Elapsed.Seconds.ToString()).X /2, 30), Color.Red);
                if (whatStateAreWeInScotty == GameState.pause)
                {
                    spriteBatch.Draw(pointerTwoTex, new Rectangle(290, 246 + (pauseMenuIndex * 35), 220, 20), Color.White);
                    spriteBatch.Draw(pauseMenuTex, new Rectangle(200, 80, 400, 320), Color.White);                    
                }
            }
            else if (whatStateAreWeInScotty == GameState.mainMenu)
            {
                spriteBatch.Draw(mainMenuTex, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                spriteBatch.Draw(pointerTex, new Rectangle(290, 333 + (mainMenuIndex * 28), 220, 20), Color.White);
            }
            else if (whatStateAreWeInScotty == GameState.credits)
            {
                spriteBatch.Draw(creditsTex, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
            }
            else if (whatStateAreWeInScotty == GameState.endGAME)
            {
                spriteBatch.Draw(bg1, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                spriteBatch.DrawString(font, "You died " + deathCounter.ToString() + " times. You really suck!", new Vector2(400 - font.MeasureString("You died " + deathCounter.ToString() + " times. You really suck!").X / 2, 25), Color.Red);
                for (int i = 0; i < 12; i++)
                {
                    spriteBatch.DrawString(font, "Level " + (i+1).ToString() + ": " + tSL.ElementAt<String>(i), new Vector2((200-(font.MeasureString("Level " + (i+1).ToString() + ": " + tSL.ElementAt<String>(i)).X)/2), 30+(i + 1) * 30), Color.Red);
                }
                for (int j = 12; j < tSL.Count-1; j++)
                {
                    spriteBatch.DrawString(font, "Level " + (j+1).ToString() + ": " + tSL.ElementAt<String>(j), new Vector2((600-(font.MeasureString("Level " + (j+1).ToString() + ": " + tSL.ElementAt<String>(j)).X)/2), 30+(j + 1 -12) * 30), Color.Red);
                }
                spriteBatch.DrawString(font, "Level 25: " + tSL.ElementAt<String>(24), new Vector2((400-(font.MeasureString("Level 25: " + tSL.ElementAt<String>(24)).X)/2), 420), Color.Red);

                spriteBatch.Draw(textureVikBig, new Rectangle(250, 100, aniSpriteWidthvikAni, aniSpriteHeightvikAni), aniSpriteRecvikAni, Color.White);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}