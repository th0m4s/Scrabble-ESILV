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

        public override int GetHashCode()
        {
            return lettre << 4 + score;
        }

        public override bool Equals(object obj)
        {
            return obj is Jeton && (Jeton)obj == this;
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
            return (ReferenceEquals(a, null) && ReferenceEquals(b, null)) || (!ReferenceEquals(b, null) && a == b.Lettre);
        }

        public static bool operator !=(Jeton a, Jeton b)
        {
            return !(a == b);
        }
    }
}
