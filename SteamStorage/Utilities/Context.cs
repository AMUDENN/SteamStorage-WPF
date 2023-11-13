using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using SteamStorage.Entities;
using SteamStorage.Models.EntityModels;
using SteamStorage.Services.Logger;
using SteamStorage.Services.Parser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SteamStorage.Utilities
{
    public class Context : ObservableObject
    {
        #region Fields
        private readonly SteamStorageDbContext _dbContext = new();
        private readonly SteamStorageDbContext _dbContextAdditional = new();
        private IEnumerable<RemainElementModel> _remainElementModels;
        private IEnumerable<ArchiveElementModel> _archiveElementModels;
        private IEnumerable<RemainGroupElementModel> _remainGroupModels;
        private IEnumerable<ArchiveGroupElementModel> _archiveGroupModels;

        private readonly LoggerService _loggerService;
        private readonly SteamParseService _steamParseService;
        #endregion Fields

        #region Properties
        public SteamStorageDbContext DBContext => _dbContext;
        public SteamStorageDbContext DBContextAdditional => _dbContextAdditional;
        public IEnumerable<RemainElementModel> RemainElementModels
        {
            get => _remainElementModels.ToList();
            set => SetProperty(ref _remainElementModels, value);
        }
        public IEnumerable<ArchiveElementModel> ArchiveElementModels
        {
            get => _archiveElementModels.ToList();
            set => SetProperty(ref _archiveElementModels, value);
        }
        public IEnumerable<RemainGroupElementModel> RemainGroupModels
        {
            get => _remainGroupModels.ToList();
            set => SetProperty(ref _remainGroupModels, value);
        }
        public IEnumerable<ArchiveGroupElementModel> ArchiveGroupModels
        {
            get => _archiveGroupModels.ToList();
            set => SetProperty(ref _archiveGroupModels, value);
        }
        #endregion Properties

        #region Constructor
        public Context(LoggerService logger, SteamParseService steamParseService)
        {
            _loggerService = logger;
            _steamParseService = steamParseService;
            UpdateAll();
        }
        #endregion Constructor

        #region Methods
        public List<RemainElementModel> GetRemainModels(RemainGroupElementModel? groupModel)
        {
            return RemainElementModels.Where(x => groupModel is null || x.RemainGroup == groupModel.RemainGroup).ToList();
        }
        public List<ArchiveElementModel> GetArchiveModels(ArchiveGroupElementModel? groupModel)
        {
            return ArchiveElementModels.Where(x => groupModel is null || x.ArchiveGroup == groupModel.ArchiveGroup).ToList();
        }
        public void AddRemainGroup(RemainGroup remainGroup)
        {
            DBContext.RemainGroups.Add(remainGroup);
            SaveChanges();
            UpdateRemainGroupModels();
        }
        public void AddArchiveGroup(ArchiveGroup archiveGroup)
        {
            DBContext.ArchiveGroups.Add(archiveGroup);
            SaveChanges();
            UpdateArchiveGroupModels();
        }
        public void AddRemain(Remain remain)
        {
            DBContext.Remains.Add(remain);
            SaveChanges();
            UpdateRemainGroupModels();
            UpdateRemainModels();
        }
        public void AddArchive(Archive archive)
        {
            DBContext.Archives.Add(archive);
            SaveChanges();
            UpdateArchiveGroupModels();
            UpdateArchiveModels();
        }
        public void RemoveRemainGroup(RemainGroup remainGroup)
        {
            DBContext.RemainGroups.Remove(remainGroup);
            SaveChanges();
            UpdateRemainModels();
            UpdateRemainGroupModels();
        }
        public void RemoveArchiveGroup(ArchiveGroup archiveGroup)
        {
            DBContext.ArchiveGroups.Remove(archiveGroup);
            SaveChanges();
            UpdateArchiveModels();
            UpdateArchiveGroupModels();
        }
        public void RemoveRemain(Remain remain)
        {
            DBContext.Remains.Remove(remain);
            SaveChanges();
            UpdateRemainModels();
            UpdateRemainGroupModels();
        }
        public void RemoveArchive(Archive archive)
        {
            DBContext.Archives.Remove(archive);
            SaveChanges();
            UpdateArchiveModels();
            UpdateArchiveGroupModels();
        }
        public void EditRemainGroup(RemainGroup remainGroup, string title)
        {
            remainGroup.Title = title;
            SaveChanges();
            UpdateRemainGroupModels();
        }
        public void EditArchiveGroup(ArchiveGroup archiveGroup, string title)
        {
            archiveGroup.Title = title;
            SaveChanges();
            UpdateArchiveGroupModels();
        }
        public void EditRemain(Remain remainModel, string url, long count, double costPurchase, DateTime datePurchase, RemainGroupElementModel? remainGroupModel)
        {
            var skin = GetSkin(url);
            if (skin is null) throw new Exception("Ссылка на скин неверна!");
            remainModel.IdSkinNavigation = skin;
            remainModel.Count = count;
            remainModel.CostPurchase = costPurchase;
            remainModel.DatePurchase = datePurchase.ToString(ProgramConstants.DateTimeFormat);
            remainModel.IdGroup = remainGroupModel is null ? 1 : remainGroupModel.RemainGroup.Id;
            SaveChanges();
            UpdateRemainModels();
            UpdateRemainGroupModels();
        }
        public void EditRemain(Remain remainModel, RemainGroupElementModel? remainGroupModel)
        {
            remainModel.IdGroup = remainGroupModel is null ? 1 : remainGroupModel.RemainGroup.Id;
            SaveChanges();
            UpdateRemainModels();
            UpdateRemainGroupModels();
        }
        public void EditArchive(Archive archiveModel, string url, long count, double costPurchase, double costSold, DateTime datePurchase, DateTime dateSold, ArchiveGroupElementModel? archiveGroupModel)
        {
            var skin = GetSkin(url);
            if (skin is null) throw new Exception("Ссылка на скин неверна!");
            archiveModel.IdSkinNavigation = skin;
            archiveModel.Count = count;
            archiveModel.CostPurchase = costPurchase;
            archiveModel.CostSold = costSold;
            archiveModel.DatePurchase = datePurchase.ToString(ProgramConstants.DateTimeFormat);
            archiveModel.DateSold = dateSold.ToString(ProgramConstants.DateTimeFormat);
            archiveModel.IdGroup = archiveGroupModel is null ? 1 : archiveGroupModel.ArchiveGroup.Id;
            SaveChanges();
            UpdateArchiveModels();
            UpdateArchiveGroupModels();
        }
        public void EditArchive(Archive archiveModel, ArchiveGroupElementModel? archiveGroupModel)
        {
            archiveModel.IdGroup = archiveGroupModel is null ? 1 : archiveGroupModel.ArchiveGroup.Id;
            SaveChanges();
            UpdateArchiveModels();
            UpdateArchiveGroupModels();
        }
        public Skin? GetSkin(string url)
        {
            var skins = DBContext.Skins.Where(x => x.Url == url);
            if (skins.Any())
                return skins.First();
            else
            {
                try
                {
                    string title = _steamParseService.GetSkinTitle(url);
                    if (title == string.Empty) return null;
                    Skin skin = new()
                    {
                        Url = url,
                        Title = title
                    };
                    DBContext.Skins.Add(skin);
                    SaveChanges();
                    _loggerService.WriteMessage($"Добавление нового элемента по ссылке \"{url}\" удалось!", GetType());
                    return skin;
                }
                catch (Exception ex)
                {
                    _loggerService.WriteMessage(ex, $"Добавление нового элемента по ссылке \"{url}\" не удалось!");
                    UndoChanges();
                    return null;
                }
            }
        }
        public void AddPriceDynamic(RemainElementModel remainModel)
        {
            try
            {
                var (DateUpdate, Price) = _steamParseService.GetCurrentSkinInfo(remainModel.Url);
                if (Price == -1) throw new Exception("Невозможно получить цену");   
                PriceDynamic priceDynamic = new()
                {
                    IdRemainNavigation = remainModel.Remain,
                    CostUpdate = Price,
                    DateUpdate = DateUpdate.ToString(ProgramConstants.DateTimeFormat)
                };
                DBContextAdditional.PriceDynamics.Add(priceDynamic);
                SaveChanges();
                _loggerService.WriteMessage($"Добавление новой записи успешно!", typeof(PriceDynamic));
            }
            catch (Exception ex)
            {
                _loggerService.WriteMessage(ex, $"Добавление новой записи неудачно!");
                UndoChanges();
            }
        }
        public void SaveChanges()
        {
            DBContext.SaveChanges();
        }
        public void UndoChanges()
        {
            foreach (var entry in DBContext.ChangeTracker.Entries())
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
        private void UpdateAll()
        {
            UpdateRemainModels();
            UpdateArchiveModels();
            UpdateRemainGroupModels();
            UpdateArchiveGroupModels();
        }
        public void UpdateRemainModels()
        {
            RemainElementModels = DBContext.Remains.Select(x => new RemainElementModel(x));
        }
        public void UpdateArchiveModels()
        {
            ArchiveElementModels = DBContext.Archives.Select(x => new ArchiveElementModel(x));
        }
        public void UpdateRemainGroupModels()
        {
            RemainGroupModels = DBContext.RemainGroups.Select(x => new RemainGroupElementModel(x));
        }
        public void UpdateArchiveGroupModels()
        {
            ArchiveGroupModels = DBContext.ArchiveGroups.Select(x => new ArchiveGroupElementModel(x));
        }
        public void ClearDatabase()
        {
            try
            {
                DBContext.Archives.RemoveRange(DBContext.Archives);
                DBContext.ArchiveGroups.RemoveRange(DBContext.ArchiveGroups.Skip(1));
                DBContext.PriceDynamics.RemoveRange(DBContext.PriceDynamics);
                DBContext.Remains.RemoveRange(DBContext.Remains);
                DBContext.RemainGroups.RemoveRange(DBContext.RemainGroups.Skip(1));
                DBContext.Skins.RemoveRange(DBContext.Skins);
                SaveChanges();
                UpdateAll();
                _loggerService.WriteMessage("Очистка базы данных прошла успешно!", typeof(Context));
            }
            catch (Exception ex)
            {
                _loggerService.WriteMessage(ex, "Очистка базы данных прошла неудачно!");
                UndoChanges();
            }
        }
        #endregion Methods
    }
}