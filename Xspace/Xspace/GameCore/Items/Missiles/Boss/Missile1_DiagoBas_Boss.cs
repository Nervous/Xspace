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
    class Missile1_DiagoHaut_Boss : Missiles
    {
        public Missile1_DiagoHaut_Boss(Texture2D texture, Vector2 position, Vaisseau owner, Boss ownerB)
            : base(texture, true, 20, position, new Vector2(-1, -1), 0.60f, owner, ownerB)
        { }
    }
}