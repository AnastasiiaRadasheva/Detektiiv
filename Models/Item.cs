namespace DetektivGame.Models;

public class Item
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Emoji { get; set; } = "❓";
    public string ImageSource { get; set; } = "";
    public string Description { get; set; } = "";
    public bool IsPickable { get; set; } = true;
    public bool IsCollected { get; set; } = false;

    // ── Room 1 ─────────────────────────────────────────────────────
    public static Item Apple => new()
    {
        Id = "apple",
        Name = "Õun",
        Emoji = "🍎",
        ImageSource = "oun.png",
        Description = "Punane värske õun."
    };
    public static Item Stick => new()
    {
        Id = "stick",
        Name = "Kepp",
        Emoji = "🪵",
        ImageSource = "puu.png",
        Description = "Tugev puidust kepp."
    };
    public static Item Chest => new()
    {
        Id = "chest",
        Name = "Seif",
        Emoji = "🗃️",
        ImageSource = "ceif.png",
        Description = "Lukustatud seif. Vajan võtit.",
        IsPickable = false
    };
    public static Item ChestOpen => new()
    {
        Id = "chest_open",
        Name = "Avatud Seif",
        Emoji = "🎉",
        ImageSource = "avatudseif.png",
        Description = "Seif on avatud!",
        IsPickable = false
    };

    // ── Room 2 ─────────────────────────────────────────────────────
    public static Item Vase => new()
    {
        Id = "vase",
        Name = "Kuvšin",
        Emoji = "🏺",
        ImageSource = "kuvsin.png",
        Description = "Ilus kuvšin. Millegi sees.",
        IsPickable = false
    };
    public static Item VaseBroken => new()
    {
        Id = "vase_broken",
        Name = "Purunenud kuvšin",
        Emoji = "💥",
        ImageSource = "kuvsinanet.png",
        Description = "Kuvšin on katki.",
        IsPickable = false
    };
    public static Item Stone => new()
    {
        Id = "stone",
        Name = "Kivi",
        Emoji = "🪨",
        ImageSource = "kivi.png",
        Description = "Terav kivi."
    };

    // ── Room 3 ─────────────────────────────────────────────────────
    public static Item Raven => new()
    {
        Id = "raven",
        Name = "Ronk",
        Emoji = "🐦",
        ImageSource = "voron.png",
        Description = "Must ronk vaatab sind tarkade silmadega.",
        IsPickable = false
    };
    public static Item RavenFed => new()
    {
        Id = "raven_fed",
        Name = "Ronk (söönud)",
        Emoji = "🐦",
        ImageSource = "voronverevka.png",
        Description = "Ronk hoiab nööri.",
        IsPickable = false
    };

    // ── Inventory / craft items ────────────────────────────────────
    public static Item Key => new()
    {
        Id = "key",
        Name = "Võti",
        Emoji = "🗝️",
        ImageSource = "vott.png",
        Description = "Kuldne võti. Seifiks?"
    };
    public static Item Rope => new()
    {
        Id = "rope",
        Name = "Nöör",
        Emoji = "🪢",
        ImageSource = "verevka.png",
        Description = "Pikk tugev nöör."
    };
    public static Item StickWithRope => new()
    {
        Id = "stick_rope",
        Name = "Kepp + Nöör",
        Emoji = "🪵",
        ImageSource = "puuiverevka.png",
        Description = "Kepp seotud nööriga."
    };
    public static Item Pickaxe => new()
    {
        Id = "pickaxe",
        Name = "Kirves",
        Emoji = "⛏️",
        ImageSource = "kirves.png",
        Description = "Kirves on valmis!"
    };
}