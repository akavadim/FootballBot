using System;
using System.Collections.Generic;
using System.Text;

namespace FootballBot
{
    class Match
    {
        public Team Left;
        public Team Right;
        public int ScoreLeft;
        public int ScoreRight;

        public Match(Team left, Team right, int scoreLeft, int scoreRight)
        {
            Left = left;
            Right = right;
            ScoreLeft = scoreLeft;
            ScoreRight = scoreRight;

            Relation leftRelation = Left.Relations.Find(r => r.Team == Right);
            Relation rightRelation = Right.Relations.Find(r => r.Team == Left);

            if(leftRelation==null)
            {
                leftRelation = new Relation(Right);
                Left.Relations.Add(leftRelation);
            }
            if (rightRelation == null)
            {
                rightRelation = new Relation(Left);
                Right.Relations.Add(rightRelation);
            }

            if(scoreLeft>scoreRight)
            {
                Left.Wins++;
                Right.Loses++;
                leftRelation.Wins++;
                rightRelation.Loses++;
            }
            if (scoreLeft < scoreRight)
            {
                Right.Wins++;
                Left.Loses++;
                leftRelation.Loses++;
                rightRelation.Wins++;
            }
        }
    }
}
