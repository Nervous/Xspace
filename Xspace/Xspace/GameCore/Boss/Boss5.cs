using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Xspace
{
    class Boss5 : Boss
    {
        public Texture2D T_Partiel;
        protected int addX, addY;

        public Boss5(Texture2D _sprite, int[] phaseArray)
            : base(_sprite, 1700, 1700, 1000, phaseArray, 1, new Vector2(1200, 250), 100, 1000, 2, "Nathalisboulax")
        {
            addX = 10;
            addY = 10;
            Vitesse = 2f;
            base._origin = new Vector2(_sprite.Width / 2, _sprite.Height / 2);
        }

        override public void LoadContent(ContentManager content)
        {
            T_Partiel = content.Load<Texture2D>("Sprites\\Missiles\\Ennemi\\Calvin");
        }

        override public void Update(float fps_fix, double time, List<Missiles> listeMissile, List<Vaisseau> listeVaisseau)
        {
            checkPhase(fps_fix);
            if ((Existe) && (!Init))
            {
                switch (Phase)
                {
                    case 1:
                        break;
                    default:
                        if (time - LastTir > _timingAttack)
                        {
                            Vector2 pos = new Vector2(Position.X - 35, Position.Y + _sprite.Height / 3 - 6);
                            listeMissile.Add(new Boss5_partiel(T_Partiel, pos, null, this));
                            LastTir = time;
                        }
                        break;
                }

                PositionY += addY * Vitesse;
                _angle -= 5;

                if (Position.Y - _sprite.Height / 2 < 50) // haut   
                    addY = -addY;

                if (Position.Y - _sprite.Height / 2 > 400) // bas
                    addY = -addY;
            }
            else // Initialisation du boss 
            {
                do
                {
                    PositionX -= addX * 0.1f;
                }
                while (Position.X - _sprite.Width / 2 - 10 < 850);

                if (Position.X - _sprite.Width / 2 - 10 <= 851)
                {
                    Init = false;
                    Invincible = false;
                }
            }
            updateRectangle();
        }
    }
}
