using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Xspace
{
    class Boss5_partiel : Missiles
    {
        public Boss5_partiel(Texture2D texture, Vector2 position, Vaisseau owner, Boss ownerB)
            : base(texture, true, 50, position, new Vector2(-1, 0), 1.0f, owner, ownerB)
        { }
    }
}
