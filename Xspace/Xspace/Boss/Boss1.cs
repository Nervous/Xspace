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

        public Boss1(Texture2D texture, int[] phaseArray)
            : base(texture, 1000, 1000, 1000, phaseArray, 1, new Vector2(500, 500), 100, 1000)
        {
            _texture = texture;
        }

        public void LoadContent(ContentManager content)
        {
            _T_Missile1 = content.Load<Texture2D>("Sprites\\Missiles\\Ennemi\\missile_new1");
            _T_Missile2 = content.Load<Texture2D>("Sprites\\Missiles\\Ennemi\\drone");
            _T_Missile3 = content.Load<Texture2D>("Sprites\\Missiles\\Ennemi\\missile_boule1");
        }

        public void Update(float fps_fix, double time, List<Missiles> listeMissile)
        {
            Update(fps_fix);
            if(Existe)
            switch (Phase)
            {
                case 1:
                    {
                        if (time - LastTir > _timingAttack)
                        {
                            Vector2 pos = new Vector2(Position.X - 35, Position.Y + _texture.Height / 3 - 6);
                            listeMissile.Add(new Missile_drone(_T_Missile1, pos));
                            LastTir = time;
                            
                        }
                        
                    } 
                    break;
                case 2:
                    {
                        if (time - LastTir > _timingAttack)
                        {
                            Vector2 pos = new Vector2(Position.X - 35, Position.Y + _texture.Height / 3 - 6);
                            listeMissile.Add(new Missile1_boss(_T_Missile2, pos));
                            LastTir = time;
                        }

                    } 
                    break;
                case 3:
                    {
                        if (time - LastTir > _timingAttack)
                        {
                            Vector2 pos = new Vector2(Position.X - 35, Position.Y + _texture.Height / 3 - 6);
                            listeMissile.Add(new Missile2_boss(_T_Missile3, pos));
                            LastTir = time;
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

        }





    }
}
