using System;
using System.Linq;

namespace A2_POO_Scrabble
{
    class Utils
    {
        /// <summary>
        /// Repète un caractères plusieurs fois dans un string.
        /// </summary>
        /// <param name="c">Le caractère à répéter.</param>
        /// <param name="count">Le nombre de fois qu'il faut répéter le caractère.</param>
        /// <returns>Le string contenant le caractère répété.</returns>
        internal static string RepeatChar(char c, int count)
        {
            return string.Join("", Enumerable.Repeat(0, Math.Max(0, count)).Select(x => c));
        }

        /// <summary>
        /// Ecrit du texte à une position donnée de l'écran.
        /// </summary>
        /// <param name="left">La marge à gauche du texte.</param>
        /// <param name="top">La marge en haut du texte.</param>
        /// <param name="message">Le message à afficher.</param>
        internal static void WriteAtPosition(int left, int top, string message)
        {
            string[] lignes = message.Split('\n');
            WriteAtPosition(left, top, lignes);
        }

        /// <summary>
        /// Ecrit des lignes à une position donnée de l'écran. Chaque ligne aura la même marge à gauche.
        /// </summary>
        /// <param name="left">La marge à gauche des lignes.</param>
        /// <param name="top">La marge au-dessus de la 1re ligne de texte.</param>
        /// <param name="lignes">Les lignes à afficher.</param>
        internal static void WriteAtPosition(int left, int top, string[] lignes)
        {
            for (int i = 0; i < lignes.Length; i++)
            {
                Console.SetCursorPosition(left, top + i);
                Console.Write(lignes[i]);
            }
        }

        /// <summary>
        /// Pose une question à l'utilisateur.
        /// </summary>
        /// <param name="question">La question à poser.</param>
        /// <param name="below">Un texte à afficher sous la zone de réponse.</param>
        /// <returns>La réponse de l'utilisateur.</returns>
        internal static string PoserQuestionLarge(string question, string below = "")
        {
            Console.Clear();

            int questionStartY = 1;
            int questionStartX = 2;

            int boxWidth = Console.WindowWidth - 2 * questionStartX;
            string xBorder = RepeatChar('═', boxWidth - 2);
            string[] lignes = question.Split('\n');
            string emptyLine = "║" + Utils.RepeatChar(' ', boxWidth - 2) + "║";

            WriteAtPosition(questionStartX, questionStartY, "╔" + xBorder + "╗\n" // border top
                + string.Join("\n", lignes.Select(ligne => "║ " + ligne + Utils.RepeatChar(' ', boxWidth - 3 - ligne.Length) + "║")) // question lines
                + "\n" + emptyLine); // empty line for answer

            string belowAndBorder = "╚" + xBorder + "╝"; // border bottom
            if(below != null && below.Trim().Length > 0)
                belowAndBorder = emptyLine + "\n" + string.Join("\n", below.Split('\n').Select(ligne => "║ " + ligne + RepeatChar(' ', boxWidth - 3 - ligne.Length) + "║")) + "\n" + belowAndBorder;
            // margin line + below lines

            WriteAtPosition(questionStartX, questionStartY + lignes.Length + 2, belowAndBorder);

            Console.SetCursorPosition(questionStartX + 2, questionStartY + lignes.Length + 1);
            return Console.ReadLine();
        }
    }
}
