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
    class kamikaze : Vaisseau
    {
        public kamikaze(Texture2D sprite, Vector2 position)
            : base(sprite, 1, 0, 0, 100, 1f, position, true, 10000, 15, 0)
        { }
    }
}
