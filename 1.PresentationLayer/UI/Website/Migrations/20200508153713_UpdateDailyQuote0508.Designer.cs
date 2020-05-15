﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Website.Data;

namespace Website.Migrations
{
    [DbContext(typeof(LineWebhookContext))]
    [Migration("20200508153713_UpdateDailyQuote0508")]
    partial class UpdateDailyQuote0508
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Core.Domain.Entities.TWSE_Stock.Exchange.DailyQuote", b =>
                {
                    b.Property<DateTime>("Date")
                        .HasColumnType("date");

                    b.Property<string>("StockCode")
                        .HasColumnType("nvarchar(8)")
                        .HasMaxLength(8);

                    b.Property<float>("Change")
                        .HasColumnType("real");

                    b.Property<float>("ClosingPrice")
                        .HasColumnType("real");

                    b.Property<DateTime>("CreateDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("Direction")
                        .HasColumnType("int");

                    b.Property<float>("HighestPrice")
                        .HasColumnType("real");

                    b.Property<float>("LastBestAskPrice")
                        .HasColumnType("real");

                    b.Property<int>("LastBestAskVolume")
                        .HasColumnType("int");

                    b.Property<float>("LastBestBidPrice")
                        .HasColumnType("real");

                    b.Property<int>("LastBestBidVolume")
                        .HasColumnType("int");

                    b.Property<float>("LowestPrice")
                        .HasColumnType("real");

                    b.Property<float>("OpeningPrice")
                        .HasColumnType("real");

                    b.Property<float>("PriceEarningRatio")
                        .HasColumnType("real");

                    b.Property<int>("TradeValue")
                        .HasColumnType("int");

                    b.Property<int>("TradeVolume")
                        .HasColumnType("int");

                    b.Property<int>("Transaction")
                        .HasColumnType("int");

                    b.HasKey("Date", "StockCode");

                    b.ToTable("DailyQuotes");
                });
#pragma warning restore 612, 618
        }
    }
}
