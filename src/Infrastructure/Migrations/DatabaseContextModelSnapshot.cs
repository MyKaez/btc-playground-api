﻿// <auto-generated />
using System;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Infrastructure.Database.Connection", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<Guid?>("SessionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("SessionId");

                    b.ToTable("Connections");
                });

            modelBuilder.Entity("Infrastructure.Database.Interaction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<Guid>("SessionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("SessionId");

                    b.HasIndex("UserId");

                    b.ToTable("Interactions");
                });

            modelBuilder.Entity("Infrastructure.Database.Message", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("SessionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Text")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("SessionId");

                    b.HasIndex("UserId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("Infrastructure.Database.Session", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Configuration")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ControlId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("StartTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Updated")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("Infrastructure.Database.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Configuration")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ConnectionId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<Guid>("ControlId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ConnectionId")
                        .IsUnique()
                        .HasFilter("[ConnectionId] IS NOT NULL");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Infrastructure.Database.Connection", b =>
                {
                    b.HasOne("Infrastructure.Database.Session", "Session")
                        .WithMany("Connections")
                        .HasForeignKey("SessionId");

                    b.Navigation("Session");
                });

            modelBuilder.Entity("Infrastructure.Database.Interaction", b =>
                {
                    b.HasOne("Infrastructure.Database.Session", "Session")
                        .WithMany("Interactions")
                        .HasForeignKey("SessionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Infrastructure.Database.User", "User")
                        .WithMany("Interactions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Session");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Infrastructure.Database.Message", b =>
                {
                    b.HasOne("Infrastructure.Database.Session", "Session")
                        .WithMany("Messages")
                        .HasForeignKey("SessionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Infrastructure.Database.User", "User")
                        .WithMany("Messages")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Session");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Infrastructure.Database.User", b =>
                {
                    b.HasOne("Infrastructure.Database.Connection", "Connection")
                        .WithOne("User")
                        .HasForeignKey("Infrastructure.Database.User", "ConnectionId");

                    b.Navigation("Connection");
                });

            modelBuilder.Entity("Infrastructure.Database.Connection", b =>
                {
                    b.Navigation("User");
                });

            modelBuilder.Entity("Infrastructure.Database.Session", b =>
                {
                    b.Navigation("Connections");

                    b.Navigation("Interactions");

                    b.Navigation("Messages");
                });

            modelBuilder.Entity("Infrastructure.Database.User", b =>
                {
                    b.Navigation("Interactions");

                    b.Navigation("Messages");
                });
#pragma warning restore 612, 618
        }
    }
}
