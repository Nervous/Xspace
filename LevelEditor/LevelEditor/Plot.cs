using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LevelEditor
{
    class Plot
    {
        string _position, _time;
        int _selected,  _compteur;
        int _page;

        public Plot(string position, string time, int page, int compteur, int selected, System.Windows.Forms.PictureBox pictureBox, System.Drawing.Image saveimage)
        {
            Stream stream;
            _position = position;
            _time = time;
            _selected = selected;
            _page = page;
            _compteur = compteur;
            if (_compteur == 0 && _page == 0 && _selected != (-1)) //suppression de l'ancien fichier de travail
            {
                stream = new FileStream("Work", FileMode.Create);
                /*StreamWriter sw2 = new StreamWriter(stream);
                for (int i = 0; i < 72; i++)
                    sw2.Write('\n');*/
            }
            else
                stream = new FileStream("Work", FileMode.Open);
            StreamWriter sw = new StreamWriter(stream);
            StreamReader sr = new StreamReader(stream);
            string vaisseau;
            string text;
                 switch (selected) // ICI AJOUTER LES NOUVEAUX VAISSEAUX/BONUS
                {
                    case 1:
                        vaisseau = "drone";
                        break;
                    case 2:
                        vaisseau = "rapidshooter";
                        break;
                    case 3:
                        vaisseau = "blasterer";
                        break;
                    case 4:
                        vaisseau = "kamikaze";
                        break;
                    case 5:
                        vaisseau = "vie";
                        break;
                    case 6:
                        vaisseau = "baseWeapon";
                        break;
                    case 7:
                        vaisseau = "vie";
                        break;
                    default:
                        vaisseau = "drone";
                        break;
                }
             //   if (_selected >0)
               // {
                    if ((page == 0)||(selected == 0))
                    {
                        if (_compteur == 0)
                        {
                            for (int i = 0; i < Convert.ToInt16(_position); i++)
                                sw.Write("\n");

                            for (int i = Convert.ToInt16(_position); i < 72; i++)
                            {
                                if (i == 71)
                                    sw.Write("");
                                else
                                    sw.Write("\n");
                            }

                            _compteur++;
                        }
                    }
                    else // on démarre de la fin du fichier pour une nouvelle page
                    {
                        string texttest = sr.ReadToEnd();
                        string[] texttestArray = texttest.Split('\n');
                        if (texttestArray.Length < 72 * (page + 1))
                        {
                            if (_compteur == 0)
                            {
                                for (int i = 0; i < Convert.ToInt16(_position); i++)
                                    sw.Write("\n");

                                for (int i = Convert.ToInt16(_position); i < 72; i++)
                                {
                                    if (i == 73)
                                        sw.Write("");
                                    else
                                        sw.Write("\n");
                                }

                                _compteur++;
                            }
                        }
                    }
               // }
                sw.Close();
                stream.Close();
                Stream stream_int = new FileStream("Work", FileMode.Open);
                StreamReader sr_int = new StreamReader(stream_int);

                text = sr_int.ReadToEnd();
                string[] textArray = text.Split('\n');
                sr_int.Close();
                stream_int.Close();

                Stream stream_new = new FileStream("Work", FileMode.Open);
                StreamReader sr_new = new StreamReader(stream_new);

                string type = "";

                if ((selected == 6) || (selected == 7))
                    type = "bonus";
                else
                    type = "vaisseau";

                if ((selected == 0)&&(compteur!=0))
                    textArray[Convert.ToInt16(_position) + page * 72 - 1 + 9 * ((Convert.ToInt32(_time) - 400) / ((400 + 3200 * page)))] = ("                                                                                                                       ");// epic bug de buffer confirmed by nurelin, xavier, et lecuyer, don't try to guess why.
                else if ((selected < 10)&&(selected >0))
                    textArray[Convert.ToInt16(_position) + page * 72 - 1 + 9 * ((Convert.ToInt32(_time) - 400) / ((400 + 3200*page)))] = (type + ";" + vaisseau + ":" + position + " " + time);

                StreamWriter sw_new = new StreamWriter(stream_new);
                for (int i = 0; i < textArray.Length; i++)
                {
                    if (i == textArray.Length - 1)
                        sw_new.Write(textArray[i]);
                    else
                        sw_new.Write(textArray[i] + '\n');
                }
                sw_new.Close();
                stream_new.Close();

                pictureBox.Image = saveimage;
            
        }

        public void Load(List<System.Windows.Forms.PictureBox> boxList, int page) 
        {
            List<System.Drawing.Image> listImageToLoad = new List<System.Drawing.Image>();
            Stream stream = new FileStream("Work", FileMode.Open);
            StreamReader sr = new StreamReader(stream);
            string text = sr.ReadToEnd();
            string[] textArray = text.Split('\n');

                for (int i = 72 * page; i < textArray.Length; i++) // ICI AJOUTER LES NOUVEAUX VAISSEAUX/BONUS
                {
                    if (textArray[i].Contains("drone"))
                        listImageToLoad.Add(global::LevelEditor.Properties.Resources.drone);
                    else if (textArray[i].Contains("kamikaze"))
                        listImageToLoad.Add(global::LevelEditor.Properties.Resources.Kamikaze);
                    else if (textArray[i].Contains("blasterer"))
                        listImageToLoad.Add(global::LevelEditor.Properties.Resources.energizer);
                    else if (textArray[i].Contains("rapidshooter"))
                        listImageToLoad.Add(global::LevelEditor.Properties.Resources.doubleshooter);
                    else if (textArray[i].Contains("Life"))
                        listImageToLoad.Add(global::LevelEditor.Properties.Resources.Life);
                    else if (textArray[i].Contains("baseWeapon"))
                        listImageToLoad.Add(global::LevelEditor.Properties.Resources.DoubleBaseWeapon);
                    else
                        listImageToLoad.Add(global::LevelEditor.Properties.Resources.Sans_titre);
                }

                foreach (System.Windows.Forms.PictureBox box in boxList)
                {
                    box.Image = listImageToLoad.First();
                    listImageToLoad.RemoveAt(0);
                }
                sr.Close();
                stream.Close();
        }
        
        internal int New_compteur()
        {
            return (_compteur);
        }


    }
}
