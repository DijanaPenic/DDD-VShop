﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using VShop.Modules.Identity.Infrastructure.DAL;

#nullable disable

namespace VShop.Modules.Identity.Infrastructure.DAL.Migrations.Identity
{
    [DbContext(typeof(IdentityDbContext))]
    partial class IdentityDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("user_role", b =>
                {
                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid")
                        .HasColumnName("role_id");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.Property<Instant>("DateCreated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_created");

                    b.Property<Instant>("DateUpdated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_updated");

                    b.HasKey("RoleId", "UserId")
                        .HasName("pk_user_role");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_user_role_user_id");

                    b.ToTable("user_role", "identity");
                });

            modelBuilder.Entity("VShop.Modules.Identity.Infrastructure.DAL.Entities.Client", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<int>("AccessTokenLifeTime")
                        .HasColumnType("integer")
                        .HasColumnName("access_token_life_time");

                    b.Property<bool>("Active")
                        .HasColumnType("boolean")
                        .HasColumnName("active");

                    b.Property<string>("AllowedOrigin")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("allowed_origin");

                    b.Property<Instant>("DateCreated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_created");

                    b.Property<Instant>("DateUpdated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_updated");

                    b.Property<string>("Description")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("name");

                    b.Property<int>("RefreshTokenLifeTime")
                        .HasColumnType("integer")
                        .HasColumnName("refresh_token_life_time");

                    b.Property<string>("Secret")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("secret");

                    b.Property<uint>("xmin")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("Id")
                        .HasName("pk_client");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("client_name_index");

                    b.ToTable("client", "identity");

                    b.HasData(
                        new
                        {
                            Id = new Guid("5c52160a-4ab4-49c6-ba5f-56df9c5730b6"),
                            AccessTokenLifeTime = 20,
                            Active = true,
                            AllowedOrigin = "*",
                            DateCreated = NodaTime.Instant.FromUnixTimeTicks(16409952000000000L),
                            DateUpdated = NodaTime.Instant.FromUnixTimeTicks(16409952000000000L),
                            Description = "Web API Application",
                            Name = "WebApiApplication",
                            RefreshTokenLifeTime = 60,
                            Secret = "PX23zsV/7nm6+ZI9LmrKXSBf2O47cYtiJGk2WJ/G/PdU2eD7Y929MZeItkGpBY2v6a2tXhGINq8bAQYz1bQC6A=="
                        });
                });

            modelBuilder.Entity("VShop.Modules.Identity.Infrastructure.DAL.Entities.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Instant>("DateCreated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_created");

                    b.Property<Instant>("DateUpdated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_updated");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("name");

                    b.Property<string>("NormalizedName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("normalized_name");

                    b.Property<bool>("Stackable")
                        .HasColumnType("boolean")
                        .HasColumnName("stackable");

                    b.Property<uint>("xmin")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("Id")
                        .HasName("pk_role");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("role_name_index");

                    b.ToTable("role", "identity");

                    b.HasData(
                        new
                        {
                            Id = new Guid("d72ef5e5-f08a-4173-b83a-74618893891b"),
                            DateCreated = NodaTime.Instant.FromUnixTimeTicks(16409952000000000L),
                            DateUpdated = NodaTime.Instant.FromUnixTimeTicks(16409952000000000L),
                            Name = "Admin",
                            NormalizedName = "ADMIN",
                            Stackable = false
                        },
                        new
                        {
                            Id = new Guid("d82ef5e5-f08a-4173-b83a-74618893891b"),
                            DateCreated = NodaTime.Instant.FromUnixTimeTicks(16409952000000000L),
                            DateUpdated = NodaTime.Instant.FromUnixTimeTicks(16409952000000000L),
                            Name = "Customer",
                            NormalizedName = "CUSTOMER",
                            Stackable = true
                        },
                        new
                        {
                            Id = new Guid("d92ef5e5-f08a-4173-b83a-74618893891b"),
                            DateCreated = NodaTime.Instant.FromUnixTimeTicks(16409952000000000L),
                            DateUpdated = NodaTime.Instant.FromUnixTimeTicks(16409952000000000L),
                            Name = "Store Manager",
                            NormalizedName = "STORE MANAGER",
                            Stackable = true
                        },
                        new
                        {
                            Id = new Guid("9621c09c-06b1-45fb-8baf-38e0757e2f59"),
                            DateCreated = NodaTime.Instant.FromUnixTimeTicks(16409952000000000L),
                            DateUpdated = NodaTime.Instant.FromUnixTimeTicks(16409952000000000L),
                            Name = "Guest",
                            NormalizedName = "GUEST",
                            Stackable = false
                        });
                });

            modelBuilder.Entity("VShop.Modules.Identity.Infrastructure.DAL.Entities.RoleClaim", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("ClaimType")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("claim_type");

                    b.Property<string>("ClaimValue")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("claim_value");

                    b.Property<Instant>("DateCreated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_created");

                    b.Property<Instant>("DateUpdated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_updated");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid")
                        .HasColumnName("role_id");

                    b.Property<uint>("xmin")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("Id")
                        .HasName("pk_role_claim");

                    b.HasIndex("RoleId")
                        .HasDatabaseName("ix_role_claim_role_id");

                    b.ToTable("role_claim", "identity");

                    b.HasData(
                        new
                        {
                            Id = new Guid("a368b420-2aef-532a-a413-93089dd1fa75"),
                            ClaimType = "permission",
                            ClaimValue = "orders",
                            DateCreated = NodaTime.Instant.FromUnixTimeTicks(16409952000000000L),
                            DateUpdated = NodaTime.Instant.FromUnixTimeTicks(16409952000000000L),
                            RoleId = new Guid("d72ef5e5-f08a-4173-b83a-74618893891b")
                        },
                        new
                        {
                            Id = new Guid("63ed58ac-924b-551d-99f5-4de7a949db97"),
                            ClaimType = "permission",
                            ClaimValue = "shopping_carts",
                            DateCreated = NodaTime.Instant.FromUnixTimeTicks(16409952000000000L),
                            DateUpdated = NodaTime.Instant.FromUnixTimeTicks(16409952000000000L),
                            RoleId = new Guid("d72ef5e5-f08a-4173-b83a-74618893891b")
                        },
                        new
                        {
                            Id = new Guid("688d85e7-178b-5a6f-bce8-753f331af68b"),
                            ClaimType = "permission",
                            ClaimValue = "auth",
                            DateCreated = NodaTime.Instant.FromUnixTimeTicks(16409952000000000L),
                            DateUpdated = NodaTime.Instant.FromUnixTimeTicks(16409952000000000L),
                            RoleId = new Guid("d72ef5e5-f08a-4173-b83a-74618893891b")
                        },
                        new
                        {
                            Id = new Guid("c86fcd6d-8d01-5a0c-9860-6f7d4e9e6cf6"),
                            ClaimType = "permission",
                            ClaimValue = "payments",
                            DateCreated = NodaTime.Instant.FromUnixTimeTicks(16409952000000000L),
                            DateUpdated = NodaTime.Instant.FromUnixTimeTicks(16409952000000000L),
                            RoleId = new Guid("d72ef5e5-f08a-4173-b83a-74618893891b")
                        },
                        new
                        {
                            Id = new Guid("860cb18e-717c-58b4-808f-04f84a548a6f"),
                            ClaimType = "permission",
                            ClaimValue = "categories",
                            DateCreated = NodaTime.Instant.FromUnixTimeTicks(16409952000000000L),
                            DateUpdated = NodaTime.Instant.FromUnixTimeTicks(16409952000000000L),
                            RoleId = new Guid("d72ef5e5-f08a-4173-b83a-74618893891b")
                        },
                        new
                        {
                            Id = new Guid("e11969e7-6f99-514d-9ab4-169b842258e0"),
                            ClaimType = "permission",
                            ClaimValue = "products",
                            DateCreated = NodaTime.Instant.FromUnixTimeTicks(16409952000000000L),
                            DateUpdated = NodaTime.Instant.FromUnixTimeTicks(16409952000000000L),
                            RoleId = new Guid("d72ef5e5-f08a-4173-b83a-74618893891b")
                        },
                        new
                        {
                            Id = new Guid("c8b65e13-1730-5033-8074-e14d0a7b88ea"),
                            ClaimType = "permission",
                            ClaimValue = "categories",
                            DateCreated = NodaTime.Instant.FromUnixTimeTicks(16409952000000000L),
                            DateUpdated = NodaTime.Instant.FromUnixTimeTicks(16409952000000000L),
                            RoleId = new Guid("d92ef5e5-f08a-4173-b83a-74618893891b")
                        },
                        new
                        {
                            Id = new Guid("c33b6d82-374e-5acd-a20a-56115f3efd1a"),
                            ClaimType = "permission",
                            ClaimValue = "products",
                            DateCreated = NodaTime.Instant.FromUnixTimeTicks(16409952000000000L),
                            DateUpdated = NodaTime.Instant.FromUnixTimeTicks(16409952000000000L),
                            RoleId = new Guid("d92ef5e5-f08a-4173-b83a-74618893891b")
                        });
                });

            modelBuilder.Entity("VShop.Modules.Identity.Infrastructure.DAL.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer")
                        .HasColumnName("access_failed_count");

                    b.Property<Instant>("DateCreated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_created");

                    b.Property<Instant>("DateUpdated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_updated");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("email");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean")
                        .HasColumnName("email_confirmed");

                    b.Property<bool>("IsApproved")
                        .HasColumnType("boolean")
                        .HasColumnName("is_approved");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean")
                        .HasColumnName("lockout_enabled");

                    b.Property<Instant?>("LockoutEndDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("lockout_end_date");

                    b.Property<string>("NormalizedEmail")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("normalized_email");

                    b.Property<string>("NormalizedUserName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("normalized_user_name");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text")
                        .HasColumnName("password_hash");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text")
                        .HasColumnName("phone_number");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean")
                        .HasColumnName("phone_number_confirmed");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text")
                        .HasColumnName("security_stamp");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean")
                        .HasColumnName("two_factor_enabled");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("user_name");

                    b.Property<uint>("xmin")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("Id")
                        .HasName("pk_user");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("user_email_index");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("user_name_index");

                    b.ToTable("user", "identity");
                });

            modelBuilder.Entity("VShop.Modules.Identity.Infrastructure.DAL.Entities.UserClaim", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("ClaimType")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("claim_type");

                    b.Property<string>("ClaimValue")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("claim_value");

                    b.Property<Instant>("DateCreated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_created");

                    b.Property<Instant>("DateUpdated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_updated");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.Property<uint>("xmin")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("Id")
                        .HasName("pk_user_claim");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_user_claim_user_id");

                    b.ToTable("user_claim", "identity");
                });

            modelBuilder.Entity("VShop.Modules.Identity.Infrastructure.DAL.Entities.UserLogin", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)")
                        .HasColumnName("login_provider");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)")
                        .HasColumnName("provider_key");

                    b.Property<Instant>("DateCreated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_created");

                    b.Property<Instant>("DateUpdated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_updated");

                    b.Property<bool>("IsConfirmed")
                        .HasColumnType("boolean")
                        .HasColumnName("is_confirmed");

                    b.Property<string>("ProviderDisplayName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("provider_display_name");

                    b.Property<string>("Token")
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)")
                        .HasColumnName("token");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.Property<uint>("xmin")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("LoginProvider", "ProviderKey")
                        .HasName("pk_user_login");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_user_login_user_id");

                    b.ToTable("user_login", "identity");
                });

            modelBuilder.Entity("VShop.Modules.Identity.Infrastructure.DAL.Entities.UserRefreshToken", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.Property<Guid>("ClientId")
                        .HasColumnType("uuid")
                        .HasColumnName("client_id");

                    b.Property<Instant>("DateCreated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_created");

                    b.Property<Instant>("DateExpires")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_expires");

                    b.Property<Instant>("DateUpdated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_updated");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("value");

                    b.Property<uint>("xmin")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("UserId", "ClientId")
                        .HasName("pk_user_refresh_token");

                    b.HasIndex("ClientId")
                        .HasDatabaseName("ix_user_refresh_token_client_id");

                    b.ToTable("user_refresh_token", "identity");
                });

            modelBuilder.Entity("VShop.Modules.Identity.Infrastructure.DAL.Entities.UserToken", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)")
                        .HasColumnName("login_provider");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)")
                        .HasColumnName("name");

                    b.Property<Instant>("DateCreated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_created");

                    b.Property<Instant>("DateUpdated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_updated");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("value");

                    b.Property<uint>("xmin")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("UserId", "LoginProvider", "Name")
                        .HasName("pk_user_token");

                    b.ToTable("user_token", "identity");
                });

            modelBuilder.Entity("user_role", b =>
                {
                    b.HasOne("VShop.Modules.Identity.Infrastructure.DAL.Entities.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_role_roles_role_id");

                    b.HasOne("VShop.Modules.Identity.Infrastructure.DAL.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_role_users_user_id");

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("VShop.Modules.Identity.Infrastructure.DAL.Entities.RoleClaim", b =>
                {
                    b.HasOne("VShop.Modules.Identity.Infrastructure.DAL.Entities.Role", "Role")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_role_claim_roles_role_id");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("VShop.Modules.Identity.Infrastructure.DAL.Entities.UserClaim", b =>
                {
                    b.HasOne("VShop.Modules.Identity.Infrastructure.DAL.Entities.User", "User")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_claim_users_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("VShop.Modules.Identity.Infrastructure.DAL.Entities.UserLogin", b =>
                {
                    b.HasOne("VShop.Modules.Identity.Infrastructure.DAL.Entities.User", "User")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_login_user_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("VShop.Modules.Identity.Infrastructure.DAL.Entities.UserRefreshToken", b =>
                {
                    b.HasOne("VShop.Modules.Identity.Infrastructure.DAL.Entities.Client", "Client")
                        .WithMany("RefreshTokens")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_refresh_token_client_client_id");

                    b.HasOne("VShop.Modules.Identity.Infrastructure.DAL.Entities.User", "User")
                        .WithMany("RefreshTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_refresh_token_user_user_id");

                    b.Navigation("Client");

                    b.Navigation("User");
                });

            modelBuilder.Entity("VShop.Modules.Identity.Infrastructure.DAL.Entities.UserToken", b =>
                {
                    b.HasOne("VShop.Modules.Identity.Infrastructure.DAL.Entities.User", "User")
                        .WithMany("UserTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_token_user_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("VShop.Modules.Identity.Infrastructure.DAL.Entities.Client", b =>
                {
                    b.Navigation("RefreshTokens");
                });

            modelBuilder.Entity("VShop.Modules.Identity.Infrastructure.DAL.Entities.Role", b =>
                {
                    b.Navigation("Claims");
                });

            modelBuilder.Entity("VShop.Modules.Identity.Infrastructure.DAL.Entities.User", b =>
                {
                    b.Navigation("Claims");

                    b.Navigation("Logins");

                    b.Navigation("RefreshTokens");

                    b.Navigation("UserTokens");
                });
#pragma warning restore 612, 618
        }
    }
}
