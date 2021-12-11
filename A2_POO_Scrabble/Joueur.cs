using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2_POO_Scrabble
{
    class Joueur
    {
        string nom;
        int score;
        List<string> motsTrouves;
        List<Jeton> mainCourante;

        public string Nom => nom;
        public int Score => score;

        public Joueur(string nom)
        {
            this.nom = nom;
            this.score = 0;
            this.motsTrouves = new List<string>();
            this.mainCourante = new List<Jeton>();
        }

        public Joueur(string[] lignes)
        {
            string[][] parts = lignes.Select(x => x.Split(";")).ToArray();

            this.nom = parts[0][0];
            this.score = int.Parse(parts[0][1]);

            this.motsTrouves = parts[1].ToList();
            this.mainCourante = parts[2].Select(x => new Jeton(x[0])).ToList();
        }

        public override string ToString()
        {
            return nom + " : " + score + " point" + (score == 1 ? "" : "s") + "\nMots trouvés :\n   " + string.Join(", ", motsTrouves)
                + "\nJetons courants : \n" + string.Join("\n", mainCourante.Select(j => "  - " + j.ToString()));
        }

        public void Add_Mot(string mot)
        {
            motsTrouves.Add(mot);
        }

        public void Add_Score(int val)
        {
            if (val > 0)
                score += val;
        }

        public int Nombre_Lettre(char lettre)
        {
            return mainCourante.Where(x => x == lettre).Count();
        }

        public int Nombre_Jetons()
        {
            return mainCourante.Count;
        }

        public void Add_Main_Courante(Jeton monjeton)
        {
            mainCourante.Add(monjeton);
        }

        public void Remove_Main_Courante(Jeton monjeton)
        {
            mainCourante.Remove(monjeton);
        }

        public void Remove_Main_Courante(char lettre)
        {
            Remove_Main_Courante(new Jeton(lettre));
        }

        public void Remplacer_Jetons(Sac_Jetons sac, Random random)
        {
            int nb = mainCourante.Count;

            foreach (Jeton j in mainCourante)
                sac.Ajouter_Jeton(j);

            mainCourante.Clear();

            for (int i = 0; i < nb; i++)
                mainCourante.Add(sac.Retire_Jeton(random));
        }

        public void AfficherMain()
        {
            foreach(Jeton j in mainCourante)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.DarkYellow;

                Console.Write(j.Lettre + " ");

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.BackgroundColor = ConsoleColor.Black;

                Console.Write("  ");
            }
        }
    }
}
