using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;
using System.IO;
using System.Net;
using System.Linq;

namespace FootballBot.Parser
{
    class Parser
    {
        class ScoresException : ApplicationException { }

        const string site = "https://www.sport-express.ru";
        const string matchesUrl1 = "/live/";
        const string matchesUrl2 = "/football/";
        DateTime startData = new DateTime(2018, 1, 1);
        public HtmlDocument LoadPage(string url)
        {
            var result = "";
            var request = (HttpWebRequest)WebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var recieveStream = response.GetResponseStream();
                if (recieveStream != null)
                {
                    StreamReader streamReader;
                    if (response.CharacterSet == null)
                        streamReader = new StreamReader(recieveStream);
                    else
                    {
                        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                        streamReader = new StreamReader(recieveStream, Encoding.GetEncoding(response.CharacterSet));
                    }
                    result = streamReader.ReadToEnd();
                    streamReader.Close();
                }
                response.Close();
            }

            var document = new HtmlDocument();
            document.LoadHtml(result);

            return document;
        }

        private HtmlNode[] GetMatches(HtmlDocument page)
        {
            var nodes = page.DocumentNode.SelectNodes("//tr[@class]");
            return nodes.ToArray();
        }

        private (string left, string right) GetNameTeams(HtmlNode matchNode)
        {
            var rightName = matchNode.SelectSingleNode("./td[@class='t_right w_240 ph_10']");
            var leftName = matchNode.SelectSingleNode("./td[@class='t_left ph_10']");

            string left = GetName(rightName);
            string right = GetName(leftName);

            return (left, right);
        }

        private string GetName(HtmlNode nameNode)
        {
            HtmlNode name = nameNode.SelectSingleNode(".//b[@class]");
            if (name == null)
                name = nameNode.SelectSingleNode(".//span[@id]");
            return name.InnerText;
        }

        private (int left, int right) GetScores(HtmlNode matchNode)
        {
            var scoreNode = matchNode.SelectSingleNode("./td[@class='fs_22 ph_10 w_1p nowrap']").ChildNodes.FindFirst("span");
            int scoreLeft;
            int scoreRight;
            try
            {
                string[] score = scoreNode.InnerText.Split(":");
                scoreLeft = int.Parse(score[0]);
                scoreRight = int.Parse(new string(new char[] { score[1][0], score[1][1], score[1][2] }));
            }
            catch
            {
                throw new ScoresException();
            }
            return (scoreLeft, scoreRight);
        }

        private HtmlDocument NextPage(HtmlDocument page)
        {
            HtmlDocument nextPage = new HtmlDocument();
            startData = startData.AddDays(1);
            if (startData == DateTime.Today.AddDays(1))
                return null;
            string urlNP = site + matchesUrl1 + String.Format("{0:d2}-{1:d2}-{2}",startData.Day, startData.Month, startData.Year) + matchesUrl2;
            nextPage = LoadPage(urlNP);
            return nextPage;
        }

        public (Match[], Team[]) Start()
        {
            string url= site + matchesUrl1 + String.Format("{0:d2}-{1:d2}-{2}", startData.Day, startData.Month, startData.Year) + matchesUrl2;
            HtmlDocument page = LoadPage(url);
            List<Match> matches = new List<Match>();
            List<Team> teams = new List<Team>();
            do
            {
                var matchNodes = GetMatches(page);
                foreach( HtmlNode matchNode in matchNodes)
                {
                    var names = GetNameTeams(matchNode);
                    var scores = GetScores(matchNode);
                    Team left = teams.Find(t => t.Name == names.left);
                    Team right = teams.Find(t => t.Name == names.right);
                    if(left==null)
                    {
                        left = new Team(names.left);
                        teams.Add(left);
                    }
                    if(right==null)
                    {
                        right = new Team(names.right);
                        teams.Add(right);
                    }
                    Match match = new Match(left, right, scores.left, scores.right);
                    matches.Add(match);
                }
            } while ((page = NextPage(page)) != null);
            return (matches.ToArray(), teams.ToArray());
        }

        public (Match[], Team[]) StartTest()
        {
            string url = site + matchesUrl1 + String.Format("{0:d2}-{1:d2}-{2}", startData.Day, startData.Month, startData.Year) + matchesUrl2;
            HtmlDocument page = LoadPage(url);

            List<Match> matches = new List<Match>();
            List<Team> teams = new List<Team>();
                var matchNodes = GetMatches(page);
                foreach (HtmlNode matchNode in matchNodes)
                {
                    var names = GetNameTeams(matchNode);
                    var scores = GetScores(matchNode);
                    Team left = teams.Find(t => t.Name == names.left);
                    Team right = teams.Find(t => t.Name == names.right);
                    if (left == null)
                    {
                        left = new Team(names.left);
                        teams.Add(left);
                    }
                    if (right == null)
                    {
                        right = new Team(names.right);
                        teams.Add(right);
                    }
                    Match match = new Match(left, right, scores.left, scores.right);
                    matches.Add(match);
                }
            return (matches.ToArray(), teams.ToArray());
        }
    }
}
