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
        private StreamReader sr;
        private string path;
        private Vector2 position_score1;
        public string score;
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
            score = stock;
            position_score1.X = 0;
            position_score1.Y = 0;


          if(_content == null)
          _content = new ContentManager(SceneManager.Game.Services, "Content");
          _gamefont = _content.Load<SpriteFont>("gamefont");
          

            SpriteBatch spriteBatch = SceneManager.SpriteBatch;
            spriteBatch.Begin();

                spriteBatch.DrawString(_gamefont, score, position_score1, Color.Red);

            spriteBatch.End();
        }

    }
}