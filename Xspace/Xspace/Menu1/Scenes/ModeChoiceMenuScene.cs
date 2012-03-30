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


namespace MenuSample.Scenes
{
    /// <summary>
    /// Le menu principal est la première chose affichée lors du lancement du binaire
    /// </summary>
    public class ModeChoiceMenuScene : AbstractMenuScene
    {

        /// <summary>
        /// Le constructeur remplit le menu
        /// </summary>
        /// 

        protected int _act, _level;
        private ContentManager _content;

        Microsoft.Xna.Framework.GraphicsDeviceManager graphics;
        public ModeChoiceMenuScene(SceneManager sceneMgr, Microsoft.Xna.Framework.GraphicsDeviceManager graphicsReceive)
            : base(sceneMgr, "")
        {

            // Création des options
            var campagneMenuItem = new MenuItem("Campagne");
            var coopMenuItem = new MenuItem("Cooperation");
            var extremMenuItem = new MenuItem("Extreme");
            var libreMenuItem = new MenuItem("Libre");
            var back = new MenuItem("Retour");

            // Gestion des évènements
            campagneMenuItem.Selected += CampagneMenuItemSelected;
            extremMenuItem.Selected += ExtremMenuItemSelected;
            back.Selected += OnCancel;
            coopMenuItem.Selected += CoopMenuItemSelected;

            // Ajout des options du menu
            MenuItems.Add(campagneMenuItem);
            MenuItems.Add(coopMenuItem);
            MenuItems.Add(extremMenuItem);
            MenuItems.Add(back);


            graphics = graphicsReceive;
            if (_content == null)
                _content = new ContentManager(SceneManager.Game.Services, "Content");

            back.Selected += OnCancel;


        }


        private void CampagneMenuItemSelected(object sender, EventArgs e)
        {
            new ActChoiceMenuScene(SceneManager, graphics).Add();
        }

        private void ExtremMenuItemSelected(object sender, EventArgs e)
        {
            _level = 1;
            _act = 4;
            LoadingScene.Load(SceneManager, true, new GameplayScene(SceneManager, graphics, _level, _act));
        }

        private void CoopMenuItemSelected(object sender, EventArgs e)
        {
            new CoopChoiceMenu(SceneManager, graphics).Add();
        }

    }
}
