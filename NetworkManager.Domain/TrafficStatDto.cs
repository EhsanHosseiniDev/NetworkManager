namespace NetworkManager.Domain;

public class TrafficStatDto
{
    public string Uuid { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public long Upload { get; set; }
    public long Download { get; set; }
}
