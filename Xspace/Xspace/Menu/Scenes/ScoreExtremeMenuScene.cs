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
    public class ScoreExtremeMenuScene : AbstractMenuScene
    {
        private ContentManager _content;
        private SpriteFont _gamefont;
        private Texture2D _score_board, _score_surbrillance, _score_lvl, _score_surbrillance2;
        private StreamReader sr_arcade, sr_arcade_level; //sr_level, 
        private string path_arcade, path_level, path_extreme_level, path_extreme_level_best;
        private Vector2 position_Nv;
        private Vector2 position_board;
        public string[] score_extreme, score_level, score_extreme_level, score_extreme_level_best;
        private static KeyboardState _keyboardState;
        private static KeyboardState _lastKeyboardState;
        private int i;
        private bool level_selected, backSelected, firstTime;

        /* Be careful, level ID begins at 0. (level 1 has ID 0, for score / i / lvl) */
        /*
         * TODO:
         * Nothin???!!!*/

        public ScoreExtremeMenuScene(SceneManager sceneMgr)
            : base(sceneMgr, "Score Arcade")
        {
            path_extreme_level = "Scores\\Extreme\\lvl.score";
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
            level_selected = true;
            backSelected = false;
            firstTime = true;
        }
        public override void Draw(GameTime gameTime)
        {
            score_extreme_level = System.IO.File.ReadAllLines(@path_extreme_level);

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

            spriteBatch.Draw(_score_lvl, position_board, Color.White);
            position_Nv.X = 250;
            position_Nv.Y = 253;

                spriteBatch.DrawString(_gamefont, score_extreme_level[0], new Vector2(220 + 373 * ((i) / 5), (240 + ((i) % 5) * 47)), Color.LightGreen, 0, new Vector2(0, 0), 0.7f, SpriteEffects.None, 0);

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

                if (score_level.Length < 10)
                {
                    for (int pos = 0; pos < score_level.Length; pos++) // score for each levels (5)
                        spriteBatch.DrawString(_gamefont, score_level[pos], new Vector2(452 + 151 * ((pos) % 2), 240 + (pos / 2) * (55)), Color.LightGreen, 0, new Vector2(0, 0), 0.7f, SpriteEffects.None, 0);
                }
                else
                    for (int pos = 0; pos < 10; pos++) // score for each levels (5)
                        spriteBatch.DrawString(_gamefont, score_level[pos], new Vector2(452 + 151 * ((pos) % 2), 240 + (pos / 2) * (55)), Color.LightGreen, 0, new Vector2(0, 0), 0.7f, SpriteEffects.None, 0);
            
            spriteBatch.End();
            sr_arcade.Close();
            sr_arcade_level.Close();
        }

        public override void Update(GameTime gameTime)
        {
            path_level = "Scores\\Extreme\\lvl.score";

            position_Nv.X = 359 * (i / 5) + 109;
            position_Nv.Y = 230 + (i % 5) * 56;

            _keyboardState = Keyboard.GetState();

            if (_keyboardState.IsKeyDown(Keys.Enter)) // removing the scene
                Remove();

            _lastKeyboardState = _keyboardState;
            base.Update(gameTime);
        }

    }
}