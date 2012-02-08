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
    public class MainMenuScene : AbstractMenuScene
    {

        /// <summary>
        /// Le constructeur remplit le menu
        /// </summary>
        /// 


        private ContentManager _content;

        Microsoft.Xna.Framework.GraphicsDeviceManager graphics;
        public MainMenuScene(SceneManager sceneMgr, Microsoft.Xna.Framework.GraphicsDeviceManager graphicsReceive)
            : base(sceneMgr, "")
        {
            
            // Création des options
            var playGameMenuItem = new MenuItem("Lancer le jeu");
            var ScoreMenuItem = new MenuItem("Scores");
            var OptionMenuItem = new MenuItem("Options");
            var exitMenuItem = new MenuItem("Quitter");

            // Gestion des évènements
            playGameMenuItem.Selected += PlayGameMenuItemSelected;
            OptionMenuItem.Selected += OptionMenuItemSelected;
            exitMenuItem.Selected += ConfirmExitMessageBoxAccepted;
            ScoreMenuItem.Selected += ScoreMenuItemSelected;

            // Ajout des options du menu
            MenuItems.Add(playGameMenuItem);
            MenuItems.Add(ScoreMenuItem);
            MenuItems.Add(OptionMenuItem);
            MenuItems.Add(exitMenuItem);


            graphics = graphicsReceive;
            if (_content == null)
                _content = new ContentManager(SceneManager.Game.Services, "Content");

            AudioPlayer.Initialize();
            System.Threading.Thread.Sleep(50); // Sert à éviter un bug dû à la Race Condition du thread lancé par Initialize().

            AudioPlayer.SetVolume(1f);
            AudioPlayer.PlayMusic("Content\\Musiques\\Menu\\Musique.flac");
        }


        private void PlayGameMenuItemSelected(object sender, EventArgs e)
        {
            LoadingScene.Load(SceneManager, true, new GameplayScene(SceneManager, graphics));
        }

        private void OptionMenuItemSelected(object sender, EventArgs e)
        {
            new OptionMenuScene(SceneManager, graphics).Add();
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
