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
    class Vaisseau
    {
        public Vector2 _emplacement, _deplacementDirectionY, _deplacementDirectionX;
        public Texture2D _textureVaisseau;
        public float _vitesseVaisseau;
        protected int _vie, _armure;
        protected double _timingAttack;
        protected string _typeVaisseau, _position;
        protected bool _ennemi;
        protected double _lastTir;

        protected bool constr(Texture2D texture, int vie, int armure, float vitesseVaisseau, Vector2 startPosition, bool ennemi, double timingAttack)
        {
            _textureVaisseau = texture;
            _vie = vie;
            _armure = armure;
            _vitesseVaisseau = vitesseVaisseau;
            _emplacement = startPosition;
            _deplacementDirectionX = Vector2.Normalize(new Vector2(5, 0));
            _deplacementDirectionY = Vector2.Normalize(new Vector2(0, 5));
            _ennemi = ennemi;
            _timingAttack = timingAttack;
            return true;
        }

         

        public Vaisseau(Texture2D texture, int vie, int armure, float vitesseVaisseau, Vector2 startPosition, bool ennemi)
        {
            _textureVaisseau = texture;
            _vie = vie;
            _armure = armure;
            _vitesseVaisseau = vitesseVaisseau;
            _emplacement = startPosition;
            _deplacementDirectionX = Vector2.Normalize(new Vector2(5, 0));
            _deplacementDirectionY = Vector2.Normalize(new Vector2(0, 5));
            _ennemi = ennemi;
        }

        public int vie
        {
            get { return _vie; }
        }

        public double timingAttack
        {
            get { return _timingAttack; }
        }

        public double lastTir
        {
            get { return _lastTir; }
            set {this._lastTir = value;}
        }

        public Vector2 position
        {
            get { return _emplacement; }
        }

        public Texture2D sprite
        {
            get { return _textureVaisseau; }
        }

        public bool hurt(int ammount)
        {
            this._vie -= ammount;
            return (this._vie <= 0);
        }

        public bool ennemi
        {
            get { return _ennemi; }
        }

        public void Update(float fps_fix)
        {
             _emplacement -= _deplacementDirectionX * _vitesseVaisseau * fps_fix;
        }

        public void Update(float fps_fix, KeyboardState keyboard)
        {
            UpdateJoueur(fps_fix, keyboard);
        }

        public void UpdateJoueur(float fps_fix, KeyboardState keyboard)
        {

            if (keyboard.IsKeyDown(Keys.Z))
            {
                if (_emplacement.Y - _textureVaisseau.Height / 2 + 20 >= 0)
                    _emplacement -= _deplacementDirectionY * _vitesseVaisseau * fps_fix;
            }

            if (keyboard.IsKeyDown(Keys.S))
            {
                if (_emplacement.Y - _textureVaisseau.Height / 2 - 10 <= 540)
                    _emplacement += _deplacementDirectionY * _vitesseVaisseau * fps_fix;
            }

            if (keyboard.IsKeyDown(Keys.Q))
            {
                if (_emplacement.X - _textureVaisseau.Width / 2 + 20 >= 0)
                    _emplacement -= _deplacementDirectionX * _vitesseVaisseau * fps_fix;
            }

            if (keyboard.IsKeyDown(Keys.D))
            {
                if (_emplacement.X - _textureVaisseau.Width / 2 - 10 <= 1085)
                    _emplacement += _deplacementDirectionX * _vitesseVaisseau * fps_fix;
            }
        }


        public void Draw(SpriteBatch batch)
        {
            if (existe)
            {
                batch.Draw(_textureVaisseau, _emplacement, Color.White);
            }
        }
    }
}
