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
    class Missiles : item
    {
        private Vaisseau _ownerV;
        private Boss _ownerB;
        protected int _degats;
        private bool _ennemi;
        float _vitesse;

        public Missiles(Texture2D texture, bool ennemi, int degats, Vector2 emplacement, Vector2 deplacement, float vitesse, Vaisseau owner, Boss ownerB)
            :base(texture, emplacement, deplacement, 1, 0)
        {
            _sprite = texture;
            _degats = degats;
            _ennemi = ennemi;
            _vitesse = vitesse;
            _ownerV = owner;
            _ownerB = ownerB;
        }

        public void initialiserTexture(Texture2D texture)
        {
            _sprite = texture;
        }

        public Vector2 pos
        {
            get { return _pos; }
        }

        public int degats
        {
            get { return _degats; }
        }

        public Texture2D sprite
        {
            get { return _sprite; }
        }

        public bool ennemi
        {
            get { return _ennemi; }
        }

        public bool isAlive
        {
            get { return this._existe; }
        }

        public void kill()
        {
            this._existe = false;
        }

        public bool isOwner(Vaisseau v, Boss b)
        {
            return ((v == null && this._ownerB != b) || (b == null && this._ownerV != v));
        }

        public void Update(float fps_fix)
        {
            _pos.X += fps_fix * _deplacement.X * _vitesse;
            _pos.Y += fps_fix * _deplacement.Y * _vitesse;
            updateRectangle();
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(_sprite, _pos, Color.White);
        }
    }
}