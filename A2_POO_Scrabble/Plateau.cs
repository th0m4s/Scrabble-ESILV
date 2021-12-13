using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace A2_POO_Scrabble
{
    public class Plateau
    {
        #region Attributs
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
        #endregion


        #region Constructeurs
        /// <summary>
        /// Créé un plateau vide sans aucun jeton.
        /// </summary>
        public Plateau()
        {
            VerifierPoids();
            grille = new Jeton[poids.GetLength(0), poids.GetLength(1)];
        }

        /// <summary>
        /// Créé un plateau à partir des lignes d'un fichier de sauvegarde.
        /// </summary>
        /// <param name="lignes">Les lignes du fichier de sauvegarde contenant les jetons placés.</param>
        public Plateau(string[] lignes)
        {
            VerifierPoids();
            int nb_lignes = poids.GetLength(0), nb_col = poids.GetLength(1);
            grille = new Jeton[nb_lignes, nb_col];

            if (lignes.Select(x => x.Trim().Length > 0).Count() == nb_lignes)
            {
                for (int i = 0; i < nb_lignes; i++)
                {
                    string[] parts = lignes[i].Split(';');
                    for (int j = 0; j < Math.Min(parts.Length, nb_col); j++)
                    {
                        string part = parts[j];
                        if (part.Length == 1 && part[0] != '_') grille[i, j] = new Jeton(part[0]);
                        else if (part.Length == 2 && part[1] == '*') grille[i, j] = new Jeton(part[0], 0);
                    }
                }
            }
        }
        #endregion


        #region Méthodes publiques
        /// <summary>
        /// Sauvegarde les jestons placés sur le plateau dans un fichier.
        /// </summary>
        /// <param name="fichier">Le nom du fichier à sauvegarder.</param>
        public void SauvegarderPlateau(string fichier)
        {
            List<string> lignes = new List<string>();
            for(int i = 0; i < grille.GetLength(0); i++)
            {
                List<string> parts = new List<string>();
                for(int j = 0; j < grille.GetLength(1); j++)
                {
                    Jeton jeton = grille[i, j];
                    if (jeton == null) parts.Add("_");
                    else if (jeton.Score == 0) parts.Add(jeton.Lettre + "*");
                    else parts.Add(jeton.Lettre.ToString());
                }
                lignes.Add(string.Join(';', parts));
            }

            File.WriteAllLines(fichier, lignes);
        }

        /// <summary>
        /// Affiche la grille, les poids et jetons placés sur la Console en utilisant des couleurs.
        /// </summary>
        public void Afficher()
        {
            string grilleSeparatorX = Utils.RepeatChar('═', 5 + poids.GetLength(1)*2);
            Console.WriteLine("\n  ╔" + grilleSeparatorX + "╗");

            Console.Write("  ║ ");
            for(int i = 1; i <= poids.GetLength(1); i+=2)
            {
                Console.Write("  " + (i <= 11 ? " " : "") + i);
            }
            Console.WriteLine(" ║");

            Console.Write("  ║   ");
            for (int i = 2; i <= poids.GetLength(1); i += 2)
            {
                Console.Write("  " + (i <= 11 ? " " : "") + i);
            }
            Console.WriteLine("   ║");

            for (int y = 0; y < poids.GetLength(0); y++)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;

                Console.Write("  ║ " + (y < 9 ? " " : "") + (y + 1) + " ");

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

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine(" ║");
            }

            Console.WriteLine("  ╠" + grilleSeparatorX + "╣");

            int lineLength = grilleSeparatorX.Length;

            AfficherLigneLegende(ConsoleColor.Cyan, "Lettre compte double", lineLength);
            AfficherLigneLegende(ConsoleColor.Blue, "Lettre compte triple", lineLength);
            AfficherLigneLegende(ConsoleColor.Magenta, "Mot compte double", lineLength);
            AfficherLigneLegende(ConsoleColor.Red, "Mot compte triple", lineLength);

            Console.WriteLine("  ╚" + grilleSeparatorX + "╝");
        }

        /// <summary>
        /// Méthode "foure-tout" demandée par l'énoncé qui renvoie si un mot peut être placé sur le plateau en fonction de la main d'un joueur et des jetons déjà placés.
        /// </summary>
        /// <param name="mot">Le mot à placer.</param>
        /// <param name="ligne">L'indice de la ligne de la 1re case.</param>
        /// <param name="colonne">L'indice de la colonne de la 1re case.</param>
        /// <param name="direction">L pour ligne ou C pour colonne.</param>
        /// <param name="dico">L'instance de dictionnaire à utiliser pour vérifier les mots.</param>
        /// <param name="joueur">Le joueur qui cherche à placer de mot.</param>
        /// <returns>Un bool indiquant si le mot peut être placé.</returns>
        public bool Test_Plateau(string mot, int ligne, int colonne, char direction, Dictionnaire dico, Joueur joueur)
        {
            // On vérifie si la direction est valide
            if (direction != 'L' && direction != 'C') return false;

            // On vérifie la position de la 1re case (est-ce qu'elle se trouve sur le plateau)
            if (ligne < 0 || colonne < 0) return false;

            int ligneFin = ligne, colonneFin = colonne;
            if (direction == 'L') colonneFin += mot.Length;
            else ligneFin += mot.Length;

            // On vérifie la position de la dernière case du mot
            if (ligneFin >= grille.GetLength(0) || colonneFin >= grille.GetLength(1)) return false;

            // On vérifie que le mot se trouve dans le dictionnaire
            if (!dico.RechercheDichoRecursif(mot)) return false;

            bool hasCommonLetter = false;
            List<char> requiredChars = new List<char>();
            List<string> autresMots = new List<string>();
            int score = 0;

            // On créé des variables pour modifier la position d'un curseur en fonction de la direction
            int dx = 0, dy = 0;
            if (direction == 'L') dx++;
            else dy++;

            bool auMilieu = false;

            // On parcourt le mot une 1re fois pour vérifier si les lettres déjà placées sont bonnes et
            // que les nouveaux mots formés existent
            for (int i = 0; i < mot.Length; i++)
            {
                if (poids[ligne, colonne] == 9) auMilieu = true;

                // Lettre actuelle ou '\0', on utilise l'operateur null coalescing ??
                char actuel = grille[ligne, colonne]?.Lettre ?? '\0';
                char dansMot = mot[i];
                if (actuel == '\0')
                {
                    // La lettre n'est pas sur le plateau, il va falloir la prendre dans la main
                    requiredChars.Add(dansMot);

                    string nouveau = "";
                    if (dx == 0) // mot vertical, donc on check sur la même ligne
                    {
                        nouveau = MotDepuisPosition(ligne, colonne, 'L', dansMot);
                    }
                    else // mot horizontal
                    {
                        nouveau = MotDepuisPosition(ligne, colonne, 'C', dansMot);
                    }

                    // Si un mot est formé par les lettres adjacentes, on le sauvegarde ainsi que son possible score
                    if (nouveau.Length > 1)
                    {
                        autresMots.Add(nouveau);
                        foreach (char c in nouveau)
                        {
                            score += new Jeton(c).Score;
                        }
                    }
                }
                else if (actuel == dansMot) hasCommonLetter = true; // Si la lettre est déjà placée, on note que c'est une lettre commune
                else return false; // S'il y a déjà une autre lettre, on ne peut pas placer le mot...

                ligne += dy;
                colonne += dx;
            }

            // Si il n'y a pas de lettre commune et que ce n'est pas le 1er mot, on ne peut pas placer le mot
            if (!hasCommonLetter && !auMilieu) return false;

            // On vérifie que tous les autres mots formés existent dans le dictionnaire
            if (autresMots.Where(x => !dico.RechercheDichoRecursif(x)).Count() > 0) return false;

            // On créé une collection d'object représentants les lettres requises et leur nombre
            // On doit utiliser var car c'est un type anonyme décrit par 2 attributs Lettre et Nombre
            var requiredCounts = requiredChars.GroupBy(x => x).Select(s => new
            {
                Lettre = s.Key,
                Nombre = s.Count()
            });

            // On récupère parmi les lettres requises celles que le joueur n'a pas
            Dictionary<char, int> manquants = requiredCounts.Where(x => joueur.Nombre_Lettre(x.Lettre) < x.Nombre).Select(x => new
            {
                Lettre = x.Lettre,
                Nombre = x.Nombre - joueur.Nombre_Lettre(x.Lettre) // on soustrait ce qu'il a, par exemple il peut avoir besoin de 2*B mais en avoir au moins 1
            }).ToDictionary(x => x.Lettre, x => x.Nombre);
            int nbManquants = manquants.Values.Sum();
            int jokers = joueur.Nombre_Lettre('*');

            // Si on a pas assez de jokers pour pallier à ces manques, on ne peut pas placer le mot
            if (nbManquants - jokers > 0) return false;

            // Pour chaque lettre requise, on enlève un jeton de la main
            foreach (var requiredPair in requiredCounts)
            {
                for (int i = 0; i < requiredPair.Nombre; i++)
                    joueur.Remove_Main_Courante(requiredPair.Lettre);
            }

            // On enlève autant de jokers que de jetons manquants
            for (int i = 0; i < nbManquants; i++)
                joueur.Remove_Main_Courante('*');

            // On replace notre "curseur" au début du mot car on va reparcourir la grille pour le score et placer les jetons
            ligne -= dy * mot.Length;
            colonne -= dx * mot.Length;

            int scoreMot = 0;
            int motMult = 1;

            for (int i = 0; i < mot.Length; i++)
            {
                char lettre = mot[i];

                if (manquants.GetValueOrDefault(lettre, 0) > 0)
                {
                    // S'il nous manque ce jeton, on place un joker sur la grille et on augmente pas le score

                    manquants[lettre]--;
                    grille[ligne, colonne] = new Jeton(lettre, 0);
                }
                else
                {
                    // Sinon on place le jeton sur la grille

                    int lettreMult = 1;
                    int _poids = poids[ligne, colonne];

                    if (_poids > 0 && _poids < 9) lettreMult = _poids; // on multiplie le score de cette lettre
                    else if (_poids < 0) motMult = Math.Max(motMult, -_poids); // on multiplie le score de tout le mot

                    Jeton j = new Jeton(lettre);
                    scoreMot += lettreMult * j.Score;

                    grille[ligne, colonne] = j;
                }

                ligne += dy;
                colonne += dx;
            }

            if (auMilieu && !hasCommonLetter)
                motMult = 2; // le 1er mot placé a son score doublé

            joueur.Add_Score(score + scoreMot * motMult);
            joueur.Add_Mot(mot);

            foreach (string autreMot in autresMots)
                joueur.Add_Mot(autreMot);

            return true;
        }
        #endregion


        #region Méthodes privées
        /// <summary>
        /// Construit la grille des poids à partir d'un quart (le supérieur gauche) de la grille.
        /// </summary>
        static void VerifierPoids()
        {
            if (poids == null)
            {
                int taille = poidsQuart.GetLength(0) * 2 - 1;
                milieu = poidsQuart.GetLength(0) - 1;
                poids = new int[taille, taille];

                for (int y = 0; y < taille; y++)
                {
                    for (int x = 0; x < taille; x++)
                    {
                        int dx = x, dy = y;
                        if (dx > milieu) dx = taille - x - 1;
                        if (dy > milieu) dy = taille - y - 1;

                        poids[y, x] = poidsQuart[dy, dx];
                    }
                }
            }
        }

        /// <summary>
        /// Affiche une ligne de la légende sous le plateau.
        /// </summary>
        /// <param name="color">La couleur de la zone.</param>
        /// <param name="legende">Le texte correspondant à cette couleur.</param>
        /// <param name="length">La largeur de la zone contenant les légendes.</param>
        private void AfficherLigneLegende(ConsoleColor color, string legende, int length)
        {
            Console.Write("  ║ ");

            Console.BackgroundColor = color;
            Console.Write("  ");

            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine(" " + legende + Utils.RepeatChar(' ', length - 4 - legende.Length) + "║");
        }

        /// <summary>
        /// Méthode utilitaire pour extraire une colonne d'une matrice sous forme de liste.
        /// </summary>
        /// <param name="x">L'indice de la colonne à récupérer.</param>
        /// <returns>La colonne sous forme de liste.</returns>
        private List<char> GetColumn(int x)
        {
            return Enumerable.Range(0, grille.GetLength(0)).Select(y => grille[y, x]?.Lettre ?? '\0').ToList();
        }

        /// <summary>
        /// Méthode utilitaire pour extraire une ligne d'une matrice sous forme de liste.
        /// </summary>
        /// <param name="y">L'indice de la ligne à récupérer.</param>
        /// <returns>La ligne sous forme de liste.</returns>
        private List<char> GetRow(int y)
        {
            return Enumerable.Range(0, grille.GetLength(1)).Select(x => grille[y, x]?.Lettre ?? '\0').ToList();
        }

        /// <summary>
        /// Méthode utilitaire pour récupérer le mot formé par les possibles jetons autour d'une case.
        /// </summary>
        /// <param name="ligne">L'indice de la ligne du jeton placé.</param>
        /// <param name="colonne">L'indice de la colonne du jeton placé.</param>
        /// <param name="direction">C pour colonne ou L pour ligne.</param>
        /// <param name="lettre">La lettre placée sur la case donnée par ligne et colonne.</param>
        /// <returns>Le possible mot formé autour de cette position ou seulement la lettre si les cases autour sont vides.</returns>
        private string MotDepuisPosition(int ligne, int colonne, char direction, char lettre)
        {
            return direction == 'L' ? (string.Join("", GetRow(ligne).SkipLast(grille.GetLength(1) - colonne).Reverse().TakeWhile(x => x != '\0').Reverse()))
                + lettre + string.Join("", GetRow(ligne).Skip(colonne + 1).TakeWhile(x => x != '\0'))
                : (string.Join("", GetColumn(colonne).SkipLast(grille.GetLength(0) - ligne).Reverse().TakeWhile(x => x != '\0').Reverse())
                + lettre + string.Join("", GetColumn(colonne).Skip(ligne + 1).TakeWhile(x => x != '\0')));
        }
        #endregion
    }
}
