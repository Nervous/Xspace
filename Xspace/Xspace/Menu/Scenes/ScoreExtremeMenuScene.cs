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
        private string path_extreme_level;
        private Vector2 position_Nv;
        private Vector2 position_board;
        public string[] score_extreme, score_level, score_extreme_level, score_extreme_level_best;
        private static KeyboardState _keyboardState;
        private static KeyboardState _lastKeyboardState;
        private int i;
        private bool  firstTime;

        public ScoreExtremeMenuScene(SceneManager sceneMgr)
            : base(sceneMgr, "Score Arcade")
        {
            path_extreme_level = "Scores\\Extreme\\lvl.score";
            _keyboardState = new KeyboardState();
            _lastKeyboardState = new KeyboardState();
            i = 0;
            var back = new MenuItem("Retour");
            var Nv1 = new MenuItem("Nv.1");
            position_board = new Vector2();
            MenuItems.Add(back);
        }

        public override void Initialize()
        {
            base.Initialize();
            firstTime = true;
        }
        public override void Draw(GameTime gameTime)
        {
            FileStream fs1 = new FileStream(@path_extreme_level, FileMode.OpenOrCreate);
            fs1.Close();
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

                score_level = System.IO.File.ReadAllLines(@path_extreme_level);

                if (score_level.Length < 10)
                {
                    FileStream fs = new FileStream(@path_extreme_level, FileMode.Open);
                    StreamReader sr = new StreamReader(fs);
                    sr.ReadToEnd();
                    StreamWriter sw = new StreamWriter(fs);
                    if (score_level.Length > 0)
                        sw.Write('\n');

                    for (int k = score_level.Length; k < 10; k++)
                    {
                        if (k % 2 == 0)
                            sw.WriteLine("-");
                        else
                            sw.WriteLine("9999");
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
        }

        public override void Update(GameTime gameTime)
        {
            position_Nv.X = 359 * (i / 5) + 109;
            position_Nv.Y = 230 + (i % 5) * 56;

            _keyboardState = Keyboard.GetState();

            if (_keyboardState.IsKeyDown(Keys.Enter) && _lastKeyboardState.IsKeyUp(Keys.Enter)) // removing the scene
            {
                if (firstTime)
                    firstTime = false;
                else
                Remove();
            }

            _lastKeyboardState = _keyboardState;
            base.Update(gameTime);
        }

    }
}