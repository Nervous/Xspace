using System;
using MenuSample.Scenes.Core;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MenuSample.Scenes
{
    public class ScoreArcadeMenuScene : AbstractMenuScene
    {
        private readonly MenuItem _scoreMenuItem;
        private ContentManager _content;
        private SpriteFont _gamefont;
        private Texture2D _score_board;
        private StreamReader sr;
        private string path;
        private Vector2 position_score1;
        private Vector2 position_board;
        public string[] score;
        private string stock;

        public ScoreArcadeMenuScene(SceneManager sceneMgr)
            : base(sceneMgr, "Score Arcade")
        {
            path = "Arcade.txt";
            var back = new MenuItem("Retour");
            sr = new StreamReader(path);


            back.Selected += OnCancel;
            
            MenuItems.Add(back);
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(GameTime gameTime)
        {

            stock = sr.ReadToEnd();
            score = System.IO.File.ReadAllLines(@path);
            position_score1.X = 415;
            position_score1.Y = 170;
            position_board.X = 0;
            position_board.Y = 0;


          if(_content == null)
          _content = new ContentManager(SceneManager.Game.Services, "Content");
          _gamefont = _content.Load<SpriteFont>("Fonts\\Jeu\\Jeu");
          _score_board = _content.Load<Texture2D>("Sprites\\Menu\\Score\\score-proto");
          

            SpriteBatch spriteBatch = SceneManager.SpriteBatch;
            spriteBatch.Begin();
            
                spriteBatch.Draw(_score_board, position_board, Color.White);
                spriteBatch.DrawString(_gamefont, score[0], position_score1, Color.Green);
            spriteBatch.End();
        }

    }
}