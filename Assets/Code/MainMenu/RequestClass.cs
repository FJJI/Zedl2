public class Request
{
    public string from_id;
    public string from_name;
    public string target_id;

    public Request(string from, string name, string to)
    {
        this.from_id = from;
        this.from_name = name;
        this.target_id = to;
    }
}
