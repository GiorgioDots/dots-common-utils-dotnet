using System;
using System.Collections.Generic;

using Dots.Commons;
using Dots.Commons.DALs;
using Microsoft.EntityFrameworkCore;

namespace Dots.Auth.DAL.Models;

public partial class Accounts : BaseUpdatable 
{
    public string Username { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public virtual ICollection<AccountsApplications> AccountsApplications { get; set; } = new List<AccountsApplications>();

    public virtual ICollection<AccountsTokens> AccountsTokens { get; set; } = new List<AccountsTokens>();

    public override async Task DeleteReferences(object dc)
    {
        if(dc is not DotsAuthContext)
        {
            return;
        }
        var context = dc as DotsAuthContext;
        if(Id == 0) return;
        var query = "";
        query += $"UPDATE AccountsApplications SET Status = {(int)eEntityStatus.DELETED_BY_REF} WHERE IdAccount = {Id} AND Status = {(int)eEntityStatus.OK};\n";
        query += $"UPDATE AccountsTokens SET Status = {(int)eEntityStatus.DELETED_BY_REF} WHERE IdAccount = {Id} AND Status = {(int)eEntityStatus.OK};\n";
        if(string.IsNullOrEmpty(query)) return;
        await context.Database.ExecuteSqlRawAsync(query);
    }
}

