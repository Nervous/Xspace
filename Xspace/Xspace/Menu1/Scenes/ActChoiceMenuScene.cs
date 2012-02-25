using System;
using MenuSample.Scenes.Core;

namespace MenuSample.Scenes
{
    public class ActChoiceMenuScene : AbstractMenuScene
    {
        Microsoft.Xna.Framework.GraphicsDeviceManager graphics;
        public ActChoiceMenuScene(SceneManager sceneMgr, Microsoft.Xna.Framework.GraphicsDeviceManager graphicsReceive)
            : base(sceneMgr, "Niveaux")
        {
            var Act1 = new MenuItem("Acte I");
            var Act2 = new MenuItem("Acte II");
            var Act3 = new MenuItem("Acte III");
            var back = new MenuItem("Retour");
            graphics = graphicsReceive;
            back.Selected += OnCancel;
            Act1.Selected += Act1MenuItemSelected;
            Act2.Selected += Act2MenuItemSelected;
            Act3.Selected += Act3MenuItemSelected;
            MenuItems.Add(Act1);
            MenuItems.Add(Act2);
            MenuItems.Add(Act3);
            MenuItems.Add(back);

        }

        private void Act1MenuItemSelected(object sender, EventArgs e)
        {
            new LevelChoice1MenuScene(SceneManager, graphics).Add();
        }

        private void Act2MenuItemSelected(object sender, EventArgs e)
        {
            new LevelChoice2MenuScene(SceneManager, graphics).Add();
        }

        private void Act3MenuItemSelected(object sender, EventArgs e)
        {
            new LevelChoice3MenuScene(SceneManager, graphics).Add();
        }
    }
}
