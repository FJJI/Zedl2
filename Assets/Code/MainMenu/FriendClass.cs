public class Friend
{
    public string friend_name;
    public string friend_id;
    public int wins;
    public int loses;

    public Friend(string fn, string fi, int wins, int loses)
    {
        this.friend_name = fn;
        this.friend_id = fi;
        this.wins = wins;
        this.loses = loses;
    }
}
