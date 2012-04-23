using MenuSample.Scenes.Core;
using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MenuSample.Scenes
{
    public class OptionMenuScene : AbstractMenuScene
    {

        //private ContentManager _content;
        //private SpriteFont _gamefont;
        protected Microsoft.Xna.Framework.GraphicsDeviceManager graphics;


        public OptionMenuScene(SceneManager sceneMgr,Microsoft.Xna.Framework.GraphicsDeviceManager gr)
            : base(sceneMgr, "Options")
        {

            var back = new MenuItem("Retour");
            var full_screen = new MenuItem("Plein ecran");
          

            full_screen.Selected += Full_ScreenMenuItemSelected;
            back.Selected += OnCancel;
            this.graphics = gr;

            MenuItems.Add(full_screen);
            MenuItems.Add(back);
           

        }

        private void Full_ScreenMenuItemSelected(object sender, EventArgs e)
        {
            graphics.ToggleFullScreen();
        }

        

    }
}