using System;
using MenuSample.Scenes.Core;

namespace MenuSample.Scenes
{
    public class ScoreMenuScene : AbstractMenuScene
    {


        public ScoreMenuScene(SceneManager sceneMgr)
            : base(sceneMgr, "Scores")
        {
            var scoreArcade = new MenuItem("Score mode arcade");
            var scoreExtreme = new MenuItem("  Score mode extreme");
            var back = new MenuItem("Retour");

            back.Selected += OnCancel;
            scoreArcade.Selected += ScoreArcadeMenuItemSelected;

            MenuItems.Add(scoreArcade);
            MenuItems.Add(scoreExtreme);
            MenuItems.Add(back);

            

        }

        private void ScoreArcadeMenuItemSelected(object sender, EventArgs e)
        {
            new ScoreArcadeMenuScene(SceneManager).Add();
        }

        private void ScoreExtremeMenuItemSelected(object sender, EventArgs e)
        {
            new ScoreExtremeMenuScene(SceneManager).Add();
        }
    }
}
