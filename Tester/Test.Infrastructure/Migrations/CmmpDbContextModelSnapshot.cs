﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Test.Infrastructure;

namespace Test.Infrastructure.Migrations
{
    [DbContext(typeof(CmmpDbContext))]
    partial class CmmpDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Test.Domain.Models.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasMaxLength(36);

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime");

                    b.Property<Guid>("Creator")
                        .HasColumnType("uniqueidentifier")
                        .HasMaxLength(36);

                    b.Property<string>("Desc")
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(20)");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("UpdateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime");

                    b.HasKey("Id");

                    b.ToTable("Role");
                });

            modelBuilder.Entity("Test.Domain.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasMaxLength(36);

                    b.Property<string>("ContactNumber")
                        .HasColumnType("nvarchar(12)");

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime");

                    b.Property<Guid>("Creator")
                        .HasColumnType("uniqueidentifier")
                        .HasMaxLength(36);

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("UpdateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime");

                    b.Property<string>("UserAccount")
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("UserPassword")
                        .HasColumnType("nvarchar(36)");

                    b.Property<int>("UserType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("User");

                    b.HasData(
                        new
                        {
                            Id = new Guid("58205e0e-1552-4282-bedc-a92d0afb37df"),
                            CreateTime = new DateTime(2020, 10, 14, 15, 31, 33, 365, DateTimeKind.Local).AddTicks(2746),
                            Creator = new Guid("58205e0e-1552-4282-bedc-a92d0afb37df"),
                            State = 0,
                            TenantId = new Guid("f339be29-7ce2-4876-bcca-d3abe3d16f75"),
                            UpdateTime = new DateTime(2020, 10, 14, 15, 31, 33, 365, DateTimeKind.Local).AddTicks(2771),
                            UserAccount = "admin",
                            UserName = "系统管理员",
                            UserPassword = "21232F297A57A5A743894A0E4A801FC3",
                            UserType = 0
                        });
                });

            modelBuilder.Entity("Test.Domain.Models.UserRole", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRole");
                });

            modelBuilder.Entity("Test.Domain.Models.UserRole", b =>
                {
                    b.HasOne("Test.Domain.Models.Role", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Test.Domain.Models.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
