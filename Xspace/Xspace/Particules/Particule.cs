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
    class Particule
    {
        private readonly ParticuleOptions _options;
        private readonly ParticuleMgr _mgr;

        public bool Active; // etat de la particule, active ou non

        public Vector2 Position { get; set; } // vecteur de position courante de la particule
        public double ActifTime { get; set; } // temps d'activité restante de la particule
        public Vector2 Vitesse { get; set; }
    }
}
