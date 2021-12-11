using System;
using System.Collections.Generic;
using System.Linq;

namespace A2_POO_Scrabble
{
    class Joueur
    {
        string nom;
        int score;
        List<string> motsTrouves;
        List<Jeton> mainCourante;
        int tours;

        public int Tours
        {
            set { tours = value; }
            get { return tours;  }
        }

        public string Nom => nom;
        public int Score => score;

        /// <summary>
        /// Créé un joueur à partir d'un nom. Tous les autres champs sont remplis par défaut.
        /// </summary>
        /// <param name="nom"></param>
        public Joueur(string nom)
        {
            this.nom = nom;
            this.score = 0;
            this.tours = 0;
            this.motsTrouves = new List<string>();
            this.mainCourante = new List<Jeton>();
        }

        /// <summary>
        /// Créé un joueur à partir de 3 lignes de sauvegarde provenant d'un fichier.
        /// Sur la 1re ligne doit se trouver le nom, le score et le nombre de tours joués, sur la 2e les mots et la 3e les jetons dans sa main.
        /// </summary>
        /// <param name="lignes">Les lignes provenant du fichier de sauvegarde.</param>
        public Joueur(string[] lignes)
        {
            string[][] parts = lignes.Select(x => x.Split(";")).ToArray();

            this.nom = parts[0][0];
            if (!int.TryParse(parts[0].ElementAtOrDefault(2) ?? "0", out this.tours)) this.tours = 0; // default value for string is null
            this.score = int.Parse(parts[0][1]);

            this.motsTrouves = parts[1].Where(x => x.Length > 0).ToList();
            this.mainCourante = parts[2].Select(x => new Jeton(x[0])).ToList();
        }

        public override string ToString()
        {
            return nom + " : " + score + " point" + (score == 1 ? "" : "s") + "\nMots trouvés :\n   " + string.Join(", ", motsTrouves)
                + "\nJetons courants : \n" + string.Join("\n", mainCourante.Select(j => "  - " + j.ToString()));
        }

        /// <summary>
        /// Ajoute un mot à la liste des mots trouvés.
        /// </summary>
        /// <param name="mot">Le mot à ajouter.</param>
        public void Add_Mot(string mot)
        {
            motsTrouves.Add(mot);
        }

        /// <summary>
        /// Ajoute des points au score du joueur.
        /// </summary>
        /// <param name="val">Le nombre de points à ajouter. Doit être positif.</param>
        public void Add_Score(int val)
        {
            if (val > 0)
                score += val;
        }

        /// <summary>
        /// Compte le nombre de fois qu'une lettre est présente dans la main du joueur.
        /// </summary>
        /// <param name="lettre">La lettre à compter.</param>
        /// <returns>Le nombre de fois que cette lettre est présente.</returns>
        public int Nombre_Lettre(char lettre)
        {
            return mainCourante.Where(x => x == lettre).Count();
        }

        /// <summary>
        /// Retourne le nombre complet de jetons dans la main du joueur.
        /// </summary>
        /// <returns>Le nombre de jetons dans la main du joueur.</returns>
        public int Nombre_Jetons()
        {
            return mainCourante.Count;
        }

        /// <summary>
        /// Ajoute un jeton dans la main du joueur.
        /// </summary>
        /// <param name="monjeton">Le jeton à ajouter dans la main du joueur.</param>
        public void Add_Main_Courante(Jeton monjeton)
        {
            mainCourante.Add(monjeton);
        }

        /// <summary>
        /// Retire un jeton de la main du joueur.
        /// </summary>
        /// <param name="monjeton">Le jeton à retirer de la main.</param>
        public void Remove_Main_Courante(Jeton monjeton)
        {
            mainCourante.Remove(monjeton);
        }

        /// <summary>
        /// Retire un jeton de la main du joueur à partir de la lettre qu'il représente.
        /// </summary>
        /// <param name="lettre">La lettre du jeton à retirer.</param>
        public void Remove_Main_Courante(char lettre)
        {
            Remove_Main_Courante(new Jeton(lettre));
        }

        /// <summary>
        /// Remplace tous les jetons de la main d'un joueur.
        /// </summary>
        /// <param name="sac">Le sac de jetons dans quel remettre les jetons et en reprendre.</param>
        /// <param name="random">L'instance de Random servant à piocher les jetons.</param>
        public void Remplacer_Jetons(Sac_Jetons sac, Random random)
        {
            int nb = mainCourante.Count;

            foreach (Jeton j in mainCourante)
                sac.Ajouter_Jeton(j);

            mainCourante.Clear();

            for (int i = 0; i < nb; i++)
                mainCourante.Add(sac.Retire_Jeton(random));
        }

        /// <summary>
        /// Affiche avec des couleurs tous les jetons dans la main d'un joueur sur la Console.
        /// </summary>
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

        /// <summary>
        /// Renvoie les 3 lignes qui servent à la sauvegarde d'un joueur dans un fichier texte.
        /// Voir le constructeur pour les détails sur le contenu de ces 3 lignes.
        /// </summary>
        /// <returns>Les 3 lignes représentant le joueur.</returns>
        public string[] Sauvegarder()
        {
            return new string[] {
                nom + ";" + score + ";" + tours, motsTrouves.Count == 0 ? ";" : string.Join(";", motsTrouves), string.Join(";", mainCourante.Select(x => x.Lettre))
            };
        }

        /// <summary>
        /// Permet de transformer les lignes d'un fichier de sauvegarde en une liste de joueurs.
        /// Chaque trio de ligne est transformé en joueur via le constructeur.
        /// </summary>
        /// <param name="lignes">Les lignes du fichier de sauvegarde.</param>
        /// <returns>La liste des joueurs contenus dans la sauvegarde.</returns>
        public static List<Joueur> ChargerJoueurs(string[] lignes)
        {
            List<string> _lignes = lignes.Select(x => x.Trim()).Where(x => x.Length > 0).ToList();
            if (_lignes.Count % 3 != 0) return null;

            List<Joueur> joueurs = new List<Joueur>();
            for(int i = 0; i < _lignes.Count; i+=3)
                joueurs.Add(new Joueur(_lignes.GetRange(i, 3).ToArray()));

            return joueurs;
        }
    }
}
