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

using Xspace.Son;

namespace Xspace
{
    class ScrollingBackground
    {


        // scrolling background

        private Vector2 screenposition, origine, texturesize;
        private Texture2D ma_texture;
        private int screenHeight;
        private float vitesseBackground;


        public ScrollingBackground() 
        {
            vitesseBackground = 0.6f; 
        }

        public void Load(GraphicsDevice device, Texture2D backgroundTexture)
        {
            ma_texture = backgroundTexture;
            screenHeight = device.Viewport.Height;
            int screenWidth = device.Viewport.Width;
            // permet de definir l'origine du fond
            // haut, centre ->
            origine = new Vector2(0, ma_texture.Height / 2);
            // Place l'ecran au centre de l'image
            screenposition = new Vector2(screenWidth / 2, screenHeight / 2);
            texturesize = new Vector2(ma_texture.Width, 0);
        }
        public void Update(float dX)
        {

            screenposition.X -= dX * vitesseBackground;
            //screenposition.X -= dX * AudioPlayer.GetFreq() / 100000;

            screenposition.X = screenposition.X % ma_texture.Width;




        }

        public void Draw(SpriteBatch batch)
        {
            // Dessin de la texture
            if (screenposition.X < screenHeight)
            {
                batch.Draw(ma_texture, screenposition, null,
                     Color.White, 0, origine, 1, SpriteEffects.None, 0f);
            }
            // Redessine la texture pour le scrolling
            batch.Draw(ma_texture, screenposition + texturesize, null,
                 Color.White, 0, origine, 1, SpriteEffects.None, 0f);
        }
    }
}