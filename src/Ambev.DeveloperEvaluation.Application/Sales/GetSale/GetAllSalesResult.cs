namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Response model for GetAllSales operation
/// </summary>
public class GetAllSalesResult
{
    /// <summary>
    /// The collection of sales
    /// </summary>
    public ICollection<SaleDto> Sales { get; set; } = new List<SaleDto>();

    /// <summary>
    /// DTO for Sale information in the GetAllSales response
    /// </summary>
    public class SaleDto
    {
        /// <summary>
        /// The unique identifier of the sale
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// The sale number
        /// </summary>
        public string SaleNumber { get; set; } = string.Empty;
        
        /// <summary>
        /// The date when the sale was made
        /// </summary>
        public DateTime SaleDate { get; set; }
        
        /// <summary>
        /// The customer ID
        /// </summary>
        public Guid CustomerId { get; set; }
        
        /// <summary>
        /// The customer's name
        /// </summary>
        public string CustomerName { get; set; } = string.Empty;
        
        /// <summary>
        /// The branch ID where the sale was made
        /// </summary>
        public Guid BranchId { get; set; }
        
        /// <summary>
        /// The branch name where the sale was made
        /// </summary>
        public string BranchName { get; set; } = string.Empty;
        
        /// <summary>
        /// The total amount of the sale
        /// </summary>
        public decimal TotalAmount { get; set; }
        
        /// <summary>
        /// Whether the sale has been cancelled
        /// </summary>
        public bool IsCancelled { get; set; }
        
        /// <summary>
        /// The number of items in the sale
        /// </summary>
        public int ItemCount { get; set; }
    }
}