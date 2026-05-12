namespace DetektivGame.Models;

public class Theme
{
    public string Name { get; set; } = "";
    public Color BackgroundColor { get; set; } = Colors.Black;
    public Color TextColor { get; set; } = Colors.White;
    public Color AccentColor { get; set; } = Colors.Gold;
    public Color CardColor { get; set; } = Colors.DarkSlateGray;
    public Color InventoryColor { get; set; } = Colors.DarkBlue;

    public static Theme Dark => new()
    {
        Name = "Dark",
        BackgroundColor = Color.FromArgb("#1A1A2E"),
        TextColor = Color.FromArgb("#E0E0E0"),
        AccentColor = Color.FromArgb("#C9A84C"),
        CardColor = Color.FromArgb("#16213E"),
        InventoryColor = Color.FromArgb("#0F3460"),
    };

    public static Theme Sepia => new()
    {
        Name = "Sepia",
        BackgroundColor = Color.FromArgb("#2C1810"),
        TextColor = Color.FromArgb("#F5DEB3"),
        AccentColor = Color.FromArgb("#D4A853"),
        CardColor = Color.FromArgb("#3D2314"),
        InventoryColor = Color.FromArgb("#4A2C1A"),
    };

    public static Theme Forest => new()
    {
        Name = "Forest",
        BackgroundColor = Color.FromArgb("#1B2D1B"),
        TextColor = Color.FromArgb("#C8E6C9"),
        AccentColor = Color.FromArgb("#66BB6A"),
        CardColor = Color.FromArgb("#2E4A2E"),
        InventoryColor = Color.FromArgb("#1A3A1A"),
    };

    public void Apply(ContentPage page)
    {
        page.BackgroundColor = BackgroundColor;
    }
}