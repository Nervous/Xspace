﻿using System;
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
    class Bonus_Vie : Bonus
    {
        public Bonus_Vie(Texture2D texture, Vector2 position)
            : base(texture, 0.10f, position, "vie", 30, -1)
        { }
    }
}