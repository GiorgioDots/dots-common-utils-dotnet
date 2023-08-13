using System;
using System.Collections.Generic;

using UserManager.DTO;
using Microsoft.EntityFrameworkCore;
using Dots.Commons;

namespace UserManager.DAL.Models;

public partial class Tags
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int Status { get; set; }

    public DateTime? DateCreate { get; set; }

    public DateTime? DateUpdate { get; set; }

    public virtual ICollection<UsersTags> UsersTags { get; set; } = new List<UsersTags>();

    public async Task DeleteReferences(UserManagerEntities dc)
    {
        if(Id == 0) return;
        var query = "";
        query += $"UPDATE UsersTags SET Status = {(int)eEntityStatus.DELETED_BY_REF} WHERE IdTag = {Id} AND Status = {(int)eEntityStatus.OK};\n";
        if(string.IsNullOrEmpty(query)) return;
        await dc.Database.ExecuteSqlRawAsync(query);
    }
}

