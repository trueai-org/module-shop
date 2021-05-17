using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Shop.WebApi.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Catalog_Brand",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 450, nullable: false),
                    Slug = table.Column<string>(maxLength: 450, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    IsPublished = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog_Brand", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Catalog_ProductAttributeGroup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 450, nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog_ProductAttributeGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Catalog_ProductAttributeTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 450, nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog_ProductAttributeTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Catalog_ProductOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 450, nullable: false),
                    DisplayType = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog_ProductOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Catalog_Unit",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog_Unit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Core_AppSetting",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Module = table.Column<string>(maxLength: 450, nullable: true),
                    FormatType = table.Column<int>(nullable: false),
                    Type = table.Column<string>(maxLength: 450, nullable: true),
                    Value = table.Column<string>(nullable: true),
                    IsVisibleInCommonSettingPage = table.Column<bool>(nullable: false),
                    Note = table.Column<string>(maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_AppSetting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Core_Country",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 450, nullable: false),
                    TwoLetterIsoCode = table.Column<string>(maxLength: 450, nullable: false),
                    ThreeLetterIsoCode = table.Column<string>(nullable: false),
                    NumericIsoCode = table.Column<int>(nullable: false),
                    IsBillingEnabled = table.Column<bool>(nullable: false),
                    IsShippingEnabled = table.Column<bool>(nullable: false),
                    IsCityEnabled = table.Column<bool>(nullable: false),
                    IsDistrictEnabled = table.Column<bool>(nullable: false),
                    DisplayOrder = table.Column<int>(nullable: false),
                    IsPublished = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_Country", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Core_EmailSend",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    From = table.Column<string>(nullable: false),
                    To = table.Column<string>(nullable: true),
                    Cc = table.Column<string>(nullable: true),
                    Bcc = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    IsHtml = table.Column<bool>(nullable: false),
                    OutId = table.Column<string>(maxLength: 450, nullable: true),
                    ReceiptId = table.Column<string>(maxLength: 450, nullable: true),
                    IsSucceed = table.Column<bool>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_EmailSend", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Core_EntityType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 450, nullable: false),
                    IsMenuable = table.Column<bool>(nullable: false),
                    Module = table.Column<string>(maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_EntityType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Core_Media",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Caption = table.Column<string>(maxLength: 450, nullable: true),
                    FileSize = table.Column<int>(nullable: false),
                    FileName = table.Column<string>(maxLength: 450, nullable: false),
                    Url = table.Column<string>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    Host = table.Column<string>(maxLength: 450, nullable: true),
                    Hash = table.Column<string>(maxLength: 450, nullable: true),
                    Md5 = table.Column<string>(maxLength: 32, nullable: true),
                    MediaType = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_Media", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Core_Role",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Core_SmsSend",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PhoneNumber = table.Column<string>(maxLength: 450, nullable: false),
                    Value = table.Column<string>(maxLength: 450, nullable: true),
                    SignName = table.Column<string>(maxLength: 450, nullable: true),
                    TemplateType = table.Column<int>(nullable: true),
                    TemplateCode = table.Column<string>(maxLength: 450, nullable: true),
                    TemplateParam = table.Column<string>(nullable: true),
                    OutId = table.Column<string>(maxLength: 450, nullable: true),
                    ReceiptId = table.Column<string>(maxLength: 450, nullable: true),
                    IsUsed = table.Column<bool>(nullable: false),
                    IsSucceed = table.Column<bool>(nullable: false),
                    IsTest = table.Column<bool>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_SmsSend", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Core_Widget",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 450, nullable: false),
                    ViewComponentName = table.Column<string>(nullable: true),
                    CreateUrl = table.Column<string>(nullable: true),
                    EditUrl = table.Column<string>(nullable: true),
                    IsPublished = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_Widget", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Core_WidgetZone",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 450, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_WidgetZone", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shipping_FreightTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 450, nullable: true),
                    Note = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipping_FreightTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Catalog_ProductAttribute",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 450, nullable: false),
                    GroupId = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog_ProductAttribute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Catalog_ProductAttribute_Catalog_ProductAttributeGroup_Group~",
                        column: x => x.GroupId,
                        principalTable: "Catalog_ProductAttributeGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Catalog_ProductOptionData",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OptionId = table.Column<int>(nullable: false),
                    Value = table.Column<string>(maxLength: 450, nullable: false),
                    Display = table.Column<string>(maxLength: 450, nullable: true),
                    Description = table.Column<string>(nullable: true),
                    IsPublished = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog_ProductOptionData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Catalog_ProductOptionData_Catalog_ProductOption_OptionId",
                        column: x => x.OptionId,
                        principalTable: "Catalog_ProductOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Core_StateOrProvince",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ParentId = table.Column<int>(nullable: true),
                    CountryId = table.Column<int>(nullable: false),
                    Code = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 450, nullable: false),
                    Level = table.Column<int>(nullable: false),
                    DisplayOrder = table.Column<int>(nullable: false),
                    IsPublished = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_StateOrProvince", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Core_StateOrProvince_Core_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Core_Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Core_StateOrProvince_Core_StateOrProvince_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Core_StateOrProvince",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Core_Entity",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 450, nullable: false),
                    Slug = table.Column<string>(maxLength: 450, nullable: false),
                    EntityId = table.Column<int>(nullable: false),
                    EntityTypeId = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_Entity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Core_Entity_Core_EntityType_EntityTypeId",
                        column: x => x.EntityTypeId,
                        principalTable: "Core_EntityType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Catalog_Category",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 450, nullable: false),
                    Slug = table.Column<string>(maxLength: 450, nullable: false),
                    MetaTitle = table.Column<string>(nullable: true),
                    MetaKeywords = table.Column<string>(nullable: true),
                    MetaDescription = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    DisplayOrder = table.Column<int>(nullable: false),
                    IsPublished = table.Column<bool>(nullable: false),
                    IncludeInMenu = table.Column<bool>(nullable: false),
                    ParentId = table.Column<int>(nullable: true),
                    MediaId = table.Column<int>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog_Category", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Catalog_Category_Core_Media_MediaId",
                        column: x => x.MediaId,
                        principalTable: "Core_Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Catalog_Category_Catalog_Category_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Catalog_Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Core_RoleClaim",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<int>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_RoleClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Core_RoleClaim_Core_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Core_Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Core_WidgetInstance",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    PublishStart = table.Column<DateTime>(nullable: true),
                    PublishEnd = table.Column<DateTime>(nullable: true),
                    WidgetId = table.Column<int>(nullable: false),
                    WidgetZoneId = table.Column<int>(nullable: false),
                    DisplayOrder = table.Column<int>(nullable: false),
                    Data = table.Column<string>(nullable: true),
                    HtmlData = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_WidgetInstance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Core_WidgetInstance_Core_Widget_WidgetId",
                        column: x => x.WidgetId,
                        principalTable: "Core_Widget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Core_WidgetInstance_Core_WidgetZone_WidgetZoneId",
                        column: x => x.WidgetZoneId,
                        principalTable: "Core_WidgetZone",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Catalog_ProductAttributeData",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AttributeId = table.Column<int>(nullable: false),
                    Value = table.Column<string>(maxLength: 450, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    IsPublished = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog_ProductAttributeData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Catalog_ProductAttributeData_Catalog_ProductAttribute_Attrib~",
                        column: x => x.AttributeId,
                        principalTable: "Catalog_ProductAttribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Catalog_ProductAttributeTemplateRelation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TemplateId = table.Column<int>(nullable: false),
                    AttributeId = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog_ProductAttributeTemplateRelation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Catalog_ProductAttributeTemplateRelation_Catalog_ProductAttr~",
                        column: x => x.AttributeId,
                        principalTable: "Catalog_ProductAttribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Catalog_ProductAttributeTemplateRelation_Catalog_ProductAtt~1",
                        column: x => x.TemplateId,
                        principalTable: "Catalog_ProductAttributeTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Core_Address",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ContactName = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    AddressLine1 = table.Column<string>(nullable: true),
                    AddressLine2 = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    ZipCode = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Company = table.Column<string>(nullable: true),
                    StateOrProvinceId = table.Column<int>(nullable: false),
                    CountryId = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_Address", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Core_Address_Core_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Core_Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Core_Address_Core_StateOrProvince_StateOrProvinceId",
                        column: x => x.StateOrProvinceId,
                        principalTable: "Core_StateOrProvince",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Shipping_PriceAndDestination",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FreightTemplateId = table.Column<int>(nullable: false),
                    CountryId = table.Column<int>(nullable: false),
                    StateOrProvinceId = table.Column<int>(nullable: true),
                    MinOrderSubtotal = table.Column<decimal>(nullable: false),
                    ShippingPrice = table.Column<decimal>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    IsEnabled = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipping_PriceAndDestination", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shipping_PriceAndDestination_Core_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Core_Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Shipping_PriceAndDestination_Shipping_FreightTemplate_Freigh~",
                        column: x => x.FreightTemplateId,
                        principalTable: "Shipping_FreightTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Shipping_PriceAndDestination_Core_StateOrProvince_StateOrPro~",
                        column: x => x.StateOrProvinceId,
                        principalTable: "Core_StateOrProvince",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Inventory_Warehouse",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 450, nullable: false),
                    AddressId = table.Column<int>(nullable: false),
                    AdminRemark = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory_Warehouse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inventory_Warehouse_Core_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Core_Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Catalog_Product",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ParentGroupedProductId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 450, nullable: false),
                    Slug = table.Column<string>(maxLength: 450, nullable: false),
                    MetaTitle = table.Column<string>(nullable: true),
                    MetaKeywords = table.Column<string>(nullable: true),
                    MetaDescription = table.Column<string>(nullable: true),
                    ShortDescription = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Specification = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(nullable: false),
                    OldPrice = table.Column<decimal>(nullable: true),
                    SpecialPrice = table.Column<decimal>(nullable: true),
                    SpecialPriceStart = table.Column<DateTime>(nullable: true),
                    SpecialPriceEnd = table.Column<DateTime>(nullable: true),
                    HasOptions = table.Column<bool>(nullable: false),
                    IsVisibleIndividually = table.Column<bool>(nullable: false),
                    IsFeatured = table.Column<bool>(nullable: false),
                    IsCallForPricing = table.Column<bool>(nullable: false),
                    IsAllowToOrder = table.Column<bool>(nullable: false),
                    StockTrackingIsEnabled = table.Column<bool>(nullable: false),
                    Sku = table.Column<string>(maxLength: 450, nullable: true),
                    Gtin = table.Column<string>(maxLength: 450, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 450, nullable: true),
                    ThumbnailImageId = table.Column<int>(nullable: true),
                    ReviewsCount = table.Column<int>(nullable: false),
                    RatingAverage = table.Column<double>(nullable: true),
                    BrandId = table.Column<int>(nullable: true),
                    Barcode = table.Column<string>(nullable: true),
                    ValidThru = table.Column<int>(nullable: true),
                    OrderMinimumQuantity = table.Column<int>(nullable: false),
                    OrderMaximumQuantity = table.Column<int>(nullable: false),
                    DisplayStockAvailability = table.Column<bool>(nullable: false),
                    DisplayStockQuantity = table.Column<bool>(nullable: false),
                    StockReduceStrategy = table.Column<int>(nullable: false),
                    NotReturnable = table.Column<bool>(nullable: false),
                    PublishType = table.Column<int>(nullable: false),
                    DisplayOrder = table.Column<int>(nullable: false),
                    IsPublished = table.Column<bool>(nullable: false),
                    PublishedOn = table.Column<DateTime>(nullable: true),
                    UnpublishedOn = table.Column<DateTime>(nullable: true),
                    UnpublishedReason = table.Column<string>(nullable: true),
                    IsShipEnabled = table.Column<bool>(nullable: false),
                    Weight = table.Column<decimal>(nullable: false),
                    Length = table.Column<decimal>(nullable: false),
                    Width = table.Column<decimal>(nullable: false),
                    Height = table.Column<decimal>(nullable: false),
                    IsFreeShipping = table.Column<bool>(nullable: false),
                    AdditionalShippingCharge = table.Column<decimal>(nullable: false),
                    FreightTemplateId = table.Column<int>(nullable: true),
                    UnitId = table.Column<int>(nullable: true),
                    AdminRemark = table.Column<string>(nullable: true),
                    DeliveryTime = table.Column<int>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Catalog_Product_Catalog_Brand_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Catalog_Brand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Catalog_Product_Shipping_FreightTemplate_FreightTemplateId",
                        column: x => x.FreightTemplateId,
                        principalTable: "Shipping_FreightTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Catalog_Product_Catalog_Product_ParentGroupedProductId",
                        column: x => x.ParentGroupedProductId,
                        principalTable: "Catalog_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Catalog_Product_Core_Media_ThumbnailImageId",
                        column: x => x.ThumbnailImageId,
                        principalTable: "Core_Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Catalog_Product_Catalog_Unit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Catalog_Unit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Catalog_ProductAttributeValue",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AttributeId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog_ProductAttributeValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Catalog_ProductAttributeValue_Catalog_ProductAttribute_Attri~",
                        column: x => x.AttributeId,
                        principalTable: "Catalog_ProductAttribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Catalog_ProductAttributeValue_Catalog_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Catalog_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Catalog_ProductCategory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CategoryId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    IsFeaturedProduct = table.Column<bool>(nullable: false),
                    DisplayOrder = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog_ProductCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Catalog_ProductCategory_Catalog_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Catalog_Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Catalog_ProductCategory_Catalog_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Catalog_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Catalog_ProductLink",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<int>(nullable: false),
                    LinkedProductId = table.Column<int>(nullable: false),
                    LinkType = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog_ProductLink", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Catalog_ProductLink_Catalog_Product_LinkedProductId",
                        column: x => x.LinkedProductId,
                        principalTable: "Catalog_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Catalog_ProductLink_Catalog_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Catalog_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Catalog_ProductMedia",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<int>(nullable: false),
                    MediaId = table.Column<int>(nullable: false),
                    DisplayOrder = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog_ProductMedia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Catalog_ProductMedia_Core_Media_MediaId",
                        column: x => x.MediaId,
                        principalTable: "Core_Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Catalog_ProductMedia_Catalog_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Catalog_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Catalog_ProductOptionCombination",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<int>(nullable: false),
                    OptionId = table.Column<int>(nullable: false),
                    DisplayOrder = table.Column<int>(nullable: false),
                    Value = table.Column<string>(maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog_ProductOptionCombination", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Catalog_ProductOptionCombination_Catalog_ProductOption_Optio~",
                        column: x => x.OptionId,
                        principalTable: "Catalog_ProductOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Catalog_ProductOptionCombination_Catalog_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Catalog_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Catalog_ProductOptionValue",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OptionId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    Value = table.Column<string>(maxLength: 450, nullable: true),
                    Display = table.Column<string>(maxLength: 450, nullable: true),
                    DisplayOrder = table.Column<int>(nullable: false),
                    MediaId = table.Column<int>(nullable: true),
                    IsDefault = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog_ProductOptionValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Catalog_ProductOptionValue_Core_Media_MediaId",
                        column: x => x.MediaId,
                        principalTable: "Core_Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Catalog_ProductOptionValue_Catalog_ProductOption_OptionId",
                        column: x => x.OptionId,
                        principalTable: "Catalog_ProductOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Catalog_ProductOptionValue_Catalog_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Catalog_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Inventory_Stock",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StockQuantity = table.Column<int>(nullable: false),
                    LockedStockQuantity = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    WarehouseId = table.Column<int>(nullable: false),
                    DisplayOrder = table.Column<int>(nullable: false),
                    IsEnabled = table.Column<bool>(nullable: false),
                    Note = table.Column<string>(maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory_Stock", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inventory_Stock_Catalog_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Catalog_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Inventory_Stock_Inventory_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Inventory_Warehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Catalog_ProductPriceHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<int>(nullable: true),
                    Price = table.Column<decimal>(nullable: true),
                    OldPrice = table.Column<decimal>(nullable: true),
                    SpecialPrice = table.Column<decimal>(nullable: true),
                    SpecialPriceStart = table.Column<DateTime>(nullable: true),
                    SpecialPriceEnd = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog_ProductPriceHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Catalog_ProductPriceHistory_Catalog_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Catalog_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Catalog_ProductRecentlyViewed",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<int>(nullable: false),
                    CustomerId = table.Column<int>(nullable: false),
                    ViewedCount = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    LatestViewedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog_ProductRecentlyViewed", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Catalog_ProductRecentlyViewed_Catalog_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Catalog_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Catalog_ProductWishlist",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<int>(nullable: false),
                    CustomerId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog_ProductWishlist", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Catalog_ProductWishlist_Catalog_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Catalog_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Inventory_StockHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<int>(nullable: false),
                    WarehouseId = table.Column<int>(nullable: false),
                    AdjustedQuantity = table.Column<int>(nullable: false),
                    StockQuantity = table.Column<int>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory_StockHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inventory_StockHistory_Catalog_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Catalog_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Inventory_StockHistory_Inventory_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Inventory_Warehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Orders_OrderItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OrderId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    ProductPrice = table.Column<decimal>(nullable: false),
                    ProductName = table.Column<string>(nullable: true),
                    ProductMediaUrl = table.Column<string>(nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    ShippedQuantity = table.Column<int>(nullable: false),
                    DiscountAmount = table.Column<decimal>(nullable: false),
                    ItemAmount = table.Column<decimal>(nullable: false),
                    ItemWeight = table.Column<decimal>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders_OrderItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_OrderItem_Catalog_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Catalog_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Shipments_ShipmentItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ShipmentId = table.Column<int>(nullable: false),
                    OrderItemId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipments_ShipmentItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shipments_ShipmentItem_Orders_OrderItem_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "Orders_OrderItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Shipments_ShipmentItem_Catalog_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Catalog_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingCart_CartItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    CartId = table.Column<int>(nullable: false),
                    IsChecked = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCart_CartItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShoppingCart_CartItem_Catalog_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Catalog_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Core_UserAddress",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    AddressId = table.Column<int>(nullable: false),
                    AddressType = table.Column<int>(nullable: false),
                    LastUsedOn = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_UserAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Core_UserAddress_Core_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Core_Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Core_User",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    UserGuid = table.Column<Guid>(nullable: false),
                    FullName = table.Column<string>(maxLength: 450, nullable: false),
                    DefaultShippingAddressId = table.Column<int>(nullable: true),
                    DefaultBillingAddressId = table.Column<int>(nullable: true),
                    RefreshTokenHash = table.Column<string>(nullable: true),
                    Culture = table.Column<string>(nullable: true),
                    ExtensionData = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    LastIpAddress = table.Column<string>(maxLength: 450, nullable: true),
                    LastLoginOn = table.Column<DateTime>(nullable: true),
                    LastActivityOn = table.Column<DateTime>(nullable: true),
                    AvatarUrl = table.Column<string>(maxLength: 450, nullable: true),
                    AvatarId = table.Column<int>(nullable: true),
                    AdminRemark = table.Column<string>(maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Core_User_Core_Media_AvatarId",
                        column: x => x.AvatarId,
                        principalTable: "Core_Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Core_User_Core_UserAddress_DefaultBillingAddressId",
                        column: x => x.DefaultBillingAddressId,
                        principalTable: "Core_UserAddress",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Core_User_Core_UserAddress_DefaultShippingAddressId",
                        column: x => x.DefaultShippingAddressId,
                        principalTable: "Core_UserAddress",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Core_UserClaim",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_UserClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Core_UserClaim_Core_User_UserId",
                        column: x => x.UserId,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Core_UserLogin",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    UnionId = table.Column<string>(maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_UserLogin", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_Core_UserLogin_Core_User_UserId",
                        column: x => x.UserId,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Core_UserRole",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_UserRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_Core_UserRole_Core_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Core_Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Core_UserRole_Core_User_UserId",
                        column: x => x.UserId,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Core_UserToken",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_UserToken", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_Core_UserToken_Core_User_UserId",
                        column: x => x.UserId,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Feedbacks_Feedback",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: true),
                    Contact = table.Column<string>(nullable: true),
                    Title = table.Column<string>(maxLength: 450, nullable: true),
                    Content = table.Column<string>(maxLength: 450, nullable: true),
                    Type = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks_Feedback", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Feedback_Core_User_UserId",
                        column: x => x.UserId,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reviews_Review",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    Title = table.Column<string>(maxLength: 450, nullable: true),
                    Comment = table.Column<string>(maxLength: 450, nullable: true),
                    Rating = table.Column<int>(nullable: false),
                    ReviewerName = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    EntityTypeId = table.Column<int>(nullable: false),
                    EntityId = table.Column<int>(nullable: false),
                    SourceId = table.Column<int>(nullable: true),
                    SourceType = table.Column<int>(nullable: true),
                    IsAnonymous = table.Column<bool>(nullable: false),
                    SupportCount = table.Column<int>(nullable: false),
                    IsSystem = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews_Review", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Review_Core_User_UserId",
                        column: x => x.UserId,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingCart_Cart",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CustomerId = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    CouponCode = table.Column<string>(nullable: true),
                    CouponRuleName = table.Column<string>(nullable: true),
                    ShippingMethod = table.Column<string>(nullable: true),
                    IsProductPriceIncludeTax = table.Column<bool>(nullable: false),
                    ShippingAmount = table.Column<decimal>(nullable: true),
                    ShippingData = table.Column<string>(nullable: true),
                    OrderNote = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCart_Cart", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShoppingCart_Cart_Core_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShoppingCart_Cart_Core_User_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShoppingCart_Cart_Core_User_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reviews_Reply",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ParentId = table.Column<int>(nullable: true),
                    ReviewId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(maxLength: 450, nullable: true),
                    ReplierName = table.Column<string>(maxLength: 450, nullable: true),
                    ToUserId = table.Column<int>(nullable: true),
                    ToUserName = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    IsAnonymous = table.Column<bool>(nullable: false),
                    SupportCount = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews_Reply", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Reply_Reviews_Reply_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Reviews_Reply",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_Reply_Reviews_Review_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "Reviews_Review",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_Reply_Core_User_ToUserId",
                        column: x => x.ToUserId,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_Reply_Core_User_UserId",
                        column: x => x.UserId,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reviews_ReviewMedia",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ReviewId = table.Column<int>(nullable: false),
                    MediaId = table.Column<int>(nullable: false),
                    DisplayOrder = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews_ReviewMedia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_ReviewMedia_Core_Media_MediaId",
                        column: x => x.MediaId,
                        principalTable: "Core_Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_ReviewMedia_Reviews_Review_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "Reviews_Review",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reviews_Support",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    EntityTypeId = table.Column<int>(nullable: false),
                    EntityId = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    ReviewId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews_Support", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Support_Reviews_Review_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "Reviews_Review",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_Support_Core_User_UserId",
                        column: x => x.UserId,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Orders_OrderAddress",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OrderId = table.Column<int>(nullable: false),
                    ContactName = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    AddressLine1 = table.Column<string>(nullable: true),
                    AddressLine2 = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    ZipCode = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Company = table.Column<string>(nullable: true),
                    AddressType = table.Column<int>(nullable: false),
                    StateOrProvinceId = table.Column<int>(nullable: false),
                    CountryId = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders_OrderAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_OrderAddress_Core_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Core_Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_OrderAddress_Core_StateOrProvince_StateOrProvinceId",
                        column: x => x.StateOrProvinceId,
                        principalTable: "Core_StateOrProvince",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Orders_Order",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    No = table.Column<long>(nullable: false),
                    CustomerId = table.Column<int>(nullable: false),
                    ShippingAddressId = table.Column<int>(nullable: true),
                    BillingAddressId = table.Column<int>(nullable: true),
                    OrderStatus = table.Column<int>(nullable: false),
                    PaymentType = table.Column<int>(nullable: false),
                    ShippingStatus = table.Column<int>(nullable: true),
                    ShippedOn = table.Column<DateTime>(nullable: true),
                    DeliveredOn = table.Column<DateTime>(nullable: true),
                    DeliveredEndOn = table.Column<DateTime>(nullable: true),
                    RefundStatus = table.Column<int>(nullable: true),
                    RefundReason = table.Column<string>(nullable: true),
                    RefundOn = table.Column<DateTime>(nullable: true),
                    RefundAmount = table.Column<decimal>(nullable: false),
                    ShippingMethod = table.Column<int>(nullable: false),
                    ShippingFeeAmount = table.Column<decimal>(nullable: false),
                    PaymentMethod = table.Column<int>(nullable: true),
                    PaymentFeeAmount = table.Column<decimal>(nullable: false),
                    PaymentOn = table.Column<DateTime>(nullable: true),
                    PaymentEndOn = table.Column<DateTime>(nullable: true),
                    CouponCode = table.Column<string>(nullable: true),
                    CouponRuleName = table.Column<string>(nullable: true),
                    SubTotal = table.Column<decimal>(nullable: false),
                    SubTotalWithDiscount = table.Column<decimal>(nullable: false),
                    OrderTotal = table.Column<decimal>(nullable: false),
                    DiscountAmount = table.Column<decimal>(nullable: false),
                    OrderNote = table.Column<string>(maxLength: 450, nullable: true),
                    AdminNote = table.Column<string>(maxLength: 450, nullable: true),
                    CancelReason = table.Column<string>(maxLength: 450, nullable: true),
                    OnHoldReason = table.Column<string>(maxLength: 450, nullable: true),
                    CancelOn = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders_Order", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Order_Orders_OrderAddress_BillingAddressId",
                        column: x => x.BillingAddressId,
                        principalTable: "Orders_OrderAddress",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Order_Core_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Order_Core_User_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Order_Orders_OrderAddress_ShippingAddressId",
                        column: x => x.ShippingAddressId,
                        principalTable: "Orders_OrderAddress",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Order_Core_User_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Orders_OrderHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OrderId = table.Column<int>(nullable: false),
                    OldStatus = table.Column<int>(nullable: true),
                    NewStatus = table.Column<int>(nullable: false),
                    OrderSnapshot = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders_OrderHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_OrderHistory_Core_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_OrderHistory_Orders_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders_Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_OrderHistory_Core_User_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Shipments_Shipment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OrderId = table.Column<int>(nullable: false),
                    TotalWeight = table.Column<decimal>(nullable: false),
                    TrackingNumber = table.Column<string>(maxLength: 450, nullable: true),
                    ShippedOn = table.Column<DateTime>(nullable: true),
                    DeliveredOn = table.Column<DateTime>(nullable: true),
                    AdminComment = table.Column<string>(nullable: true),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipments_Shipment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shipments_Shipment_Core_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Shipments_Shipment_Orders_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders_Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Shipments_Shipment_Core_User_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Catalog_ProductOption",
                columns: new[] { "Id", "CreatedOn", "DisplayType", "IsDeleted", "Name", "UpdatedOn" },
                values: new object[,]
                {
                    { 1, new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), 1, false, "Color", new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Local) },
                    { 2, new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), 0, false, "Size", new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Local) }
                });

            migrationBuilder.InsertData(
                table: "Core_AppSetting",
                columns: new[] { "Id", "FormatType", "IsVisibleInCommonSettingPage", "Module", "Note", "Type", "Value" },
                values: new object[,]
                {
                    { "Global.AssetVersion", 0, false, "Core", null, null, "1.0" },
                    { "Catalog.ProductPageSize", 0, true, "Catalog", null, null, "10" },
                    { "OrderAutoCanceledTimeForMinute", 0, true, "Orders", "订单下单后超时自动取消订单时间（单位：分钟）", "System.Int32", "120" },
                    { "OrderAutoCompleteTimeForMinute", 0, true, "Orders", "订单支付后超时自动完成订单时间（买家未在指定的时间内确认收货,则系统自动确认收货完成订单，单位：分钟）", "System.Int32", "10080" },
                    { "OrderCompleteAutoReviewTimeForMinute", 0, true, "Orders", "订单完成后超时自动好评时间（买家未在指定的时间内评价,则系统自动好评，单位：分钟）", "System.Int32", "10080" },
                    { "IsReviewAutoApproved", 0, true, "Reviews", "启用评论自动审核功能", "System.Boolean", "false" },
                    { "IsReplyAutoApproved", 0, true, "Reviews", "启用回复自动审核功能", "System.Boolean", "true" }
                });

            migrationBuilder.InsertData(
                table: "Core_Country",
                columns: new[] { "Id", "CreatedOn", "DisplayOrder", "IsBillingEnabled", "IsCityEnabled", "IsDeleted", "IsDistrictEnabled", "IsPublished", "IsShippingEnabled", "Name", "NumericIsoCode", "ThreeLetterIsoCode", "TwoLetterIsoCode", "UpdatedOn" },
                values: new object[,]
                {
                    { 1, new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), 0, true, true, false, false, false, true, "China", 156, "CHN", "CN", new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Local) },
                    { 2, new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), 0, false, true, false, true, false, false, "United States", 840, "USA", "US", new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Local) }
                });

            migrationBuilder.InsertData(
                table: "Core_EntityType",
                columns: new[] { "Id", "IsMenuable", "Module", "Name" },
                values: new object[,]
                {
                    { 11, false, "Reviews", "Reply" },
                    { 3, false, "Catalog", "Product" },
                    { 10, false, "Reviews", "Review" },
                    { 1, true, "Catalog", "Category" },
                    { 2, true, "Catalog", "Brand" }
                });

            migrationBuilder.InsertData(
                table: "Core_Role",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 1, "4776a1b2-dbe4-4056-82ec-8bed211d1454", "admin", "ADMIN" },
                    { 2, "00d172be-03a0-4856-8b12-26d63fcf4374", "customer", "CUSTOMER" },
                    { 3, "d4754388-8355-4018-b728-218018836817", "guest", "GUEST" }
                });

            migrationBuilder.InsertData(
                table: "Core_User",
                columns: new[] { "Id", "AccessFailedCount", "AdminRemark", "AvatarId", "AvatarUrl", "ConcurrencyStamp", "CreatedOn", "Culture", "DefaultBillingAddressId", "DefaultShippingAddressId", "Email", "EmailConfirmed", "ExtensionData", "FullName", "IsActive", "IsDeleted", "LastActivityOn", "LastIpAddress", "LastLoginOn", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RefreshTokenHash", "SecurityStamp", "TwoFactorEnabled", "UpdatedOn", "UserGuid", "UserName" },
                values: new object[,]
                {
                    { 1, 0, null, null, null, "101cd6ae-a8ef-4a37-97fd-04ac2dd630e4", new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), null, null, null, "system@trueai.org", false, null, "System User", true, false, null, null, null, true, null, "SYSTEM@TRUEAI.ORG", "SYSTEM", "AQAAAAEAACcQAAAAEH9OxxtYF2U/mdHwU0b18pjw1UfBbIFY5wrkLS092drNBcKFM99LmxcBUAv3+CTjNQ==", null, false, null, "B3IACJI4YP56GU7Y2Y3TJ63ISYLCQM6W", false, new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), new Guid("5f72f83b-7436-4221-869c-1b69b2e23aae"), "system" },
                    { 10, 0, null, null, null, "c83afcbc-312c-4589-bad7-8686bd4754c0", new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), null, null, null, "admin@trueai.org", false, null, "Shop Admin", true, false, null, null, null, true, null, "ADMIN@TRUEAI.ORG", "ADMIN", "AQAAAAEAACcQAAAAECWSPYhRBTHbETKsda3xnUUxDaH++2r3TmeHcn/agfSEOMmQFGhRdyKGlg3qJL+1HA==", null, false, null, "d6847450-47f0-4c7a-9fed-0c66234bf61f", false, new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), new Guid("ed8210c3-24b0-4823-a744-80078cf12eb4"), "admin" }
                });

            migrationBuilder.InsertData(
                table: "Core_Widget",
                columns: new[] { "Id", "CreateUrl", "CreatedOn", "EditUrl", "IsPublished", "Name", "ViewComponentName" },
                values: new object[,]
                {
                    { 1, "widget-category-create", new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "widget-category-edit", true, "Category Widget", "CategoryWidget" },
                    { 2, "widget-product-create", new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "widget-product-edit", true, "Product Widget", "ProductWidget" },
                    { 3, "widget-simple-product-create", new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "widget-simple-product-edit", true, "Simple Product Widget", "SimpleProductWidget" },
                    { 4, "widget-html-create", new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "widget-html-edit", true, "Html Widget", "HtmlWidget" },
                    { 5, "widget-carousel-create", new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "widget-carousel-edit", true, "Carousel Widget", "CarouselWidget" },
                    { 7, "widget-recently-viewed-create", new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "widget-recently-viewed-edit", true, "Recently Viewed Widget", "RecentlyViewedWidget" }
                });

            migrationBuilder.InsertData(
                table: "Core_WidgetZone",
                columns: new[] { "Id", "CreatedOn", "Description", "Name" },
                values: new object[,]
                {
                    { 2, new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), null, "Home Main Content" },
                    { 1, new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), null, "Home Featured" },
                    { 3, new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), null, "Home After Main Content" }
                });

            migrationBuilder.InsertData(
                table: "Core_UserRole",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { 1, 1 });

            migrationBuilder.InsertData(
                table: "Core_UserRole",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { 10, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_Category_MediaId",
                table: "Catalog_Category",
                column: "MediaId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_Category_ParentId",
                table: "Catalog_Category",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_Product_BrandId",
                table: "Catalog_Product",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_Product_CreatedById",
                table: "Catalog_Product",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_Product_FreightTemplateId",
                table: "Catalog_Product",
                column: "FreightTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_Product_IsDeleted",
                table: "Catalog_Product",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_Product_IsPublished",
                table: "Catalog_Product",
                column: "IsPublished");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_Product_Name",
                table: "Catalog_Product",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_Product_ParentGroupedProductId",
                table: "Catalog_Product",
                column: "ParentGroupedProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_Product_Sku",
                table: "Catalog_Product",
                column: "Sku");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_Product_Slug",
                table: "Catalog_Product",
                column: "Slug");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_Product_ThumbnailImageId",
                table: "Catalog_Product",
                column: "ThumbnailImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_Product_UnitId",
                table: "Catalog_Product",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_Product_UpdatedById",
                table: "Catalog_Product",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductAttribute_GroupId",
                table: "Catalog_ProductAttribute",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductAttributeData_AttributeId",
                table: "Catalog_ProductAttributeData",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductAttributeTemplateRelation_AttributeId",
                table: "Catalog_ProductAttributeTemplateRelation",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductAttributeTemplateRelation_TemplateId",
                table: "Catalog_ProductAttributeTemplateRelation",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductAttributeValue_AttributeId",
                table: "Catalog_ProductAttributeValue",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductAttributeValue_ProductId",
                table: "Catalog_ProductAttributeValue",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductCategory_CategoryId",
                table: "Catalog_ProductCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductCategory_ProductId",
                table: "Catalog_ProductCategory",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductLink_LinkedProductId",
                table: "Catalog_ProductLink",
                column: "LinkedProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductLink_ProductId",
                table: "Catalog_ProductLink",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductMedia_MediaId",
                table: "Catalog_ProductMedia",
                column: "MediaId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductMedia_ProductId",
                table: "Catalog_ProductMedia",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductOptionCombination_OptionId",
                table: "Catalog_ProductOptionCombination",
                column: "OptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductOptionCombination_ProductId",
                table: "Catalog_ProductOptionCombination",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductOptionData_OptionId",
                table: "Catalog_ProductOptionData",
                column: "OptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductOptionValue_MediaId",
                table: "Catalog_ProductOptionValue",
                column: "MediaId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductOptionValue_OptionId",
                table: "Catalog_ProductOptionValue",
                column: "OptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductOptionValue_ProductId",
                table: "Catalog_ProductOptionValue",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductPriceHistory_CreatedById",
                table: "Catalog_ProductPriceHistory",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductPriceHistory_ProductId",
                table: "Catalog_ProductPriceHistory",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductPriceHistory_UpdatedById",
                table: "Catalog_ProductPriceHistory",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductRecentlyViewed_CustomerId",
                table: "Catalog_ProductRecentlyViewed",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductRecentlyViewed_ProductId",
                table: "Catalog_ProductRecentlyViewed",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductWishlist_CustomerId",
                table: "Catalog_ProductWishlist",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductWishlist_ProductId",
                table: "Catalog_ProductWishlist",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_Address_CountryId",
                table: "Core_Address",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_Address_StateOrProvinceId",
                table: "Core_Address",
                column: "StateOrProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_Entity_EntityTypeId",
                table: "Core_Entity",
                column: "EntityTypeId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Core_Role",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Core_RoleClaim_RoleId",
                table: "Core_RoleClaim",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_SmsSend_IsSucceed",
                table: "Core_SmsSend",
                column: "IsSucceed");

            migrationBuilder.CreateIndex(
                name: "IX_Core_SmsSend_IsUsed",
                table: "Core_SmsSend",
                column: "IsUsed");

            migrationBuilder.CreateIndex(
                name: "IX_Core_SmsSend_PhoneNumber",
                table: "Core_SmsSend",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Core_StateOrProvince_CountryId",
                table: "Core_StateOrProvince",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_StateOrProvince_ParentId",
                table: "Core_StateOrProvince",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_User_AvatarId",
                table: "Core_User",
                column: "AvatarId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_User_DefaultBillingAddressId",
                table: "Core_User",
                column: "DefaultBillingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_User_DefaultShippingAddressId",
                table: "Core_User",
                column: "DefaultShippingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_User_Email",
                table: "Core_User",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Core_User",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Core_User",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Core_User_PhoneNumber",
                table: "Core_User",
                column: "PhoneNumber",
                unique: true,
                filter: "[PhoneNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Core_User_UserName",
                table: "Core_User",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Core_UserAddress_AddressId",
                table: "Core_UserAddress",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_UserAddress_UserId",
                table: "Core_UserAddress",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_UserClaim_UserId",
                table: "Core_UserClaim",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_UserLogin_UserId",
                table: "Core_UserLogin",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_UserRole_RoleId",
                table: "Core_UserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_WidgetInstance_WidgetId",
                table: "Core_WidgetInstance",
                column: "WidgetId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_WidgetInstance_WidgetZoneId",
                table: "Core_WidgetInstance",
                column: "WidgetZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_Feedback_UserId",
                table: "Feedbacks_Feedback",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_Stock_ProductId",
                table: "Inventory_Stock",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_Stock_WarehouseId",
                table: "Inventory_Stock",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_StockHistory_CreatedById",
                table: "Inventory_StockHistory",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_StockHistory_ProductId",
                table: "Inventory_StockHistory",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_StockHistory_UpdatedById",
                table: "Inventory_StockHistory",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_StockHistory_WarehouseId",
                table: "Inventory_StockHistory",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_Warehouse_AddressId",
                table: "Inventory_Warehouse",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Order_BillingAddressId",
                table: "Orders_Order",
                column: "BillingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Order_CreatedById",
                table: "Orders_Order",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Order_CustomerId",
                table: "Orders_Order",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Order_No",
                table: "Orders_Order",
                column: "No",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Order_ShippingAddressId",
                table: "Orders_Order",
                column: "ShippingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Order_UpdatedById",
                table: "Orders_Order",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderAddress_CountryId",
                table: "Orders_OrderAddress",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderAddress_OrderId",
                table: "Orders_OrderAddress",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderAddress_StateOrProvinceId",
                table: "Orders_OrderAddress",
                column: "StateOrProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderHistory_CreatedById",
                table: "Orders_OrderHistory",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderHistory_OrderId",
                table: "Orders_OrderHistory",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderHistory_UpdatedById",
                table: "Orders_OrderHistory",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderItem_CreatedById",
                table: "Orders_OrderItem",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderItem_OrderId",
                table: "Orders_OrderItem",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderItem_ProductId",
                table: "Orders_OrderItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderItem_UpdatedById",
                table: "Orders_OrderItem",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_Reply_ParentId",
                table: "Reviews_Reply",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_Reply_ReviewId",
                table: "Reviews_Reply",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_Reply_ToUserId",
                table: "Reviews_Reply",
                column: "ToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_Reply_UserId",
                table: "Reviews_Reply",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_Review_UserId",
                table: "Reviews_Review",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ReviewMedia_MediaId",
                table: "Reviews_ReviewMedia",
                column: "MediaId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ReviewMedia_ReviewId",
                table: "Reviews_ReviewMedia",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_Support_ReviewId",
                table: "Reviews_Support",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_Support_UserId",
                table: "Reviews_Support",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_Shipment_CreatedById",
                table: "Shipments_Shipment",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_Shipment_IsDeleted",
                table: "Shipments_Shipment",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_Shipment_OrderId",
                table: "Shipments_Shipment",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_Shipment_TrackingNumber",
                table: "Shipments_Shipment",
                column: "TrackingNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_Shipment_UpdatedById",
                table: "Shipments_Shipment",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_ShipmentItem_CreatedById",
                table: "Shipments_ShipmentItem",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_ShipmentItem_OrderItemId",
                table: "Shipments_ShipmentItem",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_ShipmentItem_ProductId",
                table: "Shipments_ShipmentItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_ShipmentItem_ShipmentId",
                table: "Shipments_ShipmentItem",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_ShipmentItem_UpdatedById",
                table: "Shipments_ShipmentItem",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Shipping_PriceAndDestination_CountryId",
                table: "Shipping_PriceAndDestination",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipping_PriceAndDestination_FreightTemplateId",
                table: "Shipping_PriceAndDestination",
                column: "FreightTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipping_PriceAndDestination_StateOrProvinceId",
                table: "Shipping_PriceAndDestination",
                column: "StateOrProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCart_Cart_CreatedById",
                table: "ShoppingCart_Cart",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCart_Cart_CustomerId",
                table: "ShoppingCart_Cart",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCart_Cart_UpdatedById",
                table: "ShoppingCart_Cart",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCart_CartItem_CartId",
                table: "ShoppingCart_CartItem",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCart_CartItem_CreatedById",
                table: "ShoppingCart_CartItem",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCart_CartItem_ProductId",
                table: "ShoppingCart_CartItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCart_CartItem_UpdatedById",
                table: "ShoppingCart_CartItem",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Catalog_Product_Core_User_CreatedById",
                table: "Catalog_Product",
                column: "CreatedById",
                principalTable: "Core_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Catalog_Product_Core_User_UpdatedById",
                table: "Catalog_Product",
                column: "UpdatedById",
                principalTable: "Core_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Catalog_ProductPriceHistory_Core_User_CreatedById",
                table: "Catalog_ProductPriceHistory",
                column: "CreatedById",
                principalTable: "Core_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Catalog_ProductPriceHistory_Core_User_UpdatedById",
                table: "Catalog_ProductPriceHistory",
                column: "UpdatedById",
                principalTable: "Core_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Catalog_ProductRecentlyViewed_Core_User_CustomerId",
                table: "Catalog_ProductRecentlyViewed",
                column: "CustomerId",
                principalTable: "Core_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Catalog_ProductWishlist_Core_User_CustomerId",
                table: "Catalog_ProductWishlist",
                column: "CustomerId",
                principalTable: "Core_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Inventory_StockHistory_Core_User_CreatedById",
                table: "Inventory_StockHistory",
                column: "CreatedById",
                principalTable: "Core_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Inventory_StockHistory_Core_User_UpdatedById",
                table: "Inventory_StockHistory",
                column: "UpdatedById",
                principalTable: "Core_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderItem_Core_User_CreatedById",
                table: "Orders_OrderItem",
                column: "CreatedById",
                principalTable: "Core_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderItem_Core_User_UpdatedById",
                table: "Orders_OrderItem",
                column: "UpdatedById",
                principalTable: "Core_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderItem_Orders_Order_OrderId",
                table: "Orders_OrderItem",
                column: "OrderId",
                principalTable: "Orders_Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shipments_ShipmentItem_Core_User_CreatedById",
                table: "Shipments_ShipmentItem",
                column: "CreatedById",
                principalTable: "Core_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shipments_ShipmentItem_Core_User_UpdatedById",
                table: "Shipments_ShipmentItem",
                column: "UpdatedById",
                principalTable: "Core_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shipments_ShipmentItem_Shipments_Shipment_ShipmentId",
                table: "Shipments_ShipmentItem",
                column: "ShipmentId",
                principalTable: "Shipments_Shipment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCart_CartItem_Core_User_CreatedById",
                table: "ShoppingCart_CartItem",
                column: "CreatedById",
                principalTable: "Core_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCart_CartItem_Core_User_UpdatedById",
                table: "ShoppingCart_CartItem",
                column: "UpdatedById",
                principalTable: "Core_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCart_CartItem_ShoppingCart_Cart_CartId",
                table: "ShoppingCart_CartItem",
                column: "CartId",
                principalTable: "ShoppingCart_Cart",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Core_UserAddress_Core_User_UserId",
                table: "Core_UserAddress",
                column: "UserId",
                principalTable: "Core_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderAddress_Orders_Order_OrderId",
                table: "Orders_OrderAddress",
                column: "OrderId",
                principalTable: "Orders_Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Core_User_Core_Media_AvatarId",
                table: "Core_User");

            migrationBuilder.DropForeignKey(
                name: "FK_Core_UserAddress_Core_User_UserId",
                table: "Core_UserAddress");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Order_Core_User_CreatedById",
                table: "Orders_Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Order_Core_User_CustomerId",
                table: "Orders_Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Order_Core_User_UpdatedById",
                table: "Orders_Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Core_StateOrProvince_Core_Country_CountryId",
                table: "Core_StateOrProvince");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrderAddress_Core_Country_CountryId",
                table: "Orders_OrderAddress");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrderAddress_Core_StateOrProvince_StateOrProvinceId",
                table: "Orders_OrderAddress");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Order_Orders_OrderAddress_BillingAddressId",
                table: "Orders_Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Order_Orders_OrderAddress_ShippingAddressId",
                table: "Orders_Order");

            migrationBuilder.DropTable(
                name: "Catalog_ProductAttributeData");

            migrationBuilder.DropTable(
                name: "Catalog_ProductAttributeTemplateRelation");

            migrationBuilder.DropTable(
                name: "Catalog_ProductAttributeValue");

            migrationBuilder.DropTable(
                name: "Catalog_ProductCategory");

            migrationBuilder.DropTable(
                name: "Catalog_ProductLink");

            migrationBuilder.DropTable(
                name: "Catalog_ProductMedia");

            migrationBuilder.DropTable(
                name: "Catalog_ProductOptionCombination");

            migrationBuilder.DropTable(
                name: "Catalog_ProductOptionData");

            migrationBuilder.DropTable(
                name: "Catalog_ProductOptionValue");

            migrationBuilder.DropTable(
                name: "Catalog_ProductPriceHistory");

            migrationBuilder.DropTable(
                name: "Catalog_ProductRecentlyViewed");

            migrationBuilder.DropTable(
                name: "Catalog_ProductWishlist");

            migrationBuilder.DropTable(
                name: "Core_AppSetting");

            migrationBuilder.DropTable(
                name: "Core_EmailSend");

            migrationBuilder.DropTable(
                name: "Core_Entity");

            migrationBuilder.DropTable(
                name: "Core_RoleClaim");

            migrationBuilder.DropTable(
                name: "Core_SmsSend");

            migrationBuilder.DropTable(
                name: "Core_UserClaim");

            migrationBuilder.DropTable(
                name: "Core_UserLogin");

            migrationBuilder.DropTable(
                name: "Core_UserRole");

            migrationBuilder.DropTable(
                name: "Core_UserToken");

            migrationBuilder.DropTable(
                name: "Core_WidgetInstance");

            migrationBuilder.DropTable(
                name: "Feedbacks_Feedback");

            migrationBuilder.DropTable(
                name: "Inventory_Stock");

            migrationBuilder.DropTable(
                name: "Inventory_StockHistory");

            migrationBuilder.DropTable(
                name: "Orders_OrderHistory");

            migrationBuilder.DropTable(
                name: "Reviews_Reply");

            migrationBuilder.DropTable(
                name: "Reviews_ReviewMedia");

            migrationBuilder.DropTable(
                name: "Reviews_Support");

            migrationBuilder.DropTable(
                name: "Shipments_ShipmentItem");

            migrationBuilder.DropTable(
                name: "Shipping_PriceAndDestination");

            migrationBuilder.DropTable(
                name: "ShoppingCart_CartItem");

            migrationBuilder.DropTable(
                name: "Catalog_ProductAttributeTemplate");

            migrationBuilder.DropTable(
                name: "Catalog_ProductAttribute");

            migrationBuilder.DropTable(
                name: "Catalog_Category");

            migrationBuilder.DropTable(
                name: "Catalog_ProductOption");

            migrationBuilder.DropTable(
                name: "Core_EntityType");

            migrationBuilder.DropTable(
                name: "Core_Role");

            migrationBuilder.DropTable(
                name: "Core_Widget");

            migrationBuilder.DropTable(
                name: "Core_WidgetZone");

            migrationBuilder.DropTable(
                name: "Inventory_Warehouse");

            migrationBuilder.DropTable(
                name: "Reviews_Review");

            migrationBuilder.DropTable(
                name: "Orders_OrderItem");

            migrationBuilder.DropTable(
                name: "Shipments_Shipment");

            migrationBuilder.DropTable(
                name: "ShoppingCart_Cart");

            migrationBuilder.DropTable(
                name: "Catalog_ProductAttributeGroup");

            migrationBuilder.DropTable(
                name: "Catalog_Product");

            migrationBuilder.DropTable(
                name: "Catalog_Brand");

            migrationBuilder.DropTable(
                name: "Shipping_FreightTemplate");

            migrationBuilder.DropTable(
                name: "Catalog_Unit");

            migrationBuilder.DropTable(
                name: "Core_Media");

            migrationBuilder.DropTable(
                name: "Core_User");

            migrationBuilder.DropTable(
                name: "Core_UserAddress");

            migrationBuilder.DropTable(
                name: "Core_Address");

            migrationBuilder.DropTable(
                name: "Core_Country");

            migrationBuilder.DropTable(
                name: "Core_StateOrProvince");

            migrationBuilder.DropTable(
                name: "Orders_OrderAddress");

            migrationBuilder.DropTable(
                name: "Orders_Order");
        }
    }
}
