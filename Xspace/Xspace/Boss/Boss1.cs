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

namespace Xspace.Boss
{
    
    class Boss1 : Boss
    {
        public Texture2D _T_Missile1, _T_Missile2, _T_Missile3;
        protected int addX, addY, i;
        protected double pause;
        
        public Boss1(Texture2D texture, int[] phaseArray)
            : base(texture, 1000, 1000, 100, phaseArray, 1, new Vector2(1400, 150), 100, 1000)
        {
            _texture = texture;
            addX = 10;
            addY = 10;
            Vitesse = 0.3f;
            pause = 5000;
            i=0;
        }

        public void LoadContent(ContentManager content)
        {
            _T_Missile1 = content.Load<Texture2D>("Sprites\\Missiles\\Ennemi\\missile_new1");
            _T_Missile2 = content.Load<Texture2D>("Sprites\\Missiles\\Ennemi\\drone");
            _T_Missile3 = content.Load<Texture2D>("Sprites\\Missiles\\Ennemi\\drone");
        }

        public void Update(float fps_fix, double time, List<Missiles> listeMissile)
        {
            Update(fps_fix);
            if ((Existe) && (!Init))
            {
                switch (Phase)
                {
                    case 1:
                        {
                            if (time - LastTir > _timingAttack)
                            {
                                _timingAttack = 500;
                                Vector2 pos = new Vector2(Position.X - 35, Position.Y + _texture.Height / 3 - 6);
                                listeMissile.Add(new Missile_drone(_T_Missile1, pos));
                                LastTir = time;
                                Vitesse = 0.5f;
                            }

                        }
                        break;
                    case 2:
                        {
                            if ((time - LastTir > _timingAttack) && (i < 10))
                            {
                                _timingAttack = 200;
                                Invincible = false;
                                Vector2 pos = new Vector2(Position.X - 35, Position.Y + _texture.Height / 3 - 6);
                                listeMissile.Add(new Missile1_boss(_T_Missile2, pos));
                                LastTir = time;
                                Vitesse = 0.2f;
                                i++;
                            }
                            else if ((i >= 10) && (i < 30) && (time - LastTir > _timingAttack)) // ouai, on aurait pu mettre dans le meme if mais non, explain later
                            {
                                _timingAttack = 200;
                                Invincible = true;
                                Vector2 pos = new Vector2(Position.X - 35, Position.Y + _texture.Height / 3 - 6);
                                listeMissile.Add(new Missile1_boss(_T_Missile2, pos));
                                LastTir = time;
                                Vitesse = 0.4f;
                                i++;
                            }
                            else if (i >= 30) 
                                i = 0;
                            
                        }
                        break;
                    case 3:
                        {
                           
                            if (time - LastTir > _timingAttack)
                            {
                                _timingAttack = 300;
                                Vector2 pos = new Vector2(Position.X - 35, Position.Y + _texture.Height / 3 - 6);
                                listeMissile.Add(new Missile2_boss(_T_Missile3, pos));
                                LastTir = time;
                                Vitesse = 0.6f;
                            }

                        }
                        break;
                    default:
                        {
                            if (time - LastTir > Vitesse)
                            {
                                Vector2 pos = new Vector2(Position.X - 35, Position.Y + _texture.Height / 3 - 6);
                                listeMissile.Add(new Missile2_boss(_T_Missile1, pos));
                                LastTir = time;
                            }

                        }
                        break;
                }
                //Mouvements
                PositionY += addY * Vitesse;

                if (Position.Y - Texture.Height / 2 < -Texture.Height / 2 + 100 ) // haut   
                    addY = -addY;

                if (Position.Y - Texture.Height / 2 > 400) // bas
                    addY = -addY;

                if (Position.X - Texture.Width / 2 < 500) // gauche
                    addX = -addX;

                if (Position.X - Texture.Width / 2 - 10 > 1024) // droite
                    addX = -addX;
            }
            else // Initialisation du boss 
            {

                do
                    PositionX -= addX * 0.1f;
                while ((Position.X - Texture.Width / 2 - 10 < 850));

                if (((Position.X - Texture.Width / 2 - 10 <= 851)))
                {
                    Init = false;
                    Invincible = false;
                }

            }

        }



    }
}
