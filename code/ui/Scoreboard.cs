using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
public class Scoreboard : Sandbox.UI.Scoreboard<ScoreboardEntry>
{

    public Scoreboard()
    {
        StyleSheet.Load("/ui/Scoreboard.scss");
    }

    protected override void AddHeader()
    {
        Header = Add.Panel("header");
        Header.Add.Label("Player", "name");
        Header.Add.Label("Score", "score");
        Header.Add.Label("Karma", "karma");
        Header.Add.Label("Deaths", "deaths");
        Header.Add.Label("Ping", "ping");
    }
}

public class ScoreboardEntry : Sandbox.UI.ScoreboardEntry
{
    public Label Score;
    public Label Karma;

    public ScoreboardEntry() : base()
    {
        AddClass("entry");

        Score = Add.Label("", "score");
        Karma = Add.Label("", "karma");
    }

    public override void UpdateFrom(PlayerScore.Entry entry)
    {
        Entry = entry;

        PlayerName.Text = entry.GetString("name");
        Score.Text = entry.Get<int>("kills", 0).ToString();
        Deaths.Text = entry.Get<int>("deaths", 0).ToString();
        Karma.Text = entry.Get<int>("karma", 0).ToString();
        Ping.Text = entry.Get<int>("ping", 0).ToString();

        SetClass("me", Local.Client != null && entry.Get<ulong>("steamid", 0) == Local.Client.SteamId);
    }
}

}
