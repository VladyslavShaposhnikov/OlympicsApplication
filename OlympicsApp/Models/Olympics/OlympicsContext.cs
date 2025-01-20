using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OlympicsApp.Models.Olympics;

public partial class OlympicsContext : DbContext
{
    public OlympicsContext()
    {
    }

    public OlympicsContext(DbContextOptions<OlympicsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<CompetitorEvent> CompetitorEvents { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<GamesCity> GamesCities { get; set; }

    public virtual DbSet<GamesCompetitor> GamesCompetitors { get; set; }

    public virtual DbSet<Medal> Medals { get; set; }

    public virtual DbSet<NocRegion> NocRegions { get; set; }

    public virtual DbSet<Person> People { get; set; }

    public virtual DbSet<PersonRegion> PersonRegions { get; set; }

    public virtual DbSet<Sport> Sports { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>(entity =>
        {
            entity.ToTable("city");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CityName)
                .HasDefaultValueSql("NULL")
                .HasColumnName("city_name");
        });

        modelBuilder.Entity<CompetitorEvent>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("competitor_event");

            entity.Property(e => e.CompetitorId)
                .HasDefaultValueSql("NULL")
                .HasColumnName("competitor_id");
            entity.Property(e => e.EventId)
                .HasDefaultValueSql("NULL")
                .HasColumnName("event_id");
            entity.Property(e => e.MedalId)
                .HasDefaultValueSql("NULL")
                .HasColumnName("medal_id");

            entity.HasOne(d => d.Competitor).WithMany().HasForeignKey(d => d.CompetitorId);

            entity.HasOne(d => d.Event).WithMany().HasForeignKey(d => d.EventId);

            entity.HasOne(d => d.Medal).WithMany().HasForeignKey(d => d.MedalId);
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.ToTable("event");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.EventName)
                .HasDefaultValueSql("NULL")
                .HasColumnName("event_name");
            entity.Property(e => e.SportId)
                .HasDefaultValueSql("NULL")
                .HasColumnName("sport_id");

            entity.HasOne(d => d.Sport).WithMany(p => p.Events).HasForeignKey(d => d.SportId);
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.ToTable("games");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.GamesName)
                .HasDefaultValueSql("NULL")
                .HasColumnName("games_name");
            entity.Property(e => e.GamesYear)
                .HasDefaultValueSql("NULL")
                .HasColumnName("games_year");
            entity.Property(e => e.Season)
                .HasDefaultValueSql("NULL")
                .HasColumnName("season");
        });

        modelBuilder.Entity<GamesCity>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("games_city");

            entity.Property(e => e.CityId)
                .HasDefaultValueSql("NULL")
                .HasColumnName("city_id");
            entity.Property(e => e.GamesId)
                .HasDefaultValueSql("NULL")
                .HasColumnName("games_id");

            entity.HasOne(d => d.City).WithMany().HasForeignKey(d => d.CityId);

            entity.HasOne(d => d.Games).WithMany().HasForeignKey(d => d.GamesId);
        });

        modelBuilder.Entity<GamesCompetitor>(entity =>
        {
            entity.ToTable("games_competitor");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Age)
                .HasDefaultValueSql("NULL")
                .HasColumnName("age");
            entity.Property(e => e.GamesId)
                .HasDefaultValueSql("NULL")
                .HasColumnName("games_id");
            entity.Property(e => e.PersonId)
                .HasDefaultValueSql("NULL")
                .HasColumnName("person_id");

            entity.HasOne(d => d.Games).WithMany(p => p.GamesCompetitors).HasForeignKey(d => d.GamesId);

            entity.HasOne(d => d.Person).WithMany(p => p.GamesCompetitors).HasForeignKey(d => d.PersonId);
        });

        modelBuilder.Entity<Medal>(entity =>
        {
            entity.ToTable("medal");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.MedalName)
                .HasDefaultValueSql("NULL")
                .HasColumnName("medal_name");
        });

        modelBuilder.Entity<NocRegion>(entity =>
        {
            entity.ToTable("noc_region");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Noc)
                .HasDefaultValueSql("NULL")
                .HasColumnName("noc");
            entity.Property(e => e.RegionName)
                .HasDefaultValueSql("NULL")
                .HasColumnName("region_name");
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.ToTable("person");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.FullName)
                .HasDefaultValueSql("NULL")
                .HasColumnName("full_name");
            entity.Property(e => e.Gender)
                .HasDefaultValueSql("NULL")
                .HasColumnName("gender");
            entity.Property(e => e.Height)
                .HasDefaultValueSql("NULL")
                .HasColumnName("height");
            entity.Property(e => e.Weight)
                .HasDefaultValueSql("NULL")
                .HasColumnName("weight");
        });

        modelBuilder.Entity<PersonRegion>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("person_region");

            entity.Property(e => e.PersonId)
                .HasDefaultValueSql("NULL")
                .HasColumnName("person_id");
            entity.Property(e => e.RegionId)
                .HasDefaultValueSql("NULL")
                .HasColumnName("region_id");

            entity.HasOne(d => d.Person).WithMany().HasForeignKey(d => d.PersonId);

            entity.HasOne(d => d.Region).WithMany().HasForeignKey(d => d.RegionId);
        });

        modelBuilder.Entity<Sport>(entity =>
        {
            entity.ToTable("sport");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.SportName)
                .HasDefaultValueSql("NULL")
                .HasColumnName("sport_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
