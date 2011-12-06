using System;
using System.Collections.Generic;
using System.Threading;
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

using ProjectMercury;
using ProjectMercury.Emitters;
using ProjectMercury.Modifiers;
using ProjectMercury.Renderers;

using Xspace;


namespace MenuSample.Scenes
{
    /// <summary>
    /// Le jeu!
    /// </summary>
    public class GameplayScene : AbstractGameScene
    {

        private ContentManager _content;
        private SpriteFont _gameFont;

        private Vector2 _playerPosition = new Vector2(100, 100);
        private Vector2 _enemyPosition = new Vector2(100, 100);

        private readonly Random _random = new Random();
        private float _pauseAlpha;

        struct doneParticles
        {
            public bool _done;
            public Vector2 startingParticle;

            public doneParticles(bool done, Vector2 pos)
            {
                _done = done;
                startingParticle = pos;
            }
        };
        private doneParticles partManage;

        public SpriteBatch spriteBatch;

        private Texture2D textureVaisseau_joueur, textureMissile_joueur_base, textureMissile_ennemi1, textureVaisseau_ennemi1;
        private List<Texture2D> listeTextureVaisseauxEnnemis;

        private Song musique, musique_menu;
        private SoundEffect musique_tir;

        private KeyboardState keyboardState;
        private gestionLevels thisLevel;
        private List<gestionLevels> infLevel;

        Renderer particleRenderer;
        ParticleEffect particleEffect;


        // TODO : Déclaration de tous les objets Vaisseau en dessous
        private Vaisseau_joueur joueur1;
        List<Vaisseau> listeVaisseauEnnemi, listeVaisseauEnnemiToRemove;
        List<Missiles> listeMissileJoueur, listeMissileEnnemi, listeMissileToRemove;
        // TODO : Déclaration de tous les objets missiles en dessous

        int nbreMaxMissiles, i = 0, j = 0;
        int nbreMaxMissiles_e;
        float fps_fix;
        double time, lastTime;
        char[] delimitationFilesInfo = new char[] { ' ' }, delimitationFilesInfo2 = new char[] { ';' }, delimitationFilesInfo3 = new char[] { ':' };



        public GameplayScene(SceneManager sceneMgr, GraphicsDeviceManager graphics)
            : base(sceneMgr)
        {

            particleRenderer = new SpriteBatchRenderer
            {
                GraphicsDeviceService = graphics
            };
            particleEffect = new ParticleEffect();

            nbreMaxMissiles = 15;
            lastTime = 0;
            nbreMaxMissiles_e = 20;

        }



        public override void Initialize()
        {
		    base.Initialize();

         }

        private ScrollingBackground fond_ecran;
        protected override void LoadContent()
        {
            if (_content == null)
                _content = new ContentManager(SceneManager.Game.Services, "Content");


            spriteBatch = new SpriteBatch(GraphicsDevice);

            musique = _content.Load<Song>("Musiques\\Jeu\\Musique");
            MediaPlayer.Play(musique);
            musique_tir = _content.Load<SoundEffect>("Sons\\Tir\\Tir");

            fond_ecran = new ScrollingBackground();
            Texture2D fond_image = _content.Load<Texture2D>("Sprites\\Background\\Background");
            fond_ecran.Load(GraphicsDevice, fond_image);

            // TODO : Chargement de toutes les textures des vaisseau en dessous
            textureVaisseau_joueur = _content.Load<Texture2D>("Sprites\\Vaisseaux\\Joueur\\Vaisseau1");
            textureVaisseau_ennemi1 = _content.Load<Texture2D>("Sprites\\Vaisseaux\\Ennemi\\Vaisseau1");
            musique_menu = _content.Load<Song>("Musiques\\Menu\\Musique");

            _gameFont = _content.Load<SpriteFont>("Fonts\\Menu\\Menu");

            particleRenderer.LoadContent(_content);
            particleEffect = _content.Load<ParticleEffect>(("Collisions\\BasicExplosion\\BasicExplosion"));
            particleEffect.LoadContent(_content);
            particleEffect.Initialise();

            Thread.Sleep(500);
            SceneManager.Game.ResetElapsedTime();

            // TODO : Chargement de toutes les textures des missiles en dessous
            textureMissile_joueur_base = _content.Load<Texture2D>("Sprites\\Missiles\\Joueur\\Missile1");
            textureMissile_ennemi1 = _content.Load<Texture2D>("Sprites\\Missiles\\Ennemi\\Missile1");

            // TODO : Chargement de tous les objets vaisseau en dessous
            listeVaisseauEnnemi = new List<Vaisseau>();
            listeVaisseauEnnemiToRemove = new List<Vaisseau>();
            joueur1 = new Vaisseau_joueur(textureVaisseau_joueur);


            // TODO : Chargement de tous les objets missiles en dessous
            listeMissileJoueur = new List<Missiles>();
            listeMissileEnnemi = new List<Missiles>();
            listeMissileToRemove = new List<Missiles>();


            /*for (int i = 0; i < nbreMaxMissiles_e; i++)
                listeMissileEnnemi[i] = new Missiles(textureMissile_ennemi1, true, 50);

            for (int i = 0; i < nbreMaxMissiles_e - 1; i++)
            {
                if (listeMissileEnnemi[i] != null)
                    listeMissileEnnemi[i].initialiserTexture(textureMissile_ennemi1);
            }
            listeMissile.Add(listeMissileEnnemi);*/


            // TODO : Chargement du level en dessous
            listeTextureVaisseauxEnnemis = new List<Texture2D>();
            listeTextureVaisseauxEnnemis.Add(textureVaisseau_ennemi1);
            thisLevel = new gestionLevels(0, listeTextureVaisseauxEnnemis);
            infLevel = new List<gestionLevels>();
            thisLevel.readInfos(delimitationFilesInfo, delimitationFilesInfo2, delimitationFilesInfo3, infLevel);
        }


