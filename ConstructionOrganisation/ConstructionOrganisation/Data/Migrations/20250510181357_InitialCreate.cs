using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConstructionOrganisation.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "characteristic_gr",
                columns: table => new
                {
                    CharacteristicGrID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharacteristicGrName = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.CharacteristicGrID);
                });

            migrationBuilder.CreateTable(
                name: "characteristic_ob",
                columns: table => new
                {
                    CharacteristicObNameID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharacteristicObName = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.CharacteristicObNameID);
                });

            migrationBuilder.CreateTable(
                name: "contract",
                columns: table => new
                {
                    ContractNumber = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Price = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ContractNumber);
                });

            migrationBuilder.CreateTable(
                name: "group",
                columns: table => new
                {
                    GroupNameID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupName = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.GroupNameID);
                });

            migrationBuilder.CreateTable(
                name: "machine_type",
                columns: table => new
                {
                    MachineTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MachineType = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.MachineTypeID);
                });

            migrationBuilder.CreateTable(
                name: "material",
                columns: table => new
                {
                    MaterialID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MeasurementUnits = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: true),
                    MaterialName = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.MaterialID);
                });

            migrationBuilder.CreateTable(
                name: "object_type",
                columns: table => new
                {
                    ObjectTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObjectType = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ObjectTypeID);
                });

            migrationBuilder.CreateTable(
                name: "work_type",
                columns: table => new
                {
                    WorkTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkTypeName = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.WorkTypeID);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "employee",
                columns: table => new
                {
                    EmployeeCode = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupNameID = table.Column<int>(type: "int", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    HireDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.EmployeeCode);
                    table.ForeignKey(
                        name: "GroupNameIDEmployee",
                        column: x => x.GroupNameID,
                        principalTable: "group",
                        principalColumn: "GroupNameID");
                });

            migrationBuilder.CreateTable(
                name: "group_characteristic",
                columns: table => new
                {
                    GroupNameID = table.Column<int>(type: "int", nullable: false),
                    CharacteristicGrID = table.Column<int>(type: "int", nullable: false),
                    ValueGrCh = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.GroupNameID, x.CharacteristicGrID });
                    table.ForeignKey(
                        name: "CharacteristicGrID",
                        column: x => x.CharacteristicGrID,
                        principalTable: "characteristic_gr",
                        principalColumn: "CharacteristicGrID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "GroupNameID",
                        column: x => x.GroupNameID,
                        principalTable: "group",
                        principalColumn: "GroupNameID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "brigade",
                columns: table => new
                {
                    BrigadeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Foreman = table.Column<int>(type: "int", nullable: true),
                    BrigadeName = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.BrigadeID);
                    table.ForeignKey(
                        name: "Foreman_brigade",
                        column: x => x.Foreman,
                        principalTable: "employee",
                        principalColumn: "EmployeeCode");
                });

            migrationBuilder.CreateTable(
                name: "management",
                columns: table => new
                {
                    ManagementNumber = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Director = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ManagementNumber);
                    table.ForeignKey(
                        name: "ManagementDirector",
                        column: x => x.Director,
                        principalTable: "employee",
                        principalColumn: "EmployeeCode");
                });

            migrationBuilder.CreateTable(
                name: "section",
                columns: table => new
                {
                    SectionNameID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Manager = table.Column<int>(type: "int", nullable: false),
                    ManagementNumber = table.Column<int>(type: "int", nullable: true),
                    SectionName = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.SectionNameID);
                    table.ForeignKey(
                        name: "ManagerSection",
                        column: x => x.Manager,
                        principalTable: "employee",
                        principalColumn: "EmployeeCode");
                });

            migrationBuilder.CreateTable(
                name: "section_employee",
                columns: table => new
                {
                    SectionNameID = table.Column<int>(type: "int", nullable: false),
                    EmployeeCode = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "EmployeeCode_section_employee",
                        column: x => x.EmployeeCode,
                        principalTable: "employee",
                        principalColumn: "EmployeeCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "brigade_employee",
                columns: table => new
                {
                    EmployeeCode = table.Column<int>(type: "int", nullable: false),
                    BrigadeID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.EmployeeCode, x.BrigadeID });
                    table.ForeignKey(
                        name: "BrigadeID_brigade_employee",
                        column: x => x.BrigadeID,
                        principalTable: "brigade",
                        principalColumn: "BrigadeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "EmployeeCode_brigade_employee",
                        column: x => x.EmployeeCode,
                        principalTable: "employee",
                        principalColumn: "EmployeeCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "machine",
                columns: table => new
                {
                    SerialNumber = table.Column<int>(type: "int", nullable: false),
                    ManagementNumber = table.Column<int>(type: "int", nullable: true),
                    MachineTypeID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.SerialNumber);
                    table.ForeignKey(
                        name: "MachineTypeID_machine",
                        column: x => x.MachineTypeID,
                        principalTable: "machine_type",
                        principalColumn: "MachineTypeID");
                    table.ForeignKey(
                        name: "ManagementNumber_machine",
                        column: x => x.ManagementNumber,
                        principalTable: "management",
                        principalColumn: "ManagementNumber");
                });

            migrationBuilder.CreateTable(
                name: "object",
                columns: table => new
                {
                    ObjectNameID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Supervisor = table.Column<int>(type: "int", nullable: true),
                    SectionNameID = table.Column<int>(type: "int", nullable: true),
                    ContractNumber = table.Column<int>(type: "int", nullable: true),
                    ObjectTypeID = table.Column<int>(type: "int", nullable: true),
                    ObjectName = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ObjectNameID);
                    table.ForeignKey(
                        name: "ContractNumber_object",
                        column: x => x.ContractNumber,
                        principalTable: "contract",
                        principalColumn: "ContractNumber",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ObjectTypeID_object",
                        column: x => x.ObjectTypeID,
                        principalTable: "object_type",
                        principalColumn: "ObjectTypeID");
                    table.ForeignKey(
                        name: "SectionNameID_object",
                        column: x => x.SectionNameID,
                        principalTable: "section",
                        principalColumn: "SectionNameID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "Supervisor_object",
                        column: x => x.Supervisor,
                        principalTable: "employee",
                        principalColumn: "EmployeeCode");
                });

            migrationBuilder.CreateTable(
                name: "object_characteristic",
                columns: table => new
                {
                    ObjectNameID = table.Column<int>(type: "int", nullable: false),
                    CharacteristicObName = table.Column<int>(type: "int", nullable: false),
                    ValueObCh = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ObjectNameID, x.CharacteristicObName });
                    table.ForeignKey(
                        name: "CharacteristicObName_object_characteristic",
                        column: x => x.CharacteristicObName,
                        principalTable: "characteristic_ob",
                        principalColumn: "CharacteristicObNameID");
                    table.ForeignKey(
                        name: "ObjectNameID_object_characteristic",
                        column: x => x.ObjectNameID,
                        principalTable: "object",
                        principalColumn: "ObjectNameID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "work",
                columns: table => new
                {
                    WorkNumber = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObjectNameID = table.Column<int>(type: "int", nullable: false),
                    SectionNameID = table.Column<int>(type: "int", nullable: false),
                    WorkTypeID = table.Column<int>(type: "int", nullable: false),
                    BrigadeID = table.Column<int>(type: "int", nullable: true),
                    PlannedStartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PlannedEndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    RealStartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    RealEndDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.WorkNumber);
                    table.ForeignKey(
                        name: "BrigadeID_work",
                        column: x => x.BrigadeID,
                        principalTable: "brigade",
                        principalColumn: "BrigadeID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "ObjectNameID_work",
                        column: x => x.ObjectNameID,
                        principalTable: "object",
                        principalColumn: "ObjectNameID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "SectionNameID_work",
                        column: x => x.SectionNameID,
                        principalTable: "section",
                        principalColumn: "SectionNameID");
                    table.ForeignKey(
                        name: "WorkTypeID_work",
                        column: x => x.WorkTypeID,
                        principalTable: "work_type",
                        principalColumn: "WorkTypeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "estimate",
                columns: table => new
                {
                    WorkNumber = table.Column<int>(type: "int", nullable: false),
                    MaterialID = table.Column<int>(type: "int", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    PlannedQuantity = table.Column<decimal>(type: "decimal(9,3)", precision: 9, scale: 3, nullable: false),
                    RealQuantity = table.Column<decimal>(type: "decimal(10,3)", precision: 10, scale: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.WorkNumber, x.MaterialID });
                    table.ForeignKey(
                        name: "MaterialID_estimate",
                        column: x => x.MaterialID,
                        principalTable: "material",
                        principalColumn: "MaterialID");
                    table.ForeignKey(
                        name: "WorkNumber_estimate",
                        column: x => x.WorkNumber,
                        principalTable: "work",
                        principalColumn: "WorkNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "work_machine",
                columns: table => new
                {
                    WorkNumber = table.Column<int>(type: "int", nullable: false),
                    SerialNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.WorkNumber, x.SerialNumber });
                    table.ForeignKey(
                        name: "SerialNumber_work_machine",
                        column: x => x.SerialNumber,
                        principalTable: "machine",
                        principalColumn: "SerialNumber",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "WorkNumber_work_machine",
                        column: x => x.WorkNumber,
                        principalTable: "work",
                        principalColumn: "WorkNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "Foreman_brigade_idx",
                table: "brigade",
                column: "Foreman");

            migrationBuilder.CreateIndex(
                name: "BrigadeID_brigade_employee_idx",
                table: "brigade_employee",
                column: "BrigadeID");

            migrationBuilder.CreateIndex(
                name: "EmployeeCode_UNIQUE",
                table: "brigade_employee",
                column: "EmployeeCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "GroupNameID_idx",
                table: "employee",
                column: "GroupNameID");

            migrationBuilder.CreateIndex(
                name: "MaterialID_estimate_idx",
                table: "estimate",
                column: "MaterialID");

            migrationBuilder.CreateIndex(
                name: "GroupName_UNIQUE",
                table: "group",
                column: "GroupName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "CharacteristicGrID_idx",
                table: "group_characteristic",
                column: "CharacteristicGrID");

            migrationBuilder.CreateIndex(
                name: "MachineTypeID_machine_idx",
                table: "machine",
                column: "MachineTypeID");

            migrationBuilder.CreateIndex(
                name: "ManagementNumber_machine_idx",
                table: "machine",
                column: "ManagementNumber");

            migrationBuilder.CreateIndex(
                name: "Director_UNIQUE",
                table: "management",
                column: "Director",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ContractNumber_object_idx",
                table: "object",
                column: "ContractNumber");

            migrationBuilder.CreateIndex(
                name: "ObjectTypeID_object_idx",
                table: "object",
                column: "ObjectTypeID");

            migrationBuilder.CreateIndex(
                name: "SectionNameID_object_idx",
                table: "object",
                column: "SectionNameID");

            migrationBuilder.CreateIndex(
                name: "Supervisor_object_idx",
                table: "object",
                column: "Supervisor");

            migrationBuilder.CreateIndex(
                name: "CharacteristicObName_object_characteristic_idx",
                table: "object_characteristic",
                column: "CharacteristicObName");

            migrationBuilder.CreateIndex(
                name: "ManagementNumberSection_idx",
                table: "section",
                column: "ManagementNumber");

            migrationBuilder.CreateIndex(
                name: "ManagerSection_idx",
                table: "section",
                column: "Manager");

            migrationBuilder.CreateIndex(
                name: "EmployeeCode_UNIQUE",
                table: "section_employee",
                column: "EmployeeCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "BrigadeID _work_idx",
                table: "work",
                column: "BrigadeID");

            migrationBuilder.CreateIndex(
                name: "ObjectNameID_work_idx",
                table: "work",
                column: "ObjectNameID");

            migrationBuilder.CreateIndex(
                name: "SectionNameID _work_idx",
                table: "work",
                column: "SectionNameID");

            migrationBuilder.CreateIndex(
                name: "WorkTypeID _work_idx",
                table: "work",
                column: "WorkTypeID");

            migrationBuilder.CreateIndex(
                name: "SerialNumber_work_machine_idx",
                table: "work_machine",
                column: "SerialNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "brigade_employee");

            migrationBuilder.DropTable(
                name: "estimate");

            migrationBuilder.DropTable(
                name: "group_characteristic");

            migrationBuilder.DropTable(
                name: "object_characteristic");

            migrationBuilder.DropTable(
                name: "section_employee");

            migrationBuilder.DropTable(
                name: "work_machine");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "material");

            migrationBuilder.DropTable(
                name: "characteristic_gr");

            migrationBuilder.DropTable(
                name: "characteristic_ob");

            migrationBuilder.DropTable(
                name: "machine");

            migrationBuilder.DropTable(
                name: "work");

            migrationBuilder.DropTable(
                name: "machine_type");

            migrationBuilder.DropTable(
                name: "management");

            migrationBuilder.DropTable(
                name: "brigade");

            migrationBuilder.DropTable(
                name: "object");

            migrationBuilder.DropTable(
                name: "work_type");

            migrationBuilder.DropTable(
                name: "contract");

            migrationBuilder.DropTable(
                name: "object_type");

            migrationBuilder.DropTable(
                name: "section");

            migrationBuilder.DropTable(
                name: "employee");

            migrationBuilder.DropTable(
                name: "group");
        }
    }
}
