using System;
using System.Collections.Generic;

using Dots.Commons;
using Dots.Commons.DALs;
using Microsoft.EntityFrameworkCore;

namespace Dots.Auth.DAL.Models;

public partial class Applications : BaseUpdatable 
{
    public string Name { get; set; }

    public string Audience { get; set; }

    public string Secret { get; set; }

    public virtual ICollection<AccountsApplications> AccountsApplications { get; set; } = new List<AccountsApplications>();

    public override async Task DeleteReferences(object dc)
    {
        if(dc is not DotsAuthContext)
        {
            return;
        }
        var context = dc as DotsAuthContext;
        if(Id == 0) return;
        var query = "";
        query += $"UPDATE AccountsApplications SET Status = {(int)eEntityStatus.DELETED_BY_REF} WHERE IdApplication = {Id} AND Status = {(int)eEntityStatus.OK};\n";
        if(string.IsNullOrEmpty(query)) return;
        await context.Database.ExecuteSqlRawAsync(query);
    }
}

