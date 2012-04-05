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
        protected int _vie, _armure, _damageCollision, _armeActuelle, _vieMax, _score, _energie, _energieMax;
        protected double _timingAttack;
        protected bool _ennemi, _existe;
        protected double _lastTir;
        protected double _lastDamage;
        protected Vector2 stucked;

        public Vaisseau(Texture2D texture, int vie, int vieMax, int energie, int energieMax, int armure, int damageCollision, float vitesseVaisseau, Vector2 startPosition, bool ennemi, double timingAttack, int score, int arme)
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
            _armeActuelle = arme;
            _score = score;
            _lastDamage = -500;
            _energie = energie;
            _energieMax = energieMax;
            stucked = Vector2.Zero;
        }

        public void applyBonus(string effect, int amount, int time)
        {
            switch (effect)
            {
                case "vie":
                    this.heal(amount);
                    break;
                case "weapon":
                    this.changeWeapon(amount);
                    break;
                default:
                    break;
            }
        }

        public void move(Vector2 amount, float fps_fix)
        {
            _emplacement -= amount;
        }

        public int vie
        {
            get { return _vie; }
        }

        public int vieMax
        {
            get { return _vieMax; }
        }

        public int Energie
        {
            get { return _energie; }
        }

        public int EnergieMax
        {
            get { return _energieMax; }
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

        public bool hurt(int amount, double time)
        {
            this._vie -= amount;
            this._lastDamage = time;
            return (this._vie <= 0);
        }

        public bool useEnergy(int amount)
        {
            this._energie -= amount;
            return (this._energie <= 0);
        }

        public void heal(int amount)
        {
            if (amount + vie <= vieMax)
                this._vie += amount;
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

        public double lastDamage
        {
            set { this._lastDamage = value; }
        }

        public void changeWeapon(int nouveau)
        {
            this._armeActuelle = nouveau;
        }

        public void Update(float fps_fix)
        {
             _emplacement -= _deplacementDirectionX * _vitesseVaisseau * fps_fix;
        }

        public void Update(float fps_fix, KeyboardState keyboard, List<Obstacles> obstacles)
        {
            UpdateJoueur(fps_fix, keyboard, obstacles);
        }

        public void UpdateJoueur(float fps_fix, KeyboardState keyboard, List<Obstacles> obstacles)
        {
            foreach(Obstacles obstacle in obstacles)
            {
                if (obstacle is Obstacles_Hole)
                {
                    // On commence par passer en coordonées polaires
                    float xa = _emplacement.X - obstacle.position.X;
                    float ya = _emplacement.Y - obstacle.position.Y;
                    float r = (float)Math.Sqrt(Math.Pow(xa, 2) + Math.Pow(ya, 2));
                    float theta = (float) Math.Atan2(ya, xa);
                    
                    if(r < 10)
                        stucked = obstacle.position;

                    // On retourne en cartésien avec « x = (r - dr) * cos(θ - dθ) »,
                    // Puis on retourne dans le repère d'origine avec « + obstacle.position.X ».
                    if (stucked == Vector2.Zero)
                    {
                        _emplacement.X = (float)((r - 50000 / Math.Pow(r, 2)) * Math.Cos(theta - fps_fix * 30 / (1 * Math.Pow(r, 2))) + obstacle.position.X);
                        _emplacement.Y = (float)((r - 50000 / Math.Pow(r, 2)) * Math.Sin(theta - fps_fix * 30 / (1 * Math.Pow(r, 2))) + obstacle.position.Y);
                    }
                }
            }

            if (stucked != Vector2.Zero)
            {
                _emplacement = stucked;
            }
            else
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
                    if (_emplacement.X - _textureVaisseau.Width / 2 >= -18)
                        _emplacement -= _deplacementDirectionX * _vitesseVaisseau * fps_fix;
                }

                if (keyboard.IsKeyDown(Keys.D))
                {
                    if (_emplacement.X - _textureVaisseau.Width / 2 - 10 <= 1070)
                        _emplacement += _deplacementDirectionX * _vitesseVaisseau * fps_fix;
                }
            }
        }


        public void Draw(SpriteBatch batch, double time)
        {
            if (existe)
            {
                Color color;
                if (this._lastDamage + 120 > time)
                    color = Color.Tomato;
                else
                    color = Color.White;
                batch.Draw(_textureVaisseau, _emplacement, color);
            }
        }
    }
}
