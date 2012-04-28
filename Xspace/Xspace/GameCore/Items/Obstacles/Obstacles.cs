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
    class Obstacles : item
    {
        protected float _vitesseObstacle;
        protected string _categorie;
        int lastDeg;

        public Obstacles(Texture2D texture, float vitesse, Vector2 startPosition, Vector2 deplacement, string categorie)
            :base(texture, startPosition, deplacement, 0, 0)
        {
            _vitesseObstacle = vitesse;
            //_deplacement = Vector2.Normalize(new Vector2(5, 0));
            _categorie = categorie;
            lastDeg = 0;
        }

        public string Categorie
        {
            get { return _categorie; }
        }

        public Texture2D sprite
        {
            get { return _sprite; }
        }

        public void Update(float fps_fix)
        {
            _pos -= _deplacement * _vitesseObstacle * fps_fix;
            updateRectangle();
        }

        public void Draw(SpriteBatch batch)
        {
            //batch.Draw(_textureObstacle, _emplacement, Color.White);
            batch.Draw(_sprite,                                  // Texture (Image)
                     _pos,                               // Position de l'image
                     null,                                       // Zone de l'image à afficher
                     Color.White,                                // Teinte
                     MathHelper.ToRadians(lastDeg--),       // Rotation (en rad)
                     new Vector2(_sprite.Width / 2, _sprite.Height / 2),  // Origine
                     1.0f,                                       // Echelle
                     SpriteEffects.None,                         // Effet
                     0);                                         // Profondeur
        }
    }
}
