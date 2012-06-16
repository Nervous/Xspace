using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;

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

using ProjectMercury;
using ProjectMercury.Emitters;
using ProjectMercury.Modifiers;
using ProjectMercury.Renderers;


namespace MenuSample.Scenes
{
    class NameScene : AbstractGameScene
    {
        protected KeyboardState keyboardState;
        protected Keys lastKey;
       protected string name;
        protected bool lastKeyDown;

       public NameScene(SceneManager sceneMgr, GameTime gameTime)
           : base (sceneMgr)
       {
           name = "";
           this.Update(gameTime);
       }

       public override void  Update(GameTime gameTime)
       {
           base.Update(gameTime);
           keyboardState = Keyboard.GetState();
               if (keyboardState.GetPressedKeys().Length > 0)
               {
                   if (name.Length < 5)
                   {
                       if (lastKeyDown)
                       {
                           name += keyboardState.GetPressedKeys()[0].ToString();
                           lastKey = keyboardState.GetPressedKeys()[0];
                           lastKeyDown = false;
                       }
                       else if (keyboardState.IsKeyUp(lastKey))
                           lastKeyDown = true;
                   }
                   else
                       Remove();
               }
           
       }

       public string Name
       {
           get { return name; }
       }



    }
}
