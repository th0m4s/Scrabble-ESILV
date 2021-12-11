using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace A2_POO_Scrabble
{
    class Plateau
    {
        static int[,] poidsQuart = new int[,] {
            { -3,  0,  0,  2,  0,  0,  0, -3 },
            {  0, -2,  0,  0,  0,  3,  0,  0 },
            {  0,  0, -2,  0,  0,  0,  2,  0 },
            {  2,  0,  0, -2,  0,  0,  0,  2 },
            {  0,  0,  0,  0, -2,  0,  0,  0 },
            {  0,  3,  0,  0,  0,  3,  0,  0 },
            {  0,  0,  2,  0,  0,  0,  2,  0 },
            { -3,  0,  0,  2,  0,  0,  0,  9 }
        };
        // on pourrait construire la grille avec 1/8 seulement, mais pour simplier la création on prend 1/4

        static int[,] poids = null;
        static int milieu = -1;

        Jeton[,] grille;

        static void VerifierPoids()
        {
            if(poids == null)
            {
                int taille = poidsQuart.GetLength(0) * 2 - 1;
                milieu = poidsQuart.GetLength(0) - 1;
                poids = new int[taille, taille];

                for(int y = 0; y < taille; y++)
                {
                    for(int x = 0; x < taille; x++)
                    {
                        int dx = x, dy = y;
                        if (dx > milieu) dx = taille - x - 1;
                        if (dy > milieu) dy = taille - y - 1;

                        poids[y, x] = poidsQuart[dy, dx];
                    }
                }
            }
        }

        public Plateau()
        {
            VerifierPoids();
            grille = new Jeton[poids.GetLength(0), poids.GetLength(1)];
        }

        public Plateau(string[] lignes)
        {
            VerifierPoids();
        }

        public void Afficher()
        {
            Console.Write("   ");
            for(int i = 1; i <= poids.GetLength(1); i+=2)
            {
                Console.Write((i <= 11 ? " " : "") + i + "  ");
            }
            Console.WriteLine();

            Console.Write("     ");
            for (int i = 2; i <= poids.GetLength(1); i += 2)
            {
                Console.Write((i <= 10 ? " " : "") + i + "  ");
            }
            Console.WriteLine();

            for (int y = 0; y < poids.GetLength(0); y++)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;

                Console.Write((y < 9 ? "  " : " ") + (y + 1) + " ");

                for(int x = 0; x < poids.GetLength(1); x++)
                {
                    char c = grille[y, x]?.Lettre ?? '\0';
                    if(c == '\0')
                    {
                        switch(poids[y, x])
                        {
                            case -3:
                                Console.BackgroundColor = ConsoleColor.Red;
                                break;
                            case -2:
                                Console.BackgroundColor = ConsoleColor.Magenta;
                                break;
                            case 2:
                                Console.BackgroundColor = ConsoleColor.Cyan;
                                break;
                            case 3:
                                Console.BackgroundColor = ConsoleColor.Blue;
                                break;
                            case 9: // case du départ, ne change pas le score
                                Console.BackgroundColor = ConsoleColor.DarkRed;
                                break;
                            case 0: // pas très utile de mettre ce case mais c'est pour montrer qu'il existe
                            default:
                                Console.BackgroundColor = ConsoleColor.DarkGreen;
                                break;

                        }

                        Console.Write("  ");
                    } else
                    {
                        Console.BackgroundColor = ConsoleColor.DarkYellow;

                        if (grille[y, x].Score == 0)
                            Console.ForegroundColor = ConsoleColor.White;
                        else Console.ForegroundColor = ConsoleColor.Black;

                        Console.Write(c + " ");
                    }
                }

                Console.WriteLine();
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        List<char> GetColumn(int x)
        {
            return Enumerable.Range(0, grille.GetLength(0)).Select(y => grille[y, x]?.Lettre ?? '\0').ToList();
        }

        List<char> GetRow(int y)
        {
            return Enumerable.Range(0, grille.GetLength(1)).Select(x => grille[y, x]?.Lettre ?? '\0').ToList();
        }

        string MotDepuisPosition(int ligne, int colonne, char direction, char lettre)
        {
            return direction == 'L' ? (string.Join("", GetRow(ligne).SkipLast(grille.GetLength(1) - colonne).Reverse().TakeWhile(x => x != '\0').Reverse()))
                + lettre + string.Join("", GetRow(ligne).Skip(colonne + 1).TakeWhile(x => x != '\0'))
                : (string.Join("", GetColumn(colonne).SkipLast(grille.GetLength(0) - ligne).Reverse().TakeWhile(x => x != '\0').Reverse())
                + lettre + string.Join("", GetColumn(colonne).Skip(ligne + 1).TakeWhile(x => x != '\0')));
        }

        public bool Test_Plateau(string mot, int ligne, int colonne, char direction, Dictionnaire dico, Joueur joueur)
        {
            if (direction != 'L' && direction != 'C') return false;

            if (ligne < 0 || colonne < 0) return false;
            int ligneFin = ligne, colonneFin = colonne;

            if (direction == 'L') colonneFin += mot.Length;
            else ligneFin += mot.Length;

            if (ligneFin >= grille.GetLength(0) || colonneFin >= grille.GetLength(1)) return false;

            if (!dico.RechercheDichoRecursif(mot)) return false;

            bool hasCommonLetter = false;
            List<char> requiredChars = new List<char>();
            List<string> autresMots = new List<string>();
            int score = 0;

            int dx = 0, dy = 0;
            if (direction == 'L') dx++;
            else dy++;

            bool auMilieu = false;

            for(int i = 0; i < mot.Length; i++)
            {
                if (poids[ligne, colonne] == 9) auMilieu = true;

                char actuel = grille[ligne, colonne]?.Lettre ?? '\0';
                char dansMot = mot[i];
                if (actuel == '\0') requiredChars.Add(dansMot);
                else if (actuel == dansMot) hasCommonLetter = true;
                else return false;

                string nouveau = "";
                if(dx == 0) // mot vertical, donc on check sur la même ligne
                {
                    nouveau = MotDepuisPosition(ligne, colonne, 'L', dansMot);
                } else // mot horizontal
                {
                    nouveau = MotDepuisPosition(ligne, colonne, 'C', dansMot);
                }

                if (nouveau.Length > 1)
                {
                    autresMots.Add(nouveau);
                    foreach(char c in nouveau)
                    {
                        score += new Jeton(c).Score;
                    }
                }

                ligne += dy;
                colonne += dx;
            }

            if (!hasCommonLetter && !auMilieu) return false;
            if (autresMots.Where(x => !dico.RechercheDichoRecursif(x)).Count() > 0) return false;

            var requiredCounts = requiredChars.GroupBy(x => x).Select(s => new
            {
                Lettre = s.Key,
                Nombre = s.Count()
            });

            Dictionary<char, int> manquants = requiredCounts.Where(x => joueur.Nombre_Lettre(x.Lettre) < x.Nombre).Select(x => new
            {
                Lettre = x.Lettre,
                Nombre = x.Nombre - joueur.Nombre_Lettre(x.Lettre)
            }).ToDictionary(x => x.Lettre, x => x.Nombre);
            int nbManquants = manquants.Values.Sum();
            int jokers = joueur.Nombre_Lettre('*');

            if (nbManquants - jokers > 0) return false;

            foreach (var requiredPair in requiredCounts)
            {
                for (int i = 0; i < requiredPair.Nombre; i++)
                    joueur.Remove_Main_Courante(requiredPair.Lettre);
            }

            for(int i = 0; i < nbManquants; i++)
            {
                joueur.Remove_Main_Courante('*');
            }

            ligne -= dy * mot.Length;
            colonne -= dx * mot.Length;

            int scoreMot = 0;
            int motMult = 1;

            for (int i = 0; i < mot.Length; i++)
            {
                char lettre = mot[i];

                if(manquants.GetValueOrDefault(lettre, 0) > 0)
                {
                    manquants[lettre]--;
                    grille[ligne, colonne] = new Jeton(lettre, 0);
                } else
                {
                    int lettreMult = 1;
                    int _poids = poids[ligne, colonne];

                    if (_poids > 0 && _poids < 9) lettreMult = _poids;
                    else if (_poids < 0) motMult = Math.Max(motMult, -_poids);

                    Jeton j = new Jeton(lettre);
                    scoreMot += lettreMult * j.Score;

                    grille[ligne, colonne] = j;
                }

                ligne += dy;
                colonne += dx;
            }

            joueur.Add_Score(score + scoreMot * motMult);
            joueur.Add_Mot(mot);

            foreach (string autreMot in autresMots)
                joueur.Add_Mot(autreMot);

            return true;
        }
    }
}