        // gestion des collisions
        doneParticles collisions(List<Vaisseau> listeVaisseau, List<Missiles> listeMissiles, float spentTime, ParticleEffect particleEffect)
        {

            int vaisseauActuel = 0;
            foreach(Vaisseau vaisseau in listeVaisseau)
            {
                vaisseauActuel = listeVaisseau.IndexOf(vaisseau);
                foreach(Missiles missile in listeMissileJoueur)
                {

                    if (((missile.position.X + missile.sprite.Width > listeVaisseau[vaisseauActuel].position.X)
                        && (missile.position.X + missile.sprite.Width < listeVaisseau[vaisseauActuel].position.X + listeVaisseau[vaisseauActuel].sprite.Width))
                        && ((missile.position.Y + missile.sprite.Height / 2 > listeVaisseau[vaisseauActuel].position.Y)
                        && (missile.position.Y + missile.sprite.Height / 2 < listeVaisseau[vaisseauActuel].position.Y + listeVaisseau[vaisseauActuel].sprite.Height))
                        )
                        {  
                                // Collision missile => Vaisseau trouvée
                            if (missile.ennemi == false)
                            {
                                listeMissileToRemove.Add(missile);

                                if (listeVaisseau[vaisseauActuel].hurt(missile.degats) == true)
                                {
                                    // Vaisseau dead
                                            
                                    listeVaisseau[vaisseauActuel].kill();
                                    return new doneParticles(false, listeVaisseau[vaisseauActuel].position);
                                            
                                }
                            }
                                    
                                    
                                    
                        }
                    
                        
                    }
                }

            return new doneParticles(true, new Vector2(0, 0));
        }
                public override void HandleInput()
        {
            
            KeyboardState keyboardState = InputState.CurrentKeyboardState;

            // Le menu de pause s'enclenche si un joueur appuie sur la touche assignée
            // au menu de pause, ou lorsque qu'une manette branchée est déconnectée

            if (InputState.IsPauseGame())
                new PauseMenuScene(SceneManager, this).Add();
            else
            {
                if (keyboardState.IsKeyDown(Keys.Z))
                {
                    if (joueur1._emplacement.Y - joueur1._textureVaisseau.Height / 2 + 20 >= 0)
                        joueur1._emplacement -= joueur1._deplacementDirectionY * joueur1._vitesseVaisseau * fps_fix;
                }

                if (keyboardState.IsKeyDown(Keys.S))
                {
                    if (joueur1._emplacement.Y - joueur1._textureVaisseau.Height / 2 - 10 <= 540)
                        joueur1._emplacement += joueur1._deplacementDirectionY * joueur1._vitesseVaisseau * fps_fix;
                }

                if (keyboardState.IsKeyDown(Keys.Q))
                {
                    if (joueur1._emplacement.X - joueur1._textureVaisseau.Width / 2 + 20 >= 0)
                        joueur1._emplacement -= joueur1._deplacementDirectionX * joueur1._vitesseVaisseau * fps_fix;
                }

                if (keyboardState.IsKeyDown(Keys.D))
                {
                    if (joueur1._emplacement.X - joueur1._textureVaisseau.Width / 2 - 10 <= 1085)
                        joueur1._emplacement += joueur1._deplacementDirectionX * joueur1._vitesseVaisseau * fps_fix;
                }
               
            }
        }

