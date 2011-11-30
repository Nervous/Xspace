using System;
using Microsoft.Xna.Framework.Graphics;
using MenuSample.Scenes.Core;

namespace MenuSample.Scenes
{
    /// <summary>
    /// Le menu principal est la première chose affichée lors du lancement du binaire
    /// </summary>
    public class MainMenuScene : AbstractMenuScene
    {

        /// <summary>
        /// Le constructeur remplit le menu
        /// </summary>
        /// 
        Microsoft.Xna.Framework.GraphicsDeviceManager graphics;
        public MainMenuScene(SceneManager sceneMgr, Microsoft.Xna.Framework.GraphicsDeviceManager graphicsReceive)
            : base(sceneMgr, "")
        {
            // Création des options
            var playGameMenuItem = new MenuItem("Lancer le jeu");
            var ScoreMenuItem = new MenuItem("Scores");
            var optionsMenuItem = new MenuItem("A delete");
            var exitMenuItem = new MenuItem("Quitter");

            // Gestion des évènements
            playGameMenuItem.Selected += PlayGameMenuItemSelected;
            optionsMenuItem.Selected += OptionsMenuItemSelected;
            exitMenuItem.Selected += ConfirmExitMessageBoxAccepted;
            ScoreMenuItem.Selected += ScoreMenuItemSelected;

            // Ajout des options du menu
            MenuItems.Add(playGameMenuItem);
            MenuItems.Add(ScoreMenuItem);
            //MenuItems.Add(optionsMenuItem);
            MenuItems.Add(exitMenuItem);
            

            graphics = graphicsReceive;
        }


        private void PlayGameMenuItemSelected(object sender, EventArgs e)
        {
            LoadingScene.Load(SceneManager, true, new GameplayScene(SceneManager, graphics));
        }

        private void OptionsMenuItemSelected(object sender, EventArgs e)
        {
            new OptionsMenuScene(SceneManager).Add();
        }

        private void ScoreMenuItemSelected(object sender, EventArgs e)
        {
            new ScoreMenuScene(SceneManager).Add();
        }

        protected override void OnCancel() // unused
        {
            const string message = "Etes vous sur de vouloir quitter le sample?\n";
            var confirmExitMessageBox = new MessageBoxScene(SceneManager, message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;
            confirmExitMessageBox.Add();
        }

        private void ConfirmExitMessageBoxAccepted(object sender, EventArgs e)
        {
            SceneManager.Game.Exit();
        }

    }
}
