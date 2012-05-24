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
    class BC : Vaisseau
    {
        public BC(Texture2D sprite, Vector2 position)
            : base(sprite, 50, 50, 0, 0, 30, 0.20f, position, Vector2.Normalize(new Vector2(1, 0)), true, 9000, 150, 4)
        {
            this.chargement = false;
            this.tir = false;
        }

        /*public void Update(float fps_fix)
        {
        }*/
    }
}
