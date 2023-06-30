using SteamStorage.Entities;
using SteamStorage.Utilities;

namespace SteamStorage.Models
{
    public class RemainGroupModel
    {
        #region Fields
        private RemainGroup remainGroup;
        private long? remainsCount;
        private double? remainsAmount;
        private double? remainsPercent;

        private Context context = Singleton.GetObject<Context>();
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
        public RemainGroupModel()
        {
            remainGroup = new();
            context.AddRemainGroup(remainGroup);
        }
        #endregion Constructor

        #region Methods
        private void UpdateRemains()
        {
            var remainModels = context.GetRemainModels(this);
            remainsCount = CalculationModel.GetRemainTotalCount(remainModels);
            remainsAmount = CalculationModel.GetRemainTotalAmountPurchase(remainModels);
            remainsPercent = CalculationModel.GetRemainAveragePercent(remainModels);
        }
        public void EditGroup(string title)
        {
            remainGroup.Title = title;
        }
        public void DeleteGroup()
        {

        }
        public void DeletGroupWithSkins()
        {

        }
        #endregion Methods
    }
}
