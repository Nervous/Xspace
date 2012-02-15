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
        protected int _vie, _vitesse, _NbShootsperPhases, _score, _vieMax, _armeActuelle, _phase;
        protected int[] _phaseList;
        protected double _timingAttack, _lastTir;
        protected Missiles[] _missiles;
        protected Vector2 _position;
        protected bool _existe, _phase1, _phase2, _phase3;
        /* Phase list: Example: [100,60,20]: As soon as phase[0] < vie, second phase begin, then third phase when phase[1] (so at 20 of life) < vie, etc..
         * So, you should ALWAYS have phase[0] >= vieMax
        Same thing for missiles, NbShootsperPhases determines how many missiles you have in one phase.
         * So, if you have NbShootsperPhases = 3, then the three first missiles of the list will be launched during phase 1.
         * Then, missiles[3] will be in phase 2, missiles[6] in 3 etc.. 
         * If missiles[n+1] doesn't exist, then missiles[n] will be launch..
         
         WARNING: Only three phases maximum are supported right now*/

        public Boss(Texture2D texture, int vie, int vieMax, double timingAttack, int[] phaseList, int vitesse, Missiles[] missiles, int NbShootsperPhases, Vector2 position, int damageCollision, int score)
        {
            _texture = texture;
            _vie = vie;
            _vieMax = vieMax;
            _score = score;
            _vitesse = vitesse;
            _phaseList = phaseList;
            _missiles = missiles;
            _position = position;
            _NbShootsperPhases = NbShootsperPhases;
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
            if (_phaseList.Length == 3)
            {
                if ((_vie > _phaseList[1]) && (_phaseList[0] >= _vie))
                    _phase = 1;
                

                else if ((_vie > _phaseList[2]) && (_phaseList[1] >= _vie))
                    _phase = 2;
                

                else if ((_vie > 0) && (_phaseList[2] >= _vie))
                    _phase = 3;
                
            }
            else if (_phaseList.Length == 2)
            {
                if ((_vie > _phaseList[1]) && (_phaseList[0] >= _vie))
                    _phase = 1;

                else if ((_vie > 0) && (_phaseList[1] >= _vie))
                    _phase = 2;
            }
        }
    }
}
