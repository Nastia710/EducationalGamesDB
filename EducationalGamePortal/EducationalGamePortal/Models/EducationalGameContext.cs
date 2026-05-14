using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EducationalGamePortal.Models;

public partial class EducationalGameContext : DbContext
{
    public EducationalGameContext()
    {
    }

    public EducationalGameContext(DbContextOptions<EducationalGameContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Achievement> Achievements { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<GameSession> GameSessions { get; set; }

    public virtual DbSet<Level> Levels { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAchievement> UserAchievements { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Achievement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("achievements_pkey");

            entity.ToTable("achievements");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CriteriaType)
                .HasMaxLength(50)
                .HasColumnName("criteria_type");
            entity.Property(e => e.CriteriaValue).HasColumnName("criteria_value");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("games_pkey");

            entity.ToTable("games");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Subject)
                .HasMaxLength(100)
                .HasColumnName("subject");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");
        });

        modelBuilder.Entity<GameSession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("game_sessions_pkey");

            entity.ToTable("game_sessions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsCompleted)
                .HasDefaultValue(false)
                .HasColumnName("is_completed");
            entity.Property(e => e.LevelId).HasColumnName("level_id");
            entity.Property(e => e.PlayedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("played_at");
            entity.Property(e => e.Score)
                .HasDefaultValue(0)
                .HasColumnName("score");
            entity.Property(e => e.TimeSpent)
                .HasDefaultValue(0)
                .HasColumnName("time_spent");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Level).WithMany(p => p.GameSessions)
                .HasForeignKey(d => d.LevelId)
                .HasConstraintName("game_sessions_level_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.GameSessions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("game_sessions_user_id_fkey");
        });

        modelBuilder.Entity<Level>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("levels_pkey");

            entity.ToTable("levels");

            entity.HasIndex(e => new { e.GameId, e.LevelNumber }, "levels_game_id_level_number_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Difficulty)
                .HasMaxLength(20)
                .HasColumnName("difficulty");
            entity.Property(e => e.GameId).HasColumnName("game_id");
            entity.Property(e => e.LevelNumber).HasColumnName("level_number");
            entity.Property(e => e.MaxScore).HasColumnName("max_score");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");

            entity.HasOne(d => d.Game).WithMany(p => p.Levels)
                .HasForeignKey(d => d.GameId)
                .HasConstraintName("levels_game_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.HasIndex(e => e.Nickname, "users_nickname_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Nickname)
                .HasMaxLength(50)
                .HasColumnName("nickname");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.RegistrationDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("registration_date");
        });

        modelBuilder.Entity<UserAchievement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_achievements_pkey");

            entity.ToTable("user_achievements");

            entity.HasIndex(e => new { e.UserId, e.AchievementId }, "user_achievements_user_id_achievement_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AchievementId).HasColumnName("achievement_id");
            entity.Property(e => e.UnlockedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("unlocked_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Achievement).WithMany(p => p.UserAchievements)
                .HasForeignKey(d => d.AchievementId)
                .HasConstraintName("user_achievements_achievement_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserAchievements)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_achievements_user_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
