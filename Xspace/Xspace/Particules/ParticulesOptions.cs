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
    public class ParticulesOptions
    {
        private int p;
        private Color color;

        public double ActifTime { get; set; } // temps d'activité restante de la particule
        public Color CouleurInit { get; set; }

        public struct ParticulesOptions
        {
            public Color CouleurInit { get; set; } // couleur des particules

            public int ParticulesMax { get; set; } // nombre max de particules
            public double ActifTime { get; set; }

            public ParticulesOptions(double actifTime, Color couleurInit, int particulesMax = 300)
                : this()
            {
                CouleurInit = couleurInit;
                ActifTime = actifTime;
                ParticulesMax = particulesMax;
            }
        }





        public int ParticulesMax { get; set; }
    }

}
