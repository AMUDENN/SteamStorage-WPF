using System.Collections.Generic;

namespace SteamStorage.Entities;

public partial class ArchiveGroup
{
    public long Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<Archive> Archives { get; set; } = new List<Archive>();
}
