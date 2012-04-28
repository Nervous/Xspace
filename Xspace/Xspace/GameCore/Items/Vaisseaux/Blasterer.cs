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
    class Blasterer : Vaisseau
    {
        public Blasterer(Texture2D sprite, Vector2 position)
            : base(sprite, 80, 0,0, 0, 30, 0.09f, position, Vector2.Normalize(new Vector2(1, 0)), true, 2400, 130, 2)
        { }
    }
}
