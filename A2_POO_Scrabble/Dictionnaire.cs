using System.Collections.Generic;
using System.Linq;

namespace A2_POO_Scrabble
{
    public class Dictionnaire
    {
        #region Attributs
        Dictionary<int, List<string>> mots = new Dictionary<int, List<string>>();
        string langue;
        #endregion


        #region Propriétés
        public string Langue => langue;
        #endregion


        #region Constructeurs
        /// <summary>
        /// Créé un dictionnaire à partir de sa langue et d'un tableau de lignes provenant d'un fichier de sauvegarde.
        /// On peut par exemple utiliser <i>File.ReadAllLines("Francais.txt")</i>.
        /// </summary>
        /// <param name="langue">La langue du dictionnaire.</param>
        /// <param name="lignes">Les lignes contenant les tailles et les mots.</param>
        public Dictionnaire(string langue, string[] lignes)
        {
            this.langue = langue;
            for(int i = 0; i < lignes.Length; i += 2)
                mots.Add(int.Parse(lignes[i]), lignes[i + 1].Split(" ").ToList());
        }
        #endregion


        #region Méthodes
        /// <summary>
        /// Teste l'existence d'un mot dans le dictionnaire.
        /// </summary>
        /// <param name="mot">Le mot à rechercher.</param>
        /// <returns>Un bool représentant la présence du mot dans le dictionnaire.</returns>
        public bool RechercheDichoRecursif(string mot)
        {
            return mots.ContainsKey(mot.Length) && RechercheDichoRecursif(0, mots[mot.Length].Count - 1, mots[mot.Length], mot);
        }

        /// <summary>
        /// Recherche dichotomique d'un mot dans une liste triée de string.
        /// </summary>
        /// <param name="debut">L'indice à partir duquel commencer la recherche.</param>
        /// <param name="fin">L'indice jusqu'auquel effectuer la recherche.</param>
        /// <param name="liste">La liste de mot dans laquelle chercher le mot en particulier.</param>
        /// <param name="element">Le mot à rechercher dans la liste.</param>
        /// <returns>Un bool représentant la présence de l'élément entre les indices debut et fin de la liste.</returns>
        bool RechercheDichoRecursif(int debut, int fin, List<string> liste, string element)
        {
            if (liste.Count == 0 || debut == fin || debut + 1 == fin) return false;

            int pos = (debut + fin) / 2;

            string m = liste[pos];
            if (m == element || liste[pos+1] == element) return true; 				 	 	    					 

            if (element.CompareTo(m) > 0) return RechercheDichoRecursif(pos, fin, liste, element);
            else return RechercheDichoRecursif(0, pos, liste, element);
        }
        #endregion
    }
}
