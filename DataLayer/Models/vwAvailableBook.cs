﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DataLayer.Models;

public partial class vwAvailableBook
{
    public bool? isBorrowed { get; set; }

    public string UserName { get; set; }

    public string BookTitle { get; set; }

    public string DateExpectedReturn { get; set; }

    public int BookInventoryId { get; set; }

    public string ISBN { get; set; }

    public int CopiesNumber { get; set; }
}