        public override void Update(GameTime gameTime, bool othersceneHasFocus, bool coveredByOtherscene)
        {

            fps_fix = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            time += gameTime.ElapsedGameTime.TotalMilliseconds;

            base.Update(gameTime, othersceneHasFocus, false);

            _pauseAlpha = coveredByOtherscene
                ? Math.Min(_pauseAlpha + 1f / 32, 1)
                : Math.Max(_pauseAlpha - 1f / 32, 0);

            // play musique du jeu et menu
            if (InputState.IsPauseGame())
            {
                MediaPlayer.Stop();
            } else if ((MediaPlayer.State == MediaState.Stopped)&&(!InputState.IsPauseGame()))
                MediaPlayer.Play(musique_menu);

            

            foreach (gestionLevels spawn in infLevel)
            {
                if (spawn.isTime(time))
                {
                    spawn.makeItSpawn();
                    switch (spawn.Categorie)
                    {
                        case "vaisseau":
                            if(spawn.Adresse != null)
                                listeVaisseauEnnemi.Add(spawn.Adresse);
                            else
                                Game.Exit();
                            break;
                        default:
                            break;
                    }
                }
            }

            fond_ecran.Update(fps_fix);
            keyboardState = Keyboard.GetState();
            // affichage des missiles des joueurs
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                if (time - lastTime > 150 || lastTime == 0)
                {
                    musique_tir.Play();
                    listeMissileJoueur.Add(new Missile1_Joueur(textureMissile_joueur_base, joueur1.position));
                    lastTime = time;
                }
            }
            
            // affichage des missiles des ennemis
            foreach (Vaisseau vaisseau in listeVaisseauEnnemi)
            {
                if (vaisseau.existe)
                {
                    if (time - vaisseau.lastTir > vaisseau.timingAttack || vaisseau.lastTir == 0)
                    {
                        Vector2 spawnPosition = new Vector2(vaisseau.position.X -100, vaisseau.position.Y);
                        // FAIRE EN FONCTION DU TYPE DE MISSILE
                        listeMissileEnnemi.Add(new Missile_Drone(textureMissile_ennemi1, spawnPosition));
                        vaisseau.lastTir = time;
                            
                    }
                }           
            }

            
            foreach(Missiles missile in listeMissileJoueur)
            {
                if (missile.position.X < 1150)
                    missile.avancerMissile(fps_fix);
                else 
                    listeMissileToRemove.Add(missile);
            }

            foreach (Missiles missile in listeMissileEnnemi)
            {
                if (missile.position.X > 0)
                    missile.avancerMissile(fps_fix);
                else 
                    listeMissileToRemove.Add(missile);
            }

            foreach (Missiles missile in listeMissileToRemove)
            {
                if (missile.ennemi == false)
                    listeMissileJoueur.Remove(missile);
                else
                    listeMissileEnnemi.Remove(missile);
            }

            listeMissileToRemove.Clear();

                if(!(partManage.startingParticle == Vector2.Zero))
                    particleEffect.Trigger(partManage.startingParticle);
                partManage = collisions(listeVaisseauEnnemi, listeMissileJoueur, fps_fix, particleEffect);

           particleEffect.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            foreach (Vaisseau vaisseau in listeVaisseauEnnemi)
            {
                if (vaisseau.existe == false)
                {
                    listeVaisseauEnnemiToRemove.Add(vaisseau);
                    i++;
                }
                else
                    vaisseau.Update(fps_fix);
            }

            for (j = 0; j < i; j++)
            {
                    listeVaisseauEnnemi.Remove(listeVaisseauEnnemiToRemove[j]);
            }

            listeVaisseauEnnemiToRemove.Clear();
            i = 0;
            
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = SceneManager.SpriteBatch;
            SceneManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Transparent, 0, 0);
           // spriteBatch.Begin();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            fond_ecran.Draw(spriteBatch);
            particleRenderer.RenderEffect(particleEffect);
            joueur1.Draw(spriteBatch); // Draw du joueur
            
            
            foreach (Vaisseau vaisseau in listeVaisseauEnnemi)
            {
                vaisseau.Draw(spriteBatch);
            }
            foreach (Missiles sMissile in listeMissileJoueur)
            {
                sMissile.Draw(spriteBatch);
            }

            foreach (Missiles sMissile in listeMissileEnnemi)
            {
                sMissile.Draw(spriteBatch);
            }
            base.Draw(gameTime);

            spriteBatch.End();

            if (TransitionPosition > 0 || _pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, _pauseAlpha / 2);
                SceneManager.FadeBackBufferToBlack(alpha);
            }
            
        }

        protected override void UnloadContent()
        {
            _content.Unload();
        }


    }
}

    

