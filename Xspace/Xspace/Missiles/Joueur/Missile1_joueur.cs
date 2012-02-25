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
    class Missile1_joueur : Missiles
    {
        public Missile1_joueur(Texture2D texture, Vector2 position)
            : base(texture, false, 20, position, new Vector2(7, 0), new Vector2(0, 0), 1.0f)
        { }
    }
}
