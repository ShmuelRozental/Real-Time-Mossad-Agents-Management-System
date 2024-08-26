﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Real_Time_Mossad_Agents_Management_System.Data;

#nullable disable

namespace Real_Time_Mossad_Agents_Management_System.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240824091751_addCollomPhotoUrlToAgent")]
    partial class addCollomPhotoUrlToAgent
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Real_Time_Mossad_Agents_Management_System.Models.Agent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.Property<string>("nickname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("photoUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Agents");
                });

            modelBuilder.Entity("Real_Time_Mossad_Agents_Management_System.Models.Mission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AgentId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("TargetId")
                        .HasColumnType("int");

                    b.Property<DateTime>("TimeLeft")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("AgentId");

                    b.HasIndex("TargetId");

                    b.ToTable("Missions");
                });

            modelBuilder.Entity("Real_Time_Mossad_Agents_Management_System.Models.Target", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Position")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.Property<string>("photoUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Targets");
                });

            modelBuilder.Entity("Real_Time_Mossad_Agents_Management_System.Models.Agent", b =>
                {
                    b.OwnsOne("Real_Time_Mossad_Agents_Management_System.Models.PinLocation", "Location", b1 =>
                        {
                            b1.Property<int>("AgentId")
                                .HasColumnType("int");

                            b1.Property<int>("X")
                                .HasColumnType("int");

                            b1.Property<int>("Y")
                                .HasColumnType("int");

                            b1.HasKey("AgentId");

                            b1.ToTable("Agents");

                            b1.WithOwner()
                                .HasForeignKey("AgentId");
                        });

                    b.Navigation("Location");
                });

            modelBuilder.Entity("Real_Time_Mossad_Agents_Management_System.Models.Mission", b =>
                {
                    b.HasOne("Real_Time_Mossad_Agents_Management_System.Models.Agent", "Agent")
                        .WithMany()
                        .HasForeignKey("AgentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Real_Time_Mossad_Agents_Management_System.Models.Target", "Target")
                        .WithMany()
                        .HasForeignKey("TargetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Agent");

                    b.Navigation("Target");
                });

            modelBuilder.Entity("Real_Time_Mossad_Agents_Management_System.Models.Target", b =>
                {
                    b.OwnsOne("Real_Time_Mossad_Agents_Management_System.Models.PinLocation", "Location", b1 =>
                        {
                            b1.Property<int>("TargetId")
                                .HasColumnType("int");

                            b1.Property<int>("X")
                                .HasColumnType("int");

                            b1.Property<int>("Y")
                                .HasColumnType("int");

                            b1.HasKey("TargetId");

                            b1.ToTable("Targets");

                            b1.WithOwner()
                                .HasForeignKey("TargetId");
                        });

                    b.Navigation("Location");
                });
#pragma warning restore 612, 618
        }
    }
}
