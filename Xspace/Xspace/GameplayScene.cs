using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Net;
using System.IO;

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
using Xspace.Son;


namespace MenuSample.Scenes
{
    /// <summary>
    /// Le jeu!
    /// </summary>
    public class GameplayScene : AbstractGameScene
    {
        #region Déclaration variables usuelles
        private int score;
        private float fps_fix, _pauseAlpha;
        private double time, lastTime, lastTimeSpectre, lastTimeEnergy;
        private string path_level, stock_score_inferieur, stock_score_superieur;
        private string[] score_level;
        private StreamWriter sw_level;
        private StreamReader sr_level;
        private char[] delimitationFilesInfo = new char[] { ' ' }, delimitationFilesInfo2 = new char[] { ';' }, delimitationFilesInfo3 = new char[] { ':' };
        private float[] spectre;
        private bool drawSpectre;
        private float music_energy;
        #endregion
        #region Déclaration variables relatives au jeu
        private doneParticles partManage;
        private ScrollingBackground fond_ecran, fond_ecran_front;
        public SpriteBatch spriteBatch;
        private Texture2D T_Vaisseau_Joueur, T_Vaisseau_Drone, T_Missile_Joueur_1, T_Missile_Drone, T_Bonus_Vie, T_Bonus_Weapon1, barre_vie, T_HUD, T_HUD_bars;
        private List<Texture2D> listeTextureVaisseauxEnnemis, listeTextureBonus;
        private SoundEffect musique_tir;
        private KeyboardState keyboardState;
        bool lastKeyDown = true;
        private gestionLevels thisLevel;
        private List<gestionLevels> infLevel;
        Renderer particleRenderer;
        ParticleEffect particleEffect;
        List<Vaisseau> listeVaisseau, listeVaisseauToRemove;
        List<Missiles> listeMissile, listeMissileToRemove;
        List<Bonus> listeBonus, listeBonusToRemove;
        private ContentManager _content;
        private SpriteFont _gameFont, _ingameFont;
        #endregion
        #region Déclaration structures relatives au jeu
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
        #endregion

        private readonly Random _random = new Random();
        private AffichageInformations HUD = new AffichageInformations();
        public GameplayScene(SceneManager sceneMgr, GraphicsDeviceManager graphics)
            : base(sceneMgr)
        {

            particleRenderer = new SpriteBatchRenderer
            {
                GraphicsDeviceService = graphics
            };
            particleEffect = new ParticleEffect();

            score = 0;
            lastTime = 0;
            lastTimeEnergy = 0;
            lastTimeSpectre = 150;
            spectre = new float[128];
        }

