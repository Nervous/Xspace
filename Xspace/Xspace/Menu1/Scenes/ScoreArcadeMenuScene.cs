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
        private StreamReader sr;
        private string path;
        private Vector2 position_score1, position_Nv1, position_Nv2, position_Nv3,position_surbrillance_Nv1, position_surbrillance_Nv2, position_surbrillance_Nv3;
        private Vector2 position_board;
        public string[] score;
        private string stock;
        private List<Vector2> position_list = new List<Vector2>();
        private static KeyboardState _keyboardState;
        private static KeyboardState _lastKeyboardState;
        private bool lvl1 = false, lvl2 = false, lvl3 =false;
        private int i;



        public ScoreArcadeMenuScene(SceneManager sceneMgr)
            : base(sceneMgr, "Score Arcade")
        {
            path = "Arcade.score";
            _keyboardState = new KeyboardState();
            _lastKeyboardState = new KeyboardState();
            i = 0;       
            var back = new MenuItem("Retour");
            var Nv1 = new MenuItem("Nv.1");
            sr = new StreamReader(path);
            position_Nv1.X = 130;
            position_Nv1.Y = 195;
            position_Nv2.X = 130;
            position_Nv2.Y = 230;
            position_Nv3.X = 130;
            position_Nv3.Y = 330;
            position_score1.X = 210;
            position_score1.Y = 195;
            position_board.X = 0;
            position_board.Y = 0;

            position_surbrillance_Nv1.X = 112;
            position_surbrillance_Nv2.X = 112;
            position_surbrillance_Nv3.X = 112;
            position_surbrillance_Nv1.Y = 187;
            position_surbrillance_Nv2.Y = 232;
            position_surbrillance_Nv3.Y = 277;

            position_list.Add(position_surbrillance_Nv1);
            position_list.Add(position_surbrillance_Nv2);
            position_list.Add(position_surbrillance_Nv3);

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
          _score_surbrillance = _content.Load<Texture2D>("Sprites\\Menu\\Score\\score-selected");
          _score_lvl = _content.Load<Texture2D>("Sprites\\Menu\\Score\\score-arcade-lvl");

            SpriteBatch spriteBatch = SceneManager.SpriteBatch;
            spriteBatch.Begin();
            
                spriteBatch.Draw(_score_board, position_board, Color.White);
                spriteBatch.DrawString(_gamefont, score[0], position_score1, Color.Green, 0, position_board, 0.7f, SpriteEffects.None, 0);
                spriteBatch.DrawString(_gamefont, "Nv1", position_Nv1, Color.Green, 0, position_board, 0.7f, SpriteEffects.None, 0);
                spriteBatch.Draw(_score_surbrillance, position_list[i], Color.White);
            
            if (lvl1)
            {
              spriteBatch.Draw(_score_lvl, position_board, Color.White);
            }
            else if (lvl2)
            {
              spriteBatch.Draw(_score_lvl, position_board, Color.White);
            }
            else if (lvl3)
            {
              spriteBatch.Draw(_score_lvl, position_board, Color.White);
            }

            spriteBatch.End();
        }

        
        public override void Update(GameTime gameTime)
        {


            _keyboardState = Keyboard.GetState();


            if ((_keyboardState.IsKeyDown(Keys.Up))&& (_lastKeyboardState.IsKeyUp(Keys.Up)))
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
            _lastKeyboardState = _keyboardState;

            if (_keyboardState.IsKeyDown(Keys.Space))
            {
                switch (i)
                {
                    case 0:

                        lvl1 = true;
                        lvl2 = false;
                        lvl3 = false;
                        break;

                    case 1:

                        lvl1 = false;
                        lvl2 = true;
                        lvl3 = false;
                        break;

                    default:

                        lvl1 = false;
                        lvl2 = false;
                        lvl3 = false;
                        break;
                }
            }
                
            base.Update(gameTime);

        }
        

        

    }
}