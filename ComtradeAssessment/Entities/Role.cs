using Microsoft.EntityFrameworkCore;

namespace ComtradeAssessment.Entities;

[PrimaryKey(nameof(Id))]
public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}
