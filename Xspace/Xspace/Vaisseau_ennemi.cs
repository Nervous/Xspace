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
    class Vaisseau_ennemi : Vaisseau
    {

        public Vaisseau_ennemi(Texture2D sprite, string typeVaisseau)
            : base(sprite, typeVaisseau)
        { }

        //base.constr(sprite, 100, 0, 0.70f, new Vector2(750, 225), true);


        public void Update(float fps_fix)
        {
        }
    }
}
