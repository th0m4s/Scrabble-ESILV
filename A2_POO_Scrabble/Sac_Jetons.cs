using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace A2_POO_Scrabble
{
    class Sac_Jetons
    {
        List<Jeton> jetons;

        static Dictionary<char, int> scores = new Dictionary<char, int>();

        public Sac_Jetons(string[] lignes)
        {
            this.jetons = new List<Jeton>();
            foreach(var ligne in lignes)
            {
                string[] parts = ligne.Split(";");
                int score = int.Parse(parts[1]);
                char lettre = parts[0][0];

                if(!scores.ContainsKey(lettre))
                    scores.Add(lettre, score);

                for (int i = 0; i < int.Parse(parts[2]); i++)
                    jetons.Add(new Jeton(lettre, score));
            }
        }

        public void SauvegarderSacJetons(string fichier)
        {
            File.WriteAllLines(fichier, jetons.GroupBy(x => x.Lettre).Select(x => x.Key + ";" + x.First().Score + ";" + x.Count()));
        }

        public Jeton Retire_Jeton(Random r)
        {
            int pos = r.Next(jetons.Count);
            Jeton jeton = jetons[pos];
            jetons.RemoveAt(pos);

            return jeton;
        }

        public void Ajouter_Jeton(Jeton jeton)
        {
            jetons.Add(jeton);
        }

        public int NombreJetons()
        {
            return jetons.Count;
        }

        public override string ToString()
        {
            return "Liste des jetons :\n" + string.Join("\n", jetons.Select(j => "  - " + j.ToString()));
        }

        public static int Score_Pour_Lettre(char lettre)
        {
            return scores.GetValueOrDefault(lettre, 0);
        }
    }
}
