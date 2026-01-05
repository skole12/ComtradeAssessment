using System.ServiceModel;
using ComtradeAssessment.Models;

namespace ComtradeAssessment.Interfaces;

[ServiceContract]
public interface IPurchaseImportService
{
    [OperationContract]
    Task<PurchaseImportResponse> ImportPurchases(PurchaseImportDto request);
}
