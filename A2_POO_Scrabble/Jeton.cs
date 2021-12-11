using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2_POO_Scrabble
{
    class Jeton
    {
        char lettre;
        int score;

        public char Lettre => lettre;
        public int Score => score;

        public Jeton(char lettre, int score)
        {
            this.lettre = lettre;
            this.score = score;
        }

        public Jeton(char lettre) : this(lettre, 0)
        {
            this.score = Sac_Jetons.Score_Pour_Lettre(lettre);
        }

        public override string ToString()
        {
            return "Jeton '" + lettre + "' (" + score + " point" + (score == 1 ? "" : "s") + ")";
        }

        public bool Equals(Jeton other)
        {
            return this == other;
        }

        public static bool operator ==(Jeton a, char b)
        {
            return !ReferenceEquals(a, null) && a.Lettre == b;
        }

        public static bool operator !=(Jeton a, char b)
        {
            return !(a == b);
        }

        public static bool operator ==(Jeton a, Jeton b)
        {
            return !ReferenceEquals(b, null) && a == b.Lettre;
        }

        public static bool operator !=(Jeton a, Jeton b)
        {
            return !(a == b);
        }
    }
}
