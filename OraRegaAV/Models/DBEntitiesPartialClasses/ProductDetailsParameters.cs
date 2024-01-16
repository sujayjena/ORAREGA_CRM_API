namespace OraRegaAV.Models.DBEntitiesPartialClasses
{
    public class ProductPurchaseProofSaveParameters
    {
        public int SavedProductDetailId { get; set; }
        public string FilesOriginalName { get; set; }
        public string SavedFileName { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class ProductSnaps
    {
        public int SavedProductDetailId { get; set; }
        public string FilesOriginalName { get; set; }
        public string SavedFileName { get; set; }
        public string SnapType { get; set; }
        public bool IsDeleted { get; set; }
    }
}
