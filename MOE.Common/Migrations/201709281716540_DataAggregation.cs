namespace MOE.Common.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataAggregation : DbMigration
    {
        public override void Up()
        {
            //Historical Configuration Migration
            DropForeignKey("dbo.ActionLogs", "SignalId", "dbo.Signals");
            DropForeignKey("dbo.ApproachRouteDetail", "ApproachID", "dbo.Approaches");
            DropForeignKey("dbo.SPMWatchDogErrorEvents", "SignalId", "dbo.Signals");
            DropForeignKey("dbo.MetricComments", "SignalId", "dbo.Signals");
            DropForeignKey("dbo.Approaches", "SignalId", "dbo.Signals");
            DropIndex("dbo.ActionLogs", new[] { "SignalId" });
            DropIndex("dbo.MetricComments", new[] { "SignalId" });
            DropIndex("dbo.Approaches", new[] { "SignalId" });
            DropIndex("dbo.ApproachRouteDetail", new[] { "ApproachID" });
            DropIndex("dbo.Detectors", "IX_DetectorIDUnique");
            DropIndex("dbo.SPMWatchDogErrorEvents", new[] { "SignalId" });
            DropPrimaryKey("Signals");

            CreateTable(
                "dbo.VersionActions",
                c => new
                {
                    ID = c.Int(nullable: false),
                    Description = c.String(),
                })
                .PrimaryKey(t => t.ID);

            Sql("Insert into VersionActions(ID, Description) values (1, 'New')");
            Sql("Insert into VersionActions(ID, Description) values (2, 'Edit')");
            Sql("Insert into VersionActions(ID, Description) values (3, 'Delete')");
            Sql("Insert into VersionActions(ID, Description) values (4, 'New Version')");
            Sql("Insert into VersionActions(ID, Description) values (10, 'Initial')");


            AddColumn("dbo.MetricComments", "VersionID", c => c.Int(nullable: false));
            AddColumn("dbo.Signals", "VersionID", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.Signals", "VersionActionId", c => c.Int(nullable: false, defaultValue:10));
            AddColumn("dbo.Signals", "Note", c => c.String(nullable: false, defaultValue: "Initial"));
            AddColumn("dbo.Signals", "Start", c => c.DateTime(nullable: false ));
            AddColumn("dbo.Approaches", "VersionID", c => c.Int(nullable: false));
            AlterColumn("dbo.ActionLogs", "SignalId", c => c.String(nullable: false));
            AlterColumn("dbo.MetricComments", "SignalId", c => c.String());
            AlterColumn("dbo.Approaches", "SignalId", c => c.String());
            AlterColumn("dbo.SPMWatchDogErrorEvents", "SignalId", c => c.String(nullable: false));
            AddPrimaryKey("dbo.Signals", "VersionID");
            CreateIndex("dbo.MetricComments", "VersionID");
            CreateIndex("dbo.Signals", "VersionActionId");
            CreateIndex("dbo.Approaches", "VersionID");
            //AddForeignKey("dbo.Signals", "VersionActionId", "dbo.VersionActions", "ID", cascadeDelete: true);
            //AddForeignKey("dbo.MetricComments", "VersionID", "dbo.Signals", "VersionID", cascadeDelete: true);
            //AddForeignKey("dbo.Approaches", "VersionID", "dbo.Signals", "VersionID", cascadeDelete: true);
            DropColumn("dbo.ApproachRouteDetail", "ApproachOrder");
            DropColumn("dbo.ApproachRouteDetail", "ApproachID");
            DropTable("dbo.SignalWithDetections");
            DropTable("dbo.ApproachRoute");
            DropTable("dbo.ApproachRouteDetail");
           
            //Original Data Aggragation Migration
            CreateTable(
                "dbo.ApproachCycleAggregations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BinStartTime = c.DateTime(nullable: false),
                        ApproachId = c.Int(nullable: false),
                        RedTime = c.Double(nullable: false),
                        YellowTime = c.Double(nullable: false),
                        GreenTime = c.Double(nullable: false),
                        TotalCycles = c.Int(nullable: false),
                        PedActuations = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Approaches", t => t.ApproachId, cascadeDelete: true)
                .Index(t => t.ApproachId);
            
            CreateTable(
                "dbo.ApproachPcdAggregations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BinStartTime = c.DateTime(nullable: false),
                        ApproachId = c.Int(nullable: false),
                        ArrivalsOnGreen = c.Int(nullable: false),
                        ArrivalsOnRed = c.Int(nullable: false),
                        ArrivalsOnYellow = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Approaches", t => t.ApproachId, cascadeDelete: true)
                .Index(t => t.ApproachId);
            
            CreateTable(
                "dbo.ApproachSpeedAggregations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BinStartTime = c.DateTime(nullable: false),
                        ApproachId = c.Int(nullable: false),
                        SummedSpeed = c.Double(nullable: false),
                        SpeedVolume = c.Double(nullable: false),
                        Speed85Th = c.Double(nullable: false),
                        Speed15Th = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Approaches", t => t.ApproachId, cascadeDelete: true)
                .Index(t => t.ApproachId);
            
            CreateTable(
                "dbo.ApproachSplitFailAggregations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BinStartTime = c.DateTime(nullable: false),
                        ApproachId = c.Int(nullable: false),
                        SplitFailures = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Approaches", t => t.ApproachId, cascadeDelete: true)
                .Index(t => t.ApproachId);
            
            CreateTable(
                "dbo.ApproachYellowRedActivationAggregations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BinStartTime = c.DateTime(nullable: false),
                        ApproachId = c.Int(nullable: false),
                        SevereRedLightViolations = c.Int(nullable: false),
                        TotalRedLightViolations = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Approaches", t => t.ApproachId, cascadeDelete: true)
                .Index(t => t.ApproachId);
            
            CreateTable(
                "dbo.DetectorAggregations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BinStartTime = c.DateTime(nullable: false),
                        DetectorId = c.String(nullable: false, maxLength: 10),
                        Volume = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Detectors", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.PreemptionAggregations",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        BinStartTime = c.DateTime(nullable: false),
                        SignalID = c.String(nullable: false, maxLength: 10),
                        PreemptNumber = c.Int(nullable: false),
                        PreemptRequests = c.Int(nullable: false),
                        PreemptServices = c.Int(nullable: false),
                        Signal_VersionID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Signals", t => t.Signal_VersionID)
                .Index(t => t.Signal_VersionID);
            
            CreateTable(
                "dbo.PriorityAggregations",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        BinStartTime = c.DateTime(nullable: false),
                        SignalID = c.String(nullable: false, maxLength: 10),
                        PriorityNumber = c.Int(nullable: false),
                        TotalCycles = c.Int(nullable: false),
                        PriorityRequests = c.Int(nullable: false),
                        PriorityServiceEarlyGreen = c.Int(nullable: false),
                        PriorityServiceExtendedGreen = c.Int(nullable: false),
                        Signal_VersionID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Signals", t => t.Signal_VersionID)
                .Index(t => t.Signal_VersionID);
            
            CreateTable(
                "dbo.SignalAggregations",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        BinStartTime = c.DateTime(nullable: false),
                        VersionlID = c.Int(nullable: false),
                        TotalCycles = c.Int(nullable: false),
                        AddCyclesInTransition = c.Int(nullable: false),
                        SubtractCyclesInTransition = c.Int(nullable: false),
                        DwellCyclesInTransition = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Signals", t => t.VersionlID, cascadeDelete: true)
                .Index(t => t.VersionlID);
            
            DropTable("dbo.Archived_Metrics");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Archived_Metrics",
                c => new
                    {
                        Timestamp = c.DateTime(nullable: false),
                        DetectorID = c.String(nullable: false, maxLength: 50, unicode: false),
                        BinSize = c.Int(nullable: false),
                        Volume = c.Int(),
                        speed = c.Int(),
                        delay = c.Int(),
                        AoR = c.Int(),
                        SpeedHits = c.Int(),
                        BinGreenTime = c.Int(),
                    })
                .PrimaryKey(t => new { t.Timestamp, t.DetectorID, t.BinSize });
            
            DropForeignKey("dbo.SignalAggregations", "VersionlID", "dbo.Signals");
            DropForeignKey("dbo.PriorityAggregations", "Signal_VersionID", "dbo.Signals");
            DropForeignKey("dbo.PreemptionAggregations", "Signal_VersionID", "dbo.Signals");
            DropForeignKey("dbo.DetectorAggregations", "Id", "dbo.Detectors");
            DropForeignKey("dbo.ApproachYellowRedActivationAggregations", "ApproachId", "dbo.Approaches");
            DropForeignKey("dbo.ApproachSplitFailAggregations", "ApproachId", "dbo.Approaches");
            DropForeignKey("dbo.ApproachSpeedAggregations", "ApproachId", "dbo.Approaches");
            DropForeignKey("dbo.ApproachPcdAggregations", "ApproachId", "dbo.Approaches");
            DropForeignKey("dbo.ApproachCycleAggregations", "ApproachId", "dbo.Approaches");
            DropIndex("dbo.SignalAggregations", new[] { "VersionlID" });
            DropIndex("dbo.PriorityAggregations", new[] { "Signal_VersionID" });
            DropIndex("dbo.PreemptionAggregations", new[] { "Signal_VersionID" });
            DropIndex("dbo.DetectorAggregations", new[] { "Id" });
            DropIndex("dbo.ApproachYellowRedActivationAggregations", new[] { "ApproachId" });
            DropIndex("dbo.ApproachSplitFailAggregations", new[] { "ApproachId" });
            DropIndex("dbo.ApproachSpeedAggregations", new[] { "ApproachId" });
            DropIndex("dbo.ApproachPcdAggregations", new[] { "ApproachId" });
            DropIndex("dbo.ApproachCycleAggregations", new[] { "ApproachId" });
            DropTable("dbo.SignalAggregations");
            DropTable("dbo.PriorityAggregations");
            DropTable("dbo.PreemptionAggregations");
            DropTable("dbo.DetectorAggregations");
            DropTable("dbo.ApproachYellowRedActivationAggregations");
            DropTable("dbo.ApproachSplitFailAggregations");
            DropTable("dbo.ApproachSpeedAggregations");
            DropTable("dbo.ApproachPcdAggregations");
            DropTable("dbo.ApproachCycleAggregations");

            //Historical Configuration Migration
            CreateTable(
                "dbo.SignalWithDetections",
                c => new
                {
                    SignalID = c.String(nullable: false, maxLength: 10),
                    DetectionTypeID = c.Int(nullable: false),
                    PrimaryName = c.String(),
                    Secondary_Name = c.String(),
                    Latitude = c.String(),
                    Longitude = c.String(),
                    Region = c.String(),
                })
                .PrimaryKey(t => new { t.SignalID, t.DetectionTypeID });

            AddColumn("dbo.ApproachRouteDetail", "ApproachID", c => c.Int(nullable: false));
            AddColumn("dbo.ApproachRouteDetail", "ApproachOrder", c => c.Int(nullable: false));
            DropForeignKey("dbo.Approaches", "VersionID", "dbo.Signals");
            DropForeignKey("dbo.MetricComments", "VersionID", "dbo.Signals");
            DropForeignKey("dbo.Signals", "VersionActionId", "dbo.VersionActions");
            DropForeignKey("dbo.ApproachRouteDetail", "DirectionType2_DirectionTypeID", "dbo.DirectionTypes");
            DropForeignKey("dbo.ApproachRouteDetail", "DirectionType1_DirectionTypeID", "dbo.DirectionTypes");
            DropIndex("dbo.Approaches", new[] { "VersionID" });
            DropIndex("dbo.Signals", new[] { "VersionActionId" });
            DropIndex("dbo.MetricComments", new[] { "VersionID" });
            DropIndex("dbo.ApproachRouteDetail", new[] { "DirectionType2_DirectionTypeID" });
            DropIndex("dbo.ApproachRouteDetail", new[] { "DirectionType1_DirectionTypeID" });
            AlterColumn("dbo.SPMWatchDogErrorEvents", "SignalId", c => c.String(nullable: false, maxLength: 10));
            AlterColumn("dbo.Approaches", "SignalId", c => c.String(nullable: false, maxLength: 10));
            AlterColumn("dbo.MetricComments", "SignalId", c => c.String(nullable: false, maxLength: 10));
            AlterColumn("dbo.ActionLogs", "SignalId", c => c.String(nullable: false, maxLength: 10));
            DropColumn("dbo.Approaches", "VersionID");
            DropColumn("dbo.Signals", "Start");
            DropColumn("dbo.Signals", "Note");
            DropColumn("dbo.Signals", "VersionActionId");
            DropColumn("dbo.Signals", "VersionID");
            DropColumn("dbo.MetricComments", "VersionID");
            DropTable("dbo.VersionActions");
            AddPrimaryKey("dbo.Signals", "SignalId");
            CreateIndex("dbo.SPMWatchDogErrorEvents", "SignalId");
            CreateIndex("dbo.Detectors", "DetectorID", unique: true, name: "IX_DetectorIDUnique");
            CreateIndex("dbo.ApproachRouteDetail", "ApproachID");
            CreateIndex("dbo.Approaches", "SignalId");
            CreateIndex("dbo.MetricComments", "SignalId");
            CreateIndex("dbo.ActionLogs", "SignalId");
            AddForeignKey("dbo.Approaches", "SignalId", "dbo.Signals", "SignalId", cascadeDelete: true);
            AddForeignKey("dbo.MetricComments", "SignalId", "dbo.Signals", "SignalId", cascadeDelete: true);
            AddForeignKey("dbo.SPMWatchDogErrorEvents", "SignalId", "dbo.Signals", "SignalId", cascadeDelete: true);
            AddForeignKey("dbo.ApproachRouteDetail", "ApproachID", "dbo.Approaches", "ApproachID", cascadeDelete: true);
            AddForeignKey("dbo.ActionLogs", "SignalId", "dbo.Signals", "SignalId");
        }
    }
}
