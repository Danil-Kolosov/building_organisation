using System;
using System.Collections.Generic;

namespace ConstructionOrganisation.Models;

public class SectionEmployee
{
    public int SectionNameId { get; set; }  // Часть составного ключа
    public int EmployeeCode { get; set; }  // Часть составного ключа

    // Навигационные свойства
    public Section SectionName { get; set; }
    public Employee EmployeeCodeNavigation { get; set; }
}