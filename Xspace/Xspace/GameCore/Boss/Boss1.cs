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
    
    class Boss1 : Boss
    {
        public Texture2D _T_Missile1, _T_Missile2, _T_Missile3;
        protected int addX, addY, i, i1, i3;
        protected double pausetime, pausetime2, lastpausetime;
        
        public Boss1(Texture2D _sprite, int[] phaseArray)
            : base(_sprite, 2000, 2000, 100, phaseArray, 1, new Vector2(1400, 150), 100, 1000,1, "Spaceship X42")
        {
            addX = 10;
            addY = 10;
            Vitesse = 0.3f;
            pausetime = 800;
            lastpausetime = 0;
            i=0;
            i1 = 0;
            i3 = 0;
        }

        override public void LoadContent(ContentManager content)
        {
            _T_Missile1 = content.Load<Texture2D>("Sprites\\Missiles\\Ennemi\\missile_new1");
            _T_Missile2 = content.Load<Texture2D>("Sprites\\Missiles\\Ennemi\\missile_new1");
            _T_Missile3 = content.Load<Texture2D>("Sprites\\Missiles\\Ennemi\\missile_boule1");
        }

        override public void Update(float fps_fix, double time, List<Missiles> listeMissile)
        {
            checkPhase(fps_fix);
            if ((Existe) && (!Init))
            {
                switch (Phase)
                {
                    case 1:
                        {
                            if ((time - LastTir > _timingAttack) && (i1 < 5))
                            {
                                _timingAttack = 200;
                                Vector2 pos = new Vector2(Position.X, Position.Y + _sprite.Height / 2);
                                listeMissile.Add(new Missile_drone(_T_Missile1, pos, null, this));
                                LastTir = time;
                                pausetime = time;
                                Vitesse = 0.3f;
                                i1++;
                            }
                            else if (time - pausetime > 800)
                            {
                                i1 = 0;
                            }

                        }
                        break;
                    case 2:
                        {
                            Vitesse = 0.7f;
                            pausetime = time;
                            pausetime2 = time;
                        }
                        break;
                    case 3:
                        {
                            if ((time - LastTir > _timingAttack) && (i < 3) && (time - pausetime2 < 4000))
                              {
                                    _timingAttack = 200;
                                    Invincible = true;
                                    Vector2 pos = new Vector2(Position.X - 50, Position.Y + _sprite.Height / 3 - 6);
                                    listeMissile.Add(new Missile1_boss(_T_Missile2, pos, null, this));
                                    LastTir = time;
                                    Vitesse = 0.2f;
                                    pausetime = time;
                                    i++;
                                }
                            else if ((time - LastTir > _timingAttack) && (i3 < 4) && (time - pausetime2 > 4000))
                            {
                                Invincible = false;
                                _timingAttack = 600;
                                Vector2 pos = new Vector2(Position.X - 70, Position.Y + _sprite.Height / 3 - 6);
                                listeMissile.Add(new Blaster_Ennemi(_T_Missile3, pos, null, this));
                                LastTir = time;
                                Vitesse = 0.0f;
                                pausetime = time;
                                i3++;
                            }
                            else if ((time - pausetime > 800) && (i>=3))
                                i = 0;
                            else if (i3 >= 4)
                            {
                                pausetime = time;
                                pausetime2 = time;
                                i3 = 0;
                                i = 0;
                            }
                            
                        }
                        break;
                    default:
                        {
                            if (time - LastTir > Vitesse)
                            {
                                Vector2 pos = new Vector2(Position.X - 35, Position.Y + _sprite.Height / 3 - 6);
                                listeMissile.Add(new Missile2_boss(_T_Missile1, pos, null, this));
                                LastTir = time;
                            }

                        }
                        break;
                }
                //Mouvements
                PositionY += addY * Vitesse;

                if (Position.Y - _sprite.Height / 2 < -_sprite.Height / 2) // haut   
                    addY = -addY;

                if (Position.Y - _sprite.Height / 2 > 380) // bas
                    addY = -addY;

                if (Position.X - _sprite.Width / 2 < 500) // gauche
                    addX = -addX;

                if (Position.X - _sprite.Width / 2 - 10 > 1024) // droite
                    addX = -addX;
            }
            else // Initialisation du boss 
            {

                do
                    PositionX -= addX * 0.1f;
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
