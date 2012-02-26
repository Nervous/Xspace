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
    class Drone : Vaisseau
    {
        public Drone(Texture2D sprite, Vector2 position)
            : base(sprite, 20,0,0,0, 0, 30, 0.30f, position, true, 1200, 10, 0)
        { }
    }
}
