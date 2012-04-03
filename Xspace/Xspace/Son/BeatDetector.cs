using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xspace
{
    /* Algo utilisé adapté de : http://www.youtube.com/watch?v=jZoQ1S73Bac */

    static class BeatDetector
    {
        /* Déclaration des constantes */
        const double K_ENERGIE_RATIO = 1.3; // energie1024 par rapport aux energie44100 pour trouver les peaks
        const int K_TRAIN_DIMP_SIZE = 108; // en pack de 1024 (430=10sec)

        private static uint length;
        private static float[] energie1024;
        private static float[] energie44100;
        private static float[] energie_peak;
        private static float[] conv;
        private static float[] beat;
        private static int tempo;
        private static float moy_energie1024;
        private static float moy_energie44100;

        public static void Initialize()
        {
            length = AudioPlayer.GetLength();
            energie1024 = new float[length / 1024];
            energie44100 = new float[length / 1024];
            conv = new float[length / 1024];
            beat = new float[length / 1024];
            energie_peak = new float[length / 1024 + 21];
            for (int i = 0; i < length / 1024 + 21; i++)
                energie_peak[i] = 0;
        }

        private static int energie(int[] data, int offset, int window)
        {
            float energie = 0f;
            for (int i = offset; (i < offset + window) && (i < length); i++)
            {
                energie = energie + data[i] * data[i] / window;
            }
            return (int)energie;
        }

        private static void normalize(float[] signal, int size, float max_val)
        {
            // Recherche de la valeur max du signal
            float max = 0f;
            for (int i = 0; i < size; i++)
            {
                if (Math.Abs(signal[i]) > max) max = Math.Abs(signal[i]);
            }

            // Ajustement du signal
            float ratio = max_val / max;
            for (int i = 0; i < size; i++)
            {
                signal[i] = signal[i] * ratio;
            }
        }

        private static int search_max(float[] signal, int pos, int fenetre_half_size)
        {
            float max = 0f;
            int max_pos = pos;
            for (int i = pos - fenetre_half_size; i <= pos + fenetre_half_size; i++)
            {
                if (signal[i] > max)
                {
                    max = signal[i];
                    max_pos = i;
                }
            }
            return max_pos;
        }

        public static void audio_process()
        {
            int[] data = AudioPlayer.GetMusicData();

            // Calcul des énergies instantanées
            for (int i = 0; i < length / 1024; i++)
            {
                energie1024[i] = energie(data, 1024 * i, 4096);
            }

            energie44100[0] = 0;

            float somme = 0f;
            for (int i = 0; i < 43; i++)
            {
                somme = somme + energie1024[i];
            }
            energie44100[0] = somme / 43;

            for (int i = 1; i < length / 1024 - 43; i++)
            {
                somme = somme - energie1024[i - 1] + energie1024[i + 42];
                energie44100[i] = somme / 43;
            }

            moy_energie1024 = energie1024.Sum() / energie1024.Length;
            moy_energie44100 = energie44100.Sum() / energie44100.Length;

            /* Ratio energie1024/energie44100 */
            for (int i = 21; i < length / 1024; i++)
            {
                if (energie1024[i] > K_ENERGIE_RATIO * energie44100[i - 21])
                {
                    energie_peak[i] = 1;
                }
            }

            /* Calcul des BPMs */

            List<int> T = new List<int>();
            int i_prec = 0;
            for (int i = 1; i < length / 1024; i++)
            {
                if ((energie_peak[i] == 1) && (energie_peak[i - 1] == 0))
                {
                    int di = i - i_prec;
                    if (di > 5)
                    {
                        T.Add(di);
                        i_prec = i;
                    }
                }
            }

            int T_occ_max = 0;
            float T_occ_moy = 0f;


            int[] occurences_T = new int[86];
            for (int i = 0; i < 86; i++)
                occurences_T[i] = 0;
            for (int i = 1; i < T.Count; i++)
            {
                if (T[i] < 86) occurences_T[T[i]]++;
            }
            int occ_max = 0;
            for (int i = 1; i < 86; i++)
            {
                if (occurences_T[i] > occ_max)
                {
                    T_occ_max = i;
                    occ_max = occurences_T[i];
                }
            }

            int voisin = T_occ_max - 1;

            if (occurences_T[T_occ_max + 1] > occurences_T[voisin]) voisin = T_occ_max + 1;
            float div = occurences_T[T_occ_max] + occurences_T[voisin];

            if (div == 0) T_occ_moy = 0;
            else T_occ_moy = (float)(T_occ_max * occurences_T[T_occ_max] + (voisin) * occurences_T[voisin]) / div;

            tempo = (int)(60f / (T_occ_moy * 1024f / 44100f));

            // Calcul de la Beat line
            float[] train_dimp = new float[K_TRAIN_DIMP_SIZE];
            float espace = 0f;
            train_dimp[0] = 1f;
            for (int i = 1; i < K_TRAIN_DIMP_SIZE; i++)
            {
                if (espace >= T_occ_moy)
                {
                    train_dimp[i] = 1;
                    espace = espace - T_occ_moy;
                }
                else train_dimp[i] = 0;
                espace += 1f;
            }

            for (int i = 0; i < length / 1024 - K_TRAIN_DIMP_SIZE; i++)
            {
                for (int j = 0; j < K_TRAIN_DIMP_SIZE; j++)
                {
                    conv[i] = conv[i] + energie1024[i + j] * train_dimp[j];
                }

            }
            normalize(conv, (int)length / 1024, 1f);

            for (int i = 1; i < length / 1024; i++)
                beat[i] = 0;

            float max_conv = 0f;
            int max_conv_pos = 0;
            for (int i = 1; i < length / 1024; i++)
            {
                if (conv[i] > max_conv)
                {
                    max_conv = conv[i];
                    max_conv_pos = i;
                }
            }
            beat[max_conv_pos] = 1f;

            int k = max_conv_pos + T_occ_max;
            while ((k < length / 1024) && (conv[k] > 0f))
            {
                int conv_max_pos_loc = search_max(conv, k, 2);
                beat[conv_max_pos_loc] = 1f;
                k = conv_max_pos_loc + T_occ_max;
            }

            k = max_conv_pos - T_occ_max;
            while (k > 2)
            {
                int conv_max_pos_loc = search_max(conv, k, 2);
                beat[conv_max_pos_loc] = 1f;
                k = conv_max_pos_loc - T_occ_max;
            }
            
        }

        public static float[] get_energie1024()
        {
            return energie1024;
        }

        public static float get_moy_energie1024()
        {
            return moy_energie1024;
        }

        public static float[] get_energie44100()
        {
            return energie44100;
        }

        public static float get_moy_energie44100()
        {
            return moy_energie44100;
        }

        public static float[] get_energie_peak()
        {
            return energie_peak;
        }

        public static float[] get_conv()
        {
            return conv;
        }

        public static float[] get_beat()
        {
            return beat;
        }

        public static int get_tempo()
        {
            return tempo;
        }
    }
}
