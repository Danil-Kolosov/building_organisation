using ConstructionOrganisation.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ConstructionOrganisation.Data;

public partial class ApplicationDbContext : IdentityDbContext<User> // Используем ваш User
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {              
}
    public virtual DbSet<Brigade> Brigades { get; set; }

    public virtual DbSet<BrigadeEmployee> BrigadeEmployees { get; set; }

    public virtual DbSet<CharacteristicGr> CharacteristicGrs { get; set; }

    public virtual DbSet<CharacteristicOb> CharacteristicObs { get; set; }

    public virtual DbSet<Contract> Contracts { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Estimate> Estimates { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<GroupCharacteristic> GroupCharacteristics { get; set; }

    public virtual DbSet<Machine> Machines { get; set; }

    public virtual DbSet<MachineType> MachineTypes { get; set; }

    public virtual DbSet<Management> Managements { get; set; }

    public virtual DbSet<Material> Materials { get; set; }

    public virtual DbSet<Models.Object> Objects { get; set; }

    public virtual DbSet<ObjectCharacteristic> ObjectCharacteristics { get; set; }

    public virtual DbSet<ObjectType> ObjectTypes { get; set; }

    public virtual DbSet<Section> Sections { get; set; }

    public virtual DbSet<SectionEmployee> SectionEmployees { get; set; }

    public virtual DbSet<Work> Works { get; set; }

    public virtual DbSet<WorkType> WorkTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (!options.IsConfigured)
        {
            options.UseSqlServer(
                "Server=(localdb)\\mssqllocaldb;Database=building_organisation;Trusted_Connection=True;"
            );
        }
    }
    //protected override void OnConfiguring(DbContextOptionsBuilder options)
    //    => options.UseMySql(
    //        "Server=localhost;Database=building_organisation;User=root;Password=Ammon Dgerro;",
    //        ServerVersion.AutoDetect("Server=localhost;Database=building_organisation;User=root;Password=Ammon Dgerro;")
    //    );
    //бло другое

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);  // Важно для Identity!
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Brigade>(entity =>
        {
            entity.HasKey(e => e.BrigadeId).HasName("PRIMARY");

            entity.ToTable("brigade");

            entity.HasIndex(e => e.Foreman, "Foreman_brigade_idx");

            entity.Property(e => e.BrigadeId).HasColumnName("BrigadeID");
            entity.Property(e => e.BrigadeName).HasMaxLength(45);

            entity.HasOne(d => d.ForemanNavigation).WithMany(p => p.Brigades)
                .HasForeignKey(d => d.Foreman)
                .HasConstraintName("Foreman_brigade");
        });

        modelBuilder.Entity<BrigadeEmployee>(entity =>
        {
            entity.HasKey(e => new { e.EmployeeCode, e.BrigadeId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("brigade_employee");

            entity.HasIndex(e => e.BrigadeId, "BrigadeID_brigade_employee_idx");

            entity.HasIndex(e => e.EmployeeCode, "EmployeeCode_UNIQUE").IsUnique();

            entity.Property(e => e.BrigadeId).HasColumnName("BrigadeID");

            entity.HasOne(d => d.Brigade).WithMany(p => p.BrigadeEmployees)
                .HasForeignKey(d => d.BrigadeId)
                .HasConstraintName("BrigadeID_brigade_employee");

            entity.HasOne(d => d.EmployeeCodeNavigation).WithOne(p => p.BrigadeEmployee)
                .HasForeignKey<BrigadeEmployee>(d => d.EmployeeCode)
                .HasConstraintName("EmployeeCode_brigade_employee");
        });

        modelBuilder.Entity<CharacteristicGr>(entity =>
        {
            entity.HasKey(e => e.CharacteristicGrId).HasName("PRIMARY");

            entity.ToTable("characteristic_gr");

            entity.Property(e => e.CharacteristicGrId).HasColumnName("CharacteristicGrID");
            entity.Property(e => e.CharacteristicGrName).HasMaxLength(45);
        });

        modelBuilder.Entity<CharacteristicOb>(entity =>
        {
            entity.HasKey(e => e.CharacteristicObNameId).HasName("PRIMARY");

            entity.ToTable("characteristic_ob");

            entity.Property(e => e.CharacteristicObNameId).HasColumnName("CharacteristicObNameID");
            entity.Property(e => e.CharacteristicObName).HasMaxLength(45);
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.ContractNumber).HasName("PRIMARY");

            entity.ToTable("contract");

            entity.Property(e => e.CustomerName).HasMaxLength(45);
            entity.Property(e => e.Price).HasPrecision(12, 2);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeCode).HasName("PRIMARY");

            entity.ToTable("employee");

            entity.HasIndex(e => e.GroupNameId, "GroupNameID_idx");

            entity.Property(e => e.FirstName).HasMaxLength(45);
            entity.Property(e => e.GroupNameId).HasColumnName("GroupNameID");
            entity.Property(e => e.LastName).HasMaxLength(45);

            entity.HasOne(d => d.GroupName).WithMany(p => p.Employees)
                .HasForeignKey(d => d.GroupNameId)
                .HasConstraintName("GroupNameIDEmployee");
        });

        modelBuilder.Entity<Estimate>(entity =>
        {
            entity.HasKey(e => new { e.WorkNumber, e.MaterialId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("estimate");

            entity.HasIndex(e => e.MaterialId, "MaterialID_estimate_idx");

            entity.Property(e => e.MaterialId).HasColumnName("MaterialID");
            entity.Property(e => e.Cost).HasPrecision(10, 2);
            entity.Property(e => e.PlannedQuantity).HasPrecision(9, 3);
            entity.Property(e => e.RealQuantity).HasPrecision(10, 3);

            entity.HasOne(d => d.Material).WithMany(p => p.Estimates)
                .HasForeignKey(d => d.MaterialId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("MaterialID_estimate");

            entity.HasOne(d => d.WorkNumberNavigation).WithMany(p => p.Estimates)
                .HasForeignKey(d => d.WorkNumber)
                .HasConstraintName("WorkNumber_estimate");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.GroupNameId).HasName("PRIMARY");

            entity.ToTable("group");

            entity.HasIndex(e => e.GroupName, "GroupName_UNIQUE").IsUnique();

            entity.Property(e => e.GroupNameId).HasColumnName("GroupNameID");
            entity.Property(e => e.GroupName).HasMaxLength(45);
        });

        modelBuilder.Entity<GroupCharacteristic>(entity =>
        {
            entity.HasKey(e => new { e.GroupNameId, e.CharacteristicGrId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("group_characteristic");

            entity.HasIndex(e => e.CharacteristicGrId, "CharacteristicGrID_idx");

            entity.Property(e => e.GroupNameId).HasColumnName("GroupNameID");
            entity.Property(e => e.CharacteristicGrId).HasColumnName("CharacteristicGrID");
            entity.Property(e => e.ValueGrCh).HasMaxLength(45);

            entity.HasOne(d => d.CharacteristicGr).WithMany(p => p.GroupCharacteristics)
                .HasForeignKey(d => d.CharacteristicGrId)
                .HasConstraintName("CharacteristicGrID");

            entity.HasOne(d => d.GroupName).WithMany(p => p.GroupCharacteristics)
                .HasForeignKey(d => d.GroupNameId)
                .HasConstraintName("GroupNameID");
        });

        modelBuilder.Entity<Machine>(entity =>
        {
            entity.HasKey(e => e.SerialNumber).HasName("PRIMARY");

            entity.ToTable("machine");

            entity.HasIndex(e => e.MachineTypeId, "MachineTypeID_machine_idx");

            entity.HasIndex(e => e.ManagementNumber, "ManagementNumber_machine_idx");

            entity.Property(e => e.SerialNumber).ValueGeneratedNever();
            entity.Property(e => e.MachineTypeId).HasColumnName("MachineTypeID");

            entity.HasOne(d => d.MachineType).WithMany(p => p.Machines)
                .HasForeignKey(d => d.MachineTypeId)
                .HasConstraintName("MachineTypeID_machine");

            entity.HasOne(d => d.ManagementNumberNavigation).WithMany(p => p.Machines)
                .HasForeignKey(d => d.ManagementNumber)
                .HasConstraintName("ManagementNumber_machine");
        });

        modelBuilder.Entity<MachineType>(entity =>
        {
            entity.HasKey(e => e.MachineTypeId).HasName("PRIMARY");

            entity.ToTable("machine_type");

            entity.Property(e => e.MachineTypeId).HasColumnName("MachineTypeID");
            entity.Property(e => e.MachineType1)
                .HasMaxLength(45)
                .HasColumnName("MachineType");
        });

        modelBuilder.Entity<Management>(entity =>
        {
            entity.HasKey(e => e.ManagementNumber).HasName("PRIMARY");

            entity.ToTable("management");

            entity.HasIndex(e => e.Director, "Director_UNIQUE").IsUnique();

            entity.HasOne(d => d.DirectorNavigation).WithOne(p => p.Management)
                .HasForeignKey<Management>(d => d.Director)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ManagementDirector");
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.MaterialId).HasName("PRIMARY");

            entity.ToTable("material");

            entity.Property(e => e.MaterialId).HasColumnName("MaterialID");
            entity.Property(e => e.MaterialName).HasMaxLength(45);
            entity.Property(e => e.MeasurementUnits).HasMaxLength(35);
        });

        modelBuilder.Entity<Models.Object>(entity =>
        {
            entity.HasKey(e => e.ObjectNameId).HasName("PRIMARY");

            entity.ToTable("object");

            entity.HasIndex(e => e.ContractNumber, "ContractNumber_object_idx");

            entity.HasIndex(e => e.ObjectTypeId, "ObjectTypeID_object_idx");

            entity.HasIndex(e => e.SectionNameId, "SectionNameID_object_idx");

            entity.HasIndex(e => e.Supervisor, "Supervisor_object_idx");

            entity.Property(e => e.ObjectNameId).HasColumnName("ObjectNameID");
            entity.Property(e => e.ObjectName).HasMaxLength(45);
            entity.Property(e => e.ObjectTypeId).HasColumnName("ObjectTypeID");
            entity.Property(e => e.SectionNameId).HasColumnName("SectionNameID");

            entity.HasOne(d => d.ContractNumberNavigation).WithMany(p => p.Objects)
                .HasForeignKey(d => d.ContractNumber)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("ContractNumber_object");

            entity.HasOne(d => d.ObjectType).WithMany(p => p.Objects)
                .HasForeignKey(d => d.ObjectTypeId)
                .HasConstraintName("ObjectTypeID_object");

            entity.HasOne(d => d.SectionName).WithMany(p => p.Objects)
                .HasForeignKey(d => d.SectionNameId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("SectionNameID_object");

            entity.HasOne(d => d.SupervisorNavigation).WithMany(p => p.Objects)
                .HasForeignKey(d => d.Supervisor)
                .HasConstraintName("Supervisor_object");
        });

        modelBuilder.Entity<ObjectCharacteristic>(entity =>
        {
            entity.HasKey(e => new { e.ObjectNameId, e.CharacteristicObName })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("object_characteristic");

            entity.HasIndex(e => e.CharacteristicObName, "CharacteristicObName_object_characteristic_idx");

            entity.Property(e => e.ObjectNameId).HasColumnName("ObjectNameID");
            entity.Property(e => e.ValueObCh).HasMaxLength(45);

            entity.HasOne(d => d.CharacteristicObNameNavigation).WithMany(p => p.ObjectCharacteristics)
                .HasForeignKey(d => d.CharacteristicObName)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("CharacteristicObName_object_characteristic");

            entity.HasOne(d => d.ObjectName).WithMany(p => p.ObjectCharacteristics)
                .HasForeignKey(d => d.ObjectNameId)
                .HasConstraintName("ObjectNameID_object_characteristic");
        });

        modelBuilder.Entity<ObjectType>(entity =>
        {
            entity.HasKey(e => e.ObjectTypeId).HasName("PRIMARY");

            entity.ToTable("object_type");

            entity.Property(e => e.ObjectTypeId).HasColumnName("ObjectTypeID");
            entity.Property(e => e.ObjectType1)
                .HasMaxLength(45)
                .HasColumnName("ObjectType");
        });

        modelBuilder.Entity<Section>(entity =>
        {
            entity.HasKey(e => e.SectionNameId).HasName("PRIMARY");

            entity.ToTable("section");

            entity.HasIndex(e => e.ManagementNumber, "ManagementNumberSection_idx");

            entity.HasIndex(e => e.Manager, "ManagerSection_idx");

            entity.Property(e => e.SectionNameId).HasColumnName("SectionNameID");
            entity.Property(e => e.SectionName).HasMaxLength(45);

            entity.HasOne(d => d.ManagerNavigation).WithMany(p => p.Sections)
                .HasForeignKey(d => d.Manager)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ManagerSection");
        });

        modelBuilder.Entity<SectionEmployee>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("section_employee");

            entity.HasIndex(e => e.EmployeeCode, "EmployeeCode_UNIQUE").IsUnique();

            entity.Property(e => e.SectionNameId).HasColumnName("SectionNameID");

            entity.HasOne(d => d.EmployeeCodeNavigation).WithOne()
                .HasForeignKey<SectionEmployee>(d => d.EmployeeCode)
                .HasConstraintName("EmployeeCode_section_employee");
        });

        modelBuilder.Entity<Work>(entity =>
        {
            entity.HasKey(e => e.WorkNumber).HasName("PRIMARY");

            entity.ToTable("work");

            entity.HasIndex(e => e.BrigadeId, "BrigadeID _work_idx");

            entity.HasIndex(e => e.ObjectNameId, "ObjectNameID_work_idx");

            entity.HasIndex(e => e.SectionNameId, "SectionNameID _work_idx");

            entity.HasIndex(e => e.WorkTypeId, "WorkTypeID _work_idx");

            entity.Property(e => e.BrigadeId).HasColumnName("BrigadeID");
            entity.Property(e => e.ObjectNameId).HasColumnName("ObjectNameID");
            entity.Property(e => e.SectionNameId).HasColumnName("SectionNameID");
            entity.Property(e => e.WorkTypeId).HasColumnName("WorkTypeID");

            entity.HasOne(d => d.Brigade).WithMany(p => p.Works)
                .HasForeignKey(d => d.BrigadeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("BrigadeID_work");

            entity.HasOne(d => d.ObjectName).WithMany(p => p.Works)
                .HasForeignKey(d => d.ObjectNameId)
                .HasConstraintName("ObjectNameID_work");

            entity.HasOne(d => d.SectionName).WithMany(p => p.Works)
                .HasForeignKey(d => d.SectionNameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SectionNameID_work");

            entity.HasOne(d => d.WorkType).WithMany(p => p.Works)
                .HasForeignKey(d => d.WorkTypeId)
                .HasConstraintName("WorkTypeID_work");

            entity.HasMany(d => d.SerialNumbers).WithMany(p => p.WorkNumbers)
                .UsingEntity<Dictionary<string, object>>(
                    "WorkMachine",
                    r => r.HasOne<Machine>().WithMany()
                        .HasForeignKey("SerialNumber")
                        .HasConstraintName("SerialNumber_work_machine"),
                    l => l.HasOne<Work>().WithMany()
                        .HasForeignKey("WorkNumber")
                        .HasConstraintName("WorkNumber_work_machine"),
                    j =>
                    {
                        j.HasKey("WorkNumber", "SerialNumber")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("work_machine");
                        j.HasIndex(new[] { "SerialNumber" }, "SerialNumber_work_machine_idx");
                    });
        });

        modelBuilder.Entity<WorkType>(entity =>
        {
            entity.HasKey(e => e.WorkTypeId).HasName("PRIMARY");

            entity.ToTable("work_type");

            entity.Property(e => e.WorkTypeId).HasColumnName("WorkTypeID");
            entity.Property(e => e.WorkTypeName).HasMaxLength(45);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
