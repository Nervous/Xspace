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
    class Boss2 : Boss
    {
        public Texture2D _T_Missile1, _T_Missile2, _T_Missile3, _T_Missile_Laser, _T_Missile_Diago, _T_Missile_DiagoHaut, _T_Missile_DiagoBas;
        protected int addX, addY, i, i1, i3;
        protected double pausetime, pausetime2, lastpausetime;
        
        public Boss2(Texture2D _sprite, int[] phaseArray)
            : base(_sprite, 2000, 2000, 100, phaseArray, 1, new Vector2(1400, 150), 100, 1000,2, "Metal'Krisboul")
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
            _T_Missile_DiagoHaut = content.Load<Texture2D>("Sprites\\Missiles\\Joueur\\1_DiagoBas");
            _T_Missile_DiagoBas = content.Load<Texture2D>("Sprites\\Missiles\\Joueur\\1_DiagoHaut");
            _T_Missile_Diago = content.Load<Texture2D>("Sprites\\Missiles\\Joueur\\1");
            _T_Missile_Laser = content.Load<Texture2D>("Sprites\\Missiles\\Ennemi\\Laser");
        }

        override public void Update(float fps_fix, double time, List<Missiles> listeMissile)
        {
            checkPhase(fps_fix);
            if ((Existe) && (!Init))
            {
                switch (Phase)
                { 
                    case 2:
                        {
                            if ((time - LastTir > _timingAttack) && (i1 < 1)) // tir de laser
                            {
                                _timingAttack = 200;
                                Vector2 pos = new Vector2(Position.X, Position.Y + _sprite.Height / 2);
                                listeMissile.Add(new Missile2_boss(_T_Missile_Laser, new Vector2(pos.X - 250, pos.Y), null, this));
                                LastTir = time;
                                pausetime = time;
                                Vitesse = 0.01f;
                                i1++;
                            }
                            else if (time - pausetime > 3000)
                            {
                                i1 = 0;
                                Vitesse = 0.5f;
                            }

                        }
                        break;
                    case 1:
                        {
                            Vitesse = 0.2f;
                            pausetime = time;
                            pausetime2 = time;
                        }
                        break;
                    case 3:
                        {
                            if ((time - LastTir > _timingAttack))
                              {
                                    _timingAttack = 300;
                                    Vector2 pos = new Vector2(Position.X - 50, Position.Y + _sprite.Height / 3 - 6);
                                    listeMissile.Add(new Missile1_DiagoBas_Boss(_T_Missile_DiagoBas, pos, null, this));
                                    listeMissile.Add(new Missile1_DiagoHaut_Boss(_T_Missile_DiagoHaut, pos, null, this));
                                    listeMissile.Add(new Missile1_boss(_T_Missile_Diago, pos, null, this));
                                    LastTir = time;
                                    Vitesse = 0.2f;
                                    pausetime = time;
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

                if (Position.Y - _sprite.Height / 2 > 200) // bas
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
