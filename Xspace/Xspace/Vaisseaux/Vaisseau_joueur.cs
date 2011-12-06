using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Xspace
{
    class Vaisseau_joueur : Vaisseau
    {
        protected KeyboardState keyboardState;
        protected bool accelere;

        public Vaisseau_joueur(Texture2D sprite)
            : base(sprite, 100, 100, 0.55f, new Vector2(15, 225), true)
        { accelere = false; }



        public new void Update(float fps_fix)
        {
            keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.LeftShift))
            {
                accelere = true;
            }
            else accelere = false;

            if (accelere)
            {

                if (keyboardState.IsKeyDown(Keys.Z))
                {
                    if (_emplacement.Y - _textureVaisseau.Height / 2 + 20 >= 0)
                        _emplacement -= _deplacementDirectionY * _vitesseVaisseau * 2 * fps_fix;
                }

                if (keyboardState.IsKeyDown(Keys.S))
                {
                    if (_emplacement.Y - _textureVaisseau.Height / 2 - 10 <= 540)
                        _emplacement += _deplacementDirectionY * _vitesseVaisseau * 2 * fps_fix;
                }

                if (keyboardState.IsKeyDown(Keys.Q))
                {
                    if (_emplacement.X - _textureVaisseau.Width / 2 + 20 >= 0)
                        _emplacement -= _deplacementDirectionX * _vitesseVaisseau * 2 * fps_fix;
                }

                if (keyboardState.IsKeyDown(Keys.D))
                {
                    if (_emplacement.X - _textureVaisseau.Width / 2 - 10 <= 1085)
                        _emplacement += _deplacementDirectionX * _vitesseVaisseau * 2 * fps_fix;
                }

            }
            else
            {

                if (keyboardState.IsKeyDown(Keys.Z))
                {
                    if (_emplacement.Y - _textureVaisseau.Height / 2 + 20 >= 0)
                        _emplacement -= _deplacementDirectionY * _vitesseVaisseau * fps_fix;
                }

                if (keyboardState.IsKeyDown(Keys.S))
                {
                    if (_emplacement.Y - _textureVaisseau.Height / 2 - 10 <= 540)
                        _emplacement += _deplacementDirectionY * _vitesseVaisseau * fps_fix;
                }

                if (keyboardState.IsKeyDown(Keys.Q))
                {
                    if (_emplacement.X - _textureVaisseau.Width / 2 + 20 >= 0)
                        _emplacement -= _deplacementDirectionX * _vitesseVaisseau * fps_fix;
                }

                if (keyboardState.IsKeyDown(Keys.D))
                {
                    if (_emplacement.X - _textureVaisseau.Width / 2 - 10 <= 1085)
                        _emplacement += _deplacementDirectionX * _vitesseVaisseau * fps_fix;
                }
            }
        }
    }
}
