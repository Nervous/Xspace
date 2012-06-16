using System;
using MenuSample.Scenes.Core;
using System.Collections.Generic;
using System.IO;

namespace MenuSample.Scenes
{
    public class UserMenuScene : AbstractMenuScene
    {
        protected int _level, _act = 1;
        Microsoft.Xna.Framework.GraphicsDeviceManager graphics;
        public UserMenuScene(SceneManager sceneMgr, Microsoft.Xna.Framework.GraphicsDeviceManager graphicsReceive)
            : base(sceneMgr, "Niveaux :")
        {
            string[] filePaths = Directory.GetFiles(@"Levels\Custom");
            List<string> allowed_exts = new List<string>() { ".xpa" };

            MenuItem back = new MenuItem("Retour");
            back.Selected += OnCancel;
            MenuItems.Add(back);

            foreach (string path in filePaths)
            {
                if (allowed_exts.Contains(Path.GetExtension(path)))
                {
                    MenuItem level = new MenuItem(Path.GetFileName(path));
                    level.Selected += LevelSelected;
                    MenuItems.Add(level);
                }
            }
            graphics = graphicsReceive;
        }

        private void LevelSelected(object sender, EventArgs e)
        {
            LoadingScene.Load(SceneManager, true, new GameplayScene(SceneManager, graphics, 0, 0, GameplayScene.GAME_MODE.CUSTOM, ((MenuItem)sender).Text));
        }
    }
}
