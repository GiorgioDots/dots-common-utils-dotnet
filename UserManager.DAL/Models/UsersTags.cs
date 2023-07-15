using System;
using System.Collections.Generic;

using UserManager.DTO;
using Microsoft.EntityFrameworkCore;

namespace UserManager.DAL.Models;

public partial class UsersTags
{
    public int Id { get; set; }

    public int IdUser { get; set; }

    public int IdTag { get; set; }

    public int Status { get; set; }

    public DateTime? DateCreate { get; set; }

    public DateTime? DateUpdate { get; set; }

    public virtual Tags IdTagNavigation { get; set; }

    public virtual Users IdUserNavigation { get; set; }

    public async Task DeleteReferences(UserManagerEntities dc)
    {
        if(Id == 0) return;
        var query = "";
        if(string.IsNullOrEmpty(query)) return;
        await dc.Database.ExecuteSqlRawAsync(query);
    }
}

