using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ArchiveHist.Models;

[Table("User")]
public class User
{
    [Key]
    [Column("ID")]
    public int ID { get; set; }

    [Column("Name")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
}
