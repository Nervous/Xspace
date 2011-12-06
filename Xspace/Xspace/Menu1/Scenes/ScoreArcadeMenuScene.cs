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
        private ContentManager _content;
        private SpriteFont _gamefont;
        private Texture2D _score_board;
        private StreamReader sr;
        private string path;
        private Vector2 position_score1, position_Nv1;
        private Vector2 position_board;
        public string[] score;
        private string stock;

        public ScoreArcadeMenuScene(SceneManager sceneMgr)
            : base(sceneMgr, "Score Arcade")
        {
            path = "Arcade.txt";
           var back = new MenuItem("Retour");
            var Nv1 = new MenuItem("Nv.1");
            sr = new StreamReader(path);
            position_Nv1.X = 130;
            position_Nv1.Y = 195;
            position_score1.X = 210;
            position_score1.Y = 195;
            position_board.X = 0;
            position_board.Y = 0;

            back.Selected += OnCancel;

            MenuItems.Add(back); 
            
        
        }


        public override void Draw(GameTime gameTime)
        {
            stock = sr.ReadToEnd();
            score = System.IO.File.ReadAllLines(@path);


          if(_content == null)
          _content = new ContentManager(SceneManager.Game.Services, "Content");
          _gamefont = _content.Load<SpriteFont>("Fonts\\Jeu\\Jeu");
          _score_board = _content.Load<Texture2D>("Sprites\\Menu\\Score\\score-proto");
          

            SpriteBatch spriteBatch = SceneManager.SpriteBatch;
            spriteBatch.Begin();
            
                spriteBatch.Draw(_score_board, position_board, Color.White);
                spriteBatch.DrawString(_gamefont, score[0], position_score1, Color.Green, 0, position_board, 0.7f, SpriteEffects.None, 0);
                spriteBatch.DrawString(_gamefont, "Nv1", position_Nv1, Color.Green, 0, position_board, 0.7f, SpriteEffects.None, 0);
            spriteBatch.End();
        }

    }
}