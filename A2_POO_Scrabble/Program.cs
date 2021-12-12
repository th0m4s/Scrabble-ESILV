using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace A2_POO_Scrabble
{
    class Program
    {
        public static string[] LOGO => new string[]
        {
            "███████╗ ██████╗██████╗  █████╗ ██████╗ ██████╗ ██╗     ███████╗",
            "██╔════╝██╔════╝██╔══██╗██╔══██╗██╔══██╗██╔══██╗██║     ██╔════╝",
            "███████╗██║     ██████╔╝███████║██████╔╝██████╔╝██║     █████╗  ",
            "╚════██║██║     ██╔══██╗██╔══██║██╔══██╗██╔══██╗██║     ██╔══╝  ",
            "███████║╚██████╗██║  ██║██║  ██║██████╔╝██████╔╝███████╗███████╗",
            "╚══════╝ ╚═════╝╚═╝  ╚═╝╚═╝  ╚═╝╚═════╝ ╚═════╝ ╚══════╝╚══════╝"
        };

        /// <summary>
        /// Affiche <b>une ligne</b> du titre avec les bonnes couleurs et marges.
        /// </summary>
        /// <param name="margin">La marge à côté du titre.</param>
        /// <param name="ligne">La ligne du titre à afficher.</param>
        private static void AfficherLigneTitre(string margin, string ligne)
        {
            Console.Write(margin);

            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine("   " + ligne + "   ");
            Console.BackgroundColor = ConsoleColor.DarkGreen;
        }

        /// <summary>
        /// Affiche le titre du jeu et attend une confirmation avant de commencer.
        /// </summary>
        static void AfficherTitre()
        {
            Console.Title = "Scrabble";

            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear(); // mettre toute la console en vert
            

            int margin_x = (Console.WindowWidth - 64) / 2 - 4;
            int margin_y = (Console.WindowHeight - LOGO.Length * 2) / 2;

            for (int i = 0; i < margin_y; i++)
                Console.WriteLine();

            string margin = Utils.RepeatChar(' ', margin_x);
            string empty = Utils.RepeatChar(' ', 64);

            AfficherLigneTitre(margin, empty);
            foreach (string ligne in LOGO)
                AfficherLigneTitre(margin, ligne);
            AfficherLigneTitre(margin, empty);

            Console.Write("\n\n" + margin + "                Appuyez sur une touche pour commencer...");

            Console.ReadKey();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Clear();
        }

        static void Main(string[] args)
        {
            AfficherTitre();

            if(!File.Exists("Francais.txt") || !File.Exists("Jetons.txt"))
            {
                Console.WriteLine("Merci de copier les fichiers Jetons.txt et Francais.txt du dossier runtime dans le dossier du .exe (bin\\Debug\\net5.0)");
                Console.ReadKey();
                return;
            }

            Dictionnaire dictionnaire = new("français", File.ReadAllLines("Francais.txt"));
            Sac_Jetons sacJetons = new(File.ReadAllLines("Jetons.txt"));
            Plateau plateau = new();

            List<Joueur> joueurs = null;

            if (File.Exists("InstancePlateau.txt") && File.Exists("InstanceJetons.txt") && File.Exists("Joueurs.txt"))
            {
                string reponse = Utils.PoserQuestionLarge("Une partie en cours n'a pas été terminée lors de dernière exécution du programme.\n"
                    + "Entrez o/oui ou y/yes pour continuer cette partie, autre chose pour en commencer une nouvelle :", "/!\\ Attention, commencer une nouvelle partie supprimera les données de la partie en cours !").Trim().ToUpper();
                if(reponse == "O" || reponse == "OUI" || reponse == "Y" || reponse == "YES")
                {
                    sacJetons = new(File.ReadAllLines("InstanceJetons.txt"));
                    plateau = new(File.ReadAllLines("InstancePlateau.txt"));
                    joueurs = Joueur.ChargerJoueurs(File.ReadAllLines("Joueurs.txt"));
                } else
                {
                    File.Delete("InstancePlateau.txt");
                    File.Delete("InstanceJetons.txt");
                    File.Delete("Joueurs.txt");
                }
            }

            Jeu jeu = new(dictionnaire, plateau, sacJetons, joueurs);

            if (joueurs == null)
                jeu.DemanderJoueurs();

            Console.Clear();
            Console.WriteLine("Appuyez sur une touche pour commencer la partie...");

            jeu.Jouer();
        }
    }
}