        public override void Initialize()
        {
		    base.Initialize();
        }
        protected override void LoadContent()
        {
            if (_content == null)
                _content = new ContentManager(SceneManager.Game.Services, "Content");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Thread.Sleep(500);
            SceneManager.Game.ResetElapsedTime();

            #region Chargement musiques & sons
            musique_tir = _content.Load<SoundEffect>("Sons\\Tir\\Tir");
            
            AudioPlayer.PlayMusic("Musiques\\Jeu\\fat1.wav");
            AudioPlayer.SetVolume(1f);

            #endregion
            #region Chargement des polices d'écritures
            _gameFont = _content.Load<SpriteFont>("Fonts\\Menu\\Menu");
            _ingameFont = _content.Load<SpriteFont>("Fonts\\Jeu\\Jeu");
            #endregion
            #region Chargement fond du jeu
            fond_ecran = new ScrollingBackground();
            fond_ecran.Load(GraphicsDevice, _content.Load<Texture2D>("Sprites\\Background\\space"));

            fond_ecran_front = new ScrollingBackground();
            fond_ecran_front.Load(GraphicsDevice, _content.Load<Texture2D>("Sprites\\Background\\space-front"));
            #endregion
            #region Chargement particules
            particleRenderer.LoadContent(_content);
            particleEffect = _content.Load<ParticleEffect>("Collisions\\BasicExplosion\\BasicExplosion");
            particleEffect.LoadContent(_content);
            particleEffect.Initialise();
            #endregion
            #region Chargement textures vaisseaux
            T_Vaisseau_Joueur = _content.Load<Texture2D>("Sprites\\Vaisseaux\\Joueur\\drone_joueur1");
            T_Vaisseau_Drone = _content.Load<Texture2D>("Sprites\\Vaisseaux\\Ennemi\\ennemi_gris1");
			
			drawSpectre = false;

            /* WebClient wc = new WebClient();
            wc.DownloadFile("http://nathalie.bouquet.free.fr/epita/trombi2011-12/sup/login_x.jpg", "Content\\Sprites\\Vaisseaux\\logintmp.jpg");
            T_Vaisseau_Drone = Texture2D.FromStream(GraphicsDevice, new FileStream("Content\\Sprites\\Vaisseaux\\logintmp.jpg", FileMode.Open)); */

            #endregion
            #region Chargement textures HUD
            T_HUD = _content.Load<Texture2D>("Sprites\\HUD\\interface");
            T_HUD_bars = _content.Load<Texture2D>("Sprites\\HUD\\energyBars1"); 
            #endregion
            #region Chargement textures missiles
            T_Missile_Joueur_1 = _content.Load<Texture2D>("Sprites\\Missiles\\Joueur\\missilenew1");
            T_Missile_Drone = _content.Load<Texture2D>("Sprites\\Missiles\\Ennemi\\missile_boule1");
            #endregion
            #region Chargement textures bonus
            // TODO : Chargement de toutes les textures des bonus en dessous
            T_Bonus_Vie = _content.Load<Texture2D>("Sprites\\Bonus\\Life");
            T_Bonus_Weapon1 = _content.Load<Texture2D>("Sprites\\Bonus\\DoubleBaseWeapon");
            #endregion
            #region Chargement vaisseaux
            listeVaisseau = new List<Vaisseau>();
            listeVaisseauToRemove = new List<Vaisseau>();
            listeVaisseau.Add(new Vaisseau_joueur(T_Vaisseau_Joueur));
            #endregion
            #region Chargement missiles
            listeMissile = new List<Missiles>();
            listeMissileToRemove = new List<Missiles>();
            #endregion
            #region Chargement bonus
            listeBonus = new List<Bonus>();
            listeBonusToRemove = new List<Bonus>();
            #endregion
            #region Chargement du level
            listeTextureVaisseauxEnnemis = new List<Texture2D>();
            listeTextureVaisseauxEnnemis.Add(T_Vaisseau_Drone);
            listeTextureBonus = new List<Texture2D>();
            listeTextureBonus.Add(T_Bonus_Vie);
            listeTextureBonus.Add(T_Bonus_Weapon1);
            thisLevel = new gestionLevels(0, listeTextureVaisseauxEnnemis, listeTextureBonus);
            infLevel = new List<gestionLevels>();
            thisLevel.readInfos(delimitationFilesInfo, delimitationFilesInfo2, delimitationFilesInfo3, infLevel);
            #endregion
            #region Chargement barre de vie
            barre_vie = _content.Load<Texture2D>("Sprites\\Vaisseaux\\Joueur\\barre-vie-test1");
            #endregion
        }

