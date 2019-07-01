using System;
using System.Collections.Generic;
using System.Text;

namespace FootballBot
{
    class Team
    {
        public string Name;
        public List<Relation> Relations = new List<Relation>();
        public int AllGames { get { return Wins + Loses; } }
        public double Winrate { get { return (AllGames == 0) ? 0 : (double)Wins / AllGames; } }

        public int Wins = 0;
        public int Loses = 0;
        public Team(string Name) => this.Name = Name;
    }
}
