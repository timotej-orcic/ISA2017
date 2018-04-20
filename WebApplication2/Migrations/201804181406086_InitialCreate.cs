namespace WebApplication2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Points",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Points_Type = c.Int(nullable: false),
                        PointsCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FanZones",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Posts",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        ParentUserId = c.String(),
                        Description = c.String(),
                        OfferExpireDate = c.DateTime(nullable: false),
                        ImageUrl = c.String(),
                        IsTakenByAdmin = c.Boolean(nullable: false),
                        ParentAdminId = c.String(),
                        IsChecked = c.Boolean(nullable: false),
                        IsApproved = c.Boolean(nullable: false),
                        IsGraded = c.Boolean(nullable: false),
                        FanZone_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FanZones", t => t.FanZone_Id)
                .Index(t => t.FanZone_Id);
            
            CreateTable(
                "dbo.Licitations",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ParentPostId = c.String(),
                        OfferedPrice = c.Double(nullable: false),
                        ParentUserId = c.String(),
                        IsAccepted = c.Boolean(nullable: false),
                        Post_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Posts", t => t.Post_Id)
                .Index(t => t.Post_Id);
            
            CreateTable(
                "dbo.ThemeRequisits",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        ParentUserId = c.String(),
                        Description = c.String(),
                        ImageUrl = c.String(),
                        Price = c.Double(nullable: false),
                        AvailableCount = c.Int(nullable: false),
                        FanZone_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FanZones", t => t.FanZone_Id)
                .Index(t => t.FanZone_Id);
            
            CreateTable(
                "dbo.Halls",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        RowsCount = c.Int(nullable: false),
                        ColsCount = c.Int(nullable: false),
                        ParentLocation_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Locations", t => t.ParentLocation_Id)
                .Index(t => t.ParentLocation_Id);
            
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        LocType = c.Int(nullable: false),
                        Name = c.String(),
                        Address = c.String(),
                        Description = c.String(),
                        MyAdminId = c.String(),
                        City = c.String(),
                        AvgRating = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Tickets",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        SeatColumn = c.Int(nullable: false),
                        SeatRow = c.Int(nullable: false),
                        Price = c.Double(nullable: false),
                        DiscountMultiplier = c.Double(nullable: false),
                        Projection_Id = c.Guid(),
                        Location_Id = c.Guid(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.HallTimeProjections", t => t.Projection_Id)
                .ForeignKey("dbo.Locations", t => t.Location_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.Projection_Id)
                .Index(t => t.Location_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.HallTimeProjections",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Time = c.DateTime(nullable: false),
                        TicketPrice = c.Double(nullable: false),
                        Hall_Id = c.Guid(),
                        Projection_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Halls", t => t.Hall_Id)
                .ForeignKey("dbo.Projections", t => t.Projection_Id)
                .Index(t => t.Hall_Id)
                .Index(t => t.Projection_Id);
            
            CreateTable(
                "dbo.Projections",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        ActorsList = c.String(),
                        Genre = c.String(),
                        DirectorName = c.String(),
                        DurationTime = c.Int(nullable: false),
                        PosterUrl = c.String(),
                        AvgRating = c.Double(nullable: false),
                        Description = c.String(),
                        Location_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Locations", t => t.Location_Id)
                .Index(t => t.Location_Id);
            
            CreateTable(
                "dbo.Rows",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Seats = c.String(),
                        HallTimeProjection_Id = c.Guid(),
                        Hall_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.HallTimeProjections", t => t.HallTimeProjection_Id)
                .ForeignKey("dbo.Halls", t => t.Hall_Id)
                .Index(t => t.HallTimeProjection_Id)
                .Index(t => t.Hall_Id);
            
            CreateTable(
                "dbo.Recensions",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        RatingProjection = c.Double(nullable: false),
                        RatingLocation = c.Double(nullable: false),
                        location_Id = c.Guid(),
                        projection_Id = c.Guid(),
                        RecensionUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Locations", t => t.location_Id)
                .ForeignKey("dbo.Projections", t => t.projection_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.RecensionUser_Id)
                .Index(t => t.location_Id)
                .Index(t => t.projection_Id)
                .Index(t => t.RecensionUser_Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        LastName = c.String(),
                        Password = c.String(),
                        City = c.String(),
                        Points = c.Double(nullable: false),
                        UserType = c.Int(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                        HasSetPassword = c.Boolean(),
                        Admin_Type = c.Int(),
                        MyLocationId = c.String(),
                        IsMainAdmin = c.Boolean(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Requests",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Sender_id = c.Guid(nullable: false),
                        Sender_username = c.String(),
                        Receiver_id = c.Guid(nullable: false),
                        Receiver_username = c.String(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.ApplicationUserApplicationUsers",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        ApplicationUser_Id1 = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.ApplicationUser_Id1 })
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id1)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id1);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Rows", "Hall_Id", "dbo.Halls");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Tickets", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Requests", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Recensions", "RecensionUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ApplicationUserApplicationUsers", "ApplicationUser_Id1", "dbo.AspNetUsers");
            DropForeignKey("dbo.ApplicationUserApplicationUsers", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Recensions", "projection_Id", "dbo.Projections");
            DropForeignKey("dbo.Recensions", "location_Id", "dbo.Locations");
            DropForeignKey("dbo.Projections", "Location_Id", "dbo.Locations");
            DropForeignKey("dbo.Halls", "ParentLocation_Id", "dbo.Locations");
            DropForeignKey("dbo.Tickets", "Location_Id", "dbo.Locations");
            DropForeignKey("dbo.Tickets", "Projection_Id", "dbo.HallTimeProjections");
            DropForeignKey("dbo.Rows", "HallTimeProjection_Id", "dbo.HallTimeProjections");
            DropForeignKey("dbo.HallTimeProjections", "Projection_Id", "dbo.Projections");
            DropForeignKey("dbo.HallTimeProjections", "Hall_Id", "dbo.Halls");
            DropForeignKey("dbo.ThemeRequisits", "FanZone_Id", "dbo.FanZones");
            DropForeignKey("dbo.Posts", "FanZone_Id", "dbo.FanZones");
            DropForeignKey("dbo.Licitations", "Post_Id", "dbo.Posts");
            DropIndex("dbo.ApplicationUserApplicationUsers", new[] { "ApplicationUser_Id1" });
            DropIndex("dbo.ApplicationUserApplicationUsers", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.Requests", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Recensions", new[] { "RecensionUser_Id" });
            DropIndex("dbo.Recensions", new[] { "projection_Id" });
            DropIndex("dbo.Recensions", new[] { "location_Id" });
            DropIndex("dbo.Rows", new[] { "Hall_Id" });
            DropIndex("dbo.Rows", new[] { "HallTimeProjection_Id" });
            DropIndex("dbo.Projections", new[] { "Location_Id" });
            DropIndex("dbo.HallTimeProjections", new[] { "Projection_Id" });
            DropIndex("dbo.HallTimeProjections", new[] { "Hall_Id" });
            DropIndex("dbo.Tickets", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Tickets", new[] { "Location_Id" });
            DropIndex("dbo.Tickets", new[] { "Projection_Id" });
            DropIndex("dbo.Halls", new[] { "ParentLocation_Id" });
            DropIndex("dbo.ThemeRequisits", new[] { "FanZone_Id" });
            DropIndex("dbo.Licitations", new[] { "Post_Id" });
            DropIndex("dbo.Posts", new[] { "FanZone_Id" });
            DropTable("dbo.ApplicationUserApplicationUsers");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.Requests");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Recensions");
            DropTable("dbo.Rows");
            DropTable("dbo.Projections");
            DropTable("dbo.HallTimeProjections");
            DropTable("dbo.Tickets");
            DropTable("dbo.Locations");
            DropTable("dbo.Halls");
            DropTable("dbo.ThemeRequisits");
            DropTable("dbo.Licitations");
            DropTable("dbo.Posts");
            DropTable("dbo.FanZones");
            DropTable("dbo.Points");
        }
    }
}
