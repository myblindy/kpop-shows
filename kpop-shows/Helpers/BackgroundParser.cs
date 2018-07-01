using HtmlAgilityPack;
using kpop_shows.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace kpop_shows.Helpers
{
    public class MusicShowStage
    {
        public string GroupName { get; set; }
        public string SongName { get; set; }
        public ObservableConcurrentBag<string> Links { get; set; }

        public MusicShowStage(Dispatcher dispatcher) =>
            Links = new ObservableConcurrentBag<string>(dispatcher);
    }

    public class MusicShowInstanceStageType
    {
        public string StageTypeName { get; set; }
        public ObservableConcurrentBag<string> Columns { get; set; }
        public ObservableConcurrentBag<MusicShowStage> Stages { get; set; }

        public MusicShowInstanceStageType(Dispatcher dispatcher)
        {
            Columns = new ObservableConcurrentBag<string>(dispatcher);
            Stages = new ObservableConcurrentBag<MusicShowStage>(dispatcher);
        }
    }

    public class MusicShowInstance
    {
        public DateTime Date { get; set; }
        public string URL { get; set; }
        public MusicShow MusicShow { get; set; }
        public ObservableConcurrentBag<MusicShowInstanceStageType> StageTypes { get; set; }

        public MusicShowInstance(Dispatcher dispatcher) =>
            StageTypes = new ObservableConcurrentBag<MusicShowInstanceStageType>(dispatcher);
    }

    public class MusicShow
    {
        public string URL { get; set; }
        public Show Show { get; set; }
        public ObservableConcurrentBag<MusicShowInstance> Instances { get; set; }

        public MusicShow(Dispatcher dispatcher) =>
            Instances = new ObservableConcurrentBag<MusicShowInstance>(dispatcher);
    }

    public class BackgroundParser
    {
        public ObservableConcurrentBag<MusicShow> MusicShows { get; set; }
        public ObservableConcurrentBag<MusicShowInstance> MusicShowInstances { get; set; }

        public void Start(Dispatcher dispatcher)
        {
            MusicShowInstances = new ObservableConcurrentBag<MusicShowInstance>(dispatcher);
            MusicShows = new ObservableConcurrentBag<MusicShow>(dispatcher)
            {
                new MusicShow(dispatcher) { URL = "https://www.reddit.com/r/kpop/wiki/music-shows/the-show", Show = Show.TheShow },
                new MusicShow(dispatcher) { URL = "https://www.reddit.com/r/kpop/wiki/music-shows/show-champion", Show = Show.ShowChampion }
            };

            foreach (var show in MusicShows)
                Task.Factory.StartNew(() => ParseMusicShowPage(show, dispatcher));
        }

        private void ParseMusicShowPage(MusicShow show, Dispatcher dispatcher)
        {
            var page = new HtmlWeb().Load(show.URL);
            for (var node = page.DocumentNode.SelectSingleNode("//h2[@id='wiki_archive']/following-sibling::p");
                int.TryParse(node.InnerText, out var tmpint); node = node.SelectSingleNode("./following-sibling::p"))
            {
                foreach (var stageelem in node.SelectNodes(@"./following-sibling::table[1]/tbody/tr/td[1]/a"))
                {
                    var showinstance = new MusicShowInstance(dispatcher)
                    {
                        Date = DateTime.ParseExact(stageelem.InnerText, "yyyyMMdd", CultureInfo.InvariantCulture),
                        URL = "https://www.reddit.com" + stageelem.GetAttributeValue("href", null),
                        MusicShow = show,
                    };
                    show.Instances.Add(showinstance);
                    MusicShowInstances.Add(showinstance);
                    Task.Factory.StartNew(() => ParseMusicStagePage(showinstance, dispatcher));
                }
            }
        }

        private void ParseMusicStagePage(MusicShowInstance showinstance, Dispatcher dispatcher)
        {
            var page = new HtmlWeb().Load(showinstance.URL);
            for (var stagetypenode = page.DocumentNode.SelectSingleNode("//h2[@id='wiki_lineup']/following-sibling::h3[not(preceding-sibling::h2[@id='wiki_winner'])]"); stagetypenode != null;
                stagetypenode = stagetypenode.SelectSingleNode("./following-sibling::h3[not(preceding-sibling::h2[@id='wiki_winner'])]"))
            {
                var stagetype = new MusicShowInstanceStageType(dispatcher) { StageTypeName = stagetypenode.InnerText };
                stagetype.Columns.AddRange(stagetypenode.SelectNodes(@"./following-sibling::table[1]/thead/tr/th").Skip(2).Select(e => e.InnerText));

                foreach (var stagetypeelem in stagetypenode.SelectNodes(@"./following-sibling::table[1]/tbody/tr"))
                {
                    var stage = new MusicShowStage(dispatcher)
                    {
                        GroupName = stagetypeelem.Descendants("td").First().InnerText,
                        SongName = stagetypeelem.Descendants("td").ElementAt(1).InnerText,
                    };
                    stage.Links.AddRange(stagetypeelem.Descendants("td").Skip(2).Select(e => e.InnerHtml));
                    stagetype.Stages.Add(stage);
                }

                showinstance.StageTypes.Add(stagetype);
            }
        }

        internal void Stop()
        {
        }
    }
}
