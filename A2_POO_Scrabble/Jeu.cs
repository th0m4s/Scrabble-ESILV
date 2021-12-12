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
                int.TryParse(Program.PoserQuestionLarge("Combien de joueurs vont jouer ? (entre 2 et 4)"), out nb);
            }

            joueurs = new List<Joueur>();

            for(int i = 0; i < nb; i++)
            {
                string nom = null;

                do
                {
                    nom = Program.PoserQuestionLarge((nom != null ? "Ce nom est déjà utilisé !\n" : "") + "Entrez le nom du joueur " + (i+1) + " :");

                    if (nom.Trim().Length == 0) nom = null;
                    else if (joueurs.Where(j => j.Nom == nom).Count() > 0) nom = "";
                } while (nom == null || nom.Length == 0);

                Joueur joueur = new Joueur(nom);
                for (int j = 0; j < 7; j++)
                    joueur.Add_Main_Courante(sacJetons.Retire_Jeton(random));

                joueurs.Add(joueur);
            }
        }

        const int interfaceLeftMargin = 43;
        const int interfaceTopMargin = 10;

        /// <summary>
        /// Affiche l'interface de jeu.
        /// </summary>
        /// <param name="joueurEnCours">Le joueur en cours.</param>
        /// <param name="message">Le message a afficher en haut à droite des scores.</param>
        private void AfficherInterface(string joueurEnCours = null, string message = null)
        {
            Console.Clear();

            plateau.Afficher();
            int scoresWidth = AfficherScores(joueurEnCours);

            string[] logo = Program.LOGO;
            int logoStartX = interfaceLeftMargin + (Console.WindowWidth - interfaceLeftMargin - logo[0].Length) / 2 - 1;
            for(int i = 0; i < logo.Length; i++)
            {
                string ligne = logo[i];

                Console.SetCursorPosition(logoStartX, 2 + i);
                Console.Write(ligne);
            }

            if(message != null)
            {
                int messageStartY = interfaceTopMargin;
                int messageStartX = interfaceLeftMargin + scoresWidth + 5;

                int boxWidth = Console.WindowWidth - messageStartX - 4;
                string xBorder = Program.RepeatChar('═', boxWidth - 2);

                Console.SetCursorPosition(messageStartX, messageStartY); // border top
                Console.Write("╔" + xBorder + "╗");


                string[] lines = message.Split('\n');
                string emptyLine = "║" + Program.RepeatChar(' ', boxWidth - 2) + "║";

                for(int i = 0; i < 4; i++)
                {
                    Console.SetCursorPosition(messageStartX, messageStartY + 1 + i); // message line

                    if (i < lines.Length)
                    {
                        string line = lines[i];
                        Console.Write("║ " + line + Program.RepeatChar(' ', boxWidth - line.Length - 3) + "║");
                    }
                    else Console.Write(emptyLine);
                }

                Console.SetCursorPosition(messageStartX, messageStartY + 5); // border bottom
                Console.Write("╚" + xBorder + "╝");

                if (lines.Length == 0)
                    Console.SetCursorPosition(messageStartX + 2, messageStartY + 1);
                else Console.SetCursorPosition(messageStartX + 2 + lines[lines.Length - 1].Length, messageStartY + lines.Length);
            }
        }

        private string PoserQuestion(string question)
        {
            int questionStartY = interfaceTopMargin + 5 + joueurs.Count;
            int boxWidth = Console.WindowWidth - interfaceLeftMargin - 4;

            string xBorder = Program.RepeatChar('═', boxWidth - 2);

            Console.SetCursorPosition(interfaceLeftMargin, questionStartY); // border top
            Console.Write("╔" + xBorder + "╗");

            string[] lines = question.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                Console.SetCursorPosition(interfaceLeftMargin, questionStartY + 1 + i); // question line

                string line = lines[i];
                Console.Write("║ " + line + Program.RepeatChar(' ', boxWidth - line.Length - 3) + "║");
            }

            string emptyLine = Program.RepeatChar(' ', boxWidth - 2);
            Console.SetCursorPosition(interfaceLeftMargin, questionStartY + 1 + lines.Length); // response line
            Console.Write("║" + emptyLine + "║");

            Console.SetCursorPosition(interfaceLeftMargin, questionStartY + 2 + lines.Length); // border bottom
            Console.Write("╚" + xBorder + "╝");

            // clear remaining lines from old question
            for (int i = 3 + lines.Length; i < 7; i++)
            {
                Console.SetCursorPosition(interfaceLeftMargin, questionStartY + i);
                Console.Write("  " + emptyLine);
            }

            Console.SetCursorPosition(interfaceLeftMargin + 2, questionStartY + 1 + lines.Length);
            return Console.ReadLine().Trim().ToUpper();
        }

        private int AfficherScores(string joueurEnCours)
        {
            int boxWidth = Math.Max(joueurs.Select(x => x.Nom.Length).Max() + joueurs.Select(x => x.Score.ToString().Length).Max() + 7, 12);
            string boxSeparatorX = Program.RepeatChar('═', boxWidth);

            int scoreStartY = interfaceTopMargin;

            Console.SetCursorPosition(interfaceLeftMargin, scoreStartY);
            Console.Write("╔" + boxSeparatorX + "╗");
            Console.SetCursorPosition(interfaceLeftMargin, scoreStartY + 1);
            Console.Write("║ Scores :" + Program.RepeatChar(' ', boxWidth - 10) + " ║");
            Console.SetCursorPosition(interfaceLeftMargin, scoreStartY + 2);
            Console.Write("╠" + boxSeparatorX + "╣");

            for (int i = 0; i < joueurs.Count; i++)
            {
                Console.SetCursorPosition(interfaceLeftMargin, scoreStartY + 3 + i);

                Joueur j = joueurs[i];
                bool enCours = j.Nom == joueurEnCours;

                Console.Write("║ " + (enCours ? "→ " : "") + j.Nom + " : " + j.Score + Program.RepeatChar(' ', boxWidth - j.Nom.Length - (enCours ? 7 : 5) - j.Score.ToString().Length) + " ║");
            }

            Console.SetCursorPosition(interfaceLeftMargin, scoreStartY + 3 + joueurs.Count);
            Console.Write("╚" + boxSeparatorX + "╝");

            return boxWidth;
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

                    AfficherInterface(joueur.Nom, "C'est à " + joueur.Nom +" de jouer !\nAppuie sur une touche pour voir ta main...");
                    Console.ReadKey();

                    bool motPlace = true;
                    bool tourFini = false;

                    Task task = Task.Delay(90_000).ContinueWith(_ => {
                        if (tourFini) return;

                        CancelIoEx(StdHandle, IntPtr.Zero);

                        Task.Delay(0).ContinueWith(_ =>
                        {
                            PostMessage(ConsoleWindowHnd, WM_KEYDOWN, VK_RETURN, 0); // we need to send a return key to "finish" the readline operation that was started
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
                            if (mot.Length == 0) continue;

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
                    } catch(OperationCanceledException)
                    {
                        AfficherInterface(joueur.Nom, "Dommage, le temps est écoulé !\nTu pourras jouer au prochain tour...\n\n");
                    }


                    joueur.Tours++;

                    if(tourFini && motPlace)
                    {
                        AfficherInterface(joueur.Nom, "Tu as maintenant un score de " + joueur.Score + " !\n");
                    }

                    while (joueur.Nombre_Jetons() < 7)
                        joueur.Add_Main_Courante(sacJetons.Retire_Jeton(random));

                    plateau.SauvegarderPlateau("InstancePlateau.txt");
                    sacJetons.SauvegarderSacJetons("InstanceJetons.txt");
                    SauvegarderJoueurs("Joueurs.txt");

                    Console.Write("Appuie sur une touche pour continuer...");
                    Console.ReadKey();
                }

                tour++;
            }
        }
    }
}
