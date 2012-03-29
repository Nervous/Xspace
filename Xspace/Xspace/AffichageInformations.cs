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
    class AffichageInformations
    {

        private Vector2 _position_bar;
        
        public void Drawbar(SpriteBatch spriteBatch, Texture2D texture, int vieActuelle, int vieMax)
        {
            _position_bar.Y = 662;
            for (int i=0; i <= vieActuelle;i++)
            {
                _position_bar.X = 405 + i;

                spriteBatch.Draw(texture, _position_bar, Color.White);

            }
        }
    }
}
