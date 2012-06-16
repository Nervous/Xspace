﻿using System;
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
        private Texture2D _score_board, _score_surbrillance, _score_lvl, _score_surbrillance2;
        private StreamReader sr_arcade;
        private string path_arcade, path_level, path_arcade_level, path_arcade_level_best;
        private Vector2 position_Nv;
        private Vector2 position_board;
        public string[] score_arcade, score_level, score_arcade_level, score_arcade_level_best;
        private static KeyboardState _keyboardState;
        private static KeyboardState _lastKeyboardState;
        private int i;
        private bool level_selected, backSelected, firstTime;

        /* Be careful, level ID begins at 0. (level 1 has ID 0, for score / i / lvl) */
        /*
         * TODO:
         * Nothin???!!!*/

        public ScoreArcadeMenuScene(SceneManager sceneMgr)
            : base(sceneMgr, "Score Arcade")
        {
            path_arcade = "Scores\\Arcade\\Arcade.score";
            path_arcade_level = "Scores\\Arcade\\lvl" + (i + 1) + ".score";
            FileStream fs1 = new FileStream(@path_arcade, FileMode.OpenOrCreate);
            fs1.Close();
            FileStream fs2 = new FileStream(@path_arcade_level, FileMode.OpenOrCreate);
            fs2.Close();
            _keyboardState = new KeyboardState();
            _lastKeyboardState = new KeyboardState();
            i = 0;
            var back = new MenuItem("Retour");
            var Nv1 = new MenuItem("Nv.1");
            sr_arcade = new StreamReader(path_arcade);
            position_board = new Vector2();
            MenuItems.Add(back);

        }

        public override void Initialize()
        {
            base.Initialize();
            level_selected = false;
            backSelected = false;
            firstTime = true;
        }
        public override void Draw(GameTime gameTime)
        {
            score_arcade = System.IO.File.ReadAllLines(@path_arcade);
            score_arcade_level = System.IO.File.ReadAllLines(@path_arcade_level);

            if (_content == null)
                _content = new ContentManager(SceneManager.Game.Services, "Content");
            _gamefont = _content.Load<SpriteFont>("Fonts\\Jeu\\Jeu");
            _score_board = _content.Load<Texture2D>("Sprites\\Menu\\Score\\score-proto");        
                _score_surbrillance2 = _content.Load<Texture2D>("Sprites\\Menu\\Score\\score-selected-back");
                _score_surbrillance = _content.Load<Texture2D>("Sprites\\Menu\\Score\\score-selected");
            _score_surbrillance = _content.Load<Texture2D>("Sprites\\Menu\\Score\\score-selected");
            _score_lvl = _content.Load<Texture2D>("Sprites\\Menu\\Score\\score-arcade-lvl");

            SpriteBatch spriteBatch = SceneManager.SpriteBatch;
            spriteBatch.Begin();

            spriteBatch.Draw(_score_board, position_board, Color.White);
            if (backSelected)
                spriteBatch.Draw(_score_surbrillance2, position_Nv, Color.White);
            else
                spriteBatch.Draw(_score_surbrillance, position_Nv, Color.White);
            
            for (int lvl = 0; lvl < 15; lvl++) 
            {
                path_arcade_level_best = "Scores\\Arcade\\lvl" + (lvl+1) + ".score";
               // sr_arcade_level = new StreamReader(path_arcade_level_best);
                FileStream fs3 = new FileStream(@path_arcade_level_best, FileMode.OpenOrCreate);
                fs3.Close();
                score_arcade_level_best = System.IO.File.ReadAllLines(@path_arcade_level_best);
                //sr_arcade_level.Close();
                if (score_arcade_level_best.Length == 0)
                {
                    FileStream fs = new FileStream(path_arcade_level_best, FileMode.Open);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine("-");
                    sw.Write("0");
                    sw.Close();
                    fs.Close();
                }
                StreamReader sr_arcade_level_back = new StreamReader(path_arcade_level_best);
                string[] score_arcade_level_best_back = System.IO.File.ReadAllLines(@path_arcade_level_best);
                sr_arcade_level_back.Close();
                spriteBatch.DrawString(_gamefont, "Nv." + (lvl + 1), new Vector2(130 + 356 * (lvl / 5), (240 + (lvl % 5) * 55)), Color.LightGreen, 0, new Vector2(0, 0), 0.7f, SpriteEffects.None, 0);
                spriteBatch.DrawString(_gamefont, score_arcade_level_best_back[1], new Vector2(220 + 356 * (lvl / 5), (240 + (lvl % 5) * 55)), Color.LightGreen, 0, new Vector2(0, 0), 0.7f, SpriteEffects.None, 0); // TODO
            }
            
            if ((level_selected)&&(!backSelected))
            {
                spriteBatch.DrawString(_gamefont, score_arcade_level[0], new Vector2(220 + 373 * ((i) / 5), (240 + ((i) % 5) * 47)), Color.LightGreen, 0, new Vector2(0, 0), 0.7f, SpriteEffects.None, 0);

                score_level = System.IO.File.ReadAllLines(@path_level);
               
                if (score_level.Length < 10)
                {
                    
                    FileStream fs = new FileStream(@path_level, FileMode.Open);
                    StreamReader sr = new StreamReader(fs);
                    sr.ReadToEnd();
                    StreamWriter sw = new StreamWriter(fs);
                    sw.Write("\n");
                    for (int k = score_level.Length; k < 10; k++)
                    {
                        if (k % 2 == 0)
                            sw.WriteLine("-");
                        else
                            sw.WriteLine("0");
                    }
                    sw.Close();
                    sr.Close();
                    fs.Close();
                }
                spriteBatch.Draw(_score_lvl, position_board, Color.White);
                position_Nv.X = 250;
                position_Nv.Y = 253;
                if (score_level.Length < 10)
                {
                    for (int pos = 0; pos < score_level.Length; pos++) // score for each levels (5)
                        spriteBatch.DrawString(_gamefont, score_level[pos], new Vector2(452 + 151 * ((pos) % 2), 240 + (pos / 2) * (55)), Color.LightGreen, 0, new Vector2(0, 0), 0.7f, SpriteEffects.None, 0);
                }
                else
                    for (int pos = 0; pos < 10; pos++) // score for each levels (5)
                        spriteBatch.DrawString(_gamefont, score_level[pos], new Vector2(452 + 151 * ((pos) % 2), 240 + (pos / 2) * (55)), Color.LightGreen, 0, new Vector2(0, 0), 0.7f, SpriteEffects.None, 0);
            } 
            spriteBatch.End();
            sr_arcade.Close();
        }

        public override void Update(GameTime gameTime)
        {
            path_level = "Scores\\Arcade\\lvl" + (i + 1) + ".score";

                position_Nv.X = 359 * (i / 5) + 109;
                position_Nv.Y = 230 + (i % 5) * 56;
            
            _keyboardState = Keyboard.GetState();

            if (!level_selected)
            {
                if ((_keyboardState.IsKeyDown(Keys.Up)) && (_lastKeyboardState.IsKeyUp(Keys.Up)))
                {
                    if (i > 0)
                        if (i == 15)
                            i -= 6;
                        else
                            i--;
                }
                else if ((_keyboardState.IsKeyDown(Keys.Down)) && (_lastKeyboardState.IsKeyUp(Keys.Down)))
                {
                    if (i == 4 || i == 9 || i == 14)
                        i = 15;
                    else if (i < 14)
                        i++;
                }
                else if ((_keyboardState.IsKeyDown(Keys.Right)) && (_lastKeyboardState.IsKeyUp(Keys.Right)))
                {
                    if (i < 10) 
                         i += 5; 
                }
                else if ((_keyboardState.IsKeyDown(Keys.Left)) && (_lastKeyboardState.IsKeyUp(Keys.Left)))
                {
                    if (i >= 5 && i != 15)
                        i -= 5; 
                }

                if (i == 15)
                {
                    position_Nv.X = 464;
                    position_Nv.Y = 590;
                    backSelected = true;
                }
                else backSelected = false;


                if (_keyboardState.IsKeyDown(Keys.Enter) && _lastKeyboardState.IsKeyUp(Keys.Enter))
                {
                    if (firstTime)
                    {
                        level_selected = false;
                        firstTime = false;
                    }
                    else level_selected = true; 
                }
                

            }
            else
            {
                if ((_keyboardState.IsKeyDown(Keys.Enter)) && (_lastKeyboardState.IsKeyUp(Keys.Enter)))
                    level_selected = false;
            }

            if ((backSelected) && (_keyboardState.IsKeyDown(Keys.Enter))) // removing the scene
                Remove();
            
            _lastKeyboardState = _keyboardState;
            base.Update(gameTime);
        }

    }
}