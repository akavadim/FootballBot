using System;
using System.Collections.Generic;
using System.Text;

namespace FootballBot
{
    class Relation
    {
        public Team Team;

        public int Wins = 0;
        public int Loses = 0;

        public int AllGames { get { return Wins + Loses; } }
        public double Winrate { get { return (AllGames == 0) ? 0 : (double)Wins / AllGames; } }

        public Relation(Team team)
        {
            Team = team;
        }
    }
}
