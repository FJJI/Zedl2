public class User
{
    public string username;
    public string email;
    public string password;
    public int wins;
    public int loses;
    public string fav_unit;
    public string pref_game;
    public string nemesis;


    public User(string username, string email, string password, int wins, int loses, string nemesis, string pref_game, string fav_unit)
    {
        this.username = username;
        this.email = email;
        this.password = password;
        this.wins = wins;
        this.loses = loses;
        this.nemesis = "none";
        this.pref_game = "none";
        this.fav_unit = "none";

    }
}