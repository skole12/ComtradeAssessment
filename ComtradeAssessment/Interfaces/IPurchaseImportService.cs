using System.ServiceModel;
using ComtradeAssessment.Attributes;
using ComtradeAssessment.DTO;

namespace ComtradeAssessment.Interfaces;

[ServiceContract]
public interface IPurchaseImportService
{
    [OperationContract]
    [AllowAnonymous]
    PurchaseImportResponse ImportPurchases(PurchaseImportDto request);
}
