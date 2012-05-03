using System;
using MenuSample.Scenes.Core;

namespace MenuSample.Scenes
{
    public class LevelChoice1MenuScene : AbstractMenuScene
    {
        protected int _level, _act = 1;
        Microsoft.Xna.Framework.GraphicsDeviceManager graphics;
        public LevelChoice1MenuScene(SceneManager sceneMgr, Microsoft.Xna.Framework.GraphicsDeviceManager graphicsReceive)
            : base(sceneMgr, "Niveaux")
        {
            var Level1 = new MenuItem("Collisions");
            var Level2 = new MenuItem("Zebra");
            var Level3 = new MenuItem("Missiles");
            var back = new MenuItem("Retour");
            //GameplayScene gameplayscene = new GameplayScene(sceneMgr, graphicsReceive,_level, _act, GameplayScene.GAME_MODE.CAMPAGNE);
            graphics = graphicsReceive;
            back.Selected += OnCancel;
            Level1.Selected += Level1MenuItemSelected;
            Level2.Selected += Level2MenuItemSelected;
            Level3.Selected += Level3MenuItemSelected;
            MenuItems.Add(Level1);
            MenuItems.Add(Level2);
            MenuItems.Add(Level3);
            MenuItems.Add(back);

        }

        private void Level1MenuItemSelected(object sender, EventArgs e)
        {
            _level = 1;
            LoadingScene.Load(SceneManager, true, new GameplayScene(SceneManager, graphics,_level,_act, GameplayScene.GAME_MODE.CAMPAGNE));
        }

        private void Level2MenuItemSelected(object sender, EventArgs e)
        {
            _level = 2;
            LoadingScene.Load(SceneManager, true, new GameplayScene(SceneManager, graphics, _level,_act, GameplayScene.GAME_MODE.CAMPAGNE));
        }

        private void Level3MenuItemSelected(object sender, EventArgs e)
        {
            _level = 3;
            LoadingScene.Load(SceneManager, true, new GameplayScene(SceneManager, graphics, _level,_act, GameplayScene.GAME_MODE.CAMPAGNE));
        }

    }
}
