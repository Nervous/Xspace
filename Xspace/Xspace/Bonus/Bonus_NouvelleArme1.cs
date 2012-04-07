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
    class Bonus_NouvelleArme1 : Bonus
    {
        public Bonus_NouvelleArme1(Texture2D texture, Vector2 position)
            : base(texture, 0.30f, position, "weapon", 1, -1, 10)
        { }
    }
}
