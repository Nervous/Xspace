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
    class Missiles
    {
        protected Vector2 _emplacement, _moveX, _moveY;
        protected Texture2D _textureMissile;
        private Vaisseau _ownerV;
        private Boss _ownerB;
        protected int _vie, _degats;
        private bool _ennemi, _existe;
        float _vitesse;

        public Missiles(Texture2D texture, bool ennemi, int degats, Vector2 emplacement, Vector2 moveX, Vector2 moveY, float vitesse, Vaisseau owner, Boss ownerB)
        {
            _textureMissile = texture;
            _vie = 1;
            _degats = degats;
            _emplacement = emplacement;
            _moveX = Vector2.Normalize(moveX);
            _moveY = Vector2.Normalize(moveY);
            _ennemi = ennemi;
            _vitesse = vitesse;
            _ownerV = owner;
            _ownerB = ownerB;
            _existe = true;
        }

        public void initialiserTexture(Texture2D texture)
        {
            _textureMissile = texture;
        }

        public Vector2 position
        {
            get { return _emplacement; }
        }

        public int degats
        {
            get { return _degats; }
        }

        public Texture2D sprite
        {
            get { return _textureMissile; }
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
            return ((this._ownerV != null && this._ownerV != v) || (this._ownerB != null && this._ownerB != b));
        }

        public void avancerMissile(float fps_fix)
        {
            _emplacement.X += fps_fix * _moveX.X * _vitesse;
            _emplacement.Y += fps_fix * _moveX.Y * _vitesse;
        }

        public void Update()
        {

        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(_textureMissile, _emplacement, Color.White);
        }
    }
}