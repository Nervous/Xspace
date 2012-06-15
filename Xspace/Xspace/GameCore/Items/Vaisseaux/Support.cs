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
    class Support : Vaisseau
    {
        public Support(Texture2D sprite, Vector2 position)
            : base(sprite, 50, 0, 0, 0, 20, 0.50f, position, Vector2.Normalize(new Vector2(1, 0)), true, 6000, 50, 5)
        { }
    }
}
