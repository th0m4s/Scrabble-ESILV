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

        /// <summary>
        /// Créé un sac de jetons à partir d'une liste de jetons dans un fichier de sauvegarde.<br/>
        /// Sur chaque ligne se trouve une lettre, le score associé et le nombre de fois que cette lettre est présente.
        /// </summary>
        /// <param name="lignes">Les lignes du fichier de sauvegarde.</param>
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

        /// <summary>
        /// Sauvegarde les jetons actuels du sac dans un fichier.
        /// Voir le constructeur pour le contenu du fichier.
        /// </summary>
        /// <param name="fichier">Le nom du fichier à sauvegarder.</param>
        public void SauvegarderSacJetons(string fichier)
        {
            File.WriteAllLines(fichier, jetons.GroupBy(x => x.Lettre).Select(x => x.Key + ";" + x.First().Score + ";" + x.Count()));
        }

        /// <summary>
        /// Retire un jeton au hasard du sac.
        /// </summary>
        /// <param name="r">Une instance de Random pour la sélection aléatoire.</param>
        /// <returns>Le jeton retiré.</returns>
        public Jeton Retire_Jeton(Random r)
        {
            int pos = r.Next(jetons.Count);
            Jeton jeton = jetons[pos];
            jetons.RemoveAt(pos);

            return jeton;
        }

        /// <summary>
        /// Ajoute un jeton dans le sac.
        /// </summary>
        /// <param name="jeton">Le jeton à ajouter.</param>
        public void Ajouter_Jeton(Jeton jeton)
        {
            jetons.Add(jeton);
        }

        /// <summary>
        /// Compte le nombre de jetons dans le sac.
        /// </summary>
        /// <returns>Le nombre de jetons.</returns>
        public int NombreJetons()
        {
            return jetons.Count;
        }

        public override string ToString()
        {
            return "Liste des jetons :\n" + string.Join("\n", jetons.Select(j => "  - " + j.ToString()));
        }

        /// <summary>
        /// Récupère le score associé à une lettre à partir d'une sauvegarde de jetons.
        /// </summary>
        /// <param name="lettre">La lettre pour laquelle il faut rechercher le score.</param>
        /// <returns>Le score associé à la lettre.</returns>
        public static int Score_Pour_Lettre(char lettre)
        {
            return scores.GetValueOrDefault(lettre, 0);
        }
    }
}
