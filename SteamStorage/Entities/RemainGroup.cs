using System.Collections.Generic;

namespace SteamStorage.Entities;

public partial class RemainGroup
{
    public long Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<Remain> Remains { get; set; } = new List<Remain>();
}
