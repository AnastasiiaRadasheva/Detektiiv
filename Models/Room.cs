namespace DetektivGame.Models;

public class Room
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string BackgroundImage { get; set; } = "";
    public List<RoomObject> Objects { get; set; } = new();
    public bool HasLeftRoom { get; set; } = false;
    public bool HasRightRoom { get; set; } = false;

    public RoomObject? GetObject(string objectId) =>
        Objects.FirstOrDefault(o => o.Item.Id == objectId);
}

public class RoomObject
{
    public Item Item { get; set; } = null!;
    public double X { get; set; }       // 0.0–1.0 proportional
    public double Y { get; set; }
    public double Width { get; set; } = 80;
    public double Height { get; set; } = 80;
    public bool IsVisible { get; set; } = true;
    public bool IsInteractable { get; set; } = true;
}