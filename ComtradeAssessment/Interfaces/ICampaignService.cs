using System.ServiceModel;
using ComtradeAssessment.Attributes;
using ComtradeAssessment.Constants;
using ComtradeAssessment.Entities;
using ComtradeAssessment.Models;

namespace ComtradeAssessment.Interfaces;

[ServiceContract(Namespace = "http://tempuri.org/")]
public interface ICampaignService : IBaseEntityService<Campaign, CampaignResponseDto>
{
    [AuthorizeByRole(ERole.SalesManager)]
    [OperationContract]
    Task<CampaignResponseDto> Create(CreateCampaignRequest request);

    [AuthorizeByRole(ERole.SalesManager)]
    [OperationContract]
    Task<CampaignResponseDto> Update(UpdateCampaignRequest request);

    [AuthorizeByRole(ERole.SalesManager)]
    [OperationContract]
    Task Delete(int campaignId);

    [AuthorizeByRole(ERole.SalesManager)]
    [OperationContract]
    Task<CampaignDetailsResponseDto> Details(int campaignId);

    [AuthorizeByRole(ERole.SalesManager)]
    [OperationContract]
    Task<PurchaseImportResponse> ImportPurchases(PurchaseImportDto request);

    [AuthorizeByRole(ERole.SalesManager)]
    [OperationContract]
    Task<CampaignJobResponseDto> JobStatus(Guid jobId);

    [AuthorizeByRole(ERole.SalesManager)]
    [OperationContract]
    Task<DownloadResultsFileResponse> DownloadResultsFile(int campaignId);

    [AuthorizeByRole(ERole.SalesManager)]
    [OperationContract]
    new Task<PagedResult<CampaignResponseDto>> GetAll(BaseRequest request);
}
