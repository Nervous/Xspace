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
        protected Vector2 _emplacement, _deplacementDirectionY, _deplacementDirectionX, _start;
        protected Texture2D _textureMissile;
        protected int _vie, _degats;
        protected float _vitesseMissile, _vitesseMissile_Y;
        private bool _estAffiche, _ennemi, _existe;

        public Missiles(Texture2D texture, bool ennemi, int degats)
        {
            _textureMissile = texture;
            _vie = 1;
            _degats = degats;
            _vitesseMissile = 0.95f;
            _vitesseMissile_Y= 0.85f;
            _emplacement = Vector2.Zero;
            _deplacementDirectionX = Vector2.Normalize(new Vector2(7, 0));
            _deplacementDirectionY = Vector2.Normalize(new Vector2(0, 7));
            _estAffiche = false;
            _ennemi = ennemi;
            _existe = false;
        }

        public Missiles(Texture2D texture, int vie, int degats, int armure, float vitesseMissile, Vector2 startPosition, bool ennemi)
        {
            _textureMissile = texture;
            _vie = vie;
            _degats = 10;
            _vitesseMissile = vitesseMissile;
            _emplacement = startPosition;
            _deplacementDirectionX = Vector2.Normalize(new Vector2(7, 0));
            _deplacementDirectionY = Vector2.Normalize(new Vector2(0, 7));
            _estAffiche = false;
            _ennemi = ennemi;
            _existe = false;
            
        }

        public void initialiserTexture(Texture2D texture)
        {
            _textureMissile = texture;
        }


        public bool estAffiche
        {
            get { return _estAffiche; }
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
            set { this._ennemi = value; }
        }

        public bool afficherMissile(Vector2 startPosition)
        {
            _start = startPosition;
            this._estAffiche = true;
            float startX = startPosition.X + 45;
            float startY = startPosition.Y + 15;
            Vector2 start = new Vector2(startX, startY);
            _emplacement = start;
            this._existe = true;
            return this._estAffiche;
        }

        public void avancerMissile(float fps_fix)
        {
            if (_emplacement.X < 1150)
                 
                _emplacement += fps_fix * _deplacementDirectionX * _vitesseMissile;
            else
                this._estAffiche = false;
        }
        public void avancerMissile_enemi1(float fps_fix)
        {
            if ((_emplacement.X < 1150)&&(_emplacement.X > 0))
            
               
                    _emplacement -= fps_fix * _deplacementDirectionX * _vitesseMissile;
            else
                this._estAffiche = false;
        }

        public void kill()
        {
            this._existe = false;
            this._estAffiche = false;
        }

        public bool existe
        {
            get { return _existe; }
        }

        public void Update()
        {

        }

        public void Draw(SpriteBatch batch)
        {
            if ((this._estAffiche))
                batch.Draw(_textureMissile, _emplacement, Color.DarkViolet);
        }
    }
}
