using System;
using System.Collections.Generic;

using Dots.Commons;
using Dots.Commons.DALs;
using Microsoft.EntityFrameworkCore;

namespace Dots.Auth.DAL.Models;

public partial class AccountsTokens : BaseUpdatable 
{
    public int IdAccount { get; set; }

    public string Token { get; set; }

    public int Type { get; set; }

    public DateTime? DateTo { get; set; }

    public virtual Accounts IdAccountNavigation { get; set; }

    public override async Task DeleteReferences(object dc)
    {
        if(dc is not DotsAuthContext)
        {
            return;
        }
        var context = dc as DotsAuthContext;
        if(Id == 0) return;
        var query = "";
        if(string.IsNullOrEmpty(query)) return;
        await context.Database.ExecuteSqlRawAsync(query);
    }
}

