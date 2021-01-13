﻿// <auto-generated />
using System;
using KidsAreaApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace KidsAreaApp.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20210113192040_HoursTable")]
    partial class HoursTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.1");

            modelBuilder.Entity("KidsAreaApp.Models.Hour", b =>
                {
                    b.Property<int>("HourId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<double>("HourPrice")
                        .HasColumnType("float");

                    b.HasKey("HourId");

                    b.ToTable("Hours");
                });

            modelBuilder.Entity("KidsAreaApp.Models.Receipt", b =>
                {
                    b.Property<Guid>("SerialKey")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<byte[]>("BarCode")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("QrCodePath")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SerialKey");

                    b.ToTable("Receipt");
                });

            modelBuilder.Entity("KidsAreaApp.Models.Reservation", b =>
                {
                    b.Property<int>("ReservationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<double>("Cost")
                        .HasColumnType("float");

                    b.Property<DateTime>("EndReservationTme")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("ReceiptSerialKey")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("StartReservationTme")
                        .HasColumnType("datetime2");

                    b.HasKey("ReservationId");

                    b.HasIndex("ReceiptSerialKey");

                    b.ToTable("Reservations");
                });

            modelBuilder.Entity("KidsAreaApp.Models.Reservation", b =>
                {
                    b.HasOne("KidsAreaApp.Models.Receipt", "Receipt")
                        .WithMany()
                        .HasForeignKey("ReceiptSerialKey");

                    b.Navigation("Receipt");
                });
#pragma warning restore 612, 618
        }
    }
}
