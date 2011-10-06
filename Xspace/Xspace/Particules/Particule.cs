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
        private readonly ParticulesOptions _options;
        private readonly ParticulesMgr _mgr;

        public bool Active; // etat de la particule, active ou non

        public Vector2 Position { get; set; } // vecteur de position courante de la particule
        public double ActifTime { get; set; } // temps d'activité restante de la particule
        public Vector2 Vitesse { get; set; }


        public Particule(ParticulesMgr mgr)
        {
            _options = mgr.Options;
            _mgr = mgr;
        }

        public void Reset()
        {
            Position = _mgr.Position;
            ActifTime = _options.ActifTime;// ParticuleMgr permet de creer le nombre max de particules puis de les remettre à zero lorsqu'elles sont inactives, d'où la fct Reset
            Active = true;
        }

        public void Draw(SpriteBatch particule_sp, Texture2D texture_particule)
        {
            if (!Active)
                return;

            particule_sp.Draw(texture_particule, Position, null, _options.CouleurInit, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        }

        public void Update(GameTime gameTime)
        {
            if (!Active)
                return;
            Position += Vitesse;

            ActifTime -= gameTime.ElapsedGameTime.TotalMilliseconds;
            if (ActifTime < 0)
                Active = false;
        }
    }
}
