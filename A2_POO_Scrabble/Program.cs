using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace A2_POO_Scrabble
{
    class Program
    {
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
            int margin_y = (Console.WindowHeight - 12) / 2;

            for (int i = 0; i < margin_y; i++)
                Console.WriteLine();

            string margin = RepeatChar(' ', margin_x);
            string empty = RepeatChar(' ', 64);

            AfficherLigneTitre(margin, empty);
            AfficherLigneTitre(margin, "███████╗ ██████╗██████╗  █████╗ ██████╗ ██████╗ ██╗     ███████╗");
            AfficherLigneTitre(margin, "██╔════╝██╔════╝██╔══██╗██╔══██╗██╔══██╗██╔══██╗██║     ██╔════╝");
            AfficherLigneTitre(margin, "███████╗██║     ██████╔╝███████║██████╔╝██████╔╝██║     █████╗  ");
            AfficherLigneTitre(margin, "╚════██║██║     ██╔══██╗██╔══██║██╔══██╗██╔══██╗██║     ██╔══╝  ");
            AfficherLigneTitre(margin, "███████║╚██████╗██║  ██║██║  ██║██████╔╝██████╔╝███████╗███████╗");
            AfficherLigneTitre(margin, "╚══════╝ ╚═════╝╚═╝  ╚═╝╚═╝  ╚═╝╚═════╝ ╚═════╝ ╚══════╝╚══════╝");
            AfficherLigneTitre(margin, empty);

            Console.Write("\n\n" + margin + "                Appuyez sur une touche pour commencer...");

            Console.ReadKey();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Clear();
        }

        internal static string RepeatChar(char c, int count)
        {
            return string.Join("", Enumerable.Range(0, count).Select(x => c));
        }

        internal static string PoserQuestionLarge(string question, string below = "")
        {
            Console.Clear();

            int questionStartY = 1;
            int questionStartX = 2;

            int boxWidth = Console.WindowWidth - 2 * questionStartX;
            string xBorder = Program.RepeatChar('═', boxWidth - 2);

            Console.SetCursorPosition(questionStartX, questionStartY); // border top
            Console.Write("╔" + xBorder + "╗");

            string[] lignes = question.Split('\n');
            for (int i = 0; i < lignes.Length; i++)
            {
                Console.SetCursorPosition(questionStartX, questionStartY + 1 + i);

                string ligne = lignes[i];
                Console.Write("║ " + ligne + Program.RepeatChar(' ', boxWidth - 3 - ligne.Length) + "║");
            }

            int repStartY = questionStartY + 1 + lignes.Length;
            Console.SetCursorPosition(questionStartX, repStartY);
            string emptyLine = "║" + Program.RepeatChar(' ', boxWidth - 2) + "║";
            Console.Write(emptyLine);

            int heightBelow = 0;
            if (below != null && below.Trim().Length > 0)
            {
                string[] lignesBelow = below.Split('\n');
                heightBelow = 1 + lignesBelow.Length;

                Console.SetCursorPosition(questionStartX, repStartY + 1);
                Console.Write(emptyLine);

                for (int i = 0; i < lignesBelow.Length; i++)
                {
                    string ligne = lignesBelow[i];
                    Console.SetCursorPosition(questionStartX, repStartY + 2 + i);
                    Console.Write("║ " + ligne + Program.RepeatChar(' ', boxWidth - 3 - ligne.Length) + "║");
                }
            }

            Console.SetCursorPosition(questionStartX, repStartY + 1 + heightBelow);
            Console.Write("╚" + xBorder + "╝");

            Console.SetCursorPosition(questionStartX + 2, repStartY);
            return Console.ReadLine();
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
                string reponse = PoserQuestionLarge("Une partie en cours n'a pas été terminée lors de dernière exécution du programme.\n"
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
