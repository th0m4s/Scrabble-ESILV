namespace A2_POO_Scrabble
{
    public class Jeton
    {
        char lettre;
        int score;

        public char Lettre => lettre;
        public int Score => score;

        /// <summary>
        /// Créé un jeton à partir de la lettre qu'il représente ainsi que son score.
        /// </summary>
        /// <param name="lettre">La lettre du jeton.</param>
        /// <param name="score">Le score attribué si ce jeton est joué.</param>
        public Jeton(char lettre, int score)
        {
            this.lettre = lettre;
            this.score = score;
        }

        /// <summary>
        /// Créé un jeton à partir de la lettre qu'il représente.
        /// Le score est automatiquement récupéré à partir des autres jetons chargés dans le sac de jetons.
        /// </summary>
        /// <param name="lettre">La lettre du jeton.</param>
        public Jeton(char lettre) : this(lettre, 0)
        {
            this.score = Sac_Jetons.Score_Pour_Lettre(lettre);
        }

        public override string ToString()
        {
            return "Jeton '" + lettre + "' (" + score + " point" + (score <= 1 ? "" : "s") + ")";
        }

        /// <summary>
        /// Retourne une valeur représentant un jeton de façon unique.
        /// </summary>
        /// <returns>L'entier représentant la lettre et le score du jeton.</returns>
        public override int GetHashCode()
        {
            return lettre << 16 + score;
        }

        public override bool Equals(object obj)
        {
            return (obj is Jeton && (Jeton)obj == this) || (obj is char && this == (char)obj);
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
