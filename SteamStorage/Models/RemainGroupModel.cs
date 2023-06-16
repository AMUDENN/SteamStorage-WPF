using SteamStorage.Entities;
using SteamStorage.Utilities;
using System.Linq;

namespace SteamStorage.Models
{
    public class RemainGroupModel
    {
        #region Fields
        private RemainGroup remainGroup;
        private long? remainsCount;
        private double? remainsAmount;
        private double? remainsPercent;
        #endregion Fields

        #region Properties
        public RemainGroup? RemainGroup => remainGroup;
        public string Title => remainGroup.Title;
        public long RemainsCount
        {
            get
            {
                if (remainsCount is null) UpdateRemains();
                return (long)remainsCount;
            }
        }
        public double RemainsAmount
        {
            get
            {
                if (remainsAmount is null) UpdateRemains();
                return (double)remainsAmount;
            }
        }
        public double RemainsPercent
        {
            get
            {
                if (remainsPercent is null) UpdateRemains();
                return (double)remainsPercent;
            }
        }
        #endregion Properties

        #region Constructor
        public RemainGroupModel(RemainGroup remainGroup)
        {
            this.remainGroup = remainGroup;
        }
        #endregion Constructor

        #region Methods
        private void UpdateRemains()
        {
            var remainModels = Context.GetRemainModels(this);
            remainsCount = Context.GetRemainTotalCount(remainModels);
            remainsAmount = Context.GetRemainTotalAmountPurchase(remainModels);
            remainsPercent = Context.GetRemainAveragePercent(remainModels);
        }
        #endregion Methods
    }
}
