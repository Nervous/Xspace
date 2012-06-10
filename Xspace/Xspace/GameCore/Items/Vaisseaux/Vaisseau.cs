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
    class Vaisseau : Item
    {
        public float _vitesseVaisseau;
        protected int _armure, _damageCollision, _armeActuelle, _baseWeapon, _vieMax, _energie, _energieMax;
        protected const int MAX_BASEWEAPON = 2;
        protected double _timingAttack;
        protected bool _ennemi, _laser;
        protected double _lastTir, _lastDamage;
        protected Vector2 stucked;
        protected Laser_joueur pLaser;
        protected Laser_Ennemi pLaserEnnemi;

        protected double tLancementChargement, tLancementLaser;
        protected bool chargement, tir, invicible;

        public double tLancementAoE;
        public bool AoE;

        public Vaisseau(Texture2D texture, int vie, int vieMax, int energieMax, int armure, int damageCollision, float vitesseVaisseau, Vector2 startPosition, Vector2 deplacement, bool ennemi, double timingAttack, int score, int arme)
            :base(texture, startPosition, deplacement, vie, score)
        {
            _sprite = texture;
            _vie = vie;
            _vieMax = vieMax;
            _armure = armure;
            _damageCollision = damageCollision;
            _vitesseVaisseau = vitesseVaisseau;
            _ennemi = ennemi;
            _existe = true;
            lastTir = 0;
            _timingAttack = timingAttack;
            _armeActuelle = arme;
            _score = score;
            _lastDamage = -500;
            _energie = energieMax;
            _energieMax = energieMax;
            stucked = Vector2.Zero;
            _baseWeapon = 0;
            _laser = false;


        }

        public void activerAoE()
        {
            this._deplacement.X = 0;
        }

        public bool Chargement
        {
            get { return chargement; }
        }

        public bool Tir
        {
            get { return tir; }
        }

        public void activerChargement(double time)
        {
            tLancementChargement = time;
            invicible = true;
            base._deplacement = new Vector2(0, 0);
            chargement = true;
        }

        public bool finChargement(double time)
        {
            return time - tLancementChargement > 4000;
        }

        public void terminerChargement()
        {
            tLancementChargement = 0;
            invicible = false;
            chargement = false;
        }

        public void activerTir(double time)
        {
            tir = true;
            tLancementLaser = time;
        }

        public bool finTir(double time)
        {
            return time - tLancementLaser > 2000;
        }

        public void terminerTir()
        {
            tLancementLaser = 0;
            tir = false;
            base._deplacement = new Vector2(1, 0);
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
                case "baseWeapon":
                    this.changeBaseWeapon(_baseWeapon + 1);
                    break;
                case "energie":
                    this._energie += amount;
                    if (this._energie > this._energieMax)
                        this._energie = this._energieMax;
                    break;
                default:
                    break;
            }
        }

        public void move(Vector2 amount, float fps_fix)
        {
            _pos -= amount;
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

        public Texture2D sprite
        {
            get { return _sprite; }
        }

        public bool hurt(int amount, double time)
        {
            if (this.invicible != true)
            {
                this._vie -= amount;
                this._lastDamage = time;
            }
            return (this._vie <= 0);
        }

        public bool useEnergy(int amount)
        {
            if (this._energie - amount >= 0)
            {
                this._energie -= amount;
                return false;
            }
            else
                return true; // What ?
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

        public int baseWeapon
        {
            get { return _baseWeapon; }
        }

        public double lastDamage
        {
            set { this._lastDamage = value; }
        }

        public bool laser
        {
            get { return _laser; }
        }

        public void disableLaser()
        {
            this._laser = false;
            pLaser = null;
        }

        public Laser_joueur getLaser()
        {
            return pLaser;
        }

        public Laser_Ennemi getLaserE()
        {
            return pLaserEnnemi;
        }

        public void enableLaser(Laser_joueur l)
        {
            this._laser = true;
            pLaser = l;
        }

        public void enableLaserE(Laser_Ennemi l)
        {
            this._laser = true;
            pLaserEnnemi = l;
        }

        public void changeWeapon(int nouveau)
        {
            this._armeActuelle = nouveau;
        }

        public void changeBaseWeapon(int nouveau)
        {
            if(nouveau <= MAX_BASEWEAPON)
                this._baseWeapon = nouveau;
        }

        public void Update(float fps_fix)
        {
             _pos -= _deplacement * _vitesseVaisseau * fps_fix;
             updateRectangle();
             if (this is Zebra) 
             {
                 if (_pos.Y - _sprite.Height / 2 > 550)
                     base._deplacement.Y = 1;
                 if (_pos.Y - _sprite.Height / 2 < -5)
                     base._deplacement.Y = -1;
             }
        }

        public void Update(float fps_fix, KeyboardState keyboard, List<Obstacles> obstacles)
        {
            const float K_GRAVITE = 1000;
            const float K_ANGLE_DELTA = 30;

            if (_energie < EnergieMax)
                _energie++;

            foreach(Obstacles obstacle in obstacles)
            {
                if (obstacle is Obstacles_Hole)
                {
                    // On commence par passer en coordonées polaires centré sur l'obstacle
                    float xa = _pos.X - obstacle.pos.X;
                    float ya = _pos.Y - obstacle.pos.Y;
                    float r = (float)Math.Sqrt(Math.Pow(xa, 2) + Math.Pow(ya, 2));
                    float theta = (float) Math.Atan2(ya, xa);
                    
                    if(r < 10)
                        stucked = obstacle.pos;

                    // On retourne en cartésien avec « x = (r - dr) * cos(θ - dθ) »,
                    // Puis on retourne dans le repère d'origine avec « + obstacle.position.X ».
                    if (stucked == Vector2.Zero)
                    {
                        _pos.X = (float)((r - fps_fix * K_GRAVITE / Math.Pow(r, 2)) * Math.Cos(theta - fps_fix * K_ANGLE_DELTA / (Math.Pow(r, 2))) + obstacle.pos.X);
                        _pos.Y = (float)((r - fps_fix * K_GRAVITE / Math.Pow(r, 2)) * Math.Sin(theta - fps_fix * K_ANGLE_DELTA / (Math.Pow(r, 2))) + obstacle.pos.Y);
                    }
                }
            }

            if (stucked != Vector2.Zero)
            {
                _pos = stucked;
            }
            else
            {
                if (keyboard.IsKeyDown(Keys.Z))
                {
                    if (_pos.Y - _sprite.Height / 2 >= -18)
                        _pos.Y -= _deplacement.Y * _vitesseVaisseau * fps_fix;
                }

                if (keyboard.IsKeyDown(Keys.S))
                {
                    if (_pos.Y - _sprite.Height / 2 <= 538)
                        _pos.Y += _deplacement.Y * _vitesseVaisseau * fps_fix;
                }

                if (keyboard.IsKeyDown(Keys.Q))
                {
                    if (_pos.X - _sprite.Width / 2 >= -18)
                        _pos.X -= _deplacement.X * _vitesseVaisseau * fps_fix;
                }

                if (keyboard.IsKeyDown(Keys.D))
                {
                    if (_pos.X - _sprite.Width / 2 - 10 <= 1070)
                        _pos.X += _deplacement.X * _vitesseVaisseau * fps_fix;
                }
            }

            updateRectangle();
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

                Random r = new Random();

                if (this is BC && this.Chargement)
                    batch.Draw(_sprite, _pos, new Color(r.Next(0,255), r.Next(0,255), r.Next(0,255)));
                else if (!(this is Zebra))
                    batch.Draw(_sprite, _pos, color);
                else
                    batch.Draw(_sprite, rectangle, null, color, MathHelper.PiOver4 * _deplacement.Y, new Vector2(sprite.Width / 2, sprite.Height / 2), SpriteEffects.None, 0f);
            }
        }
    }
}
