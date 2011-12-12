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
    class Bonus
    {
        public Vector2 _emplacement, _deplacement;
        public Texture2D _textureBonus;
        public float _vitesseBonus;
        getBonus bonus;
        bool _disabled;
        struct getBonus
        {
            public string _effect;
            public int _ammount, _time;

            public getBonus(string effect, int ammount, int time)
            {
                _effect = effect;
                _ammount = ammount;
                _time = time;
            }
        };

        public Bonus(Texture2D texture, float vitesseVaisseau, Vector2 startPosition, string effect, int ammount, int time)
        {
            _textureBonus = texture;
            _vitesseBonus = vitesseVaisseau;
            _emplacement = startPosition;
            _deplacement = new Vector2(5, 0);
            _disabled = false;
            bonus = new getBonus(effect, ammount, time);
        }

        public Vector2 position
        {
            get { return _emplacement; }
        }

        public string effect
        {
            get { return bonus._effect; }
        }

        public int ammount
        {
            get { return bonus._ammount; }
        }

        public int time
        {
            get { return bonus._time; }
        }

        public bool disabled
        {
            get { return _disabled; }
            set { _disabled = value; }
        }

        public Texture2D sprite
        {
            get { return _textureBonus; }
        }

        public void Update(float fps_fix)
        {
            _emplacement -=  _deplacement * _vitesseBonus * fps_fix;
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(_textureBonus, _emplacement, Color.White);
        }

    }
}
