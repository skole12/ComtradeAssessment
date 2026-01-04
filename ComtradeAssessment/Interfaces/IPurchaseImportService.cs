using System.ServiceModel;
using ComtradeAssessment.DTO;

namespace ComtradeAssessment.Interfaces;

[ServiceContract]
public interface IPurchaseImportService
{
    [OperationContract]
    Task<PurchaseImportResponse> ImportPurchases(PurchaseImportDto request);
}
