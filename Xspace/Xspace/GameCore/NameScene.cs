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

        public NameScene(SceneManager sceneMgr, GameTime gameTime)
            : base(sceneMgr)
        {
            name = "";
            this.Update(gameTime);
            allowedKeys = new Keys[26] { Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I, Keys.J, Keys.K, Keys.L, Keys.M, Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z };
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            keyboardState = Keyboard.GetState();
            
            if (keyboardState.IsKeyUp(lastKey))
                lastKeyDown = true;
            if (keyboardState.GetPressedKeys().Length == 1)
            {
                if (name.Length < 5)
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
                            Remove();
                        }
                    }
                }
                else
                    Remove();
            }

        }

        public string Name
        {
            get { return name; }
        }



    }
}