        doneParticles collisions(List<Vaisseau> listeVaisseau, List<Missiles> listeMissile, List<Bonus> listeBonus, float spentTime, ParticleEffect particleEffect, GameTime gametime)
        {
            foreach(Vaisseau vaisseau in listeVaisseau)
            {
                #region Collision joueur => bonus
                foreach (Bonus bonus in listeBonus)
                {
                    if (((listeVaisseau[0].position.X + listeVaisseau[0].sprite.Width >= bonus.position.X && listeVaisseau[0].position.X <= bonus.position.X) ||
                                (listeVaisseau[0].position.X <= bonus.position.X + bonus.sprite.Width && listeVaisseau[0].position.X + listeVaisseau[0].sprite.Width >= bonus.position.X + bonus.sprite.Width) ||
                                (listeVaisseau[0].position.X <= bonus.position.X && listeVaisseau[0].position.X + listeVaisseau[0].sprite.Width > bonus.position.X + bonus.sprite.Width))
                           && ((listeVaisseau[0].position.Y + listeVaisseau[0].sprite.Height >= bonus.position.Y && listeVaisseau[0].position.Y <= bonus.position.Y) ||
                                (listeVaisseau[0].position.Y <= bonus.position.Y + bonus.sprite.Height && listeVaisseau[0].position.Y + listeVaisseau[0].sprite.Height >= bonus.position.Y + bonus.sprite.Height) ||
                                (listeVaisseau[0].position.Y <= bonus.position.Y && listeVaisseau[0].position.Y + listeVaisseau[0].sprite.Height > bonus.position.Y + bonus.sprite.Height)))
                    {
                        if (!bonus.disabled)
                        {
                            listeVaisseau[0].applyBonus(bonus.effect, bonus.amount, bonus.time);
                            bonus.disabled = true;
                        }
                        listeBonusToRemove.Add(bonus);
                    }
                }
                #endregion
                foreach (Missiles missile in listeMissile)
                {
                    #region Collision missile => vaisseau
                    if (((missile.position.X + missile.sprite.Width > vaisseau.position.X)
                        && (missile.position.X + missile.sprite.Width < vaisseau.position.X + vaisseau.sprite.Width))
                        && ((missile.position.Y + missile.sprite.Height / 2 > vaisseau.position.Y - vaisseau.sprite.Height*0.10)
                        && (missile.position.Y + missile.sprite.Height / 2 < vaisseau.position.Y + vaisseau.sprite.Height + vaisseau.sprite.Height*0.10))
                        )
                    {  // Collision missile => Vaisseau trouvée
                                
                        if ((vaisseau.ennemi && !missile.ennemi) || (!vaisseau.ennemi && missile.ennemi))
                        {
                            listeMissileToRemove.Add(missile);

                            if (vaisseau.hurt(missile.degats))
                            {
                                // Vaisseau dead
                                            
                                vaisseau.kill();
                                score = score + vaisseau.score;
                                return new doneParticles(false, new Vector2(vaisseau.position.X + vaisseau.sprite.Width / 2, vaisseau.position.Y + vaisseau.sprite.Height / 2));        
                            }
                        }
                    }
                    #endregion
                    #region Collision joueur <=> vaisseau 
                    if ( ( (listeVaisseau[0].position.X + listeVaisseau[0].sprite.Width > vaisseau.position.X && listeVaisseau[0].position.X < vaisseau.position.X) ||
                                (listeVaisseau[0].position.X < vaisseau.position.X + vaisseau.sprite.Width && listeVaisseau[0].position.X + listeVaisseau[0].sprite.Width > vaisseau.position.X + vaisseau.sprite.Width))
                           && (  (listeVaisseau[0].position.Y + listeVaisseau[0].sprite.Height > vaisseau.position.Y && listeVaisseau[0].position.Y < vaisseau.position.Y) ||
                                 (listeVaisseau[0].position.Y < vaisseau.position.Y + vaisseau.sprite.Height && listeVaisseau[0].position.Y + listeVaisseau[0].sprite.Height > vaisseau.position.Y + vaisseau.sprite.Height)))
                    {
                        // Collision entre vaisseau joueur & ennemi trouvée
                        vaisseau.kill();
                        score = score + vaisseau.score;
                        listeVaisseauToRemove.Add(vaisseau);
                        listeVaisseau[0].hurt(vaisseau.damageCollision);
                        if(listeVaisseau[0].vie < 0)
                            listeVaisseauToRemove.Add(listeVaisseau[0]);
                        return new doneParticles(false, new Vector2(vaisseau.position.X + vaisseau.sprite.Width/2, vaisseau.position.Y + vaisseau.sprite.Height / 2));
                    }
                    #endregion
                }
            }
            return new doneParticles(true, new Vector2(0, 0));
        }

        public override void HandleInput()
        {
            KeyboardState keyboardState = InputState.CurrentKeyboardState;
            if (InputState.IsPauseGame())
                new PauseMenuScene(SceneManager, this).Add();
        }

