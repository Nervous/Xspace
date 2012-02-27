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
    class RapidShooter : Vaisseau
    {
        public RapidShooter(Texture2D sprite, Vector2 position)
            : base(sprite, 40, 0, 0,0,0, 30, 0.30f, position, true, 1300, 15, 1)
        { }
    }
}
