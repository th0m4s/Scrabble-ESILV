using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace A2_POO_Scrabble
{
    public class Jeu
    {
        const int STD_INPUT_HANDLE = -10;
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetStdHandle(int nStdHandle);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CancelIoEx(IntPtr handle, IntPtr lpOverlapped);
        static IntPtr StdHandle = GetStdHandle(STD_INPUT_HANDLE);
        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        internal static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);
        static IntPtr ConsoleWindowHnd = GetForegroundWindow();
        const int VK_RETURN = 0x0D;
        const int WM_KEYDOWN = 0x100;


        Dictionnaire dictionnaire;
        Plateau plateau;
        Sac_Jetons sacJetons;
        List<Joueur> joueurs;

        Random random;

        /// <summary>
        /// Créé une instance jeu à partir des données d'un dictionnaire, plateau, sac et d'une liste de joueurs pouvant être nulle.
        /// </summary>
        /// <param name="dictionnaire">Le dictionnaire contenant les mots autorisés pour la partie.</param>
        /// <param name="plateau">Le plateau de jeu au début de la partie.</param>
        /// <param name="sacJetons">Le sac de jetons encore disponibles.</param>
        /// <param name="joueurs">La liste des joueurs ou null si les joueurs doivent être demandés au début du jeu.</param>
        public Jeu(Dictionnaire dictionnaire, Plateau plateau, Sac_Jetons sacJetons, List<Joueur> joueurs = null)
        {
            this.dictionnaire = dictionnaire;
            this.plateau = plateau;
            this.sacJetons = sacJetons;
            this.joueurs = joueurs;

            this.random = new Random();
        }

        /// <summary>
        /// Permet de remplir la liste des joueurs si une nouvelle partie est commencée.
        /// </summary>
        public void DemanderJoueurs()
        {
            int nb = -1;
            while(nb < 2 || nb > 4)
            {
                Console.Clear();
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

                joueurs.Add(joueur);
            }
        }

        /// <summary>
        /// Affiche la grille ainsi que le numéro du tour et le nom du joueur qui doit jouer.
        /// </summary>
        /// <param name="tour">Le numéro du tour en cours.</param>
        /// <param name="nom">Le nom du joueur qui doit jouer.</param>
        private void AfficherTour(int tour, string nom = null)
        {
            Console.Clear();
            plateau.Afficher(joueurs, nom);

            if(nom != null)
                Console.WriteLine("\nTour n°" + tour + ", c'est à " + nom + " de jouer !");
        }

        /// <summary>
        /// Sauvegarde la liste des joueurs (dont leur score, mots et jetons) dans un fichier.
        /// </summary>
        /// <param name="fichier">Le nom du fichier à sauvegarder.</param>
        public void SauvegarderJoueurs(string fichier)
        {
            List<string> lignes = new List<string>();
            foreach(Joueur joueur in joueurs)
                lignes.AddRange(joueur.Sauvegarder());
            File.WriteAllLines(fichier, lignes);
        }

        /// <summary>
        /// Fonction principale s'apparentant au Main, contient toute la boucle de jeu.
        /// Cette méthode demande à tour de rôle quel mot placer sur la grille jusqu'à qu'il n'y ait plus de jetons dans le sac.
        /// </summary>
        public void Jouer()
        {
            if (joueurs == null) return;

            int tour = 1;

            int nbToursPremierJoueur = joueurs[0].Tours;
            int aPasser = (nbToursPremierJoueur - 1) * joueurs.Count + joueurs.TakeWhile(x => x.Tours == nbToursPremierJoueur).Count();

            while(sacJetons.NombreJetons() > 0)
            {
                foreach(Joueur joueur in joueurs)
                {
                    if(aPasser > 0)
                    {
                        aPasser--;
                        continue;
                    }

                    string mot;
                    int ligne = -1, colonne = -1;
                    char direction = '\0';

                    AfficherTour(tour, joueur.Nom);
                    Console.WriteLine("Appuie sur une touche pour voir ta main...");
                    Console.ReadKey();

                    bool motPlace = true;
                    bool tourFini = false;

                    Task task = Task.Delay(60_000).ContinueWith(_ => {
                        if (tourFini) return;

                        CancelIoEx(StdHandle, IntPtr.Zero);

                        Task.Delay(100).ContinueWith(_ =>
                        {
                            PostMessage(ConsoleWindowHnd, WM_KEYDOWN, VK_RETURN, 0); // we need to send a return key to "finish" the readline operation that was started
                        });
                    });

                    try
                    {
                        do
                        {
                            AfficherTour(tour, joueur.Nom);
                            Console.Write("Voici ta main :\n\n   ");
                            joueur.AfficherMain();

                            Console.Write("\n\nQuel mot veux tu jouer ? (en incluant les lettres déjà placées)\nN'écris rien pour remplacer tes jetons actuels.\n  ");
                            mot = Console.ReadLine().ToUpper();

                            if (mot.Length == 0)
                            {
                                Console.Write("Veux-tu vraiment remplacer tous tes jetons et passer ton tour ?\n(écris o/oui ou y/yes pour continuer ou autre chose pour annuler)\n  ");
                                string confirm = Console.ReadLine().Trim().ToUpper();

                                if (confirm == "O" || confirm == "OUI" || confirm == "Y" || confirm == "YES")
                                {
                                    joueur.Remplacer_Jetons(sacJetons, random);
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

                        } while (!plateau.Test_Plateau(mot, ligne - 1, colonne - 1, direction, dictionnaire, joueur));

                        tourFini = true;
                    } catch(OperationCanceledException)
                    {
                        AfficherTour(tour);
                        Console.WriteLine("\nLe temps est écoulé !\nDommage, tu pourras jouer au prochain tour...\n");
                    }


                    joueur.Tours++;

                    if(tourFini && motPlace)
                    {
                        AfficherTour(tour);
                        Console.WriteLine("\nTu as maintenant un score de " + joueur.Score);
                    }

                    while (joueur.Nombre_Jetons() < 7)
                        joueur.Add_Main_Courante(sacJetons.Retire_Jeton(random));

                    plateau.SauvegarderPlateau("InstancePlateau.txt");
                    sacJetons.SauvegarderSacJetons("InstanceJetons.txt");
                    SauvegarderJoueurs("Joueurs.txt");

                    Console.WriteLine("Appuie sur une touche pour continuer...");
                    Console.ReadKey();
                }

                tour++;
            }
        }
    }
}
