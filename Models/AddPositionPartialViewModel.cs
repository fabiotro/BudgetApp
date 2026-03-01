namespace BudgetApp.Models
{
    public class AddPositionPartialViewModel
    {
        public string FormAction { get; set; } = string.Empty;
        public string FormController { get; set; } = string.Empty;
        public string ParentIdFieldName { get; set; } = string.Empty;
        public int ParentId { get; set; }
        public string CategorySelectId { get; set; } = string.Empty;
        public string SubCategorySelectId { get; set; } = string.Empty;
        public List<PositionTypeModel> PositionTypes { get; set; } = [];
        public List<CategoryModel> Categories { get; set; } = [];
        public List<SubCategoryModel> SubCategories { get; set; } = [];

        public string ModalId { get; set; } = "addPositionModal";

        public bool ShowCampVariables { get; set; } = false;
        public int CampParticipantsCount_fc { get; set; }
        public int CampJs_PersonsCount_fc { get; set; }
        public int CampLeadersTeamCount_fc { get; set; }
    }
}
