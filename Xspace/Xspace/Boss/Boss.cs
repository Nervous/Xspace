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

namespace Xspace.Boss
{
    class Boss
    {
        protected Texture2D _texture;
        protected int _vie, _vitesse, _score, _vieMax, _armeActuelle, _phase;
        protected int[] _phaseArray;
        protected double _timingAttack, _lastTir;
        protected Vector2 _position;
        protected bool _existe;
        /* Phase list: Example: [100,60,20]: As soon as phase[0] < vie, second phase begin, then third phase when phase[1] (so at 20 of life) < vie, etc..
         * So, you should ALWAYS have phase[0] >= vieMax        
         WARNING: Only three phases maximum are supported right now*/

        public Boss(Texture2D texture, int vie, int vieMax, double timingAttack, int[] phaseArray, int vitesse, Vector2 position, int damageCollision, int score)
        {
            _texture = texture;
            _vie = vie;
            _vieMax = vieMax;
            _score = score;
            _vitesse = vitesse;
            _phaseArray = phaseArray;
            _position = position;
            _armeActuelle = 0;
            _lastTir = 0;
            _timingAttack = timingAttack;
            _existe = true;
            _phase = 1;
        }

        public Vector2 Position
        {
            get { return _position; }
        }

        public int Vitesse
        {
            get { return _vitesse; }
        }

        public int vieActuelle
        {
            get { return _vie; }
        }

        public int VieMax
        {
            get { return _vieMax; }
        }

        public int Score
        {
            get { return _score; }
        }

        public void Move(Vector2 amount, float fps_fix)
        {
            _position -= amount;
        }

        public bool Existe
        {
            get { return _existe; }
        }

        public double TimingAttack
        {
            get { return _timingAttack; }
        }

        public int ArmeActuelle
        {
            get { return _armeActuelle; }
        }

        public double LastTir
        {
            get { return _lastTir; }
            set { _lastTir = value; }
        }

        public bool Hurt(int amount)
        {
            _vie -= amount;
            return (_vie <= 0);
        }

        public int Phase
        {
            get { return(_phase);}
        }

        public void Heal(int amount)
        {
            if (amount + _vie <= _vieMax)
                _vie += amount;
            else
                _vie = _vieMax;
        }

        public void Kill()
        {
            _existe = false;
        }

        public void ChangeWeapon(int nouveau)
        {
            _armeActuelle = nouveau;
        }

        public void Update(float fps_fix)
        {
            if (_phaseArray.Length == 3)
            {
                if ((_vie > _phaseArray[1]) && (_phaseArray[0] >= _vie))
                    _phase = 1;


                else if ((_vie > _phaseArray[2]) && (_phaseArray[1] >= _vie))
                    _phase = 2;


                else if ((_vie > 0) && (_phaseArray[2] >= _vie))
                    _phase = 3;
                
            }
            else if (_phaseArray.Length == 2)
            {
                if ((_vie > _phaseArray[1]) && (_phaseArray[0] >= _vie))
                    _phase = 1;

                else if ((_vie > 0) && (_phaseArray[1] >= _vie))
                    _phase = 2;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _position, Color.White);
        }
    }
}
