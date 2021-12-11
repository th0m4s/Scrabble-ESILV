using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace A2_POO_Scrabble
{
    class Jeu
    {
        Dictionnaire dictionnaire;
        Plateau plateau;
        Sac_Jetons sacJetons;
        List<Joueur> joueurs;

        Random random;

        public Jeu(Dictionnaire dictionnaire, Plateau plateau, Sac_Jetons sacJetons)
        {
            this.dictionnaire = dictionnaire;
            this.plateau = plateau;
            this.sacJetons = sacJetons;

            this.random = new Random();
        }

        public void DemanderJoueurs()
        {
            int nb = -1;
            while(nb < 2 || nb > 4)
            {
                Console.Clear();
                Console.WriteLine("BIENVENUE AU SCRABBLE !");
                Console.Write("Combien de joueurs vont jouer ? (entre 2 et 4) ");
                int.TryParse(Console.ReadLine(), out nb);
            }

            joueurs = new List<Joueur>();

            for(int i = 0; i < nb; i++)
            {
                string nom = null;

                do
                {
                    Console.Clear();
                    if (nom != null)
                        Console.WriteLine("Ce nom est déjà utilisé !");
                    Console.Write("Entrez le nom du joueur " + (i + 1) + " : ");

                    nom = Console.ReadLine();

                    if (nom.Trim().Length == 0) nom = null;
                    else if (joueurs.Where(j => j.Nom == nom).Count() > 0) nom = "";
                } while (nom == null || nom.Length == 0);

                Joueur joueur = new Joueur(nom);
                for (int j = 0; j < 7; j++)
                    joueur.Add_Main_Courante(sacJetons.Retire_Jeton(random));
                joueur.Add_Main_Courante(new Jeton('*', 0));

                joueurs.Add(joueur);
            }
        }

        private void AfficherTour(int tour, string nom = null)
        {
            Console.Clear();
            plateau.Afficher();

            if(nom != null)
                Console.WriteLine("\nTour n°" + tour + ", c'est à " + nom + " de jouer !");
        }

        public void Jouer()
        {
            if (joueurs == null) return;

            int tour = 1;
            while(sacJetons.NombreJetons() > 0)
            {
                foreach(Joueur j in joueurs)
                {
                    string mot;
                    int ligne = -1, colonne = -1;
                    char direction = '\0';

                    AfficherTour(tour, j.Nom);
                    Console.WriteLine("Appuie sur une touche pour voir ta main...");
                    Console.ReadKey();

                    bool motPlace = true;

                    do
                    {
                        AfficherTour(tour, j.Nom);
                        Console.Write("Voici ta main :\n\n   ");
                        j.AfficherMain();

                        Console.Write("\n\nQuel mot veux tu jouer ? (en incluant les lettres déjà placées)\nN'écris rien pour remplacer tes jetons actuels.\n  ");
                        mot = Console.ReadLine().ToUpper();

                        if(mot.Length == 0)
                        {
                            Console.Write("Veux-tu vraiment remplacer tous tes jetons et passer ton tour ?\n(écris o/oui ou y/yes pour continuer ou autre chose pour annuler)\n  ");
                            string confirm = Console.ReadLine().Trim().ToUpper();

                            if (confirm == "O" || confirm == "OUI" || confirm == "Y" || confirm == "YES")
                            {
                                j.Remplacer_Jetons(sacJetons, random);
                                Console.WriteLine("\nTes jetons ont été remplacés !");
                                motPlace = false;
                                break;
                            }
                            else continue;
                        }

                        mot = Regex.Replace(mot, "[^A-Z]", "");
                        if (mot.Length == 0) continue;

                        Console.Write("\nA quelle position veux tu jouer ce mot ? (écrire la position de la 1re case - ie. en haut ou à gauche - séparés par un espace, ligne puis colonne :\n  ");
                        string pos = Console.ReadLine().Trim();
                        string[] posparts = pos.Split(',', ';', ' ', '/');

                        if (posparts.Length != 2) continue;
                        if (!int.TryParse(posparts[0], out ligne) || !int.TryParse(posparts[1], out colonne)) continue;

                        Console.Write("\nDans quelle direction veux-tu placer ce mot ? (l/ligne ou c/colonne)\n  ");
                        string dir = Console.ReadLine().Trim().ToUpper();
                        if (dir == "L" || dir == "LIGNE") direction = 'L';
                        else if (dir == "C" || dir == "COLONNE") direction = 'C';
                        else continue;

                    } while (!plateau.Test_Plateau(mot, ligne-1, colonne-1, direction, dictionnaire, j));
                    
                    if(motPlace)
                    {
                        AfficherTour(tour);
                        Console.WriteLine("\nTu as maintenant un score de " + j.Score);
                    }

                    Console.WriteLine("Appuie sur une touche pour continuer...");

                    while (j.Nombre_Jetons() < 7)
                        j.Add_Main_Courante(sacJetons.Retire_Jeton(random));

                    Console.ReadKey();
                }

                tour++;
            }
        }
    }
}
