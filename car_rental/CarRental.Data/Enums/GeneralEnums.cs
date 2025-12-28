namespace CarRental.Data.Enums
{
    public enum OrderStatus
    {
        Pending,
        Approved,
        Active,
        Completed,
        Cancelled
    }
    
    public enum ExpenseType
    {
        Insurance, // Vignette assurance
        Maintenance,
        Inspection, // Visite
        Repair, // Pannes
        Other
    }
}
