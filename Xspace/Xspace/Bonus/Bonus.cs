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
        protected Vector2 _emplacement, _deplacement;
        protected Texture2D _textureBonus;
        protected float _vitesseBonus;
        protected getBonus bonus;
        protected bool _disabled;
        protected int _score;

        protected struct getBonus
        {
            public string _effect;
            public int _amount, _time;

            public getBonus(string effect, int amount, int time)
            {
                _effect = effect;
                _amount = amount;
                _time = time;
            }
        };

        public Bonus(Texture2D texture, float vitesseVaisseau, Vector2 startPosition, string effect, int amount, int time, int score)
        {
            _textureBonus = texture;
            _vitesseBonus = vitesseVaisseau;
            _emplacement = startPosition;
            _deplacement = Vector2.Normalize(new Vector2(5, 0));
            _disabled = false;
            _score = score;
            bonus = new getBonus(effect, amount, time);
        }

        public Vector2 position
        {
            get { return _emplacement; }
        }

        public string effect
        {
            get { return bonus._effect; }
        }

        public int score
        {
            get { return this._score; }
        }

        public int amount
        {
            get { return bonus._amount; }
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