        public override void Update(GameTime gameTime, bool othersceneHasFocus, bool coveredByOtherscene)
        {
            keyboardState = Keyboard.GetState();
            _pauseAlpha = coveredByOtherscene ? Math.Min(_pauseAlpha + 1f / 32, 1) : Math.Max(_pauseAlpha - 1f / 32, 0);

            fps_fix = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            time += gameTime.ElapsedGameTime.TotalMilliseconds;
            base.Update(gameTime, othersceneHasFocus, false);
            
            float coeff_speed_variation = 0.6f; //coefficient de la variation de la vitesse des fonds.
            float coeff_speed = 0.2f; //coefficient de vitesse du fond.
            float coeff_speed_front = 0.6f; //coefficient de vitesse du fond en avant.

            fond_ecran.Update(fps_fix, (1 + (music_energy - 1) * coeff_speed_variation) * coeff_speed);
            fond_ecran_front.Update(fps_fix, (1 + (music_energy - 1) * coeff_speed_variation) * coeff_speed_front);

            AudioPlayer.Update();

           #region Gestion de la musique en cas de pause   A REACTIVER APRES PREMIERE SOUTENANCE 
            if (InputState.IsPauseGame())
            {
                AudioPlayer.SetVolume(0.2f);
            }
            else if (InputState.IsMenuSelect())
                AudioPlayer.SetVolume(1f);

            #endregion
            #region Gestion des évenements du level
            foreach (gestionLevels spawn in infLevel)
            {
                if (spawn.isTime(time))
                {
                    switch (spawn.Categorie)
                    {
                        case "vaisseau":
                            listeVaisseau.Add(spawn.Adresse);
                            break;
                        case "bonus":
                            listeBonus.Add(spawn.bonus);
                            break;
                        default:
                            break;
                    }
                }
            }
            #endregion
            #region Gestion des tirs du joueur

            if (keyboardState.IsKeyDown(Keys.F1) && lastKeyDown)
            {
                lastKeyDown = false;
                if (drawSpectre)
                    drawSpectre = false;
                else
                    drawSpectre = true;
            }
            else if(keyboardState.IsKeyUp(Keys.F1))
                lastKeyDown = true;
			
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                switch (listeVaisseau[0].armeActuelle)
                {
                    case 0:
                        if (time - lastTime > 150 || lastTime == 0)
                        {
                            musique_tir.Play();
                            Vector2 spawn = new Vector2(listeVaisseau[0].position.X + 35, listeVaisseau[0].position.Y + listeVaisseau[0]._textureVaisseau.Height / 3 - 6);
                            listeMissile.Add(new Xspace.Missile1_joueur(T_Missile_Joueur_1, spawn));
                            lastTime = time;
                        }
                        break;
                    case 1:
                        if (time - lastTime > 150 || lastTime == 0)
                        {
                            musique_tir.Play();
                            Vector2 spawn1 = new Vector2(listeVaisseau[0].position.X + 35, listeVaisseau[0].position.Y + listeVaisseau[0]._textureVaisseau.Height / 3 - 6 - 12);
                            Vector2 spawn2 = new Vector2(listeVaisseau[0].position.X + 35, listeVaisseau[0].position.Y + listeVaisseau[0]._textureVaisseau.Height / 3 - 6 + 28);
                            listeMissile.Add(new Xspace.Missile1_joueur(T_Missile_Joueur_1, spawn1));
                            listeMissile.Add(new Xspace.Missile1_joueur(T_Missile_Joueur_1, spawn2));
                            lastTime = time;
                        }
                        break;
                    default:
                        break;
                }
            }
            #endregion
            #region Update des missiles
            foreach (Missiles missile in listeMissile)
            {
                if (missile.position.X < 1150 && !missile.ennemi)
                    missile.avancerMissile(fps_fix);
                else if (missile.position.X > 0 && missile.ennemi)
                    missile.avancerMissile(fps_fix);
                else
                    listeMissileToRemove.Add(missile);
            }

            foreach (Missiles missile in listeMissileToRemove)
            {
                listeMissile.Remove(missile);
            }

            listeMissileToRemove.Clear();
            #endregion
            #region Update des vaisseaux
            foreach (Vaisseau vaisseau in listeVaisseau)
            {
                if (vaisseau.existe == false)
                    listeVaisseauToRemove.Add(vaisseau);
                else if (vaisseau.ennemi)
                    vaisseau.Update(fps_fix);
                else
                    vaisseau.Update(fps_fix, keyboardState);

                if (vaisseau.ennemi && vaisseau.existe)
                {
                    if (time - vaisseau.lastTir > vaisseau.timingAttack)
                    {
                        Vector2 spawn = new Vector2(vaisseau.position.X - 35, vaisseau.position.Y + vaisseau._textureVaisseau.Height / 3 - 6);
                        // FAIRE EN FONCTION DU TYPE DE MISSILE
                        listeMissile.Add(new Missile_drone(T_Missile_Drone, spawn));
                        vaisseau.lastTir = time;
                    }
                } 
            }

            foreach (Vaisseau vaisseau in listeVaisseauToRemove)
            {
                listeVaisseau.Remove(vaisseau);
            }
            
            listeVaisseauToRemove.Clear();
            #endregion
            #region Update des bonus
            foreach (Bonus bonus in listeBonus)
            {
                if (bonus.position.X > 0)
                    bonus.Update(fps_fix);
                else
                    listeBonusToRemove.Add(bonus);
            }

            foreach (Bonus bonus in listeBonusToRemove)
                listeBonus.Remove(bonus);
            #endregion
            #region Collisions & Update des particules
            if (!(partManage.startingParticle == Vector2.Zero))
                particleEffect.Trigger(partManage.startingParticle);
            partManage = collisions(listeVaisseau, listeMissile, listeBonus, fps_fix, particleEffect, gameTime);

