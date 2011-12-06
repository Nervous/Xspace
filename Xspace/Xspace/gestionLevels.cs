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

using Xspace;

namespace Xspace
{
    class gestionLevels
    {
        int nbLevel;
        string pathLevel;
        string[] infosLevel;


        string categorie, position;
        Vaisseau_ennemi adresse;
        double time;
        bool hasSpawned;
        List<Texture2D> listeTextureVaisseauxEnnemis;


        public gestionLevels(int numero, List<Texture2D> texturesVaisseaux)
        {
            nbLevel = numero;
            pathLevel = "levels/" + nbLevel + ".xpa"; ;
            infosLevel = lireFichier(pathLevel);
            listeTextureVaisseauxEnnemis = texturesVaisseaux;
        }

        public gestionLevels(string setCategorie, Vaisseau_ennemi setAdresse, int setTime, string setPosition)
        {
            categorie = setCategorie;
            adresse = setAdresse;
            time = setTime;
            hasSpawned = false;
            position = setPosition;
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
                Vaisseau_ennemi vaisseau = null;
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
                                    {
                                        categorie = info4;
                                    }
                                    else // Deuxième info : type de l'objet
                                    {
                                        type = info4;
                                    }
                                    i++;
                                }
                            }
                            else
                            {
                                position = info3;
                            }
                        }
                    }
                    else
                        timing = int.Parse(info2);
                    
                }
                //Fin de lecture de la ligne : on ajoute un élement dans la liste des infos du level
                if (categorie == "vaisseau")
                {
                    switch (type)
                    {
                        case "drone":
                            vaisseau = new Vaisseau_ennemi(listeTextureVaisseauxEnnemis[0], "drone", position);
                            break;
                        default:
                            break;
                    }
                }

                infLevel.Add(new gestionLevels(categorie, vaisseau, timing, position));
                

            }
        }

        public Vaisseau_ennemi Adresse
        {
            get { return adresse; }
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

        public void makeItSpawn()
        {
            switch (categorie)
            {
                case "vaisseau":
                    if(adresse != null)
                        this.adresse.creer();
                    break;
                default:
                    break;
            }
        }
    }
}
