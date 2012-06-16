using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;

using MenuSample.Inputs;
using MenuSample.Scenes;
using MenuSample.Scenes.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using ProjectMercury;
using ProjectMercury.Emitters;
using ProjectMercury.Modifiers;
using ProjectMercury.Renderers;


namespace MenuSample.Scenes
{
    class NameScene : AbstractGameScene
    {
        protected KeyboardState keyboardState;
        protected Keys lastKey;
        protected string name;
        protected bool lastKeyDown;
        private Keys[] allowedKeys;
        private SpriteFont font;
        private Color _color;
        private bool _ok=false;

        public NameScene(SceneManager sceneMgr, GameTime gameTime, SpriteFont font, Color color)
            : base(sceneMgr)
        {
            _color = color;
            name = "";
            this.Update(gameTime);
            this.font = font;
            allowedKeys = new Keys[26] { Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I, Keys.J, Keys.K, Keys.L, Keys.M, Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z };
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            keyboardState = Keyboard.GetState();
            
            if (keyboardState.IsKeyUp(lastKey))
                lastKeyDown = true;
            if (keyboardState.GetPressedKeys().Length == 1 && allowedKeys != null)
            {
                if (name.Length < 7)
                {
                    Keys pressedKey = keyboardState.GetPressedKeys()[0];
                    if (lastKeyDown)
                    {
                        if (allowedKeys.Contains<Keys>(pressedKey))
                        {
                            name += pressedKey.ToString();
                            lastKey = pressedKey;
                            lastKeyDown = false;
                        }
                        else if (pressedKey == Keys.Enter)
                        {
                            _ok = true;
                            Remove();
                        }
                    }
                }
                else
                {
                    _ok = true;
                    Remove();
                }
            }

        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = SceneManager.SpriteBatch;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.DrawString(font, "Entrez votre nom :\n" + name, new Vector2(400, 400), _color);
            spriteBatch.End();
        }

        public string Name
        {
            get { return name; }
        }

        public bool Ok
        {
            get { return _ok; }
        }


    }
}
