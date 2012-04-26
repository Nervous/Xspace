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
    class Vaisseau_joueur : Vaisseau
    {
        public Vaisseau_joueur(Texture2D sprite)
            : base(sprite, 300, 300, 100, 100, 100, -1, 0.70f, new Vector2(15, 225), Vector2.Normalize(new Vector2(1, 1)), false, 0, 0, 0)
        { }
    }

}
