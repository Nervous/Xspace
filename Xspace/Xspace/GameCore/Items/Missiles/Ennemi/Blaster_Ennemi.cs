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
    class Blaster_Ennemi : Missiles
    {
        public Blaster_Ennemi(Texture2D texture, Vector2 position, Vaisseau owner, Boss ownerB)
            : base(texture, true, 50, position, new Vector2(-2, 0), 0.30f, owner, ownerB)
        { }
    }
}
