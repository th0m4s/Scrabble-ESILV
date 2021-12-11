using System;
using System.Collections.Generic;
using System.IO;

namespace A2_POO_Scrabble
{
    class Program
    {
        static void AfficherTitre()
        {
            int margin_x = (Console.WindowWidth - 64) / 2;
            int margin_y = (Console.WindowHeight - 12) / 2;

            for (int i = 0; i < margin_y; i++)
                Console.WriteLine();

            string margin = "";
            for (int i = 0; i < margin_x; i++)
                margin += " ";

            Console.WriteLine(margin + "███████╗ ██████╗██████╗  █████╗ ██████╗ ██████╗ ██╗     ███████╗");
            Console.WriteLine(margin + "██╔════╝██╔════╝██╔══██╗██╔══██╗██╔══██╗██╔══██╗██║     ██╔════╝");
            Console.WriteLine(margin + "███████╗██║     ██████╔╝███████║██████╔╝██████╔╝██║     █████╗  ");
            Console.WriteLine(margin + "╚════██║██║     ██╔══██╗██╔══██║██╔══██╗██╔══██╗██║     ██╔══╝  ");
            Console.WriteLine(margin + "███████║╚██████╗██║  ██║██║  ██║██████╔╝██████╔╝███████╗███████╗");
            Console.WriteLine(margin + "╚══════╝ ╚═════╝╚═╝  ╚═╝╚═╝  ╚═╝╚═════╝ ╚═════╝ ╚══════╝╚══════╝");

            Console.Write("\n\n" + margin + "            Appuyez sur une touche pour commencer...");

            Console.ReadKey();
            Console.Clear();
        }

        static void Main(string[] args)
        {
            AfficherTitre();

            if(!File.Exists("Francais.txt") || !File.Exists("Jetons.txt"))
            {
                Console.WriteLine("Merci de copier les fichiers Jetons.txt et Francais.txt du dossier runtime dans le dossier du .exe (bin\\Debug\\net5.0)");
                return;
            }

            Dictionnaire dictionnaire = new("français", File.ReadAllLines("Francais.txt"));
            Sac_Jetons sacJetons = new(File.ReadAllLines("Jetons.txt"));
            Plateau plateau = new();

            List<Joueur> joueurs = null;

            if (File.Exists("InstancePlateau.txt") && File.Exists("InstanceJetons.txt") && File.Exists("Joueurs.txt"))
            {
                Console.WriteLine("Une partie en cours n'a pas été terminée lors de la dernière exécution du programme ?");
                Console.WriteLine("Souhaitez-vous la reprendre ? (o/oui ou y/yes pour continuer cette partie, autre chose pour en commencer une nouvelle)\n\n\n\n /!\\ Attention, commencer une nouvelle partie supprimera les données de la partie en cours !");

                Console.CursorTop = 3;
                Console.CursorLeft = 2;

                string reponse = Console.ReadLine().Trim().ToUpper();
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
