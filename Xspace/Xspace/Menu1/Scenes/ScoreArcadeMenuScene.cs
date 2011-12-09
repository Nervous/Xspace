using System;
using MenuSample.Scenes.Core;
using System.IO;
using MenuSample.Inputs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.Linq;

namespace MenuSample.Scenes
{
    public class ScoreArcadeMenuScene : AbstractMenuScene
    {
        private ContentManager _content;
        private SpriteFont _gamefont;
        private Texture2D _score_board, _score_surbrillance, _score_lvl;
        private StreamReader sr_arcade, sr_level;
        private string path_arcade, path_level;
        private Vector2 position_Nv;
        private Vector2 position_board; // default pos
        public string[] score_arcade, score_level;
        private static KeyboardState _keyboardState;
        private static KeyboardState _lastKeyboardState;
        private int i;
        private bool level_selected;

        /* Be careful, level ID begins at 0. (level 1 has ID 0, for score / i / lvl) */
        /*
         * TODO:
         * Complete the lvlx positions, back button, lvl name on leaderboard, more rectangles, and so much more... */

        public ScoreArcadeMenuScene(SceneManager sceneMgr)
            : base(sceneMgr, "Score Arcade")
        {
            path_arcade = "Scores\\Arcade\\Arcade.score";
            _keyboardState = new KeyboardState();
            _lastKeyboardState = new KeyboardState();
            i = 0;       
            var back = new MenuItem("Retour");
            var Nv1 = new MenuItem("Nv.1");
            sr_arcade = new StreamReader(path_arcade);
     
        }

        public override void Initialize()
        {
            base.Initialize();
            level_selected = false;
        }
        public override void Draw(GameTime gameTime)
        {
            score_arcade = System.IO.File.ReadAllLines(@path_arcade);



          if(_content == null)
          _content = new ContentManager(SceneManager.Game.Services, "Content");
          _gamefont = _content.Load<SpriteFont>("Fonts\\Jeu\\Jeu");
          _score_board = _content.Load<Texture2D>("Sprites\\Menu\\Score\\score-proto");
          _score_surbrillance = _content.Load<Texture2D>("Sprites\\Menu\\Score\\score-selected");
          _score_lvl = _content.Load<Texture2D>("Sprites\\Menu\\Score\\score-arcade-lvl");

            SpriteBatch spriteBatch = SceneManager.SpriteBatch;
            spriteBatch.Begin();
            
                spriteBatch.Draw(_score_board, position_board, Color.White);
                spriteBatch.Draw(_score_surbrillance, position_Nv, Color.White);


                for (int lvl = 0; lvl < 2; lvl++)
                {
                    spriteBatch.DrawString(_gamefont, "Nv." + (lvl + 1), new Vector2(130 + 195 * (lvl / 5), (190 + (lvl % 5) * 47)), Color.Red, 0, new Vector2(0, 0), 0.8f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(_gamefont, score_arcade[lvl], new Vector2(220 + 200 * (lvl / 5), (190 + (lvl % 5) * 47)), Color.Red, 0, new Vector2(0, 0), 0.8f, SpriteEffects.None, 0);
                }
               

                if (level_selected)
                {
                    sr_level = new StreamReader(path_level);
                    score_level = System.IO.File.ReadAllLines(@path_level);

                    for (int pos = 0; pos < 1; pos++)
                    {
                        spriteBatch.Draw(_score_lvl, position_board, Color.White);
                        spriteBatch.DrawString(_gamefont, score_level[pos], new Vector2(452,187), Color.Red, 0, new Vector2(0,0), 0.8f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(_gamefont, score_level[pos + 1], new Vector2(605, 190), Color.Red, 0, new Vector2(0, 0), 0.8f, SpriteEffects.None, 0);

                    }

                }
                
            spriteBatch.End();
        }

        
        public override void Update(GameTime gameTime)
        {
            path_level = "Scores\\Arcade\\lvl" + (i + 1) + ".score";

            position_Nv.X = 300 * (i / 5) + 112;
            position_Nv.Y = 187 + i * 45;

            _keyboardState = Keyboard.GetState();
            

            if (!level_selected)
            {
                if ((_keyboardState.IsKeyDown(Keys.Up)) && (_lastKeyboardState.IsKeyUp(Keys.Up)))
                {
                    if (i > 0)
                    {
                        i--;
                    }

                }
                else if ((_keyboardState.IsKeyDown(Keys.Down)) && (_lastKeyboardState.IsKeyUp(Keys.Down)))
                {
                    if (i < 2)
                    {
                        i++;
                    }

                }
                else if ((_keyboardState.IsKeyDown(Keys.Right)) && (_lastKeyboardState.IsKeyUp(Keys.Right)))
                {
                    if (i < 5)
                    {
                       // i += 5;  Unused until level 6
                    }
                }
                else if ((_keyboardState.IsKeyDown(Keys.Left)) && (_lastKeyboardState.IsKeyUp(Keys.Left)))
                {
                    if (i > 5)
                    {
                       // i -= 5; Unused until level 6
                    }
                }



                if (((_keyboardState.IsKeyDown(Keys.Space)) && (_lastKeyboardState.IsKeyUp(Keys.Space))) || (((_keyboardState.IsKeyDown(Keys.Enter))  && (_lastKeyboardState.IsKeyUp(Keys.Enter)))))
                    level_selected = true;

                

            }
            else
            {
                if (((_keyboardState.IsKeyDown(Keys.Space))  && (_lastKeyboardState.IsKeyUp(Keys.Space)))||(((_keyboardState.IsKeyDown(Keys.Enter))  && (_lastKeyboardState.IsKeyUp(Keys.Enter)))))
                    level_selected = false;
            }

            _lastKeyboardState = _keyboardState;
            base.Update(gameTime);

        }
        

        

    }
}