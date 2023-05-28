using System.Collections.Generic;

namespace SteamStorage.Entities;

public partial class Skin
{
    public long Id { get; set; }

    public string Title { get; set; } = null!;

    public string Url { get; set; } = null!;

    public virtual ICollection<Archive> Archives { get; set; } = new List<Archive>();

    public virtual ICollection<Remain> Remains { get; set; } = new List<Remain>();
}
