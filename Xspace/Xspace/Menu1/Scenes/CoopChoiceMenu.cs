using System;
using Microsoft.Xna.Framework.Graphics;
using MenuSample.Scenes.Core;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Xspace.Son;

namespace MenuSample.Scenes
{
    /// <summary>
    /// Le menu principal est la première chose affichée lors du lancement du binaire
    /// </summary>
    public class CoopChoiceMenu : AbstractMenuScene
    {

        /// <summary>
        /// Le constructeur remplit le menu
        /// </summary>
        /// 

        protected int _act, _level;
        private ContentManager _content;

        Microsoft.Xna.Framework.GraphicsDeviceManager graphics;
        public CoopChoiceMenu(SceneManager sceneMgr, Microsoft.Xna.Framework.GraphicsDeviceManager graphicsReceive)
            : base(sceneMgr, "")
        {

            // Création des options
            var joinMenuItem = new MenuItem("Rejoindre une partie");
            var createMenuItem = new MenuItem("Creer une partie");
            var back = new MenuItem("Retour");

            // Gestion des évènements
            joinMenuItem.Selected += JoinMenuItemSelected;
            createMenuItem.Selected += CreateMenuItemSelected;
            back.Selected += OnCancel;

            // Ajout des options du menu
            MenuItems.Add(joinMenuItem);
            MenuItems.Add(createMenuItem);
            MenuItems.Add(back);


            graphics = graphicsReceive;
            if (_content == null)
                _content = new ContentManager(SceneManager.Game.Services, "Content");

            back.Selected += OnCancel;


        }


        private void JoinMenuItemSelected(object sender, EventArgs e)
        {
        }

        private void CreateMenuItemSelected(object sender, EventArgs e)
        {
        }

    }
}
