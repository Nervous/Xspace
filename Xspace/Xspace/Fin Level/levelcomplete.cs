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
using Xspace;


namespace Xspace
{
    class levelcomplete
    {
        public void Draw_win(SpriteBatch spriteBatch, Texture2D texture, SpriteFont police, int score)
        {
            spriteBatch.Draw(texture, new Vector2(0, 0), Color.White);
            spriteBatch.DrawString(police, Convert.ToString(score), new Vector2(500, 500), Color.Red);
        }
    }
}
