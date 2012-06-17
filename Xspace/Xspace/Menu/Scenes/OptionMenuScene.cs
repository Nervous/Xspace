using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;

using MenuSample.Inputs;
using MenuSample.Scenes;
using MenuSample.Scenes.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using ProjectMercury;
using ProjectMercury.Emitters;
using ProjectMercury.Modifiers;
using ProjectMercury.Renderers;

using Xspace;
using System.Security.Cryptography;

namespace MenuSample.Scenes
{
    public class OptionMenuScene : AbstractMenuScene
    {

        //private ContentManager _content;
        //private SpriteFont _gamefont;
        protected Microsoft.Xna.Framework.GraphicsDeviceManager graphics;
        protected string easy="Facile", moyen1="Moyen", hard="Difficile", hardcore1="Hardcore";

        public OptionMenuScene(SceneManager sceneMgr,Microsoft.Xna.Framework.GraphicsDeviceManager gr)
            : base(sceneMgr, "Options")
        {

             FileStream fs1 = new FileStream("DIFF", FileMode.OpenOrCreate);
            StreamReader sr = new StreamReader(fs1);
            int nb = int.Parse(sr.ReadToEnd());

            if (sr.ReadToEnd().Length == 0)
                easy = "Facile: En cours.";
            else
            {
                switch (nb)
                {
                    case 0:
                            easy = "Facile: En cours.";
                            break;
                    case 1:
                            moyen1 = "Moyen: En cours.";
                            break;
                    case 2:
                        hard = "Difficile: En cours.";
                        break;
                    case 3:
                        hardcore1 = "Hardcore: En cours.";
                        break;
                    default:
                        break;
                }
            }
            sr.Close();
            fs1.Close();
            var back = new MenuItem("Retour");
            var facile = new MenuItem(easy);
            var moyen = new MenuItem(moyen1);
            var difficile = new MenuItem(hard);
            var hardcore = new MenuItem(hardcore1);


            facile.Selected += FacileMenuItemSelected;
            moyen.Selected += MoyenMenuItemSelected;
            difficile.Selected += DifficileMenuItemSelected;
            hardcore.Selected += HardcoreMenuItemSelected;
            back.Selected += OnCancel;

            MenuItems.Add(facile);
            MenuItems.Add(moyen);
            MenuItems.Add(difficile);
            MenuItems.Add(hardcore);
            MenuItems.Add(back);
           

        }

        private void FacileMenuItemSelected(object sender, EventArgs e)
        {
            FileStream fs = new FileStream("DIFF", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(0);
            sw.Close();
            fs.Close();
        }

        private void MoyenMenuItemSelected(object sender, EventArgs e)
        {
            FileStream fs = new FileStream("DIFF", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(1);
            sw.Close();
            fs.Close();

        }

        private void DifficileMenuItemSelected(object sender, EventArgs e)
        {
            FileStream fs = new FileStream("DIFF", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(2);
            sw.Close();
            fs.Close();
        }

        private void HardcoreMenuItemSelected(object sender, EventArgs e)
        {
            FileStream fs = new FileStream("DIFF", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(3);
            sw.Close();
            fs.Close();
        }

        

    }
}