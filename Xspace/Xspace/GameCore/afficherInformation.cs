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

        private Vector2 _position_bar, _position_energy;

        public void Drawbar(SpriteBatch spriteBatch, Texture2D texture_vie, Texture2D texture_energy, int vieActuelle, int vieMax, int energieActuelle, int energieMax)
        {
            _position_bar.Y = 662;
            _position_energy.Y = 705;
            for (int i = 0; i <= vieActuelle/1.66; i++)
            {
                _position_bar.X = 405 + i;

                spriteBatch.Draw(texture_vie, _position_bar, Color.White);

            }

            for (int i = 0; i <= energieActuelle/3.33; i++)
            {
                _position_energy.X = 405 + i;

                spriteBatch.Draw(texture_energy, _position_energy, Color.White);

            }
        }
    }
}
