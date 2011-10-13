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
    public struct ParticulesOptions
    {
        private int p;
        private Color color;
        public double AjoutFrequence { get; set; }
        public int ParticulesParAjout { get; set; }
        public Func<Vector2, double, Vector2> Vitesse { get; set; }
        public Func<Vector2, Vector2> Position { get; set; }
        public float TailleInit { get; set; }
        public float TailleFin { get; set; }
            
        public Color CouleurInit { get; set; } // couleur des particules
        public Color CouleurFin { get; set; }

        public int ParticulesMax { get; set; } // nombre max de particules
        public double ActifTime { get; set; }

        public ParticulesOptions(double actifTime, Color couleurInit, Color couleurFin, int particulesMax = 300, double ajoutFrequence = 0, int particulesParAjout = 1,
            Func<Vector2, double, Vector2> vitesse = null, Func<Vector2, Vector2> position = null, float tailleInit = 1, float tailleFin = 1)
            : this()
            {
                CouleurInit = couleurInit;
                CouleurFin = couleurFin;
                ActifTime = actifTime;
                ParticulesMax = particulesMax;
                AjoutFrequence = ajoutFrequence;
                ParticulesParAjout = particulesParAjout;
                Vitesse = vitesse;
                Position = position;
                TailleInit = tailleInit;
                TailleFin = tailleFin;
            }

       
    }

}
