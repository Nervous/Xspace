using System;
using System.Collections.Generic;
using System.Linq;
using MenuSample.Inputs;
using MenuSample.Scenes;
using MenuSample.Scenes.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Xspace
{
    public class Xspace : Microsoft.Xna.Framework.Game
    {

        public static int window_width = 1180;
        public static int window_height = 620;

        public Xspace()
        {
            GraphicsDeviceManager graphics = new GraphicsDeviceManager(this) 
            {
                PreferredBackBufferWidth = window_width,
                PreferredBackBufferHeight = window_height, 
            };
            var sceneMgr = new SceneManager(this);
            Components.Add(new InputState(this));
            Components.Add(sceneMgr);
            new BackgroundScene(sceneMgr).Add();
            new MainMenuScene(sceneMgr, graphics).Add();
            Content.RootDirectory = "Content";
            IsFixedTimeStep = false;

        }


        public static void Main()
        {
            using (var game = new Xspace())
                game.Run();
        }


    }
}


