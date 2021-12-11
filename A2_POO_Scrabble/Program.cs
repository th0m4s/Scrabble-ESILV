using System;
using System.IO;

namespace A2_POO_Scrabble
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionnaire dictionnaire = new("français", File.ReadAllLines("Francais.txt"));
            Sac_Jetons sacJetons = new(File.ReadAllLines("Jetons.txt"));
            Plateau plateau = new();

            Jeu jeu = new(dictionnaire, plateau, sacJetons);
            jeu.DemanderJoueurs();

            Console.Clear();
            Console.WriteLine("Appuyez sur une touche pour commencer la partie...");

            jeu.Jouer();
        }
    }
}
