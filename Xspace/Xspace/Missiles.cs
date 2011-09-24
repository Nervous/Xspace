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
        protected Vector2 _emplacement, _deplacementDirectionY, _deplacementDirectionX;
        protected Texture2D _textureMissile;
        protected int _vie, _degats;
        protected float _vitesseMissile;

        public Missiles(Texture2D texture)
        {
            _textureMissile = texture;
            _vie = 1;
            _degats = 10;
            _vitesseMissile = 0.45f;
            _emplacement = Vector2.Zero;
            _deplacementDirectionX = Vector2.Normalize(new Vector2(7, 0));
            _deplacementDirectionY = Vector2.Normalize(new Vector2(0, 7));
        }

        public Missiles(Texture2D texture, int vie, int armure, float vitesseMissile, Vector2 startPosition)
        {
            _textureMissile = texture;
            _vie = vie;
            _degats = 10;
            _vitesseMissile = vitesseMissile;
            _emplacement = startPosition;
            _deplacementDirectionX = Vector2.Normalize(new Vector2(7, 0));
            _deplacementDirectionY = Vector2.Normalize(new Vector2(0, 7));
        }
    }
}
