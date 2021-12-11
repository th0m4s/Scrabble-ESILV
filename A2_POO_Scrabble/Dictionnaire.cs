using System.Collections.Generic;
using System.Linq;

namespace A2_POO_Scrabble
{
    class Dictionnaire
    {
        Dictionary<int, List<string>> mots = new Dictionary<int, List<string>>();
        string langue;

        public string Langue => langue;

        public Dictionnaire(string langue, string[] lignes)
        {
            this.langue = langue;
            for(int i = 0; i < lignes.Length; i += 2)
                mots.Add(int.Parse(lignes[i]), lignes[i + 1].Split(" ").ToList());
        }

        public bool RechercheDichoRecursif(string mot)
        {
            return mots.ContainsKey(mot.Length) && RechercheDichoRecursif(0, mots[mot.Length].Count - 1, mots[mot.Length], mot);
        }

        bool RechercheDichoRecursif(int debut, int fin, List<string> liste, string element)
        {
            if (liste.Count == 0 || debut == fin || debut + 1 == fin) return false;

            int pos = (debut + fin) / 2;

            string m = liste[pos];
            if (m == element || liste[pos+1] == element) return true; 				 	 	    					 

            if (element.CompareTo(m) > 0) return RechercheDichoRecursif(pos, fin, liste, element);
            else return RechercheDichoRecursif(0, pos, liste, element);
        }
    }
}
