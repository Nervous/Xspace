using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace Xspace
{
    class gestionLevels
    {
        int nbLevel;
        string pathLevel;
        string[] infosLevel;


        string categorie, position;
        Vaisseau adresse;
        Bonus bonusAdresse;
        Obstacles obstacleAdresse;
        public Boss boss;
        double time;
        bool hasSpawned;
        List<Texture2D> listeTextureVaisseauxEnnemis, listeTextureBonus, listeTextureObstacles, listeTextureBoss;


        public gestionLevels(int numero, List<Texture2D> texturesVaisseaux, List<Texture2D> texturesBonus, List<Texture2D> textureObstacles, List<Texture2D> textureBoss)
        {
            nbLevel = numero;
            pathLevel = "levels/" + nbLevel + ".xpa"; ;
            infosLevel = lireFichier(pathLevel);
            listeTextureVaisseauxEnnemis = texturesVaisseaux;
            listeTextureBonus = texturesBonus;
            listeTextureObstacles = textureObstacles;
            listeTextureBoss = textureBoss;
        }

        public gestionLevels(string setCategorie, Vaisseau setAdresse, Bonus adresseBonus, Obstacles adresseObstacle, Boss adresseBoss, int setTime, string setPosition)
        {
            categorie = setCategorie;
            adresse = setAdresse;
            bonusAdresse = adresseBonus;
            obstacleAdresse = adresseObstacle;
            boss = adresseBoss;
            time = setTime;
            hasSpawned = false;
            position = setPosition;
        }

        public gestionLevels(int setTime)
        {
            categorie = "EOL";
            time = setTime;
        }

        public string[] lireFichier(string path)
        {
            string[] lines = System.IO.File.ReadAllLines(@path);
            return lines;
        }

        public string[] getInfosLevel
        {
            get { return infosLevel; }
        }

        public void readInfos(char[] delimitationFilesInfo, char[] delimitationFilesInfo2, char[] delimitationFilesInfo3, List<gestionLevels> infLevel)
        {
            foreach (string info in this.getInfosLevel) // Pour chacune des lignes du level ...
            {
                int timing = 0, i = 0;
                string categorie = "", type = "", position = "";
                Vector2 start;

                foreach (string info2 in info.Split(delimitationFilesInfo)) // ... On récupère 2 infos : le type de l'objet et à quelle date il doit spawn
                {
                    i = 0;
                    if (!int.TryParse(info2, out timing)) // SI l'info n'est pas un nombre, alors c'est la catégorie de l'objet (vaisseau, bonus, obstacle, etc.)
                    {
                        foreach (string info3 in info2.Split(delimitationFilesInfo3))
                        {
                            if(info3.Contains(";")) // Si on trouve le caratère ";", alors c'est les infos level (ex : vaisseau;drone)
                            {
                                foreach (string info4 in info3.Split(delimitationFilesInfo2))
                                {
                                    if (i == 0) // Premiere info : catégorie de l'objet
                                        categorie = info4;
                                    else // Deuxième info : type de l'objet
                                        type = info4;

                                    i++;
                                }
                            }
                            else
                                position = info3;
                        }
                    }
                    else
                        timing = int.Parse(info2);
                    
                }

                switch (position)
                {
                    case "0":
                        start = new Vector2(1180, 5);
                        break;
                    case "1":
                        start = new Vector2(1180, 67);
                        break;
                    case "2":
                        start = new Vector2(1180, 129);
                        break;
                    case "3":
                        start = new Vector2(1180, 191);
                        break;
                    case "4":
                        start = new Vector2(1180, 253);
                            break;
                    case "5":
                        start = new Vector2(1180, 315);
                            break;
                    case "6":
                        start = new Vector2(1180, 377); 
                            break;
                        case "7":
                        start = new Vector2(1180, 439); 
                            break;
                    case "8":
                        start = new Vector2(1180, 501); 
                            break;
                    case "9":
                        start = new Vector2(1180, 563);
                            break;
                    default:
                        start = new Vector2(1180, 620 / 3);
                        break;
                }

                //Fin de lecture de la ligne : on ajoute un élement dans la liste des infos du level
                if (categorie == "vaisseau")
                {
                    Vaisseau vaisseau = null;
                    

                    switch (type)
                    {
                        case "drone":
                            vaisseau = new Drone(listeTextureVaisseauxEnnemis[0], start);
                            break;
                        case "kamikaze":
                            vaisseau = new kamikaze(listeTextureVaisseauxEnnemis[1], start);
                            break;
                        case "rapidshooter":
                            vaisseau = new RapidShooter(listeTextureVaisseauxEnnemis[3], start);
                            break;
                        case "blasterer":
                            vaisseau = new Blasterer(listeTextureVaisseauxEnnemis[2], start);
                            break;
                        default:
                            break;
                    }
                    infLevel.Add(new gestionLevels(categorie, vaisseau, null, null, null, timing, position));
                }
                else if(categorie == "bonus")
                {
                    Bonus bonus = null;
                    switch(type)
                    {
                        case "vie":
                            bonus = new Bonus_Vie(listeTextureBonus[0], start);
                            break;
                        case "baseWeapon":
                            bonus = new Bonus_BaseWeapon(listeTextureBonus[1], start);
                            break;
                        case "score":
                            bonus = new Bonus_Score(listeTextureBonus[2], start);
                            break;
                        case "energie":
                            bonus = new Bonus_Energie(listeTextureBonus[3], start);
                            break;
                        default:
                            break;
                    }
                    infLevel.Add(new gestionLevels(categorie, null, bonus, null, null, timing, position));
                }
                else if (categorie == "obstacle")
                {
                    Obstacles obstacle = null;
                    switch (type)
                    {
                        case "hole":
                            obstacle = new Obstacles_Hole(listeTextureObstacles[0], start);
                            break;
                        default:
                            break;
                    }
                    infLevel.Add(new gestionLevels(categorie, null, null, obstacle, null, timing, position));
                }
                else if (categorie == "boss")
                {
                    Boss boss = null;
                    switch (type)
                    {
                        case "boss1":
                            boss = new Boss1(listeTextureBoss[0], Boss.phaseArray1);
                            break;
                        case "boss2":
                            boss = new Boss2(listeTextureBoss[1], Boss.phaseArray2);
                            break;
                        case "boss3":
                            boss = new Boss3(listeTextureBoss[2], Boss.phaseArray3);
                            break;
                        default:
                            break;
                    }
                    infLevel.Add(new gestionLevels(categorie, null, null, null, boss, timing, position));
                }
                else if (categorie == "eol")
                {
                    infLevel.Add(new gestionLevels(timing));
                }

                
                

            }
        }

        public Vaisseau Adresse
        {
            get { return adresse; }
        }
        public Bonus bonus
        {
            get { return bonusAdresse; }
        }
        public Obstacles Obstacle
        {
            get { return obstacleAdresse; }
        }

        public string Categorie
        {
            get { return categorie; }
        }

        public string Position
        {
            get { return position; }
        }

        public bool isTime(double actualTime)
        {
            bool toReturn = false;
            if (actualTime > this.time && hasSpawned == false)
                toReturn = true;
            else
                toReturn = false;
            if(toReturn == true)
                this.hasSpawned = true;
            return toReturn;
        }
    }
}
