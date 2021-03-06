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
    class Boss4 : Boss
    {
        public Texture2D _T_Missile3, _T_phase2, _T_phase3, _T_Cercle;
        protected int addX, addY, i, i1, i3, type, _timingAttack2;
        protected double pausetime, lastpausetime;

        public Boss4(Texture2D _sprite, int[] phaseArray)
            : base(_sprite, 1700, 1700, 100, phaseArray, 1, new Vector2(1400, 150), 100, 1000, 2, "The All-Seeing Eye")
        {
            addX = 10;
            addY = 10;
            Vitesse = 0.3f;
            pausetime = 800;
            lastpausetime = 0;
            i = 0;
            i1 = 0;
            i3 = 0;
            type = 0;
            _timingAttack2 = 1500;
        }

        override public void LoadContent(ContentManager content)
        {
          
            _T_Missile3 = content.Load<Texture2D>("Sprites\\Missiles\\Ennemi\\missile_boule1");
            _T_phase2 = content.Load<Texture2D>("Sprites\\Vaisseaux\\Boss\\oeil2");
            _T_phase3 = content.Load<Texture2D>("Sprites\\Vaisseaux\\Boss\\oeil3");
            _T_Cercle = content.Load<Texture2D>("Sprites\\Vaisseaux\\Boss\\cercle");
        }

        override public void Update(float fps_fix, double time, List<Missiles> listeMissile, List<Vaisseau> listeVaisseau)
        {
            checkPhase(fps_fix);
            if ((Existe) && (!Init))
            {
                switch (Phase)
                {
                    case 1:
                        {
                            if (time - LastTir > _timingAttack)
                            {
                                _timingAttack = 1800;
                                Vector2 pos = new Vector2(Position.X, Position.Y + _sprite.Height / 2);
                               listeMissile.Add(new Missile1_boss(_T_Missile3, new Vector2(pos.X, pos.Y - 20), null, this));
                                LastTir = time;
                                pausetime = time;
                                Vitesse = 0.2f;
                            }

                        }
                        break;
                    case 2:
                        {
                            if (i1 < 4)
                            {
                                if (time - LastTir > _timingAttack)
                                {
                                    _invincible = true;
                                    color = true;
                                    addX = 0;
                                    addY = 0;
                                    _timingAttack = 1800;
                                    Vector2 pos = new Vector2(Position.X, Position.Y + _sprite.Height / 2);
                                    Vitesse = 0.1f;
                                    LastTir = time;
                                    Random r = new Random();
                                    int nbX = r.Next(100, 1000);
                                    int nbY = r.Next(150, 320);
                                    PositionX = nbX;
                                    PositionY = nbY;
                                    i = 0;
                                    i1++;
                                }

                                if (time - LastTir < _timingAttack2)
                                {
                                    _sprite = _T_phase2;
                                    _timingAttack2 = 1000;

                                }
                                else
                                {
                                    if (i == 0)
                                    {
                                        PositionX -= 200;
                                        PositionY -= 200;
                                        i++;
                                    }

                                    listeVaisseau[0]._vitesseVaisseau = 0.3f;
                                    if(_invincible)
                                    _sprite = _T_Cercle;
                                }
                            }
                            else if (i1 < 15)
                            {
                                listeVaisseau[0]._vitesseVaisseau = 0.7f;
                                if (time - LastTir > _timingAttack)
                                {
                                    _invincible = false;
                                    _sprite = _T_phase3;
                                    addX = 0;
                                    addY = 0;
                                    _timingAttack = 1000;
                                    Vector2 pos = new Vector2(Position.X, Position.Y + _sprite.Height / 2);
                                    listeMissile.Add(new Missile1_boss(_T_Missile3, new Vector2(pos.X, pos.Y-30), null, this));
                                    Vitesse = 0.1f;
                                    PositionX = 800;
                                    PositionY = 300;
                                    LastTir = time;
                                    i1++;
                                }
                            }
                            else
                                i1 = 0;

                        }
                        break;
                    default:
                        {
                            if (time - LastTir > Vitesse)
                            {
                                Vector2 pos = new Vector2(Position.X - 35, Position.Y + _sprite.Height / 3 - 6);
                                listeMissile.Add(new Missile2_boss(_T_Missile3, pos, null, this));
                                LastTir = time;
                            }

                        }
                        break;
                }
                //Mouvements
                PositionY += addY * Vitesse;

                if (Phase == 2)
                {
                    if (Position.Y - _sprite.Height / 2 < -_sprite.Height / 2 - 1000) // haut   
                        addY = -addY;
                }
                else
                {
                    if (Position.Y - _sprite.Height / 2 < -_sprite.Height / 2) // haut   
                        addY = -addY;
                }

                    if (Position.Y - _sprite.Height / 2 > 500) // bas
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
                while (Position.X - _sprite.Width / 2 - 10 < 760);

                if (Position.X - _sprite.Width / 2 - 10 <= 761)
                {
                    Init = false;
                    Invincible = false;
                }
            }
            updateRectangle();
        }

    }
}
