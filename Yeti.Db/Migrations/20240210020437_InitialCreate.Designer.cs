﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Yeti.Db;

#nullable disable

namespace Yeti.Db.Migrations
{
    [DbContext(typeof(WriterContext))]
    [Migration("20240210020437_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ManuscriptTag", b =>
                {
                    b.Property<long>("ManuscriptsId")
                        .HasColumnType("bigint");

                    b.Property<long>("TagsId")
                        .HasColumnType("bigint");

                    b.HasKey("ManuscriptsId", "TagsId");

                    b.HasIndex("TagsId");

                    b.ToTable("ManuscriptTag");
                });

            modelBuilder.Entity("Yeti.Db.Model.Fragment", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Heading")
                        .HasColumnType("text");

                    b.Property<long>("ManuscriptId")
                        .HasColumnType("bigint");

                    b.Property<bool>("SoftDelete")
                        .HasColumnType("boolean");

                    b.Property<double>("SortBy")
                        .HasColumnType("double precision");

                    b.Property<DateTimeOffset>("Updated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("WriterId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("Created");

                    b.HasIndex("ManuscriptId");

                    b.HasIndex("SoftDelete");

                    b.HasIndex("WriterId");

                    b.ToTable("Fragments");

                    b.HasData(
                        new
                        {
                            Id = 1L,
                            Content = "By the shore of Gitche Gumbee,\nBy the shining Big-Sea-Water,\nAt the doorway of his wigwam,\nIn the pleasant Summer morning,\nHiawatha stood and waited.",
                            Created = new DateTimeOffset(new DateTime(2024, 2, 10, 2, 4, 37, 479, DateTimeKind.Unspecified).AddTicks(2510), new TimeSpan(0, 0, 0, 0, 0)),
                            ManuscriptId = 1L,
                            SoftDelete = true,
                            SortBy = 1.0,
                            Updated = new DateTimeOffset(new DateTime(2024, 2, 10, 2, 4, 37, 479, DateTimeKind.Unspecified).AddTicks(2520), new TimeSpan(0, 0, 0, 0, 0)),
                            WriterId = 1L
                        },
                        new
                        {
                            Id = 2L,
                            Content = "By the shore of Gitche Gumee,\nBy the shining Big-Sea-Water,\nAt the doorway of his wigwam,\nIn the pleasant Summer morning,\nHiawatha stood and waited.",
                            Created = new DateTimeOffset(new DateTime(2024, 2, 10, 2, 4, 37, 479, DateTimeKind.Unspecified).AddTicks(2520), new TimeSpan(0, 0, 0, 0, 0)),
                            ManuscriptId = 1L,
                            SoftDelete = false,
                            SortBy = 1.0,
                            Updated = new DateTimeOffset(new DateTime(2024, 2, 10, 2, 4, 37, 479, DateTimeKind.Unspecified).AddTicks(2520), new TimeSpan(0, 0, 0, 0, 0)),
                            WriterId = 1L
                        });
                });

            modelBuilder.Entity("Yeti.Db.Model.Manuscript", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Blurb")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("SoftDelete")
                        .HasColumnType("boolean");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("Updated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("WriterId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("Created");

                    b.HasIndex("SoftDelete");

                    b.HasIndex("Title");

                    b.HasIndex("WriterId");

                    b.ToTable("Manuscripts");

                    b.HasData(
                        new
                        {
                            Id = 1L,
                            Blurb = "An old poem",
                            Created = new DateTimeOffset(new DateTime(2024, 2, 10, 2, 4, 37, 479, DateTimeKind.Unspecified).AddTicks(2510), new TimeSpan(0, 0, 0, 0, 0)),
                            SoftDelete = false,
                            Title = "The Song of Hiawatha",
                            Updated = new DateTimeOffset(new DateTime(2024, 2, 10, 2, 4, 37, 479, DateTimeKind.Unspecified).AddTicks(2510), new TimeSpan(0, 0, 0, 0, 0)),
                            WriterId = 1L
                        });
                });

            modelBuilder.Entity("Yeti.Db.Model.Tag", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Value")
                        .IsUnique();

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Yeti.Db.Model.Writer", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("AuthorName")
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Writers");

                    b.HasData(
                        new
                        {
                            Id = 1L,
                            AuthorName = "H. Wadsworth Longfellow",
                            Username = "longfellow"
                        });
                });

            modelBuilder.Entity("ManuscriptTag", b =>
                {
                    b.HasOne("Yeti.Db.Model.Manuscript", null)
                        .WithMany()
                        .HasForeignKey("ManuscriptsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Yeti.Db.Model.Tag", null)
                        .WithMany()
                        .HasForeignKey("TagsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Yeti.Db.Model.Fragment", b =>
                {
                    b.HasOne("Yeti.Db.Model.Manuscript", "Manuscript")
                        .WithMany("Fragments")
                        .HasForeignKey("ManuscriptId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Yeti.Db.Model.Writer", "Writer")
                        .WithMany()
                        .HasForeignKey("WriterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Manuscript");

                    b.Navigation("Writer");
                });

            modelBuilder.Entity("Yeti.Db.Model.Manuscript", b =>
                {
                    b.HasOne("Yeti.Db.Model.Writer", "Writer")
                        .WithMany("Manuscripts")
                        .HasForeignKey("WriterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Writer");
                });

            modelBuilder.Entity("Yeti.Db.Model.Manuscript", b =>
                {
                    b.Navigation("Fragments");
                });

            modelBuilder.Entity("Yeti.Db.Model.Writer", b =>
                {
                    b.Navigation("Manuscripts");
                });
#pragma warning restore 612, 618
        }
    }
}
