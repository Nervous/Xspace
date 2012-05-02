using System;
using MenuSample.Scenes.Core;
using System.Collections.Generic;
using System.IO;

namespace MenuSample.Scenes
{
    public class SongChoiceMenuScene : AbstractMenuScene
    {
        protected int _level, _act = 3;
        Microsoft.Xna.Framework.GraphicsDeviceManager graphics;
        public SongChoiceMenuScene(SceneManager sceneMgr, Microsoft.Xna.Framework.GraphicsDeviceManager graphicsReceive)
            : base(sceneMgr, "Chanson")
        {
            List<MenuItem> list_songs = new List<MenuItem>();
            string[] filePaths = Directory.GetFiles(@"Musiques\Jeu\");
            List<string> allowed_exts = new List<string>() { ".mp3", ".wav", ".flac", ".ogg", ".mp2", ".alac", ".aac", ".oga", ".spx", ".ac3"};

            MenuItem back = new MenuItem("Retour");
            back.Selected += OnCancel;
            MenuItems.Add(back);

            foreach (string path in filePaths)
            {
                if (allowed_exts.Contains(Path.GetExtension(path)))
                {
                    MenuItem song = new MenuItem(Path.GetFileName(path));
                    song.Selected += SongSelected;
                    MenuItems.Add(song);
                }
            }
            graphics = graphicsReceive;
        }
        
        private void SongSelected(object sender, EventArgs e)
        {
            LoadingScene.Load(SceneManager, true, new GameplayScene(SceneManager, graphics, 0, 0, GameplayScene.GAME_MODE.LIBRE, ((MenuItem)sender).Text));
        }
    }
}
