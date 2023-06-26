using Microsoft.EntityFrameworkCore;
using SteamStorage.Entities;
using SteamStorage.Models;
using System.Collections.Generic;
using System.Linq;

namespace SteamStorage.Utilities
{
    public class Context
    {
        #region Fields
        private readonly SteamStorageDbContext DbContext = new();
        private IEnumerable<RemainModel> remainModels;
        private IEnumerable<ArchiveModel> archiveModels;
        private IEnumerable<RemainGroupModel> remainGroupModels;
        private IEnumerable<ArchiveGroupModel> archiveGroupModels;
        #endregion Fields

        #region Properties
        public SteamStorageDbContext DBContext => DbContext;
        public IEnumerable<RemainGroupModel> RemainGroups => remainGroupModels;
        public IEnumerable<ArchiveGroupModel> ArchiveGroups => archiveGroupModels;
        #endregion Properties

        #region Constructor
        public Context()
        {
            UpdateRemainModels();
            UpdateArchiveModels();
            UpdateRemainGroupModels();
            UpdateArchiveGroupModels();
        }
        #endregion Constructor

        #region Methods
        public IEnumerable<RemainModel> GetRemainModels(RemainGroupModel? groupModel)
        {
            return remainModels.Where(x => groupModel is null || x.RemainGroup == groupModel.RemainGroup);
        }
        public IEnumerable<ArchiveModel> GetArchiveModels(ArchiveGroupModel? groupModel)
        {
            return archiveModels.Where(x => groupModel is null || x.ArchiveGroup == groupModel.ArchiveGroup);
        }
        public void AddRemainGroup(RemainGroup remainGroup)
        {
            DbContext.RemainGroups.Add(remainGroup);
        }
        public void AddArchiveGroup(ArchiveGroup archiveGroup)
        {
            DbContext.ArchiveGroups.Add(archiveGroup);
        }
        public void SaveChanges()
        {
            DbContext.SaveChanges();
        }
        public void UndoChanges()
        {
            foreach (var entry in DbContext.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                }
            }
        }
        public void UpdateRemainModels()
        {
            remainModels = DBContext.Remains.Select(x => new RemainModel(x));
        }
        public void UpdateArchiveModels()
        {
            archiveModels = DBContext.Archives.Select(x => new ArchiveModel(x));
        }
        public void UpdateRemainGroupModels()
        {
            remainGroupModels = DBContext.RemainGroups.Select(x => new RemainGroupModel(x));
        }
        public void UpdateArchiveGroupModels()
        {
            archiveGroupModels = DBContext.ArchiveGroups.Select(x => new ArchiveGroupModel(x));
        }
        #endregion Methods
    }
}