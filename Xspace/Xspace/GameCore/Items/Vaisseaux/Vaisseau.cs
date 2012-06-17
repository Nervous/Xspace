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
        protected int _armure, _damageCollision, _baseWeapon, _vieMax, _energie, _energieMax;
        public int _armeActuelle;
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

        public double tNextEnergyToLife;

        public int timingAttackPlayerBaseWeapon;

        private struct removeBonus
        {
            public string type;
            public double time;

            public removeBonus(string type, double time)
            {
                this.type = type;
                this.time = time;
            }
        };

        private List<removeBonus> effect;
        private List<removeBonus> effectToRemove;

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
            tNextEnergyToLife = 0;
            timingAttackPlayerBaseWeapon = 200;

            effect = new List<removeBonus>();
            effectToRemove = new List<removeBonus>();
        }

        public void energyToLife(double time, int life)
        {
            if (this._energie == this.EnergieMax && time > tNextEnergyToLife)
            {
                this.heal(life);
                this.useEnergy(this._energieMax);
                this.tNextEnergyToLife = time + 60000;
            }
        }

        public double TNextEnergyToLife
        {
            get { return tNextEnergyToLife; }
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

        public void applyBonus(string effect, int amount, int time, double rTime)
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
                case "vitesse":
                    this._vitesseVaisseau = 1.5f;
                    this.effect.Add(new removeBonus("vitesse", rTime + 10000));
                    break;
                case "shootspeed":
                    this.timingAttackPlayerBaseWeapon = 100;
                    this.effect.Add(new removeBonus("shootspeed", rTime + 10000));
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

        public void Update(float fps_fix, double time, KeyboardState keyboard, GamePadState gamepadState, List<Obstacles> obstacles)
        {
            foreach (removeBonus o in effect)
            {
                if (o.time <= time)
                {
                    if (o.type == "vitesse")
                    {
                        this._vitesseVaisseau = 0.70f;
                    }
                    else if (o.type == "shootspeed")
                    {
                        this.timingAttackPlayerBaseWeapon = 200;
                    }
                    effectToRemove.Add(o);
                }
            }

            foreach (removeBonus o in effectToRemove)
            {
                effect.Remove(o);
            }

            effectToRemove.Clear();

            const float K_GRAVITE = 1000;

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

                    if (r < 20)
                    {
                        stucked.X = obstacle.pos.X - _sprite.Width / 2;
                        stucked.Y = obstacle.pos.Y - _sprite.Height / 2;
                    }

                    // On retourne en cartésien avec « x = (r - dr) * cos(θ - dθ) »,
                    // Puis on retourne dans le repère d'origine avec « + obstacle.position.X ».
                    if (stucked == Vector2.Zero)
                    {
                        _pos.X = (float)((r - fps_fix * K_GRAVITE / Math.Pow(r, 2)) * Math.Cos(theta) + obstacle.pos.X);
                        _pos.Y = (float)((r - fps_fix * K_GRAVITE / Math.Pow(r, 2)) * Math.Sin(theta) + obstacle.pos.Y);
                    }
                }
            }

            if (stucked != Vector2.Zero)
            {
                hurt(1, time);
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

            if (gamepadState.IsConnected)
            {
                GamePadCapabilities gamepadCaps = GamePad.GetCapabilities(PlayerIndex.One);
                if (gamepadCaps.HasLeftXThumbStick && gamepadCaps.HasLeftYThumbStick)
                {
                    _pos.X += gamepadState.ThumbSticks.Left.X * _vitesseVaisseau * 12;
                    _pos.Y += -gamepadState.ThumbSticks.Left.Y * _vitesseVaisseau * 12;
                    if (_pos.X - _sprite.Width / 2 <= -18)
                        _pos.X = -17 + _sprite.Width / 2;

                    if (_pos.Y - _sprite.Height / 2 <= -18)
                        _pos.Y = -17 + _sprite.Height / 2;

                    if (_pos.Y - _sprite.Height / 2 >= 538)
                        _pos.Y = 537 + _sprite.Width / 2;

                    if (_pos.X - _sprite.Width / 2 - 10 >= 1070)
                        _pos.X = 1069 + _sprite.Width / 2;
                }
                else if (gamepadCaps.HasLeftXThumbStick && gamepadCaps.HasLeftYThumbStick)
                    _pos += gamepadState.ThumbSticks.Right * _vitesseVaisseau * 10;

                else if (gamepadCaps.HasDPadUpButton && gamepadCaps.HasDPadLeftButton && gamepadCaps.HasDPadRightButton && gamepadCaps.HasDPadDownButton)
                {
                    if (gamepadState.IsButtonDown(Buttons.DPadUp))
                    {
                        if (_pos.Y - _sprite.Height / 2 >= -18)
                            _pos.Y -= _deplacement.Y * _vitesseVaisseau * fps_fix;
                    }
                    if (gamepadState.IsButtonDown(Buttons.DPadDown))
                    {
                        if (_pos.Y - _sprite.Height / 2 <= 538)
                            _pos.Y += _deplacement.Y * _vitesseVaisseau * fps_fix;
                    }
                    if (gamepadState.IsButtonDown(Buttons.DPadLeft))
                    {
                        if (_pos.X - _sprite.Width / 2 >= -18)
                            _pos.X -= _deplacement.X * _vitesseVaisseau * fps_fix;
                    }
                    if (gamepadState.IsButtonDown(Buttons.DPadRight))
                    {
                        if (_pos.X - _sprite.Width / 2 - 10 <= 1070)
                            _pos.X += _deplacement.X * _vitesseVaisseau * fps_fix;
                    }
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