            particleEffect.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            #endregion
            #region Update spectre & historique
            if (lastTimeSpectre < time + 25)
            {
                float[] spectre_tmp = AudioPlayer.GetSpectrum(128);
                if (spectre_tmp.Length == 128)
                {
                    for (int i = 0; i <= 127; i++)
                    {
                        spectre_tmp[i] = Math.Min(1, spectre_tmp[i] * 20);
                    }

                    lastTimeSpectre = time;
                    spectre = spectre_tmp;
                }
            }

            if (lastTimeEnergy + 50 < time)
            {
                lastTimeEnergy = time;
                music_energy = AudioPlayer.Energy();
            }

            #endregion
            #region Fin du level (fuck u)
            if (listeVaisseau[0].ennemi) // NERVOUS WOKIN' ON IT OK ?!
            {
                AudioPlayer.StopMusic();
                AudioPlayer.PlayMusic("Musiques\\Menu\\Musique.mp3");
                path_level = "Scores\\Arcade\\lvl1" + ".score";
                sr_level = new StreamReader(path_level);
                score_level = System.IO.File.ReadAllLines(@path_level);
                stock_score_inferieur = "";
                stock_score_superieur = "";

                for (int i = 0; i < 10; i+=2) 
                {
                        
                    if (score < Convert.ToInt32(score_level[i + 1]))
                        stock_score_inferieur += score_level[i] + '\n' + score_level[i+1] +'\n';
                    else
                        stock_score_superieur += score_level[i] + '\n' + score_level[i+1] +'\n';
                }

                
                sr_level.Close();
                sw_level = new StreamWriter(path_level);
                sw_level.WriteLine(stock_score_inferieur + "Nervous" + '\n' + Convert.ToString(score) + '\n' + stock_score_superieur);
                sw_level.Close();

                Remove();
            }
            #endregion
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {

            SpriteBatch spriteBatch = SceneManager.SpriteBatch;

            SceneManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Transparent, 0, 0);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            fond_ecran.Draw(spriteBatch);
            //fond_ecran_front.Draw(spriteBatch);
            
            spriteBatch.Draw(T_HUD, new Vector2(0, 380), Color.White);
            HUD.Drawbar(spriteBatch, barre_vie, listeVaisseau[0].vie, listeVaisseau[0].vieMax);
            spriteBatch.Draw(T_HUD_bars, new Vector2(380, 630), Color.White);
            if(listeVaisseau[0].vie > 0)
                spriteBatch.DrawString(_gameFont, Convert.ToString(listeVaisseau[0].vie) + "/" + Convert.ToString(listeVaisseau[0].vieMax), new Vector2(500,450), Color.Red);

            spriteBatch.DrawString(_ingameFont, Convert.ToString(score), new Vector2(147, 680), Color.Black);
            //particleRenderer.RenderEffect(particleEffect);
            #region Draw des vaisseaux
            foreach (Vaisseau vaisseau in listeVaisseau)
            {
                vaisseau.Draw(spriteBatch);
            }
            #endregion
            #region Draw des missiles
            foreach (Missiles sMissile in listeMissile)
            {
                sMissile.Draw(spriteBatch);
            }
            #endregion
            #region Draw des bonus
            foreach (Bonus bonus in listeBonus)
            {
                bonus.Draw(spriteBatch);
            }
            #endregion
            #region Draw du spectre
            if (spectre.Length == 128 && drawSpectre)
            {
                int pxBegin = (Xspace.Xspace.window_height - 512) / 2;
                Texture2D empty_texture = new Texture2D(SceneManager.GraphicsDevice, 1, 1, true, SurfaceFormat.Color);
                empty_texture.SetData(new[] { Color.White });

                for (int i = 0; i <= 127; i++)
                {
                    int lenght = (int) (spectre[i] * 250);
                    for (int j = 0; j <= lenght / 4; j++)
                    {
                        Rectangle r = new Rectangle(Xspace.Xspace.window_width - lenght + j * 4, pxBegin + i * 4, j * 4, 4);
                        spriteBatch.Draw(empty_texture, r, new Color(j, 128 - j * 4, 0));
                    }
                }

                spriteBatch.DrawString(_gameFont, Convert.ToString(music_energy), new Vector2(100, 100), Color.LightGreen);
            }
            #endregion
            #region Draw du menu de pause
            if (TransitionPosition > 0 || _pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, _pauseAlpha / 2);
                //SceneManager.FadeBackBufferToBlack(alpha);
            }
            base.Draw(gameTime);
            #endregion



            spriteBatch.End();
        }

        protected override void UnloadContent()
        {
            _content.Unload();
        }
    }
}
