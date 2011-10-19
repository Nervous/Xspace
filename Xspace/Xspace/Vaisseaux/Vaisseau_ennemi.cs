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
    class Vaisseau_ennemi : Vaisseau
    {

        public Vaisseau_ennemi(Texture2D sprite, string typeVaisseau, string position)
            : base(sprite, typeVaisseau, position)
        { }

        new public void Update(float fps_fix)
        {
            switch (_typeVaisseau)
            {
                case "drone":
                    if (_emplacement.X + _textureVaisseau.Width >= 0)
                        _emplacement -= _deplacementDirectionX * _vitesseVaisseau * fps_fix;
                    else
                        this.kill();
                    break;

                default:
                    if (_emplacement.X + _textureVaisseau.Width >= 0)
                        _emplacement -= _deplacementDirectionX * _vitesseVaisseau * fps_fix;
                    else
                        this.kill();
                    break;
            }

            
        }
    }
}
