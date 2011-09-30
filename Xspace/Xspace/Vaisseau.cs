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
    class Vaisseau
    {
        protected Vector2 _emplacement, _deplacementDirectionY, _deplacementDirectionX;
        protected Texture2D _textureVaisseau;
        protected float _vitesseVaisseau;
        protected int _vie, _armure, _timingAttack, lastTime;
        protected bool _existe;
        protected string _typeVaisseau;
        protected bool _ennemi;

        public Vaisseau(Texture2D texture)
        {
            _textureVaisseau = texture;
            _vie = 100;
            _armure = 0;
            _vitesseVaisseau = 0.45f;
            _emplacement = Vector2.Zero;
            _deplacementDirectionX = Vector2.Normalize(new Vector2(5, 0));
            _deplacementDirectionY = Vector2.Normalize(new Vector2(0, 5));
        }

        protected bool constr(Texture2D texture, int vie, int armure, float vitesseVaisseau, Vector2 startPosition, bool bexiste, bool ennemi, int timingAttack)
        {
            _textureVaisseau = texture;
            _vie = vie;
            _armure = armure;
            _vitesseVaisseau = vitesseVaisseau;
            _emplacement = startPosition;
            _deplacementDirectionX = Vector2.Normalize(new Vector2(5, 0));
            _deplacementDirectionY = Vector2.Normalize(new Vector2(0, 5));
            _existe = bexiste;
            _ennemi = ennemi;
            _timingAttack = timingAttack;
            return true;
        }

        public Vaisseau(Texture2D texture, string typeVaisseau)
        {
            _typeVaisseau = typeVaisseau;
            switch (typeVaisseau)
            {
                case "drone":
                    constr(texture, 100, 0, 0.20f, new Vector2(750, 225), true, true, 500);
                    break;
                default:
                    constr(texture, 100, 0, 0.70f, new Vector2(750, 225), true, true, 500);
                    break;
            }
        }

        public Vaisseau(Texture2D texture, int vie, int armure, float vitesseVaisseau, Vector2 startPosition)
        {
            _textureVaisseau = texture;
            _vie = vie;
            _armure = armure;
            _vitesseVaisseau = vitesseVaisseau;
            _emplacement = startPosition;
            _deplacementDirectionX = Vector2.Normalize(new Vector2(5, 0));
            _deplacementDirectionY = Vector2.Normalize(new Vector2(0, 5));
            _existe = false;
        }

        public Vaisseau(Texture2D texture, int vie, int armure, float vitesseVaisseau, Vector2 startPosition, bool bexiste)
        {
            _textureVaisseau = texture;
            _vie = vie;
            _armure = armure;
            _vitesseVaisseau = vitesseVaisseau;
            _emplacement = startPosition;
            _deplacementDirectionX = Vector2.Normalize(new Vector2(5, 0));
            _deplacementDirectionY = Vector2.Normalize(new Vector2(0, 5));
            _existe = bexiste;
        }

        public int vie
        {
            get { return _vie; }
        }

        public bool existe
        {
            get { return _existe; }
        }

        public void kill()
        {
            this._existe = false;
        }

        public Vector2 position
        {
            get { return _emplacement; }
        }

        public Texture2D sprite
        {
            get { return _textureVaisseau; }
        }

        public void creer()
        {
            this._existe = true;
        }

        public bool hurt(int ammount)
        {
            this._vie -= ammount;
            return (this._vie <= 0);
        }

        public void Update(float fps_fix)
        {
        }

        public void Draw(SpriteBatch batch, Viewport viewport)
        {
            Vector2 origin;
            origin.X = _textureVaisseau.Width / 2;
            origin.Y = _textureVaisseau.Height / 2;
            if (existe)
            {
                /*if (_ennemi)
                    batch.Draw(_textureVaisseau, _emplacement, null, Color.White, MathHelper.Pi, origin, 1.0f, SpriteEffects.None, 0f);
                else */
                    batch.Draw(_textureVaisseau, _emplacement, Color.White);
            }
        }
    }
}
