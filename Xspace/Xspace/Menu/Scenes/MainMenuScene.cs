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
using System.Diagnostics;

using Xspace;

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
            var createGameMenuItem = new MenuItem("Editeur de niveau");
            var ScoreMenuItem = new MenuItem("Scores");
            var OptionMenuItem = new MenuItem("Difficulte");
            var exitMenuItem = new MenuItem("Quitter");
            var completeItem = new MenuItem("Complete");

            // Gestion des évènements
            playGameMenuItem.Selected += PlayGameMenuItemSelected;
            OptionMenuItem.Selected += OptionMenuItemSelected;
            exitMenuItem.Selected += ConfirmExitMessageBoxAccepted;
            ScoreMenuItem.Selected += ScoreMenuItemSelected;
            createGameMenuItem.Selected += CreateGameMenuItemSelected;

            // Ajout des options du menu
            MenuItems.Add(playGameMenuItem);
            MenuItems.Add(createGameMenuItem);
            MenuItems.Add(ScoreMenuItem);
            MenuItems.Add(OptionMenuItem);
            MenuItems.Add(exitMenuItem);


            graphics = graphicsReceive;
            if (_content == null)
                _content = new ContentManager(SceneManager.Game.Services, "Content");


            AudioPlayer.Initialize();
            System.Threading.Thread.Sleep(500); // Sert à éviter un bug dû à la Race Condition du thread lancé par Initialize().

            AudioPlayer.SetVolume(1f);
            AudioPlayer.PlayMusic("Musiques\\Menu\\Musique.flac");

        }


        private void PlayGameMenuItemSelected(object sender, EventArgs e)
        {
            new ModeChoiceMenuScene(SceneManager, graphics).Add();
        }

        private void OptionMenuItemSelected(object sender, EventArgs e)
        {
            new OptionMenuScene(SceneManager, graphics).Add();
        }

        private void ScoreMenuItemSelected(object sender, EventArgs e)
        {
            new ScoreMenuScene(SceneManager).Add();
        }

        private void CreateGameMenuItemSelected(object sender, EventArgs e)
        {
            Process myInfo = new Process();
            myInfo.StartInfo.FileName = "LevelEditor.exe";
            myInfo.StartInfo.WorkingDirectory = "Editeur";
            myInfo.Start(); 
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
