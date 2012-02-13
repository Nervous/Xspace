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
        protected int _vie, _armure, _damageCollision, _armeActuelle, _vieMax, _score;
        protected double _timingAttack;
        protected bool _ennemi, _existe;
        protected double _lastTir;

        public Vaisseau(Texture2D texture, int vie, int vieMax, int armure, int damageCollision, float vitesseVaisseau, Vector2 startPosition, bool ennemi, double timingAttack, int score)
        {
            _textureVaisseau = texture;
            _vie = vie;
            _vieMax = vieMax;
            _armure = armure;
            _damageCollision = damageCollision;
            _vitesseVaisseau = vitesseVaisseau;
            _emplacement = startPosition;
            _deplacementDirectionX = Vector2.Normalize(new Vector2(5, 0));
            _deplacementDirectionY = Vector2.Normalize(new Vector2(0, 5));
            _ennemi = ennemi;
            _existe = true;
            lastTir = 0;
            _timingAttack = timingAttack;
            _armeActuelle = 0;
            _score = score;
        }

        public void applyBonus(string effect, int ammount, int time)
        {
            switch (effect)
            {
                case "vie":
                    this.heal(ammount);
                    break;
                case "weapon":
                    this.changeWeapon(ammount);
                    break;
                default:
                    break;
            }
        }

        public void move(Vector2 ammount, float fps_fix)
        {
            _emplacement -= ammount;
        }

        public int vie
        {
            get { return _vie; }
        }

        public int vieMax
        {
            get { return _vieMax; }
        }

        public int damageCollision
        {
            get { return _damageCollision; }
        }

        public bool existe
        {
            get { return _existe; }
        }

        public double timingAttack
        {
            get { return _timingAttack; }
        }

        public int armeActuelle
        {
            get { return _armeActuelle; }
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

        public void heal(int ammount)
        {
            if (ammount + vie <= vieMax)
                this._vie += ammount;
            else
                this._vie = vieMax;
        }

        public bool ennemi
        {
            get { return _ennemi; }
        }

        public void kill()
        {
            this._existe = false;
        }

        public int score
        {
            get { return _score; }
        }

        public void changeWeapon(int nouveau)
        {
            this._armeActuelle = nouveau;
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
                if (_emplacement.Y - _textureVaisseau.Height / 2 >= -18)
                    _emplacement -= _deplacementDirectionY * _vitesseVaisseau * fps_fix;
            }

            if (keyboard.IsKeyDown(Keys.S))
            {
                if (_emplacement.Y - _textureVaisseau.Height / 2 <= 530)
                    _emplacement += _deplacementDirectionY * _vitesseVaisseau * fps_fix;
            }

            if (keyboard.IsKeyDown(Keys.Q))
            {
                if (_emplacement.X - _textureVaisseau.Width / 2  >= -18)
                    _emplacement -= _deplacementDirectionX * _vitesseVaisseau * fps_fix;
            }

            if (keyboard.IsKeyDown(Keys.D))
            {
                if (_emplacement.X - _textureVaisseau.Width / 2 - 10 <= 1070)
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
