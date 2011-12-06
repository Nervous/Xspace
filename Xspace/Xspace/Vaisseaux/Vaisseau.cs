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
        public Vector2 _emplacement, _deplacementDirectionY, _deplacementDirectionX;
        public Texture2D _textureVaisseau;
        public float _vitesseVaisseau;
        protected int _vie, _armure;
        protected double _timingAttack;
        protected bool _existe;
        protected string _typeVaisseau, _position;
        protected bool _ennemi;
        protected double _lastTir;
        


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

        protected bool constr(Texture2D texture, int vie, int armure, float vitesseVaisseau, Vector2 startPosition, bool bexiste, bool ennemi, double timingAttack)
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

        public Vaisseau(Texture2D texture, string typeVaisseau, string position)
        {
            Vector2 startPosition;
            _typeVaisseau = typeVaisseau;
            _position = position;
            if (position == "bas")
                startPosition = new Vector2(1120, 500);
            else if (position == "haut")
                startPosition = new Vector2(1120, 50);
            else
                startPosition = new Vector2(1120, 225);
            switch (typeVaisseau)
            {
                case "drone":
                    constr(texture, 100, 0, 0.20f, startPosition, true, true, 500);
                    break;
                default:
                    constr(texture, 100, 0, 0.70f, startPosition, true, true, 500);
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

        public double timingAttack
        {
            get { return _timingAttack; }
        }

        public double lastTir
        {
            get { return _lastTir; }
            set {this._lastTir = value;}
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

        public bool ennemi
        {
            get { return _ennemi; }
        }

        public void Update(float fps_fix)
        {
            if (_emplacement.X >= 0)
                _emplacement -= _deplacementDirectionX * _vitesseVaisseau * fps_fix;
            else
                this.kill();
        }


        public void Draw(SpriteBatch batch)
        {
            if (existe)
            {
                batch.Draw(_textureVaisseau, _emplacement, Color.White);
            }
        }
    }
}
