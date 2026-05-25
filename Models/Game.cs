namespace DetektivGame.Models;

public class InteractionResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public bool UpdateRoom { get; set; } = false;
    public bool IsGameEnd { get; set; } = false;
}

public class CraftResult
{
    public bool Success { get; set; }
    public Item? CraftedItem { get; set; }
    public bool IsGameEnd { get; set; } = false;
    public string FailReason { get; set; } = "";
}

public class Game
{
    public Player Player { get; private set; }
    public List<Room> Rooms { get; private set; }
    public int CurrentRoomIndex { get; private set; } = 0;
    public Room CurrentRoom => Rooms[CurrentRoomIndex];



    public bool VaseDestroyed { get; private set; } = false;
    public bool ChestOpened { get; private set; } = false;
    public bool RavenFed { get; private set; } = false;



    public Theme CurrentTheme { get; private set; } = Theme.Dark;

    public Game(string playerName)
    {
        Player = new Player { Name = playerName };
        Rooms = BuildRooms();
    }

    private List<Room> BuildRooms() => new()
    {
        new Room
        {
            Id = 0, Name = "Esimene tuba", BackgroundImage = "teinetuba.png",
            HasLeftRoom = false, HasRightRoom = true,
            Objects = new()
            {
                new RoomObject { Item = Item.Apple, X = 0.13, Y = 0.52, Width = 110, Height = 110 },
                new RoomObject { Item = Item.Stick, X = 0.40, Y = 0.47, Width = 140, Height = 110 },
                new RoomObject { Item = Item.Chest, X = 0.68, Y = 0.48, Width = 160, Height = 145, IsInteractable = true },
            }
        },
        new Room
        {
            Id = 1, Name = "Teine tuba", BackgroundImage = "teinetuba.png",
            HasLeftRoom = true, HasRightRoom = true,
            Objects = new()
            {
                new RoomObject { Item = Item.Vase,  X = 0.20, Y = 0.38, Width = 115, Height = 145, IsInteractable = true },
                new RoomObject { Item = Item.Stone, X = 0.72, Y = 0.55, Width = 120, Height = 95  },
            }
        },
        new Room
        {
            Id = 2, Name = "Kolmas tuba", BackgroundImage = "tubakolm.png",
            HasLeftRoom = true, HasRightRoom = false,
            Objects = new()
            {
                new RoomObject { Item = Item.Raven, X = 0.42, Y = 0.20, Width = 145, Height = 185, IsInteractable = true },
            }
        },
    };

    public bool GoRight() { if (!CurrentRoom.HasRightRoom) return false; CurrentRoomIndex++; Player.AddStep(); return true; }
    public bool GoLeft() { if (!CurrentRoom.HasLeftRoom) return false; CurrentRoomIndex--; Player.AddStep(); return true; }
    public void SetTheme(Theme t) => CurrentTheme = t;


    public InteractionResult InteractWithObject(string objectId)
    {
        var obj = CurrentRoom.GetObject(objectId);
        if (obj == null || !obj.IsVisible)
            return Fail("Siin pole midagi.");
        if (!obj.IsInteractable)
            return Fail($"{obj.Item.Name} on juba kasutatud.");

        return objectId switch
        {
            "apple" or "stick" or "stone" => PickUp(obj),
            "chest" => InteractChest(obj),
            "vase" => InteractVase(obj),
            "raven" => InteractRaven(obj),
            _ => Fail("Ei saa sellega midagi teha.")
        };
    }

    private InteractionResult PickUp(RoomObject obj)
    {
        if (obj.Item.IsCollected)
            return Fail("Juba korjatud.");
        if (!Player.AddToInventory(obj.Item))
            return Fail("Inventar on täis! (max 8 eset)");

        obj.Item.IsCollected = true;
        obj.IsVisible = false;
        Player.AddScore(10);
        return Ok($"{obj.Item.Name} lisati inventarile.");
    }

    private InteractionResult InteractChest(RoomObject obj)
    {
        if (ChestOpened) return Fail("Seif on juba avatud.");
        if (!Player.HasItem("key"))
            return Fail("Seif on lukus. Sul pole võtit.");

        ChestOpened = true;
        obj.Item = Item.ChestOpen;
        obj.IsInteractable = false;
        Player.AddScore(50);
        return new InteractionResult { Success = true, Message = "Seif on avatud! Leidsid aarde!", UpdateRoom = true, IsGameEnd = true };
    }

    private InteractionResult InteractVase(RoomObject obj)
    {
        if (VaseDestroyed) return Fail("Kuvšin on juba purustatud.");

        bool hasPickaxe = Player.HasItem("pickaxe");

        if ( !hasPickaxe)
            return Fail("Vajan midagi kõva, et kuvšini purustada.");

        VaseDestroyed = true;


        obj.Item = Item.VaseBroken;
        obj.IsInteractable = false;
        obj.Width = 110;
        obj.Height = 90;

        Player.AddToInventory(Item.Key);
        Player.AddScore(30);
        return Ok("Kuvšin purunes! Leidsid võtme!", updateRoom: true);
    }

    private InteractionResult InteractRaven(RoomObject obj)
    {
        if (RavenFed) return Fail("Ronk on juba söönud.");
        if (!Player.HasItem("apple"))
            return Fail("Ronk vaatab sind nälgiva pilguga. Paku talle midagi.");

        RavenFed = true;
        Player.RemoveById("apple");

        obj.Item = Item.RavenFed;
        obj.IsInteractable = false;

        Player.AddToInventory(Item.Rope);
        Player.AddScore(30);
        return Ok("Ronk võttis õuna ja andis sulle nööri!", updateRoom: true);
    }

    public CraftResult TryCraft(string id1, string id2)
    {
        if (Match(id1, id2, "stick", "rope"))
        {
            var s = Player.GetItem("stick");
            var r = Player.GetItem("rope");
            if (s != null && r != null)
            {
                Player.RemoveFromInventory(s);
                Player.RemoveFromInventory(r);
                Player.AddToInventory(Item.StickWithRope);
                Player.AddScore(20);
                return new CraftResult { Success = true, CraftedItem = Item.StickWithRope };
            }
        }
        if (Match(id1, id2, "stick_rope", "stone"))
        {
            var sr = Player.GetItem("stick_rope");
            var st = Player.GetItem("stone");
            if (sr != null && st != null)
            {
                Player.RemoveFromInventory(sr);
                Player.RemoveFromInventory(st);
                Player.AddToInventory(Item.Pickaxe);
                Player.AddScore(50);
                return new CraftResult { Success = true, CraftedItem = Item.Pickaxe };
            }
        }

        return new CraftResult { Success = false, FailReason = id1 == id2 ? "Sama ese ei sobi iseendaga." : "Need esemed ei sobi kokku." };
    }

    private static bool Match(string a, string b, string x, string y) =>
        (a == x && b == y) || (a == y && b == x);

    private static InteractionResult Ok(string msg, bool updateRoom = false) =>
        new() { Success = true, Message = msg, UpdateRoom = updateRoom };
    private static InteractionResult Fail(string msg) =>
        new() { Success = false, Message = msg };
}