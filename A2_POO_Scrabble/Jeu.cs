using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
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
        #region Constantes
        const int VK_RETURN = 0x0D;
        const int WM_KEYDOWN = 0x100;

        const int STD_INPUT_HANDLE = -10;

        const int interfaceLeftMargin = 43;
        const int interfaceTopMargin = 10;
        #endregion

        #region Références externes
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
        #endregion


        #region Attributs
        Dictionnaire dictionnaire;
        Plateau plateau;
        Sac_Jetons sacJetons;
        List<Joueur> joueurs;

        Random random;

        int tour = 1;
        #endregion


        #region Constructeurs
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
        #endregion


        #region Méthodes publiques
        /// <summary>
        /// Permet de remplir la liste des joueurs si une nouvelle partie est commencée.
        /// </summary>
        public void DemanderJoueurs()
        {
            int nb = -1;
            while(nb < 2 || nb > 4)
                int.TryParse(Utils.PoserQuestionLarge("Combien de joueurs vont jouer ? (entre 2 et 4)"), out nb);

            joueurs = new List<Joueur>();

            for(int i = 0; i < nb; i++)
            {
                string nom = null;

                do
                {
                    nom = Utils.PoserQuestionLarge((nom != null ? "Ce nom est déjà utilisé !\n" : "") + "Entrez le nom du joueur " + (i+1) + " :").Trim();

                    if (nom.Length == 0) nom = null;
                    else if (nom.Length > 14 || joueurs.Where(j => j.Nom == nom).Count() > 0) nom = "";
                } while (nom == null || nom.Length == 0);

                Joueur joueur = new Joueur(nom);
                for (int j = 0; j < 7; j++)
                    joueur.Add_Main_Courante(sacJetons.Retire_Jeton(random));

                joueurs.Add(joueur);
            }
        }

        /// <summary>
        /// Sauvegarde la liste des joueurs (dont leur score, mots et jetons) dans un fichier.
        /// </summary>
        /// <param name="fichier">Le nom du fichier à sauvegarder.</param>
        public void SauvegarderJoueurs(string fichier)
        {
            List<string> lignes = new List<string>();
            foreach (Joueur joueur in joueurs)
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

            // si on reprend une partie sauvegardée, on va devoir "passer" des tours
            int nbToursPremierJoueur = joueurs[0].Tours; // le 1er joueur est toujours celui qui a le plus joué
            int aPasser = (nbToursPremierJoueur - 1) * joueurs.Count + joueurs.TakeWhile(x => x.Tours == nbToursPremierJoueur).Count();
            //                  nombre de tours complets             +      le nombre de joueurs qui n'ont pas fini le dernier tour

            tour = aPasser / joueurs.Count;
            aPasser %= joueurs.Count;

            while (sacJetons.NombreJetons() > 0)
            {
                foreach (Joueur joueur in joueurs)
                {
                    if (aPasser > 0)
                    {
                        aPasser--;
                        continue;
                    }

                    string mot;
                    int ligne = -1, colonne = -1;
                    char direction = '\0';

                    AfficherInterface(joueur.Nom, "C'est à " + joueur.Nom + " de jouer !\nAppuie sur une touche pour voir ta main...");
                    Console.ReadKey();

                    bool motPlace = true;
                    bool tourFini = false;

                    Task task = Task.Delay(90_000).ContinueWith(_ => {
                        if (tourFini) return;

                        CancelIoEx(StdHandle, IntPtr.Zero);

                        Task.Delay(0).ContinueWith(_ =>
                        {
                            PostMessage(ConsoleWindowHnd, WM_KEYDOWN, VK_RETURN, 0);
                            // we need to send a return key to "finish" the readline operation that was started
                        });
                    });

                    try
                    {
                        do
                        {
                            AfficherInterface(joueur.Nom, "Voici ta main :\n\n   ");
                            joueur.AfficherMain();

                            mot = PoserQuestion("Quel mot veux tu jouer ? (en incluant les lettres déjà placées)\nN'écris rien pour remplacer tes jetons actuels.");

                            if (mot.Length == 0)
                            {
                                string confirm = PoserQuestion("Veux-tu vraiment remplacer tous tes jetons et passer ton tour ?\n(écris o/oui ou y/yes pour continuer ou autre chose pour annuler)");

                                if (confirm == "O" || confirm == "OUI" || confirm == "Y" || confirm == "YES")
                                {
                                    joueur.Remplacer_Jetons(sacJetons, random);
                                    AfficherInterface(joueur.Nom, "Tes jetons ont été remplacés par :\n   ");
                                    int xBeforeMain = Console.CursorLeft;
                                    joueur.AfficherMain();

                                    motPlace = false;

                                    // a cause des jetons le curseur est mal placé
                                    Console.SetCursorPosition(xBeforeMain - 3, interfaceTopMargin + 4);

                                    break;
                                }
                                else continue;
                            }

                            mot = Regex.Replace(mot, "[^A-Z]", "");
                            if (mot.Length == 0 ) continue;

                            // On fait une première vérification dans le dico (comme dans Test_Plateau)
                            // pour prévenir le joueur au plus vite d'un mot non existant
                            if (!dictionnaire.RechercheDichoRecursif(mot)) continue;

                            string pos = PoserQuestion("A quelle position veux-tu jouer ce mot ?\n(écrire la position de la 1re case - ie. en haut ou à gauche -\nligne puis colonne séparées par un espace)");
                            string[] posparts = pos.Split(',', ';', ' ', '/');

                            if (posparts.Length != 2) continue;
                            if (!int.TryParse(posparts[0], out ligne) || !int.TryParse(posparts[1], out colonne)) continue;

                            string dir = PoserQuestion("Dans quelle direction veux-tu placer ce mot (l/ligne ou c/colonne)");
                            if (dir == "L" || dir == "LIGNE") direction = 'L';
                            else if (dir == "C" || dir == "COLONNE") direction = 'C';
                            else continue;

                        } while (!plateau.Test_Plateau(mot, ligne - 1, colonne - 1, direction, dictionnaire, joueur));

                        tourFini = true;
                    }
                    catch (OperationCanceledException)
                    {
                        AfficherInterface(joueur.Nom, "Dommage, le temps est écoulé !\nTu pourras jouer au prochain tour...\n\n");
                    }


                    joueur.Tours++;

                    if (tourFini && motPlace)
                        AfficherInterface(joueur.Nom, "Tu as maintenant un score de " + joueur.Score + " !\n");

                    while (joueur.Nombre_Jetons() < 7 && sacJetons.NombreJetons() > 0)
                        joueur.Add_Main_Courante(sacJetons.Retire_Jeton(random));

                    // On sauvegarder à chaque fois qu'un joueur a joué pour reprendre la partie au plus proche de son arrêt
                    plateau.SauvegarderPlateau("InstancePlateau.txt");
                    sacJetons.SauvegarderSacJetons("InstanceJetons.txt");
                    SauvegarderJoueurs("Joueurs.txt");

                    Console.Write("Appuie sur une touche pour continuer...");
                    Console.ReadKey();
                }

                tour++;
            }

            AfficherInterface(null, "La partie est terminée !\nBravo à " + joueurs.OrderByDescending(x => x.Score).First().Nom + " qui finit premier !"
                + "\n\nAppuyez sur une touche pour quitter...");
            Console.ReadKey();
        }
        #endregion


        #region Méthodes privées
        /// <summary>
        /// Affiche l'interface de jeu.
        /// </summary>
        /// <param name="joueurEnCours">Le joueur en cours.</param>
        /// <param name="message">Le message a afficher en haut à droite des scores.</param>
        private void AfficherInterface(string joueurEnCours = null, string message = null)
        {
            Console.Clear();
            plateau.Afficher();

            if(tour > 0)
            {
                int tourLen = tour >= 10 ? 2 : 1;
                int tourStartX = 30 - tourLen;

                Utils.WriteAtPosition(tourStartX, 22, "╔" + Utils.RepeatChar('═', 7 + tourLen) + "╣" // border top
                    + "\n║ tour " + tour + "\n╩"); // tour number
            }

            int scoresWidth = AfficherScores(joueurEnCours);

            string[] logo = Program.LOGO;
            int logoStartX = interfaceLeftMargin + (Console.WindowWidth - interfaceLeftMargin - logo[0].Length) / 2 - 1;
            Utils.WriteAtPosition(logoStartX, 2, logo);

            if(message != null)
            {
                int messageStartY = interfaceTopMargin;
                int messageStartX = interfaceLeftMargin + scoresWidth + 5;

                int boxWidth = Console.WindowWidth - messageStartX - 4;
                string xBorder = Utils.RepeatChar('═', boxWidth - 2);

                string[] lines = message.Split('\n');
                string[] allLines = lines.ToArray(); // create a copy

                // add lines because zone should always have 4 lines
                if (allLines.Length < 4) allLines = lines.Concat(Enumerable.Repeat(0, 4 - lines.Length).Select(x => "")).ToArray();

                string emptyLine = "║" + Utils.RepeatChar(' ', boxWidth - 2) + "║";

                Utils.WriteAtPosition(messageStartX, messageStartY, "╔" + xBorder + "╗\n" // border top
                    + string.Join("\n", allLines.Select(line => "║ " + line + Utils.RepeatChar(' ', boxWidth - line.Length - 3) + "║")) // message lines
                    + "\n╚" + xBorder + "╝"); // border bottom

                // set cursor at the end of the message
                if (lines.Length == 0)
                    Console.SetCursorPosition(messageStartX + 2, messageStartY + 1);
                else Console.SetCursorPosition(messageStartX + 2 + lines[lines.Length - 1].Length, messageStartY + lines.Length);
            }
        }

        /// <summary>
        /// Permet de poser une question dans la zone de l'interface prévue à cet effet.
        /// </summary>
        /// <param name="question">La question à poser.</param>
        /// <returns>La réponse de l'utilisateur.</returns>
        private string PoserQuestion(string question)
        {
            int questionStartY = interfaceTopMargin + 5 + joueurs.Count;
            int boxWidth = Console.WindowWidth - interfaceLeftMargin - 4;

            string xBorder = Utils.RepeatChar('═', boxWidth - 2);
            string[] lines = question.Split('\n');
            string emptyLine = Utils.RepeatChar(' ', boxWidth - 2);

            Utils.WriteAtPosition(interfaceLeftMargin, questionStartY, "╔" + xBorder + "╗\n" // border top
                + string.Join("\n", lines.Select(line => "║ " + line + Utils.RepeatChar(' ', boxWidth - line.Length - 3) + "║")) // question lines
                + "\n║" + emptyLine + "║" // empty line for answer
                + "\n╚" + xBorder + "╝\n   " + emptyLine + "\n   " + emptyLine + "\n   " + emptyLine); // border bottom + empty lines to clear the screen

            Console.SetCursorPosition(interfaceLeftMargin + 2, questionStartY + 1 + lines.Length);
            return Console.ReadLine().Trim().ToUpper();
        }

        /// <summary>
        /// Affiche les scores à côté du plateau.
        /// </summary>
        /// <param name="joueurEnCours">Le nom du joueur en cours pour afficher → devant celui-ci.</param>
        /// <returns>La largeur de la zone des scores pour afficher d'autres zones à droite de celle-ci (comme un message).</returns>
        private int AfficherScores(string joueurEnCours)
        {
            int boxWidth = Math.Max(joueurs.Select(x => x.Nom.Length).Max() + joueurs.Select(x => x.Score.ToString().Length).Max() + 7, 12);
            string boxSeparatorX = Utils.RepeatChar('═', boxWidth);

            Utils.WriteAtPosition(interfaceLeftMargin, interfaceTopMargin, "╔" + boxSeparatorX + "╗" // border top
                + "\n║ Scores: " + Utils.RepeatChar(' ', boxWidth - 10) + " ║" // score title
                + "\n╠" + boxSeparatorX + "╣\n" // separator line
                + string.Join("\n", joueurs.Select(j => "║ " + (j.Nom == joueurEnCours ? "→ " : "") + j.Nom + " : " + j.Score + Utils.RepeatChar(' ', boxWidth - j.Nom.Length - (j.Nom == joueurEnCours ? 7 : 5) - j.Score.ToString().Length) + " ║"))
                + "\n╚" + boxSeparatorX + "╝"); // border bottom

            return boxWidth;
        }
        #endregion
    }
}
