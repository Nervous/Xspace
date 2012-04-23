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
    class Bonus : item
    {
        protected Texture2D _textureBonus;
        protected float _vitesseBonus;
        protected getBonus bonus;

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

        public Bonus(Texture2D texture, float vitesseVaisseau, Vector2 startPosition, Vector2 deplacement, string effect, int amount, int time, int score)
            :base(texture, startPosition, deplacement, 1, score)
        {
            _textureBonus = texture;
            _vitesseBonus = vitesseVaisseau;
            //_deplacement = Vector2.Normalize(new Vector2(5, 0));
            _existe = false;
            _score = score;
            bonus = new getBonus(effect, amount, time);
        }

        public Vector2 pos
        {
            get { return _pos; }
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

        public bool existe
        {
            get { return _existe; }
            set { _existe = value; }
        }

        public Texture2D sprite
        {
            get { return _textureBonus; }
        }

        public void Update(float fps_fix)
        {
            _pos -=  _deplacement * _vitesseBonus * fps_fix;
            updateRectangle();
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(_textureBonus, _pos, Color.White);
        }

    }
}
