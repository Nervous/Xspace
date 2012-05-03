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
    abstract class Item
    {
        protected Texture2D _sprite;
        protected Vector2 _pos, _deplacement;

        protected bool _existe;
        protected int _vie, _score;

        protected Rectangle _rectangle;

        public Item(Texture2D text, Vector2 pos, Vector2 deplacement, int vie, int score)
        {
            _existe = true;
            _sprite = text;
            _pos = pos;
            _vie = vie;
            _score = score;
            _deplacement = deplacement;
            rectangle = new Rectangle((int)_pos.X, (int)_pos.Y, _sprite.Width, _sprite.Height);
        }

        protected void updateRectangle()
        {
            _rectangle = new Rectangle((int)_pos.X, (int)_pos.Y, _sprite.Width, _sprite.Height);
        }

        public Rectangle rectangle
        {
            get { return _rectangle; }
            set { _rectangle = value; }
        }

        public Vector2 pos
        {
            get { return _pos; }
            set { this._pos = value; }
        }
    }
}
